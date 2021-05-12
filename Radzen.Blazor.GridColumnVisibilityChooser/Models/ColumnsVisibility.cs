using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Radzen.Blazor.GridColumnVisibilityChooser.Models
{
    public class ColumnsVisibility
    {
        public string PageIdentifier { get; set; }

        public IEnumerable<ColumnVisibility> Visibilities { get; set; }
    }
}
