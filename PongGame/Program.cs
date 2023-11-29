using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System;

namespace PongGame
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing PongGame...");

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            var builder = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont();

            // Register your MainWindow
            builder.AfterSetup(_ =>
            {
                if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.MainWindow = new MainWindow();
                }
            });

            return builder;
        }
    }
}
