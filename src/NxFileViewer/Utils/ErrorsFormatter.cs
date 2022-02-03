using System;
using System.Collections.Generic;
using System.Linq;

namespace Emignatik.NxFileViewer.Utils;

public static class ErrorsFormatter
{
    public static string Format(IEnumerable<string> errors)
    {
        return errors.Aggregate("", (acc, error) => acc += $"• {error}{Environment.NewLine}");
    }
}