using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management.Models
{
    [Serializable]
    public class Product
    {
        private string code;
        private string label;
        private double priceIn;
        private double priceOut;
        private int quantity;
        private string imagePath;
        private Category category;

        public string Code
        {
            get { return code; }
            set { code = value; }
        }
        public string Label
        {
            get { return label; }
            set { label = value; }
        }
        public double PriceIn
        {
            get { return priceIn; }
            set { priceIn = value; }
        }
        public double PriceOut
        {
            get { return priceOut; }
            set { priceOut = value; }
        }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        public Category Category
        {
            get { return this.category; }
            set
            {
                if (this.category != null)
                    this.category.Products.Remove(this);

                this.category = value;
                this.category.Products.Add(this);
            }
        }
        public string ImagePath
        {
            set { this.imagePath = value; }
            get { return this.imagePath; }
        }

        public Product()
        {}
        public Product(string code)
        {
            this.code = code;
        }
        public Product(string code, string label, double price_in, double price_out, int quantity, Category category)
            : this(code)
        {
            this.label = label;
            this.priceIn = price_in;
            this.priceOut = price_out;
            this.quantity = quantity;
            this.Category = category;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Product))
                return false;

            return ((Product) obj).Code.CompareTo(this.Code) == 0;
        }
    }
}
