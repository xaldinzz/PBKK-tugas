
using System;
using System.Windows;
using System.Windows.Threading;

namespace toybox
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += (sender, e) =>
            {
                MessageBox.Show(
                    e.Exception.ToString(),
                    "Error TOYBOX",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                e.Handled = true;
            };
        }
    }
}
