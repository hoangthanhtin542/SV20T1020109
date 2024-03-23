using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static SV20T1020109.Web.WebSecurityModels;
using SV20T1020109.BusinessLayers;
using SV20T1020109.DomainModels;

namespace SV20T1020109.Web.Controllers
{
    [Authorize]

    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.Username = username;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
                return View();
            }

            var userAccount = UserAccountService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại!");
                return View();
            }

            //Đăng nhập thành công, tạo dữ liệu để lưu thông tin đăng nhập
            var userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList(),
            };
            //Thiết lập phiên đăng nhập cho 
            await HttpContext.SignInAsync(userData.CreatePrincipal());
            //Quay về trang chủ sau khi đăng nhập thành công
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassWord()
        {
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassWord(UserAccount data)
        {
            var userData = new WebUserData()
            {
                UserName = data.UserName,
                DisplayName = data.FullName,
                Email = data.Email,
                Photo = data.Photo,
                Roles = data.RoleNames.Split(',').ToList()
            };
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            return View(data);
        }

        [HttpPost]
        public IActionResult SavePassWord(string userName, string oldPassword, string newPassword, string confirmNewPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
                    ModelState.AddModelError("ChangePassFailed", "Điền đầy đủ để đổi mật khẩu");
                if (confirmNewPassword == newPassword)
                {
                    var userAccount = UserAccountService.ChangePassword(userName, oldPassword, newPassword);
                    if (!userAccount)
                    {
                        ModelState.AddModelError("oldPassword", "Mật khẩu cũ không đúng");

                    }
                    else return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("ChangePassFailed", "Xác nhận mật khẩu không hợp lệ");

                return View("Index");
            }
            catch
            {
                ModelState.AddModelError("ChangePassFailed", "Đổi mật khẩu không thành công");
                return View("Index");
            }

        }
        public IActionResult AccessDenined()
        {
            return View("MessageError");
        }
    }
}
