# Radzen.Blazor.GridColumnVisibilityChooser
Column visibility chooser for Radzen Blazor

# Nuget package
Available as nuget package at https://www.nuget.org/packages/Radzen.Blazor.GridColumnVisibilityChooser
Install it in shared code project.

# Usage

Create the control like so:

    <RadzenGridColumnVisibilityChooser Grid="@grid" RefreshParentStateAction="@(() => InvokeAsync(StateHasChanged))"/>
    <RadzenGrid @ref="@grid"/>

    @code{
      RadzenGrid<TItem> grid;
    }

To set default column visibility, provide `GetDefaultVisibility` function as a parameter:

    <RadzenGridColumnVisibilityChooser Grid="@grid" RefreshParentStateAction="@(() => InvokeAsync(StateHasChanged))" GetDefaultVisibility="@((colName) => GetDefaultColumnVisibility(colName))"/>
    
    @code{
        bool GetDefaultColumnVisibility(string colName)
        {
            return colName == "Test1" || colName == "Test2" ? false : true;
        }
    }
    
 Note: Don't use `Visible` parameter, then switching visibility of the column won't work.

# Contributions

Any contributions are welcome in the form of pull requests.

# Issues

Issues can be raised in the `Issue` section where I'll try to address all of them.
