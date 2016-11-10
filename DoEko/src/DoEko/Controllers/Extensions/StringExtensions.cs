using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.Controllers.Extensions
{
    public static class StringExtensions
    {
        public static string GetNumbers(this string text)
        {
            text = text ?? string.Empty;
            return new string(text.Where(p => char.IsDigit(p)).ToArray());
        }

        public static string AsPhoneNumber (this string text)
        {
            string[] CellOperatorPrefixes = { "50", "51", "53", "57", "60", "66", "69", "72", "73", "78", "79", "88" };

            try
            {
                string phoneNumber = text.GetNumbers();
                if (phoneNumber.Length < 9)
                {
                    throw new FormatException("String is less than 9 digits");
                }

                if (Array.BinarySearch(CellOperatorPrefixes,phoneNumber.Substring(0,2)) != 0)
                {
                    //return String.Format("{0:+48 ### ### ###}", phoneNumber);
                    return "+48 " + phoneNumber.Substring(0, 3) + " " + phoneNumber.Substring(3, 3) + " " + phoneNumber.Substring(6, 3);
                }
                else
                {
                    //return String.Format("{0:+48 ## ### ## ##}", phoneNumber);
                    return "+48 " + phoneNumber.Substring(0, 2) + " " + phoneNumber.Substring(2, 3) + " " + phoneNumber.Substring(5, 2) + " " + phoneNumber.Substring(7,2);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
