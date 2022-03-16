using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Emignatik.NxFileViewer.Services.Prompting;

public interface IPromptService
{
    string? PromptSelectDir(string title);

    string? PromptSaveFile(string defaultFileName, string? title = null, IEnumerable<CommonFileDialogFilter>? filters = null);
}