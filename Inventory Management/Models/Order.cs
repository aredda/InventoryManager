using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management.Models
{
    [Serializable]
    public class Order
    {
        private int number;
        private OrderDirection direction;
        private string client;
        private DateTime date;
        private List<OrderDetail> details;

        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        public OrderDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public string Client
        {
            get { return client; }
            set { client = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public List<OrderDetail> Details
        {
            get { return details; }
            set { details = value; }
        }

        // Additional supportive getters
        public string DirectionString { 
            get {
                return Direction == OrderDirection.Purchase ? "اشتراء" : "بيع";
            } 
        }
        public string ShortDate
        {
            get { return Date.ToShortDateString(); }
        }
        

        public Order()
        {
            this.date = DateTime.Today;

            this.details = new List<OrderDetail>();
        }
        public Order(int number, OrderDirection direction, string client, DateTime date)
            : this()
        {
            this.number = number;
            this.direction = direction;
            this.client = client;
            this.date = date;
        }

        public double getTotal()
        {
            if (details.Count == 0)
                return 0;

            // Get the sum
            return details.Sum(d => d.Total);
        }
    }
}
