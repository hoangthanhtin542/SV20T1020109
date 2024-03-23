using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020109.BusinessLayers;
using SV20T1020109.DomainModels;
using SV20T1020109.Web.Models;
using static SV20T1020109.Web.WebSecurityModels;

namespace SV20T1020109.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}, {WebUserRoles.Employee}")]
    public class SupplierController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string SUPPLIER_SEARCH = "dupplier_search"; //Tên biến dùng để lưu trong session

        public IActionResult Index()
        {
            //
            PaginationSearchInput input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);

            //Trường hợp trong session chưa có điều kiện thì tạo điều kiện mới
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
            var data = CommonDataService.ListOfSuppliers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SupplierSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm vào trong session
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loai hang";
            Supplier model = new Supplier()
            {
                SupplierID = 0
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng ";
            Supplier model = CommonDataService.GetSupplier(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Supplier data)
        {


            try
            {
                //Kiểm soát dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(data.SupplierName))
                    ModelState.AddModelError(nameof(data.SupplierName), "Tên không được để trống"); //Lưu trữ danh sách báo lỗi
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không để trống");
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError(nameof(data.Email), "Email không để trống");
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");


                //thông qua thuộc tính IsValid của Modelstate để kiểm tra xem có tồn tai lỗi hay không
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.SupplierID == 0 ? "Bộ sung nhà cung cấp " : "Cập nhật thông tin nhà cung cấp ";
                    return View("Edit", data);
                }

                if (data.SupplierID == 0)
                {
                    int id = CommonDataService.AddSupplier(data);
                    if (id <= 0)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng");
                        return View("Edit", data);
                    }

                }
                else
                {
                    bool result = CommonDataService.UpdateSupplier(data);
                    if (!result)
                    {
                        ModelState.AddModelError(nameof(data.Email), "Địa chỉ Email bị trùng với người khác");
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Không thể lưu được dữ liệu. Vui lòng sự lại sau vài phút");
                return View("Edit", data);
            }

        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetSupplier(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.IsUserSupplier(id);

            return View(model);
        }
    }
}
