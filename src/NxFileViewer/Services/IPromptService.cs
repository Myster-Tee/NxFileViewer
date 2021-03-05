namespace Emignatik.NxFileViewer.Services
{
    public interface IPromptService
    {
        string? PromptSaveDir();

        string? PromptSaveFile(string proposedFileName);
    }
}
