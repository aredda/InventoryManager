using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory_Management.Models;
using System.Data;

namespace Inventory_Management.Utilities
{
    [Serializable]
    public class Database
    {
        public Settings settings;

        public List<Category> categories;
        public List<Product> products;
        public List<Order> orders;
        public List<OrderDetail> orderDetails;

        public Database()
        {
            this.settings = new Settings();

            this.categories = new List<Category>();
            this.products = new List<Product>();
            this.orders = new List<Order>();
            this.orderDetails = new List<OrderDetail>();
        }

        // Get collection fields
        public List<FieldInfo> get_collections()
        {
            List<FieldInfo> fields = new List<FieldInfo>();

            foreach (FieldInfo f in this.GetType().GetFields())
                if (f.FieldType.IsGenericType)
                    fields.Add(f);

            return fields;
        }

        // Get collection's generic type
        public Type get_collection_type(FieldInfo collectionField)
        {
            if (!collectionField.FieldType.IsGenericType)
                throw new Exception("This field is not a generic collection!");

            return collectionField.FieldType.GenericTypeArguments.First();
        }

        // Searching for the required collection
        public FieldInfo search_collection(Type type)
        {
            foreach (FieldInfo f in get_collections())
                if (f.FieldType.GenericTypeArguments.Contains(type))
                    return f;

            return null;
        }

        // Adding item to its corresponding collection
        public void add <T> (T item)
        {
            // Search for the targeted collection
            FieldInfo field = search_collection(typeof(T));

            if (field == null)
                throw new Exception(typeof(T).Name + " doesn't have any container collection!");

            // Cast
            List<T> collection = (List<T>) field.GetValue(this);
            // Add
            collection.Add(item);
        }

        // Removing an item
        public void remove <T> (T item)
        {
            // Search for the targeted collection
            FieldInfo field = search_collection(typeof(T));

            if (field == null)
                throw new Exception(typeof(T).Name + " doesn't have any container collection!");

            // Cast
            List<T> collection = (List<T>)field.GetValue(this);
            // Add
            collection.Remove(item);
        }

        // Finding a record
        public T find<T> (T criteria)
        {
            // Search for the targeted collection
            FieldInfo field = search_collection(typeof(T));

            if (field == null)
                throw new Exception(typeof(T).Name + " doesn't have any container collection!");

            // Cast
            List<T> collection = (List<T>)field.GetValue(this);

            foreach (T item in collection)
                if (item.Equals(criteria))
                    return item;

            return default(T);
        }

        // Generating a dataset in order to use it in reporting
        public static DataSet getDataSet(Database database)
        {
            DataSet set = new DataSet();

            foreach (FieldInfo collectionField in database.get_collections())
            {
                // Get the collection's generic type
                Type collectionType = database.get_collection_type(collectionField);
                
                // Create a new table and append it to the dataset
                DataTable table = new DataTable(collectionField.Name);

                foreach (PropertyInfo field in collectionType.GetProperties())
                    table.Columns.Add(field.Name, field.PropertyType);

                set.Tables.Add(table);
            }

            return set;
        }
    }
}
