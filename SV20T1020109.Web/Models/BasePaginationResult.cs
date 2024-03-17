using SV20T1020109.DomainModels;
using static NuGet.Packaging.PackagingConstants;
namespace SV20T1020109.Web.Models
{
    /// <summary>
    /// lop cha cac lop bieu dien du lieju ket qua tim kiem , phan trang
    /// </summary>
    public abstract class BasePaginationResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public int RowCount { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int c = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    c += 1;
                return c;
            }
        }
        /// <summary>
        /// ket qua tim kiem lay danh sach khach hang 
        /// </summary>


    }
    public class CustomerSearchResult : BasePaginationResult
    {
        public List<Customer> Data { get; set; }

    }
    public class CategorySearchResult : BasePaginationResult
    {
        public List<Category> Data { get; set; }

    }
    public class EmployeeSearchResult : BasePaginationResult
    {
        public List<Employee> Data { get; set; }

    }
    public class ShipperSearchResult : BasePaginationResult
    {
        public List<Shipper> Data { get; set; }

    }
    public class SupplierSearchResult : BasePaginationResult
    {
        public List<Supplier> Data { get; set; }

    }
    public class ProductSearchResult : BasePaginationResult
    {
        public List<Product> Data { get; set; }

    }
    public class OrderSearchResult : BasePaginationResult
    {
        public List<Orders> Data { get; set; }

    }
}
