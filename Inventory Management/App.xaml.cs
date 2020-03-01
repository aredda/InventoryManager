using Inventory_Management.Utilities;
using Inventory_Management.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Inventory_Management
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
        : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Database
            Database database = null;

            try
            {
                // Try to load data from the given file path
                database = Loader.load<Database> (Loader.getDataFilePath());
            }
            catch (FileNotFoundException)
            {
                // Create a new database
                database = new Database();
            }

            // Entry window
            IDependent entry = new MainWindow();

            // Attach the database
            entry.setDatabase(database);

            // Run the window
            Linker.run(entry);

            // If this is the first time, impose configuring the system
            if (!database.settings.isConfigured || !database.settings.isActivated)
            {
                Message.show("مرحبا بك, يتبين أن هذا أول استعمال لك للبرنامج, المرجو تفعيل البرنامج و تحديد اعدادات النظام", MessageBoxImage.Information);

                Linker.run(new SettingsWindow());
            }
                
        }
    }
}
