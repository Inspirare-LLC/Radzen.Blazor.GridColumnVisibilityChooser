using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public string Placeholder { get; set; }

        public IEnumerable<string> Columns { get; set; }
        public IEnumerable<string> VisibleColumns { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (Grid != null)
            {
                //Collection initial data
                Columns = Grid.ColumnsCollection.Select(x => String.IsNullOrEmpty(x.Title) ? x.Property : x.Title).ToList();
                VisibleColumns = Grid.ColumnsCollection.Where(x => x.Visible).Select(x => String.IsNullOrEmpty(x.Title) ? x.Property : x.Title).ToList();
                InvokeAsync(StateHasChanged);
            }
        }

        void OnColumnVisibilityChanged(object value)
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
            }

        }
    }
}
