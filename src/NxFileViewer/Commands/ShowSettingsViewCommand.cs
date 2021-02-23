using System;
using System.Windows;
using System.Windows.Input;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;
using Emignatik.NxFileViewer.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Emignatik.NxFileViewer.Commands
{
    public class ShowSettingsViewCommand : CommandBase, IShowSettingsViewCommand
    {
        private readonly IServiceProvider _serviceProvider;
        private SettingsWindow? _settingsWindow;

        public ShowSettingsViewCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        private void OnQueryCloseView(object? sender, EventArgs e)
        {
            _settingsWindow?.Close();
        }

        public override void Execute(object? parameter)
        {
            if (_settingsWindow != null)
            {
                _settingsWindow.Activate();
                return;
            }

            var settingsWindowViewModel = _serviceProvider.GetRequiredService<SettingsWindowViewModel>();

            _settingsWindow = new SettingsWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = settingsWindowViewModel
            };
            settingsWindowViewModel.OnQueryCloseView += OnQueryCloseView;

            _settingsWindow.Show();
            _settingsWindow.Closed += OnSettingsWindowClosed;
        }

        private void OnSettingsWindowClosed(object? sender, EventArgs e)
        {
            _settingsWindow = null;
            TriggerCanExecuteChanged();
        }
    }

    public interface IShowSettingsViewCommand : ICommand
    {
    }
}