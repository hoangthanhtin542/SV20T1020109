using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020109.DomainModels
{
    /// <summary>
    /// thông tin khách hàng
    /// </summary>

    public class Customer
    {
        public int CustomerID { get; set; }
        public String CustomerName { get; set; } = "";
        public String ContactName { get; set; } = "";
        public String Province { get; set; } = "";
        public String Address { get; set; } = "";
        public String Phone { get; set; } = "";
        public String Email { get; set; } = "";
        public bool IsLocked { get; set; }



    }
}
