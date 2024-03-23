using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020109.BusinessLayers;
using SV20T1020109.DomainModels;
using SV20T1020109.Web.Models;
using SV20T1020109.Web;
using System.Data;
using static SV20T1020109.Web.WebSecurityModels;
namespace SV20T1020109.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator}")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 20; //muon sus dung nhieu lan phan dung const
        private const string EMPLOYEE_SEARCH = "emplyee_search"; //Tên biến dùng để lưu trong session

        public IActionResult Index()
        {

            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);

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
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            //Lưu lại điều kiện tìm kiếm vào trong session
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);

            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            Employee model = new Employee()
            {

                EmployeeID = 0,
                BirthDate = new DateTime(2002, 4, 5),
                Photo = "photo.png"

            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng ";
            Employee model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");


            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "photo.png";

            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Employee data, string birthDateInput, IFormFile? uploadPhoto)
        {


            if (data == null)
                ModelState.AddModelError("BirthDate", $"Ngày{birthDateInput}không hợp lệ. Vui lòng nhập theo định dạng dd/MM/yyyy");


            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError("FullName", " Vui lòng nhập họ và tên");
            if (string.IsNullOrWhiteSpace(data.Address))
                ModelState.AddModelError("Address", " Vui lòng nhập địa chỉ");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError("Email", "Vui lòng nhập Email");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError("Phone", "Vui lòng nhập số điện thoại");
            //if (string.IsNullOrWhiteSpace(data.BirthDate.ToString()))
            //    ModelState.AddModelError("BirthDate", "Vui lòng nhập ngày sinh");
            if (string.IsNullOrWhiteSpace(data.Photo))
                ModelState.AddModelError("Photo", "Vui lòng chọn ảnh");


            DateTime? birthDate = birthDateInput.ToDateTime();
            if (birthDate.HasValue)
                data.BirthDate = birthDate.Value;
            // xu ly anh dc upload(luu anh neu cos anh upload thif luu anh va gan lai cai ten cua file anh mi=oi cho nhan vien)
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; // ten file se luu
                string folder = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, "images\\employees"); //duong dan thu muc luu file 
                string filePath = Path.Combine(folder, fileName);//duong dan file can luu
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }
            //thông qua thuộc tính IsValid của Modelstate để kiểm tra xem có tồn tai lỗi hay không
            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.EmployeeID == 0 ? "Bộ sung khách hàng " : "Cập nhật thông tin khách hàng ";
                return View("Edit", data);
            }

            if (data.EmployeeID == 0)
            {
                int id = CommonDataService.AddEmployee(data);
                return RedirectToAction("Index");
            }
            else
            {
                bool result = CommonDataService.UpdateEmployee(data);
            }
            return RedirectToAction("Index");

        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.AllowDelete = !CommonDataService.IsUserEmployee(id);


            return View(model);
        }
    }
}
