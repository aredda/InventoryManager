using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Inventory_Management.Utilities
{
    public static class Message
    {
        public const string PROMPT_PRINT = "هل تريد طباعة الفاتورة؟ عند اختيار لا, لا يمكنك طباعتها لاحقا";
        public const string ORDER_ASC = "من الأصغر الى الأكبر";
        public const string SUCCESS_INSERT = "تمت الاضافة بنجاح!";

        public static MessageBoxResult show(string message, MessageBoxImage icon = MessageBoxImage.Error, string title = "انتباه")
        {
            return MessageBox.Show(message, title, MessageBoxButton.OK, icon, MessageBoxResult.OK, MessageBoxOptions.RightAlign);
        }

        public static bool confirm(string message)
        {
            return MessageBox.Show(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static bool match_key(string key)
        {
            string path = "copy.ak";

            // Copy the file
            File.WriteAllBytes(path, Properties.Resources.keys);

            // Open the file
            StreamReader reader = new StreamReader(path);

            // Result
            bool result = false;

            int day = 1;
            while (!reader.EndOfStream)
            {
                string fileKey = reader.ReadLine();
                
                if (DateTime.Today.Day == day)
                    if (fileKey.Trim().CompareTo(key) == 0)
                    {
                        result = true;

                        break;
                    }

                day++;
            }

            // Close
            reader.Close();

            // Dispose of the copy
            File.Delete(path);

            return result;
        }

        public static class Exceptions
        {
            public const string DATABASE_NOT_FOUND = "لم يتم العثور على أي قاعدة بيانات";
            public const string FORM_INCOMPLETE = "المرجو ادخال جميع المعلومات";
            public const string FORMAT_INCORRECT = "المرجو ادخال أرقام صحيحة";
            public const string SELECTED_NONE = "المرجو التأكد من اختيارك";
        }
    }
}
