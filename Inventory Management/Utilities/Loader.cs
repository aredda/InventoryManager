using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventory_Management.Utilities
{
    public static class Loader
    {
        public const string dataFileName = "db.bin";

        // Get full path
        public static string getDataFilePath()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory + dataFileName;
        }

        // Save data
        public static void save<T> (string path, T data)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
                new BinaryFormatter().Serialize (stream, data);
        }

        // Load data
        public static T load<T> (string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            T data = default(T);

            using (FileStream stream = new FileStream(path, FileMode.Open))
                data = (T) new BinaryFormatter().Deserialize(stream);

            return data;
        }
    }
}
