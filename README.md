# Radzen.Blazor.GridColumnVisibilityChooser
Column visibility chooser for Radzen Blazor

![Column visibility chooser for Radzen Blazor](https://user-images.githubusercontent.com/30466254/137726924-d586dc4d-ba20-41c4-86aa-016bc3a22528.png)

# Breaking changes

From version 2.0.0.0 `RadzenGrid` has been replaced with `RadzenDataGrid`.

From version 1.0.0.4 functionality of preserving column visibility state is introduced.

It is controlled by `IsPreserved` parameter. 

For it to work, configuration must be done in `Startup.ConfigureServices` method as such: `services.ConfigureColumnVisibility();`. Failing to include this will result in exceptions.

# Nuget package
Available as nuget package at https://www.nuget.org/packages/Radzen.Blazor.GridColumnVisibilityChooser
Install it in shared code project.

# Usage

**Create the control like so:**

    <Radzen.Blazor.GridColumnVisibilityChooser.RadzenGridColumnVisibilityChooser Grid="@grid" RefreshParentStateAction="@(() => InvokeAsync(StateHasChanged))"/>
    <RadzenDataGrid @ref="@grid"/>

    @code{
      RadzenDataGrid<TItem> grid;
    }

It is required to call `InvokeAsync(StateHasChanged);` for the first render, if it is not called already while doing page initialisation:

```
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            InvokeAsync(StateHasChanged);
    }
```

**To set default column visibility, provide `GetDefaultVisibility` function as a parameter:**

    <Radzen.Blazor.GridColumnVisibilityChooser.RadzenGridColumnVisibilityChooser Grid="@grid" RefreshParentStateAction="@(() => InvokeAsync(StateHasChanged))" GetDefaultVisibility="@((colName) => GetDefaultColumnVisibility(colName))"/>
    
    @code{
        bool GetDefaultColumnVisibility(string colName)
        {
            return colName == "Test1" || colName == "Test2" ? false : true;
        }
    }
    
Note: Don't use `Visible` parameter, then switching visibility of the column won't work.

**To preserve state across sessions with local storage, use:**

    <Radzen.Blazor.GridColumnVisibilityChooser.RadzenGridColumnVisibilityChooser Grid="@grid" RefreshParentStateAction="@(() => InvokeAsync(StateHasChanged))" PreserveState="true"/>
    <RadzenDataGrid @ref="@grid"/>

    @code{
      RadzenDataGrid<TItem> grid;
    }
 
This will, on each column visibility change, save the state in local storage and load it the next time the page is loaded. Pages are identified uniquely by their relative url without query string.

Note: When using `PreserveState` the `GetDefaultVisibility` is not invoked.

**To use column visibility chooser preserve state feature with more than one Grid in one page, assing `id` (html id) to each Grid:**

```
<Radzen.Blazor.GridColumnVisibilityChooser.RadzenGridColumnVisibilityChooser Grid="@grid1" RefreshParentStateAction="@(() => InvokeAsync(StateHasChanged))" PreserveState="true"/>
<RadzenDataGrid @ref="@grid1" id="grid1Id"/>

<Radzen.Blazor.GridColumnVisibilityChooser.RadzenGridColumnVisibilityChooser Grid="@grid2" RefreshParentStateAction="@(() => InvokeAsync(StateHasChanged))" PreserveState="true"/>
<RadzenDataGrid @ref="@grid2" id="grid2Id"/>

@code{
  RadzenDataGrid<TItem> grid1;
  RadzenDataGrid<TItem> grid2;
}
```

# Contributions

Any contributions are welcome in the form of pull requests.

# Issues

Issues can be raised in the `Issue` section where I'll try to address all of them.
