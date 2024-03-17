using System.Globalization;

namespace SV20T1020109.Web
{
    public static class Converter
    {
        /// <summary>
        /// chuuyển chuổi s sang giá trị kiểu DateTime (nếu không chuyển thành công thì chuyển về giá trị null)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="formats"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);

            }
            catch
            {
                return null;
            }
        }
    }
    }
