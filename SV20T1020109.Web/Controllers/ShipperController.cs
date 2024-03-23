using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static SV20T1020109.Web.WebSecurityModels;
using SV20T1020109.BusinessLayers;
using SV20T1020109.DomainModels;
using SV20T1020109.Web.Models;
using SV20T1020109.Web;

namespace SV20T1020109.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]

    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string SHIPPER_SEARCH = "shipper_search"; //Tên biến dùng dể lưu trong session

        public IActionResult Index()
        {
            //Lấy đầu vào tìm kiếm hiện đang lưu lại trong session
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);

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
            var data = CommonDataService.ListOfShippers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new ShipperSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm vào trong session
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            Shipper model = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            Shipper? model = CommonDataService.GetShipper(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data.ShipperName))
                    ModelState.AddModelError(nameof(data.ShipperName), "Tên không được để trống");
                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại cho khách hàng");
                //Thông qua thuộc tính IsValid của ModelState để kiểm tra xem có tồn tại lỗi hay không
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.ShipperID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
                    return View("Edit", data);
                }

                if (data.ShipperID == 0)
                {
                    int id = CommonDataService.AddShipper(data);
                    /*return Json(data);*/
                }
                else
                {
                    bool result = CommonDataService.UpdateShipper(data);

                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Lỗi không thể lưu được dữ liệu. Vui lòng thử lại sau ít phút");
                return View("Edit", data);
            }
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetShipper(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.IsUserShipper(id);
            return View(model);
        }
    }
}
