using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using Radzen.Blazor.GridColumnVisibilityChooser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Radzen.Blazor.GridColumnVisibilityChooser
{
    public partial class RadzenGridColumnVisibilityChooser<TItem>
    {
        [Parameter]
        public RadzenGrid<TItem> Grid { get; set; }

        [Parameter]
        public Action RefreshParentStateAction { get; set; }

        [Parameter]
        public Func<string, bool> GetDefaultVisibility { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public bool PreserveState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public ILocalStorageService LocalStorageService { get; set; }

        public IEnumerable<string> Columns { get; set; }
        public IEnumerable<string> VisibleColumns { get; set; }

        bool _isInitialVisibilitySet { get; set; }

        public static string ColumnVisibilityLocalStorageIdentifier => $"radzen-blazor-gridcolumnvisibilities";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Grid != null)
            {
                //Collection initial data
                Columns = Grid.ColumnsCollection.Select(x => String.IsNullOrEmpty(x.Title) ? x.Property : x.Title).ToList();

                //Check local storage and apply if found
                if (PreserveState && !_isInitialVisibilitySet)
                {
                    var columnVisibilities = await GetPreservedColumnVisibility();
                    if (columnVisibilities != null && columnVisibilities.Any())
                    {
                        foreach (var col in columnVisibilities)
                        {
                            if (Grid.ColumnsCollection.Any(x => x.Title == col.ColumnName || x.Property == col.ColumnName))
                                #pragma warning disable BL0005 // Component parameter should not be set outside of its component.
                                Grid.ColumnsCollection.FirstOrDefault(x => x.Title == col.ColumnName || x.Property == col.ColumnName).Visible = col.IsVisible;
                                #pragma warning restore BL0005 // Component parameter should not be set outside of its component.        
                        }

                        _isInitialVisibilitySet = true;

                        //Refresh parent component
                        RefreshParentStateAction();
                    }
                }
                else if (!_isInitialVisibilitySet)
                    await ClearColumnVisibilityFromStorage();

                //Set default visibility
                if (GetDefaultVisibility != null && !_isInitialVisibilitySet)
                {
                    foreach (var col in Columns)
                    {
                        #pragma warning disable BL0005 // Component parameter should not be set outside of its component.
                        Grid.ColumnsCollection.FirstOrDefault(x => x.Title == col || x.Property == col).Visible = GetDefaultVisibility(col);
                        #pragma warning restore BL0005 // Component parameter should not be set outside of its component.        
                    }

                    _isInitialVisibilitySet = true;

                    //Refresh parent component
                    RefreshParentStateAction();
                }

                VisibleColumns = Grid.ColumnsCollection.Where(x => x.Visible).Select(x => String.IsNullOrEmpty(x.Title) ? x.Property : x.Title).ToList();
                InvokeAsync(StateHasChanged);
            }
        }

        async void OnColumnVisibilityChanged(object value)
        {
            var converted = value as IEnumerable<string>;
            if (converted != null)
            {
                var diff = Columns.Except(converted).ToList();

                foreach (var d in diff)
                    #pragma warning disable BL0005 // Component parameter should not be set outside of its component.
                    Grid.ColumnsCollection.FirstOrDefault(x => x.Title == d || x.Property == d).Visible = false;
                    #pragma warning restore BL0005 // Component parameter should not be set outside of its component.

                var same = Columns.Intersect(converted).ToList();
                foreach (var s in same)
                    #pragma warning disable BL0005 // Component parameter should not be set outside of its component.
                    Grid.ColumnsCollection.FirstOrDefault(x => x.Title == s || x.Property == s).Visible = true;
                    #pragma warning restore BL0005 // Component parameter should not be set outside of its component.

                //Refresh parent component
                RefreshParentStateAction();

                //If preservation is enabled, preserve state
                if (PreserveState)
                    await PreserveColumnVisibility(Grid.ColumnsCollection.Select(x => new ColumnVisibility()
                    {
                        ColumnName = String.IsNullOrEmpty(x.Title) ? x.Property : x.Title,
                        IsVisible = x.Visible
                    }));
            }
        }

        /// <summary>
        /// If there're any, gets preserved column visibility from local storage
        /// </summary>
        /// <returns>List of column names</returns>
        public async Task<IEnumerable<ColumnVisibility>> GetPreservedColumnVisibility()
        {
            var identifier = GetPageIdentifier();
            var containsKey = await LocalStorageService.ContainKeyAsync(ColumnVisibilityLocalStorageIdentifier);

            if (!containsKey)
                return new List<ColumnVisibility>();

            var visibilities = await LocalStorageService.GetItemAsync<IEnumerable<ColumnsVisibility>>(ColumnVisibilityLocalStorageIdentifier);
            if (visibilities == null || !visibilities.Any())
                return new List<ColumnVisibility>();

            var pageVisibilities = visibilities.FirstOrDefault(x => x.PageIdentifier == identifier);
            if (pageVisibilities == null)
                return new List<ColumnVisibility>();

            return pageVisibilities.Visibilities;
        }

        /// <summary>
        /// Preserves the state of column visibility
        /// </summary>
        /// <param name="visibilities"></param>
        /// <returns></returns>
        public async Task PreserveColumnVisibility(IEnumerable<ColumnVisibility> visibilities)
        {
            var identifier = GetPageIdentifier();
            var containsKey = await LocalStorageService.ContainKeyAsync(ColumnVisibilityLocalStorageIdentifier);

            List<ColumnsVisibility> data;
            if (containsKey)
            {
                data = await LocalStorageService.GetItemAsync<List<ColumnsVisibility>>(ColumnVisibilityLocalStorageIdentifier);
                var pageVisibility = data.FirstOrDefault(x => x.PageIdentifier == identifier);

                //If page data is found, just update visibilities
                if (pageVisibility != null)
                    data[data.IndexOf(pageVisibility)].Visibilities = visibilities;
                //if it isn't, add new item
                else
                    data.Add(new ColumnsVisibility() { PageIdentifier = identifier, Visibilities = visibilities });
            }
            else
                data = new List<ColumnsVisibility>()
                {
                    new ColumnsVisibility(){ PageIdentifier = identifier, Visibilities = visibilities }
                };

            await LocalStorageService.SetItemAsync(ColumnVisibilityLocalStorageIdentifier, data);
        }
        
        /// <summary>
        /// Clears column visibility from local storage
        /// </summary>
        /// <returns></returns>
        public async Task ClearColumnVisibilityFromStorage()
        {
            var identifier = GetPageIdentifier();
            var containsKey = await LocalStorageService.ContainKeyAsync(ColumnVisibilityLocalStorageIdentifier);

            if (!containsKey)
                return;

            var data = await LocalStorageService.GetItemAsync<IEnumerable<ColumnsVisibility>>(ColumnVisibilityLocalStorageIdentifier);
            if (data == null || !data.Any())
                return;

            var dataCleared = data.Where(x => x.PageIdentifier != identifier).ToList();
            await LocalStorageService.SetItemAsync(ColumnVisibilityLocalStorageIdentifier, dataCleared);
        }

        /// <summary>
        /// Gets current page identifier
        /// </summary>
        /// <returns>Current page identifier</returns>
        public string GetPageIdentifier()
        {
            var relativeUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            return relativeUrl.IndexOf("?") > -1 ? relativeUrl.Substring(0, relativeUrl.IndexOf("?")) : relativeUrl;
        }
    }
}
