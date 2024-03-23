using Microsoft.IdentityModel.Protocols;
using Microsoft.VisualBasic;
using SV20T1020109.BusinessLayers;
using SV20T1020109.DataLayers.SQLServer;
using SV20T1020109.DataLayers;
using SV20T1020109.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020109.BusinessLayers
{
    /// <summary>
    /// Các chức năng nghiệp vụ liên quan đến xử lý đơn hàng
    /// </summary>
    public static class OrderDataService
    {
        private static readonly IOrderDAL orderDB;     /// <summary> 
                                                       /// Ctor     /// </summary> 
        static OrderDataService()
        {
            orderDB = new OrderDAL(Configuration.ConnectionString);
        }
        /// <summary> 
        /// Tìm kiếm và lấy danh sách đơn hàng dưới dạng phân trang 
        /// </summary> 
        public static List<Orders> ListOrders(out int rowCount, int page = 1, int pageSize = 0,
                         int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            rowCount = orderDB.Count(status, fromTime, toTime, searchValue);
            return orderDB.List(page, pageSize, status, fromTime, toTime, searchValue).ToList();
        }
        /// <summary> 
        /// Lấy thông tin của 1 đơn hàng 
        /// </summary> 
        public static Orders? GetOrder(int orderID)
        {
            return orderDB.Get(orderID);
        }
        /// <summary> 
        /// Khởi tạo 1 đơn hàng mới (tạo đơn hàng mới ở trạng thái Init).  
        /// Hàm trả về mã của đơn hàng được tạo mới 
        /// </summary> 
        public static int InitOrder(int employeeID, int customerID,
                              string deliveryProvince, string deliveryAddress,
                              IEnumerable<OrderDetails> details)
        {
            if (details.Count() == 0) return 0;
            Orders data = new Orders()
            {
                EmployeeID = employeeID,
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress
            };
            int orderID = orderDB.Add(data); if (orderID > 0)
            {
                foreach (var item in details)
                {
                    orderDB.SaveDetail(orderID, item.ProductID, item.Quantity, item.SalePrice);
                }
                return orderID;
            }
            return 0;
        }
        /// <summary> 
        /// Hủy bỏ đơn hàng     /// </summary> 
        public static bool CancelOrder(int orderID)
        {
            Orders? data = orderDB.Get(orderID); if (data == null) return false;

            if (data.Status != ConstantsModel.ORDER_FINISHED)
            {
                data.Status = ConstantsModel.ORDER_CANCEL; data.FinishedTime = DateTime.Now; return orderDB.Update(data);
            }
            return false;
        }
        /// <summary> 
        /// Từ chối đơn hàng     /// </summary> 
        public static bool RejectOrder(int orderID)
        {
            Orders? data = orderDB.Get(orderID); if (data == null) return false;

            if (data.Status == ConstantsModel.ORDER_INIT || data.Status == ConstantsModel.ORDER_ACCEPTED)
            {
                data.Status = ConstantsModel.ORDER_REJECTED; data.FinishedTime = DateTime.Now; return orderDB.Update(data);
            }
            return false;
        }
        /// <summary> 
        /// Duyệt chấp nhận đơn hàng 
        /// </summary> 
        public static bool AcceptOrder(int orderID)
        {
            Orders? data = orderDB.Get(orderID); if (data == null) return false;
            if (data.Status == ConstantsModel.ORDER_INIT)
            {
                data.Status = ConstantsModel.ORDER_ACCEPTED; data.AcceptTime = DateTime.Now; return orderDB.Update(data);
            }
            return false;
        }
        /// <summary> 
        /// Xác nhận đã chuyển hàng 
        /// </summary> 
        public static bool ShipOrder(int orderID, int shipperID)
        {
            Orders? data = orderDB.Get(orderID); if (data == null) return false;

            if (data.Status == ConstantsModel.ORDER_ACCEPTED || data.Status == ConstantsModel.ORDER_SHIPPING)
            {
                data.Status = ConstantsModel.ORDER_SHIPPING; data.ShipperID = shipperID;
                data.ShippedTime = DateTime.Now;
                return orderDB.Update(data);
            }
            return false;
        }
        /// <summary> 
        /// Ghi nhận kết thúc quá trình xử lý đơn hàng thành công 
        /// </summary> 
        public static bool FinishOrder(int orderID)
        {
            Orders? data = orderDB.Get(orderID); if (data == null) return false;

            if (data.Status == ConstantsModel.ORDER_SHIPPING)
            {
                data.Status = ConstantsModel.ORDER_FINISHED; data.FinishedTime = DateTime.Now; return orderDB.Update(data);
            }
            return false;
        }
        /// <summary> 
        /// Xóa đơn hàng và toàn bộ chi tiết của đơn hàng         
        /// </summary> 
        public static bool DeleteOrder(int orderID)
        {
            var data = orderDB.Get(orderID); if (data == null) return false;
            if (data.Status == ConstantsModel.ORDER_INIT
                        || data.Status == ConstantsModel.ORDER_CANCEL || data.Status == ConstantsModel.ORDER_REJECTED) return orderDB.Delete(orderID);

            return false;
        }
        /// <summary> 
        /// Lấy danh sách các mặt hàng được bán trong đơn hàng 
        /// </summary> 
        public static List<OrderDetails> ListOrderDetails(int orderID)
        {
            return orderDB.ListDetails(orderID).ToList();
        }
        /// <summary> 
        /// Lấy thông tin của 1 mặt hàng được bán trong đơn hàng 
        /// </summary> 
        public static OrderDetails? GetOrderDetail(int orderID, int productID)
        {
            return orderDB.GetDetail(orderID, productID);
        }
        /// <summary> 
        /// Lưu thông tin chi tiết của đơn hàng (thêm mặt hàng được bán trong đơn hàng)  
        /// theo nguyên tắc: 
        /// - Nếu mặt hàng chưa có trong chi tiết đơn hàng thì bổ sung 
        /// - Nếu mặt hàng đã có trong chi tiết đơn hàng thì cập nhật lại số lượng và giá bán 
        /// </summary> 
        public static bool SaveOrderDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            Orders? data = orderDB.Get(orderID); if (data == null) return false;

            if (data.Status == ConstantsModel.ORDER_INIT || data.Status == ConstantsModel.ORDER_ACCEPTED)
            {
                return orderDB.SaveDetail(orderID, productID, quantity, salePrice);
            }
            return false;
        }
        /// <summary> 
        /// Xóa một mặt hàng ra khỏi đơn hàng 
        /// </summary> 
        public static bool DeleteOrderDetail(int orderID, int productID)
        {
            Orders? data = orderDB.Get(orderID);
            if (data == null) return false;
            if (data.Status == ConstantsModel.ORDER_INIT || data.Status == ConstantsModel.ORDER_ACCEPTED)
            {
                return orderDB.DeleteDetail(orderID, productID);
            }
            return false;

        }
    }
}
