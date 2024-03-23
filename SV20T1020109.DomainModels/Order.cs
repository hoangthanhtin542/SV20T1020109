using SV20T1020109.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020109.DomainModels
{
    public class Orders
    {
        public int OrderID { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? AcceptTime { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerContactName { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public string DeliveryProvince { get; set; } = "";
        public string DeliveryAddress { get; set; } = "";
        public int? EmployeeID { get; set; }
        public string EmployeeName { get; set; } = "";
        public int? ShipperID { get; set; }
        public string ShipperName { get; set; } = "";
        public string ShipperPhone { get; set; } = "";
        public DateTime? ShippedTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public int Status { get; set; }
        /// <summary> 
        /// Mô tả trạng thái đơn hàng dựa trên mã trạng thái     /// </summary> 

        public string StatusDescription
        {
            get
            {
                switch (Status)
                {
                    case OrderStatus.INIT:
                        return "Đơn hàng mới. Đang chờ duyệt";
                    case OrderStatus.ACCEPTED:
                        return "Đơn đã chấp nhận. Đang chờ chuyển hàng";
                    case OrderStatus.SHIPPING:
                        return "Đơn hàng đang được giao";
                    case OrderStatus.FINISHED:
                        return "Đơn hàng đã hoàn tất";
                    case OrderStatus.CANCEL:
                        return "Đơn hàng đã bị hủy";
                    case OrderStatus.REJECTED:
                        return "Đơn hàng bị từ chối";
                    default:
                        return "";
                }
            }
        }


    }
}
