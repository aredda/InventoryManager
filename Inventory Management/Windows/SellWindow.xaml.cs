using Inventory_Management.Models;
using Inventory_Management.Utilities;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for SellWindow.xaml
    /// </summary>
    public partial class SellWindow 
        : System.Windows.Window, IDependent
    {
        private Database database;
        private Order order;

        public SellWindow()
        {
            InitializeComponent();

            this.Loaded += SellWindow_Loaded;
        }
        public SellWindow(Database database)
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
        {
            // Initialize model
            this.order = new Order();

            // Refresh grids 
            Binder.bind(dg_inventory, database.products);
            Binder.bind(dg_command, order.Details);

            // Initial tweaks
            btn_sell.IsEnabled = false;

            // Reset textboxes
            txt_discount.Text = "0";
            txt_client.Text = txt_quantity.Text = null;

            // Refresh the total label
            lbl_total.Text = order.getTotal() + " درهم";
        }

        void SellWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Configure events
                txt_code.KeyUp += txt_code_KeyUp;
                txt_discount.PreviewTextInput += Binder.allow_numeric_only;
                txt_quantity.PreviewTextInput += Binder.allow_numeric_only;
                btn_add.MouseDown += btn_add_MouseDown;
                btn_sell.MouseDown += btn_sell_MouseDown;
                btn_cancel.MouseDown += btn_cancel_MouseDown;

                Dictionary<DataGrid, Control> association = new Dictionary<DataGrid, Control>();
                association.Add(dg_inventory, btn_add);
                association.Add(dg_command, btn_cancel);

                foreach (KeyValuePair<DataGrid, Control> row in association)
                {
                    row.Value.MouseDown += delegate(object s, MouseButtonEventArgs se)
                    {
                        // Refresh the total label
                        lbl_total.Text = order.getTotal() + " درهم";

                        // Activate selling button if only there are products in the cart
                        btn_sell.IsEnabled = order.Details.Count > 0;
                    };
                }

                // Initialize
                refresh();
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dg_command.SelectedItem == null)
                    throw new Exception(Message.Exceptions.SELECTED_NONE);

                OrderDetail detail = dg_command.SelectedItem as OrderDetail;

                // Remove this detail from records
                order.Details.Remove(detail);
                database.remove(detail);

                // Refresh
                Binder.bind(dg_command, order.Details);
                Binder.bind(dg_inventory, database.products);

                // Lose focus
                dg_command.RaiseEvent(new RoutedEventArgs(DataGrid.LostFocusEvent, dg_command));
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_sell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (order.Details.Count == 0 || txt_client.Text.Length == 0)
                    throw new Exception(Message.Exceptions.FORM_INCOMPLETE);

                // Detail registering
                foreach (OrderDetail detail in order.Details)
                {
                    // Substract the quantity
                    detail.Product.Quantity -= detail.Quantity;   
                    // Add to the database
                    this.database.add(detail);
                }

                // Final setup
                order.Direction = OrderDirection.Sell;
                order.Client = txt_client.Text;
                order.Date = DateTime.Now;

                // Register
                database.add(order);

                // Ask to print
                Message.show("ستتم طباعة الفاتورة بعد قليل", MessageBoxImage.Information);

                // Convert the resource byte array to a file
                File.WriteAllBytes("facture.xlsx", Properties.Resources.BillTemplate);

                string path;

                // Open the file just to get the full path
                FileStream stream = new FileStream("facture.xlsx", FileMode.Open);
                path = stream.Name;
                stream.Close();

                // Create an excel application
                _Application app = new Microsoft.Office.Interop.Excel.Application();
                    
                // Open the document
                _Workbook doc = app.Workbooks.Open(path);

                // Get the active page
                _Worksheet pg = doc.ActiveSheet;

                // Define the static params
                Range
                    company = pg.Cells.Find("CompanyName"),
                    client = pg.Cells.Find("Client"),
                    date = pg.Cells.Find("Date"),
                    code = pg.Cells.Find("Code"),
                    detailTotal = pg.Cells.Find("DetailTotal"),
                    orderTotal = pg.Cells.Find("OrderTotal"),
                    address = pg.Cells.Find("Address"),
                    contact = pg.Cells.Find("ContactInfo");

                // Update the template's static info
                client.Value = order.Client;
                date.Value = order.ShortDate;
                orderTotal.Value = order.getTotal() + " DH";
                company.Value = database.settings.companyName;
                address.Value = database.settings.address;
                contact.Value = database.settings.email + " - " + database.settings.telephone;

                // Print out the details
                for (int y = 0; y < order.Details.Count; y++)
                    for (int x = code.Column, i = 0; x >= detailTotal.Column; x--, i++)
                    {
                        pg.Cells[y + code.Row, x].Value = order.Details[y].objectArray()[i];
                            
                        // Borders
                        XlBordersIndex[] borders = 
                        { 
                            XlBordersIndex.xlEdgeTop, 
                            XlBordersIndex.xlEdgeBottom,                            
                            XlBordersIndex.xlEdgeRight,
                            XlBordersIndex.xlEdgeLeft 
                        };

                        foreach (XlBordersIndex border in borders)
                            pg.Cells[y + code.Row, x].Borders[border].LineStyle = XlLineStyle.xlContinuous;   
                    }

                // Show the application
                app.Visible = true;

                // Show preview
                doc.PrintPreview();

                // Reset
                refresh();
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
                if (dg_inventory.SelectedItem == null)
                    throw new Exception(Message.Exceptions.SELECTED_NONE);

                OrderDetail detail = new OrderDetail();

                detail.Product = (Product) dg_inventory.SelectedItem;
                detail.Price = detail.Product.PriceOut;
                detail.Quantity = int.Parse(txt_quantity.Text);
                detail.Discount = double.Parse(txt_discount.Text);

                if (detail.Discount > 100)
                    throw new Exception("لا يمكن للتخفيض أن يكون أكبر من مئة أو أصغر من صفر");

                if (detail.Quantity == 0)
                    throw new Exception("لا يمكن لعدد الوحدات أن يكون صفر");

                if (detail.Quantity > detail.Product.Quantity)
                    throw new Exception("لا يمكن لعدد الوحدات المراد بيعها أكبر من عدد الوحدات المتواجدة في متجرك");

                if (order.Details.Exists(d => d.Product.Equals(detail.Product)))
                    throw new Exception("لقد سبق و أن أضفت هذا المنتوج الى المنتوجات المراد بيعها");

                detail.Order = order;

                // Deactivate add button
                dg_inventory.RaiseEvent(new RoutedEventArgs(DataGrid.LostFocusEvent, dg_inventory));

                // Refresh grids
                Binder.bind(dg_inventory, database.products);
                Binder.bind(dg_command, order.Details);
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

        void txt_code_KeyUp(object sender, KeyEventArgs e)
        {
            string code = txt_code.Text;

            // Search for matching results
            List<Product> products = this.database.products.Where(p => p.Code.Contains(code)).ToList ();

            if (code.Length == 0)
                products = this.database.products;

            // Bind
            Binder.bind(dg_inventory, products);
        }
    }
}
