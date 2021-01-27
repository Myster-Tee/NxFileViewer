using Emignatik.NxFileViewer.Model.Overview;

namespace Emignatik.NxFileViewer.Commands
{
    public interface ISaveTitleImageCommand
    {
        TitleInfo? Title { get; set; }
    }
}