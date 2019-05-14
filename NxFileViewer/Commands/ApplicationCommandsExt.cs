using System.Windows.Input;

namespace Emignatik.NxFileViewer.Commands
{
    public static class ApplicationCommandsExt
    {
        static ApplicationCommandsExt()
        {
            OpenLast = new RoutedUICommand("Open last", "OpenLast", typeof(ApplicationCommandsExt));
        }

        public static RoutedUICommand OpenLast { get; }

    }
}
