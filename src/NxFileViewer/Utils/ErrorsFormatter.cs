using System;
using System.Collections.Generic;
using System.Linq;
using Emignatik.NxFileViewer.Models.TreeItems;

namespace Emignatik.NxFileViewer.Utils;

public static class ErrorsFormatter
{
    public static string Format(IEnumerable<ItemError> errors)
    {
        var formattedErrors = string.Join(Environment.NewLine, errors.Select(e => $"• {e.Message}"));
        return formattedErrors;
    }
}