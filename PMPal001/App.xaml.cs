using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace PMPal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var bootStrapper = new Bootstrapper();
            bootStrapper.StartupApp(e);

        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            System.IO.File.AppendAllText($"{Util.GetLogFolder()}\\PMPal-params.txt", $"\nApplication exiting from {sender.ToString()} - {DateTime.Now}");
            Bootstrapper.ReleaseMutex();
        }

    }
   
    public class DebugTraceListener : TraceListener
    {
        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
            //Debugger.Break();
        }
    }

    
}
