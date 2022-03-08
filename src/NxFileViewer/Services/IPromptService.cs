namespace Emignatik.NxFileViewer.Services
{
    public interface IPromptService
    {
        string? PromptSelectDir(string title);

        string? PromptSaveFile(string proposedFileName);
    }
}
