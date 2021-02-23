using System.Collections.Generic;

namespace Emignatik.NxFileViewer.Model.TreeItems
{
    public interface IItemErrors : IEnumerable<string>
    {
        event ErrorsChangedHandler? ErrorsChanged;

        public int Count { get; }

        void Add(string message);

        void Add(string category, string message);

        int RemoveAll(string category);
    }

    public delegate void ErrorsChangedHandler(object sender, ErrorsChangedHandlerArgs args);

    public class ErrorsChangedHandlerArgs
    {
    }
}