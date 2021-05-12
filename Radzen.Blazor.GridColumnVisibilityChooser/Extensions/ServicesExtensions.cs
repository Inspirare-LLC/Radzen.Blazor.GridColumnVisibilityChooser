using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Radzen.Blazor.GridColumnVisibilityChooser.Extensions
{
    /// <summary>
    /// Services extensions required to set up the plugin
    /// </summary>
    public static class ServicesExtensions
    {
        public static IServiceCollection ConfigureColumnVisibility(this IServiceCollection services)
        {
            return services.AddBlazoredLocalStorage();
        }
    }
}
