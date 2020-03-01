using Inventory_Management.Models;
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
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow 
        : Window, IDependent
    {
        private Database database;
        private Product product;

        public ProductWindow()
        {
            InitializeComponent();

            this.Loaded += ProductWindow_Loaded;
        }

        public ProductWindow(Product toEdit)
            : this()
        {
            this.product = toEdit;
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
        { }

        void ProductWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (database == null)
                    throw new Exception(Message.Exceptions.DATABASE_NOT_FOUND);

                // Disable resizing
                this.ResizeMode = System.Windows.ResizeMode.NoResize;

                // Edit mode or Insert mode
                if (product != null)
                {
                    // if Edit mode, disable code and quantity textbox
                    txt_code.IsEnabled =
                    txt_quantity.IsEnabled = false;

                    // Fill zones
                    txt_code.Text = product.Code;
                    txt_name.Text = product.Label;
                    txt_price_in.Text = product.PriceIn.ToString();
                    txt_price_out.Text = product.PriceOut.ToString();
                    txt_quantity.Text = product.Quantity.ToString();
                }

                // Numeric input configuration
                txt_price_in.PreviewTextInput += Binder.allow_numeric_only;
                txt_price_out.PreviewTextInput += Binder.allow_numeric_only;
                txt_quantity.PreviewTextInput += Binder.allow_numeric_only;

                // Product's code verification
                txt_code.PreviewKeyUp += txt_code_PreviewKeyUp;

                // Load categories
                Binder.bind(cb_category, database.categories, "Name", "Id");

                // Tweaks
                if (cb_category.Items.Count > 0)
                    cb_category.SelectedItem = product == null ? cb_category.Items[0] : product.Category;

                txt_subheader.Text = product == null ? "اضافة المنتوج" : "تعديل المنتوج";

                btn_add.BtnText = product == null ? "اضافة" : "تعديل";

                // Adding button
                btn_add.MouseDown += btn_add_MouseDown;
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void txt_code_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                bool checkError = txt_code.Text.Length == 0 || database.find(new Product(txt_code.Text)) != null;

                btn_add.IsEnabled = !checkError;
                txt_code.Background = checkError ? Brushes.Tomato : Brushes.LightGreen;
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txt_name.Text.Length == 0 || txt_code.Text.Length == 0)
                    throw new Exception(Message.Exceptions.FORM_INCOMPLETE);

                // Insert mode
                if (product == null)
                {
                    this.product = new Product();

                    this.product.Code = txt_code.Text.ToLower();
                    this.product.Label = txt_name.Text;
                    this.product.PriceIn = double.Parse(txt_price_in.Text);
                    this.product.PriceOut = double.Parse(txt_price_out.Text);
                    this.product.Quantity = int.Parse(txt_quantity.Text);
                    this.product.Category = (Category)cb_category.SelectedItem;

                    // Add this product
                    this.database.add(this.product);

                    // Create a new sell object
                    Order order = new Order();
                    order.Direction = OrderDirection.Purchase;
                    order.Date = DateTime.Now;

                    // Associate sell detail
                    this.database.add(new OrderDetail(order, product, product.PriceIn, product.Quantity, 0));

                    // Add sell
                    this.database.add(order);

                    // Reset model
                    this.product = null;

                    // Clear form
                    txt_code.Text =
                    txt_name.Text =
                    txt_price_in.Text =
                    txt_price_out.Text =
                    txt_quantity.Text = "";
                }
                // Edit mode
                else
                {
                    this.product.Label = txt_name.Text;
                    this.product.PriceIn = double.Parse(txt_price_in.Text);
                    this.product.PriceOut = double.Parse(txt_price_out.Text);
                    this.product.Category = (Category) cb_category.SelectedItem;
                }

                // Show success message
                Message.show(Message.SUCCESS_INSERT, MessageBoxImage.Information);
            }
            catch (FormatException)
            {
                Message.show(Message.Exceptions.FORMAT_INCORRECT, MessageBoxImage.Error);
            }
            catch (Exception x)
            {
                Message.show(x.Message, MessageBoxImage.Error);
            }
        }
    }
}
