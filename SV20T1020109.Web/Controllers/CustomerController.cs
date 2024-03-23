using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020109.BusinessLayers;
using SV20T1020109.DomainModels;
using SV20T1020109.Web.Models;
using static SV20T1020109.Web.WebSecurityModels;

namespace SV20T1020109.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}, {WebUserRoles.Employee}")]
    public class CustomerController : Controller
    {
            
        private const int PAGE_SIZE = 20;
        private const string CUSTOMER_SEARCH = "customer_search"; //Tên biến dùng dể lưu trong session

        public IActionResult Index()
        {
            //Lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);

            //Trường hợp trong session chưa có điều kiện thì tạo ra điều kiện mới
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            return View(input);
        }

        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CustomerSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm vào trong session
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            Customer model = new Customer()
            {
                CustomerID = 0
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            Customer? model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
        public IActionResult Save(Customer data)
        {
            try
            {
                if (data.CustomerID == 0)
                {
                    int id = CommonDataService.AddCustomer(data);
                    return RedirectToAction("Index");
                }
                else
                {
                    bool result = CommonDataService.UpdateCustomer(data);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public IActionResult Delete(int id = 0)
        {
            if(Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.IsUserCustomer(id);

            return View(model);
        }

    }
}
