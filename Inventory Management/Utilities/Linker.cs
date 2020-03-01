using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Inventory_Management.Utilities
{
    public static class Linker
    {
        private static List<IDependent> windows;

        public static void configure(IDependent window)
        {
            // Center the window
            Window actualWindow = (Window)window;

            actualWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            actualWindow.WindowState = WindowState.Maximized;
        }

        public static void run(IDependent window, bool imposeSettings = true)
        {
            if (windows == null)
                windows = new List<IDependent>();

            if (windows.Count > 0)
            {
                // Hide the previous window
                ((Window)windows.Last()).Hide();
                // Pass down the database
                window.setDatabase(windows.Last().getDatabase());
            }

            // Configure closing event
            ((Window) window).Closed += delegate(object sender, EventArgs args) 
            {
                Linker.close(window, true);
            };

            // Configure
            if (imposeSettings)
                configure(window);

            // Then add this window
            windows.Add(window);

            // Show that window
            ((Window)window).Show();
        }

        public static void close(IDependent window, bool refreshPrevious = false)
        {
            // If the application is not activated, destroy application
            if (!window.getDatabase().settings.isActivated)
            {
                Message.show("البرنامج غير مفعل ,لذى سيتم اغلاق البرنامج", MessageBoxImage.Warning);

                Environment.Exit(0);
            }

            // Remove the current window
            if (windows != null)
                if (windows.Contains(window))
                    windows.Remove(window);

            // If this is the last window to be closed, save database
            if (windows.Count == 0)
                Loader.save<Database>(Loader.getDataFilePath(), window.getDatabase());

            // Destroy it
            ((Window)window).Close();

            // Show the last window
            if (windows.Count > 0)
            {
                // Configure before showing
                configure(windows.Last());
                // Show
                ((Window) windows.Last()).Show();
                // Refresh if required
                if (refreshPrevious) windows.Last().refresh();
            }
        }

        public static IDependent opened()
        {
            return windows.Last();
        }

        // If the windows list is empty then the application is not running
        public static bool isRunning()
        {
            bool notNull = windows != null;
            
            return notNull && windows.Count > 0;
        }
    }
}
