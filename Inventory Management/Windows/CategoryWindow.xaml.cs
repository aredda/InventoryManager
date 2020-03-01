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
using Inventory_Management.Utilities;

namespace Inventory_Management.Windows
{
    /// <summary>
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow 
        : Window, IDependent
    {
        private Database database;

        public CategoryWindow()
        {
            InitializeComponent();

            this.Loaded += CategoryWindow_Loaded;
        }
        public CategoryWindow(Database database)
            : this()
        {
            this.database = database;
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

        // Load event
        void CategoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (database == null)
                    throw new Exception(Message.Exceptions.DATABASE_NOT_FOUND);

                // Disable resizing
                this.ResizeMode = System.Windows.ResizeMode.NoResize;

                Binder.bind(dg_categories, database.categories);

                // Configure buttons
                btn_add.MouseDown += btn_add_MouseDown;
                btn_delete.MouseDown += btn_delete_MouseDown;
            }
            catch (Exception x)
            {
                Message.show(x.Message, MessageBoxImage.Error);
            }
        }

        void btn_add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string name = txt_name.Text;
                string description = new TextRange(txt_description.Document.ContentStart, txt_description.Document.ContentEnd).Text;

                if (name.Length == 0)
                    throw new Exception(Message.Exceptions.FORM_INCOMPLETE);

                this.database.add(new Models.Category(0, name, description));

                Binder.bind(dg_categories, database.categories);

                txt_name.Text = null;
            }
            catch (Exception x)
            {
                Message.show(x.Message, MessageBoxImage.Error);
            }
        }

        void btn_delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dg_categories.SelectedItem == null)
                    throw new Exception("Please select a line!");

                this.database.remove((Models.Category) dg_categories.SelectedItem);

                Binder.bind(dg_categories, database.categories);
            }
            catch (Exception x)
            {
                Message.show(x.Message, MessageBoxImage.Error);
            }
        }

    }
}
