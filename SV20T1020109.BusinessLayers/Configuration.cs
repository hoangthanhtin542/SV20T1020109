using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020109.BusinessLayers
{
    public static class Configuration
    {
        /// <summary>
        /// chuỗi thông số kết nối đến CSDL
        /// </summary>
        public static String ConnectionString { get; private set; } = "";
        public static void Initialize(string connectionString)
        {
            Configuration.ConnectionString = connectionString;
        }
    }
}
//static class là gì? khách gì class thông thuờg chỗ nào
