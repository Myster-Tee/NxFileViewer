using System.Windows.Input;
using Emignatik.NxFileViewer.Model.TreeItems;

namespace Emignatik.NxFileViewer.Commands
{
    public interface ISaveItemToFileCommand : ICommand
    {
        IItem? Item { get; set; }
    }
}