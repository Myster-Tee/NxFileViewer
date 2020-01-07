using System;
using System.Windows;
using Emignatik.NxFileViewer.Views;

namespace Emignatik.NxFileViewer.Commands
{
    public class ShowSettingsViewCommand : CommandBase
    {
        private readonly SettingWindowViewModel _settingWindowViewModel;

        public ShowSettingsViewCommand(SettingWindowViewModel settingWindowViewModel)
        {
            _settingWindowViewModel = settingWindowViewModel ?? throw new ArgumentNullException(nameof(settingWindowViewModel));
        }

        public override void Execute(object parameter)
        {
            var settingsWindow = new SettingsWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = _settingWindowViewModel
            };
            settingsWindow.ShowDialog();
        }
    }
}