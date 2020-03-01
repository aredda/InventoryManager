using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management.Utilities
{
    public interface IDependent
    {
        // Database setter
        void setDatabase(Database database);

        // Database getter
        Database getDatabase();

        // Notifier
        void refresh();
    }
}
