using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management.Models
{
    [Serializable]
    public class OrderDetail
    {
        private Order order;
        private Product product;
        private double price;
        private int quantity;
        private double discount;

        public Order Order
        {
            get { return this.order; }
            set 
            {
                if (this.order != null)
                    this.order.Details.Remove(this);
                
                this.order = value;
                this.order.Details.Add(this);
            }
        }
        public Product Product
        {
            get { return product; }
            set { product = value; }
        }
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }
        public double Total
        {
            get 
            {   
                return (Price - (Price * (discount / 100))) * quantity;
            }
        }

        public OrderDetail()
        {}
        public OrderDetail(Order order, Product product, double price, int quantity, double discount)
        {
            this.Order = order;
            this.product = product;
            this.price = price;
            this.quantity = quantity;
            this.discount = discount;
        }

        // To use in printing
        public object[] objectArray()
        {
            return new object[] { Product.Code, Product.Label, (Price - (Price * discount / 100)) + " DH", Quantity, Total + " DH" };
        }
    }
}
