using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management.Models
{
    [Serializable]
    public class Settings
    {
        // Application settings
        public bool isConfigured = false;
        public bool isActivated = false;

        // Login settings
        public string username;
        public string password;

        // Company info
        public string companyName = "تجهيزات بلال المكتبية";
        public string address = "عنوان الشركة";
        public string email = "البريد الالكتروني";
        public string telephone = "هاتف مكتب الشركة";

        // Raise an alert notification when a product's quantity becomes smaller than this value
        public int notifyOn = 10;
    }
}
