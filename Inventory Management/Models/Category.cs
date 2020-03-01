using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management.Models
{
    [Serializable]
    public class Category
    {
        private int number;
        private string name;
        private string description;
        private List<Product> products;

        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public List<Product> Products
        {
            get { return products; }
            set { products = value; }
        }

        public Category()
        {
            this.products = new List<Product>();
        }
        public Category(int number)
            : this ()
        {
            this.number = number;
        }
        public Category(int number, string name, string description)
            : this(number)
        {
            this.name = name;
            this.description = description;
        }
    }
}
