using Inventory_Management.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Inventory_Management.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow 
        : Window, IDependent
    {
        private Database database;

        public LoginWindow()
        {
            InitializeComponent();

            // Center it
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            this.Loaded += LoginWindow_Loaded;
        }

        // Defining IDependent methods
        public void setDatabase(Database database)
        {
            this.database = database;
        }
        public Database getDatabase()
        {
            return this.database;
        }
        public void refresh() { }

        void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                btn_signin.MouseDown += btn_signin_MouseDown;

                // No resize
                this.ResizeMode = System.Windows.ResizeMode.NoResize;

                // Destroy hacking entries
                hf_header.btn_settings.IsEnabled =
                    hf_header.btn_notify.IsEnabled = false;
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_signin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string username = txt_username.Text;
                string password = txt_password.Password;

                if (username.Length == 0 || password.Length == 0)
                    throw new Exception(Message.Exceptions.FORM_INCOMPLETE);

                if (username.CompareTo(database.settings.username) != 0 || password.CompareTo(database.settings.password) != 0)
                    throw new Exception("معلومات الدخول ليست صحيحة");

                // Reset fields
                txt_username.Text =
                txt_password.Password = null;

                Linker.run(new MainWindow());
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }
    }
}
