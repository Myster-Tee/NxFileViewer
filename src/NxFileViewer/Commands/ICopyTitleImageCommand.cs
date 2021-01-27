using System.Windows.Input;
using Emignatik.NxFileViewer.Model.Overview;

namespace Emignatik.NxFileViewer.Commands
{
    public interface ICopyTitleImageCommand : ICommand
    {
        TitleInfo? Title { get; set; }
    }
}