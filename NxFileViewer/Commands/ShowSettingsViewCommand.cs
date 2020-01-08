using System;
using System.Windows;
using Emignatik.NxFileViewer.Views;

namespace Emignatik.NxFileViewer.Commands
{
    public class ShowSettingsViewCommand : CommandBase
    {
        private readonly SettingsWindowViewModel _settingsWindowViewModel;
        private SettingsWindow _settingsWindow;

        public ShowSettingsViewCommand(SettingsWindowViewModel settingsWindowViewModel)
        {
            _settingsWindowViewModel = settingsWindowViewModel ?? throw new ArgumentNullException(nameof(settingsWindowViewModel));

            _settingsWindowViewModel.OnQueryCloseView += OnQueryCloseView;
        }

        private void OnQueryCloseView(object sender, EventArgs e)
        {
            _settingsWindow?.Close();
        }

        public override void Execute(object parameter)
        {
            if (_settingsWindow != null)
            {
                _settingsWindow.Activate();
                return;
            }

            _settingsWindow = new SettingsWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = _settingsWindowViewModel
            };
            _settingsWindow.Show();

            _settingsWindow.Closed += OnSettingsWindowClosed;
        }

        private void OnSettingsWindowClosed(object sender, EventArgs e)
        {
            _settingsWindow = null;
            TriggerCanExecuteChanged();
        }
    }
}