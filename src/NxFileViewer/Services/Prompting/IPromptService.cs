namespace Emignatik.NxFileViewer.Services.Prompting;

public interface IPromptService
{
    string? PromptSelectDir(string title);

    string? PromptSaveFile(string proposedFileName);
}