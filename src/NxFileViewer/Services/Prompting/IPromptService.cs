namespace Emignatik.NxFileViewer.Services.Prompting;

public interface IPromptService
{
    /// <summary>
    /// Prompts the user to select a directory
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    string? PromptSelectDir(string title);

    /// <summary>
    /// Prompts the user to select a location where to save a file
    /// </summary>
    /// <param name="defaultFileName"></param>
    /// <param name="title"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    string? PromptSaveFile(string defaultFileName, string? title = null, string? filter = null);
}