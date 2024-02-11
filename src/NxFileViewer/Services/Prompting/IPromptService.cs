using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Services.Prompting;

public interface IPromptService
{
    string? PromptSelectDir(string title);

    string? PromptSaveFile(string defaultFileName, string? title = null, string? filters = null);
}