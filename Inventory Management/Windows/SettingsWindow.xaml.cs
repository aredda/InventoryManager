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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow 
        : Window, IDependent
    {
        private Database database;

        public SettingsWindow()
        {
            InitializeComponent();

            this.Loaded += SettingsWindow_Loaded;
        }

        bool isEmpty(Control textBox)
        {
            if (textBox is TextBox)
                return ((TextBox) textBox).Text.Length == 0;

            return ((PasswordBox)textBox).Password.Length == 0;
        }

        void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ResizeMode = System.Windows.ResizeMode.NoResize;

                // Configure events
                btn_save.MouseDown += btn_save_MouseDown;
                txt_notify.PreviewTextInput += Binder.allow_numeric_only;
                txt_serial.PasswordChanged += txt_serial_PasswordChanged;

                // Load data
                txt_name.Text = database.settings.companyName;
                txt_address.Text = database.settings.address;
                txt_email.Text = database.settings.email;
                txt_telephone.Text = database.settings.telephone;
                txt_notify.Text = database.settings.notifyOn.ToString();

                // Disable button if app isn't activated
                btn_save.IsEnabled =
                    hf_header.btn_notify.IsEnabled = database.settings.isActivated;

                // Disable activation textbox if the app is activated
                txt_serial.IsEnabled = !database.settings.isActivated;
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        // Activation key
        void txt_serial_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string serial = txt_serial.Password;

            // State color
            Brush color = Brushes.LightGreen;

            // Activation test result
            bool activated = true;

            try
            {
                if (serial.Length == 0 || !Message.match_key(serial))
                    throw new Exception();
            }
            catch (Exception)
            {
                color = Brushes.Tomato;
                activated = false;
            }

            txt_serial.Background = color;
            btn_save.IsEnabled = activated;
        }

        void btn_save_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (isEmpty(txt_name)
                    || isEmpty(txt_address)
                    || isEmpty(txt_email)
                    || isEmpty(txt_telephone)
                    || isEmpty(txt_notify)
                    || isEmpty(txt_username)
                    || isEmpty(txt_password)
                    || isEmpty(txt_confirmation))
                    throw new Exception(Message.Exceptions.FORM_INCOMPLETE);

                if (txt_password.Password.CompareTo(txt_confirmation.Password) != 0)
                    throw new Exception("كلمات المرور لا تتشابه, المرجو التأكد");

                database.settings.companyName = txt_name.Text;
                database.settings.address = txt_address.Text;
                database.settings.email = txt_email.Text;
                database.settings.telephone = txt_telephone.Text;
                database.settings.notifyOn = int.Parse(txt_notify.Text);
                database.settings.username = txt_username.Text;
                database.settings.password = txt_password.Password;

                Message.show("تم حفظ الاعدادات بنجاح", MessageBoxImage.Information);

                // Activate application
                database.settings.isConfigured =
                    database.settings.isActivated = true;
            }
            catch (FormatException)
            {
                Message.show(Message.Exceptions.FORMAT_INCORRECT);
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
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
        public void refresh()
        {}

    }
}
