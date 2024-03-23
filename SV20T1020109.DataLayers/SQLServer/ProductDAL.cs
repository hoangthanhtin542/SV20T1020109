using Azure;
using Dapper;
using SV20T1020109.DataLayers.SQLServer;
using SV20T1020109.DataLayers;
using SV20T1020109.DomainModels;
using System;

namespace SV20T1020109.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"    insert into Products(ProductName, ProductDescription, SupplierID, CategoryId, Unit, Price, Photo, IsSelling)
                                values(@ProductName, @ProductDescription, @SupplierID, @CategoryId, @Unit, @Price, @photo, @IsSelling);
                                select @@identity";   // @@identity khong phai tham so ma la bien
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    photo = data.Photo ?? "",
                    IsSelling = data.IsSelling,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"    insert into Products(ProductID, AttributeName, AttributeValue, DisplayOrder)
                                values(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                                select @@identity";   // @@identity khong phai tham so ma la bien
                var parameters = new
                {
                    ProductId = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductPhotos(ProductID,Photo,Description,DisplayOrder,IsHidden)
                     values(@ProductID,@photo,@Description,@DisplayOrder,@IsHidden);
                     select @@identity; ";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"    SELECT COUNT(*) FROM Products 
                                WHERE 
                                    (@SearchValue = N'' OR productName LIKE @SearchValue)
                                    AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                    AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                    AND Price >= @MinPrice
                                    AND (@MaxPrice <= 0 OR Price <= @MaxPrice)";
                var parameters = new
                {
                    SearchValue = searchValue ?? "",    //nếu searchValue là null thì thay bởi rỗng
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID,
                };
                //Thực thi câu lệnh
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeId)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeId = @AttributeId";
                var parameters = new
                {
                    AttributeId = attributeId,
                };
                //Thực thi câu lệnh
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoId)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoId = @PhotoId";
                var parameters = new
                {
                    PhotoId = photoId,
                };
                //Thực thi câu lệnh
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID,
                };
                //Thực thi câu lệnh
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeId)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where AttributeId = @AttributeId";
                var parameters = new
                {
                    AttributeId = attributeId,
                };
                //Thực thi câu lệnh
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoId)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where PhotoId = @PhotoId";
                var parameters = new
                {
                    PhotoId = photoId,
                };
                //Thực thi câu lệnh
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    ProductID = productID,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"WITH cte AS (
                                SELECT  *,
                                        ROW_NUMBER() OVER (ORDER BY ProductName) AS RowNumber
                                FROM    Products
                                WHERE   (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                                    AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                    AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                    AND Price >= @MinPrice
                                    AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                            )
                            SELECT * FROM cte
                            WHERE   (@PageSize = 0) OR (RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize)";
                var parameters = new
                {
                    Page = page,
                    PageSize = pageSize,
                    searchValue = searchValue ?? "",
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productId)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"    select  *
                                from    ProductAttributes
                                where   ProductId = @ProductId";
                var parameters = new
                {
                    ProductId = productId
                };
                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productId)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"    select  *
                                from    ProductPhotos
                                where   ProductId = @ProductId";
                var parameters = new
                {
                    ProductId = productId
                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"    update Products
                                set ProductName = @ProductName,
                                    ProductDescription = @ProductDescription,
                                    SupplierID = @SupplierID,
                                    CategoryID = @CategoryID,
                                    Unit = @Unit,
                                    Price = @Price,
                                    Photo = @Photo,
                                    IsSelling = @IsSelling
                                where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"    update ProductAttributes
                                set ProductID = @ProductID,
                                    ProductName = @ProductName,
                                    AttributeValue = @AttributeValue,
                                    DisplayOrder = @DisplayOrder
                                where AttributeId = @AttributeId";
                var parameters = new
                {
                    AttributeId = data.AttributeID,
                    ProductId = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE ProductPhotos
                     SET ProductID = @productID,
                         Photo = @Photo,
                         Description = @description,
                         DisplayOrder = @displayOrder,
                         IsHidden = @isHidden
                     WHERE PhotoID = @PhotoID";
                var parameters = new
                {
                    productID = data.ProductID,
                    Photo = data.Photo ?? "",
                    description = data.Description ?? "",
                    displayOrder = data.DisplayOrder,
                    isHidden = data.IsHidden,
                    photoID = data.PhotoID
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
