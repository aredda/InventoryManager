using Inventory_Management.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Inventory_Management.Windows.Fragments
{
    /// <summary>
    /// Interaction logic for HeaderFragment.xaml
    /// </summary>
    public partial class HeaderFragment 
        : UserControl
    {
        private ImageSource isrc_notification;

        public HeaderFragment()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Loaded += HeaderFragment_Loaded;
        }

        void HeaderFragment_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Linker.isRunning())
                    return;

                // Configure buttons
                btn_notify.MouseDown += btn_notify_MouseDown;
                btn_settings.MouseDown += btn_settings_MouseDown;
                btn_about.MouseDown += btn_about_MouseDown;

                // Initital settings
                isrc_notification = btn_notify.Source;

                // Observing changes
                DispatcherTimer observerTimer = new DispatcherTimer();
                observerTimer.Interval = new TimeSpan(TimeSpan.TicksPerSecond / 2);
                observerTimer.Tick += delegate(object s, EventArgs args)
                {
                    Database d = Linker.opened().getDatabase();

                    // Company name observing
                    txt_name.Text = d.settings.companyName;

                    // Notification observer
                    if (d.products.Where(p => p.Quantity <= d.settings.notifyOn).Count() > 0)
                        btn_notify.Visibility = btn_notify.Visibility != Visibility.Hidden ? Visibility.Hidden : Visibility.Visible;
                    else
                        btn_notify.Visibility = Visibility.Hidden;
                };
                observerTimer.Start();
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_about_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new CreditsWindow().ShowDialog();
        }

        void btn_notify_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Linker.isRunning())
                    if (!(Linker.opened() is NotificationWindow))
                        Linker.run(new NotificationWindow());
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Linker.isRunning())
                    if (!(Linker.opened() is SettingsWindow))
                        Linker.run(new SettingsWindow());
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }
    }
}
