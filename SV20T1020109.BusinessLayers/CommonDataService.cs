using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV20T1020109.DataLayers.SQLServer;
using SV20T1020109.BusinessLayers;
using SV20T1020109.DataLayers;
using SV20T1020109.DomainModels;    

namespace SV20T1020109.BusinessLayers
{
    /// <summary>
    /// cung cấp các chức năng xử lí dữ liệu chung
    /// (tỉnh/thành, khách hàng, nhà cung cấp, loại hàng, người giao hàng, nhân viên)
    /// </summary>
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Province> ProvinceDB;
        private static readonly ICommonDAL<Customer> CustomerDB;
        private static readonly ICommonDAL<Supplier> SupplierDB;
        private static readonly ICommonDAL<Shipper> ShipperDB;
        private static readonly ICommonDAL<Employee> EmployeeDB;
        private static readonly ICommonDAL<Category> CategoryDB;


        /// <summary>
        /// Ctor (câu hỏi: static contruction hoạt động như thế nào 
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;
            ProvinceDB = new ProvinceDAL(connectionString);
            CustomerDB = new CustomerDAL(connectionString);
            SupplierDB = new SupplierDAL(connectionString);
            ShipperDB = new ShipperDAL(connectionString);
            EmployeeDB = new EmployeeDAL(connectionString);
            CategoryDB = new CategoryDAL(connectionString);

        }
        /// <summary>
        /// Danh sach cac tinh / thanh
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {
            return ProvinceDB.List().ToList();
        }
        public static List<Category> ListOfCatgoryLists()
        {
            return CategoryDB.List().ToList();
        }
        public static List<Supplier> ListOfSupplierLists()
        {
            return SupplierDB.List().ToList();
        }

        public static List<Customer> ListOfCustomers()
        {
            return CustomerDB.List().ToList();
        }
        public static List<Employee> ListOfEmployees()
        {
            return EmployeeDB.List().ToList();
        }

        /// <summary>
        /// tim kiem va lay danh sach khach hang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = CustomerDB.Count(searchValue);
            return CustomerDB.List(page, pageSize, searchValue).ToList();
        }
        public static Customer? GetCustomer(int id)
        {
            return CustomerDB.Get(id);
        }
        public static int AddCustomer(Customer customer)
        {
            return CustomerDB.Add(customer);
        }

        public static bool UpdateCustomer(Customer customer)
        {
            return CustomerDB.Update(customer);
        }
        /// <summary>
        /// xoa khach hang có mã id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if (CustomerDB.IsUsed(id))
                return false;
            return CustomerDB.Delete(id);
        }
        /// <summary>
        /// kiem tra xem khach hang co ma la id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUserCustomer(int id)
        {
            return CustomerDB.IsUsed(id);
        }



        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = SupplierDB.Count(searchValue);
            return SupplierDB.List(page, pageSize, searchValue).ToList();
        }

        /// <summary>
        /// tìm kiếm và lấy danh sách nhà cung cấp nếu ko phân trang.
        /// </summary>
        /// <param name="SearchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(String SearchValue)
        {
            return SupplierDB.List(1, 0, SearchValue).ToList();
        }

        public static Supplier? GetSupplier(int id)
        {
            return SupplierDB.Get(id);
        }
        public static int AddSupplier(Supplier supplier)
        {
            return SupplierDB.Add(supplier);
        }

        public static bool UpdateSupplier(Supplier supplier)
        {
            return SupplierDB.Update(supplier);
        }
        /// <summary>
        /// xoa khach hang có mã id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (SupplierDB.IsUsed(id))
                return false;
            return SupplierDB.Delete(id);
        }
        /// <summary>
        /// kiem tra xem khach hang co ma la id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUserSupplier(int id)
        {
            return SupplierDB.IsUsed(id);
        }


        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = ShipperDB.Count(searchValue);
            return ShipperDB.List(page, pageSize, searchValue).ToList();
        }
        public static Shipper? GetShipper(int id)
        {
            return ShipperDB.Get(id);
        }
        public static int AddShipper(Shipper shipper)
        {
            return ShipperDB.Add(shipper);
        }

        public static bool UpdateShipper(Shipper shipper)
        {
            return ShipperDB.Update(shipper);
        }
        /// <summary>
        /// xoa khach hang có mã id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            if (ShipperDB.IsUsed(id))
                return false;
            return ShipperDB.Delete(id);
        }
        /// <summary>
        /// kiem tra xem khach hang co ma la id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUserShipper(int id)
        {
            return ShipperDB.IsUsed(id);
        }

        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = EmployeeDB.Count(searchValue);
            return EmployeeDB.List(page, pageSize, searchValue).ToList();
        }
        public static Employee? GetEmployee(int id)
        {
            return EmployeeDB.Get(id);
        }
        public static int AddEmployee(Employee employee)
        {
            return EmployeeDB.Add(employee);
        }

        public static bool UpdateEmployee(Employee employee)
        {
            return EmployeeDB.Update(employee);
        }
        /// <summary>
        /// xoa khach hang có mã id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            if (EmployeeDB.IsUsed(id))
                return false;
            return EmployeeDB.Delete(id);
        }
        /// <summary>
        /// kiem tra xem khach hang co ma la id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUserEmployee(int id)
        {
            return EmployeeDB.IsUsed(id);
        }


        public static List<Category> ListOfCategorys(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = CategoryDB.Count(searchValue);
            return CategoryDB.List(page, pageSize, searchValue).ToList();
        }



        /// <summary>
        /// tìm kiếm và lấy danh sách nhà cung cấp nếu ko phân trang.
        /// </summary>
        /// <param name="SearchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(String SearchValue)
        {
            return CategoryDB.List(1, 0, SearchValue).ToList();
        }

        public static Category? GetCategory(int id)
        {
            return CategoryDB.Get(id);
        }
        public static int AddCategory(Category category)
        {
            return CategoryDB.Add(category);
        }

        public static bool UpdateCategory(Category category)
        {
            return CategoryDB.Update(category);
        }
        /// <summary>
        /// xoa khach hang có mã id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            if (CategoryDB.IsUsed(id))
                return false;
            return CategoryDB.Delete(id);
        }
        /// <summary>
        /// kiem tra xem khach hang co ma la id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUserCategory(int id)
        {
            return CategoryDB.IsUsed(id);
        }
    }
}