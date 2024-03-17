using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV20T1020109.DomainModels;
using Dapper;
using System.Net;
using System.Numerics;

namespace SV20T1020109.DataLayers.SQLServer
{
    public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Suppliers where Email = @Email)
                           select -1
                       else
                           begin
                               insert into Suppliers(SupplierName,ContactName,Province,Address,Phone,Email)
                               values(@SupplierName,@ContactName,@province,@Address,@Phone,@Email);
                               select @@identity; 
                           end"; //trong nay emial khong dc chung
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",

                };
                //thuwc thi cau lenh 
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return id;

        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!String.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	COUNT(*)
                             FROM	Suppliers 
                             WHERE	(@SearchValue = N'')
                                 OR	(
		                                    (SupplierName LIKE @SearchValue)
		                                    OR (ContactName LIKE @SearchValue)
		                                    OR (Address LIKE @SearchValue)
                                         OR (Phone LIKE @SearchValue)
	                                    )";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM Suppliers WHERE SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Suppliers WHERE SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where SupplierID = @SupplierID)
                         select 1
                     else 
                         select 0";
                var parameters = new
                {
                    SupplierID = id

                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return result;
        }

        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data = new List<Supplier>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                     (
                         select    *, row_number() over (order by SupplierName) as RowNumber
                         from    Suppliers
                         where    (@searchValue = N'') or ( (SupplierName LIKE @SearchValue)
		                                     OR (ContactName LIKE @SearchValue)
		                                     OR (Address LIKE @SearchValue)
                                          OR (Phone LIKE @SearchValue))
                     )
                     select * from cte
                     where  (@pageSize = 0)
                         or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                     order by RowNumber";
                var paramaters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                data = connection.Query<Supplier>(sql: sql, param: paramaters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Suppliers where SupplierID <> @SupplierID and Email = @email)
                         begin
                         UPDATE
                            Suppliers
                             SET SupplierName = @SupplierName,ContactName = @ContactName,Province = @province, Address = @Address, Phone = @Phone, Email = @Email
                             WHERE SupplierID = @SupplierID
                          end ";
                var parameters = new
                {
                    SupplierID = data.SupplierID,
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();

            }

            return result;
        }
    }
}