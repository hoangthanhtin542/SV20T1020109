using Azure;
using Dapper;
using SV20T1020109.DataLayers.SQLServer;
using SV20T1020109.DataLayers;
using SV20T1020109.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020109.DataLayers.SQLServer
{
    public class OrderDAL : _BaseDAL, IOrderDAL
    {
        public OrderDAL(string connectionString) : base(connectionString)
        {
        }


        public int Add(Orders data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Orders(CustomerId,OrderTime, DeliveryProvince, DeliveryAddress,EmployeeID, Status) 
                               values(@CustomerID, getdate(),@DeliveryProvince,@DeliveryAddress,@EmployeeID, @Status);                        
                              select @@identity";

                var parameters = new
                {
                    CustomerID = data.CustomerID,
                    OrderTime = data.OrderTime,
                    DeliveryProvince = data.DeliveryProvince,
                    DeliveryAddress = data.DeliveryAddress,
                    EmployeeID = data.EmployeeID,
                    Status = ConstantsModel.ORDER_INIT,
                };
                // Thêm đơn hàng vào cơ sở dữ liệu và lấy ID của đơn hàng vừa thêm
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;

        }

        public int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            int count = 0;
            if (!String.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            from Orders as o
                                left join Customers as c on o.CustomerID = c.CustomerID
                                left join Employees as e on o.EmployeeID = e.EmployeeID
                                left join Shippers as s on o.ShipperID = s.ShipperID

                            where (@status = 0 or o.Status = @status)
                                and (@fromTime is null or o.OrderTime >= @fromTime)
                                and (@toTime is null or o.OrderTime <= @toTime)
                                and (@searchValue = N''
                            or c.CustomerName like @searchValue
                            or e.FullName like @searchValue
                            or s.ShipperName like @searchValue)";
                var parameters = new
                {
                    Status = status,
                    searchValue = searchValue ?? "",
                    FromTime = fromTime,
                    ToTime = toTime,

                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;

        }

        public bool Delete(int orderID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM OrderDetails WHERE OrderID = @OrderID;
                            DELETE FROM Orders WHERE OrderID = @OrderID;";
                var parameters = new
                {
                    OrderID = orderID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool DeleteDetail(int orderID, int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM OrderDetails WHERE OrderID = @OrderID AND ProductID = @ProductID";
                var parameters = new
                {
                    OrderID = orderID,
                    productID = productID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Orders? Get(int orderID)
        {
            Orders? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select o.*, 
                            c.CustomerName, 
                            c.ContactName as CustomerContactName, 
                            c.Address as CustomerAddress, 
                            c.Phone as CustomerPhone, 
                            c.Email as CustomerEmail, 
                            e.FullName as EmployeeName, 
                            s.ShipperName, 
                            s.Phone as ShipperPhone                            
                      from    Orders as o 
                            left join Customers as c on o.CustomerID = c.CustomerID 
                            left join Employees as e on o.EmployeeID = e.EmployeeID                           
                            left join Shippers as s on o.ShipperID = s.ShipperID                     
                     where   o.OrderID = @OrderID";
                ;
                var parameters = new
                {
                    orderID = orderID
                };
                data = connection.QueryFirstOrDefault<Orders>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public OrderDetails? GetDetail(int orderID, int productID)
        {
            OrderDetails? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select  od.*, p.ProductName, p.Photo, p.Unit                        
                              from    OrderDetails as od 
                              join Products as p on od.ProductID = p.ProductID                        
                              where od.OrderID = @OrderID and od.ProductID = @ProductID";

                var parameters = new
                {
                    orderID = orderID,
                    productID = productID
                };
                data = connection.QueryFirstOrDefault<OrderDetails>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }



        public IList<OrderStatus> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<OrderStatus> data = new List<OrderStatus>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                                    FROM 
                                    (
	                                    SELECT	*, ROW_NUMBER() OVER (ORDER BY Description) AS RowNumber
	                                     FROM	OrderStatus
                                    ) AS t
                                    WHERE (@PageSize = 0) OR (t.RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize);";
                var paramaters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                data = connection.Query<OrderStatus>(sql: sql, param: paramaters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public IList<Orders> List(int page = 1, int pageSize = 0, int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {

            List<Orders> data = new List<Orders>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                                (
                                select row_number() over(order by o.OrderTime desc) as RowNumber,
                                o.*,

                                c.CustomerName,
                                c.ContactName as CustomerContactName,
                                c.Address as CustomerAddress,
                                c.Phone as CustomerPhone,
                                c.Email as CustomerEmail,
                                e.FullName as EmployeeName,
                                s.ShipperName,
                                s.Phone as ShipperPhone

                                from Orders as o
                                left join Customers as c on o.CustomerID = c.CustomerID
                                left join Employees as e on o.EmployeeID = e.EmployeeID
                                left join Shippers as s on o.ShipperID = s.ShipperID

                                where (@status = 0 or o.Status = @status)
                                and (@fromTime is null or o.OrderTime >= @fromTime)
                                and (@toTime is null or o.OrderTime <= @toTime)
                                and (@searchValue = N''
                                or c.CustomerName like @searchValue
                                or e.FullName like @searchValue
                                or s.ShipperName like @searchValue)

                                )
                                select * from cte
                                where (@pageSize = 0)

                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)

                                order by RowNumber";
                var paramaters = new
                {
                    Page = page,
                    PageSize = pageSize,
                    Status = status,
                    searchValue = searchValue ?? "",
                    FromTime = fromTime,
                    ToTime = toTime

                };
                data = connection.Query<Orders>(sql: sql, param: paramaters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public IList<OrderDetails> ListDetails(int orderID)
        {
            List<OrderDetails> data = new List<OrderDetails>();

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	od.*, p.ProductName, p.Unit, p.Photo		
                                    FROM	OrderDetails AS od
		                                    JOIN Products AS p ON od.ProductID = p.ProductID
                                    WHERE	od.OrderID = @OrderID";
                var paramaters = new
                {
                    orderID = orderID,
                };
                data = connection.Query<OrderDetails>(sql: sql, param: paramaters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public bool Update(Orders data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Orders 
                        set CustomerID = @CustomerID, 
                            OrderTime = @OrderTime, 
                            DeliveryProvince = @DeliveryProvince, 
                            DeliveryAddress = @DeliveryAddress, 
                            EmployeeID = @EmployeeID, 
                            AcceptTime = @AcceptTime, 
                            ShipperID = @ShipperID, 
                            ShippedTime = @ShippedTime, 
                            FinishedTime = @FinishedTime,                            
                            Status = @Status                       
                       where OrderID = @OrderID";

                var parameters = new
                {
                    OrderID = data.OrderID,
                    CustomerID = data.CustomerID,
                    OrderTime = data.OrderTime,
                    DeliveryProvince = data.DeliveryProvince,
                    DeliveryAddress = data.DeliveryAddress,
                    EmployeeID = data.EmployeeID,
                    AcceptTime = data.AcceptTime,
                    ShipperID = data.ShipperID,
                    ShippedTime = data.ShippedTime,
                    FinishedTime = data.FinishedTime,
                    Status = data.Status,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();

            }

            return result;
        }

        public bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails  
                           where OrderID = @OrderID and ProductID = @ProductID)                           
                           update OrderDetails  
  	 	 	 	           set Quantity = @Quantity,    	 	 	 	     
                           SalePrice = @SalePrice  
  	 	 	 	           where OrderID = @OrderID and ProductID = @ProductID                        
                         else 
                           insert into OrderDetails(OrderID, ProductID, Quantity, SalePrice)      	 	 	 
                           values(@OrderID, @ProductID, @Quantity, @SalePrice)";
                //TODO: Hoàn chỉnh phần code còn thiếu 
                var paramaters = new
                {
                    orderID = orderID,
                    productID = productID,
                    quantity = quantity,
                    salePrice = salePrice
                };
                // Thực thi câu lệnh SQL và lấy giá trị vô hướng
                result = connection.Execute(sql: sql, param: paramaters, commandType: System.Data.CommandType.Text) > 0;
                // Kiểm tra xem thao tác có thành công không (scalarResult sẽ là 1 nếu thành công)

                // Đóng kết nối
                connection.Close();
            }
            return result;

        }
    }
}
