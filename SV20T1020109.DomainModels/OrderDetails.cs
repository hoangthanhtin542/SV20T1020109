using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020109.DomainModels
{
    /// <summary>
    /// Thông tin chi tiết mặt hàng bán trong đơn hàng
    /// </summary>
    public class OrderDetails
    {

        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        public int OrderID { get; set; }
        /// <summary>
        /// Mã mặt hàng
        /// </summary>
        public int ProductID { get; set; }
        /// <summary>
        /// Số lượng bán
        /// </summary>
        public int Quantity { get; set; } = 0;
        /// <summary>
        /// Giá bán
        /// </summary>
        public decimal SalePrice { get; set; } = 0;
        /// <summary>
        /// Thành tiền = Số lượng * Giá bán
        /// </summary>
        public decimal TotalPrice
        {
            get
            {
                return Quantity * SalePrice;
            }
        }

    }
}
