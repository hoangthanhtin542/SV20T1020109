using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using SV20T1020109.BusinessLayers;
using SV20T1020109.DomainModels;
using SV20T1020109.Web.Models;
using SV20T1020109.Web;
using System.Drawing.Printing;
using static SV20T1020109.Web.WebSecurityModels;
using static SV20T1020109.Web.Models.OrderSearchInput;

namespace SV20T1020109.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},")]
    public class OrderController : Controller
    {
        private const int ORDER_PAGE_SIZE = 20; //muon sus dung nhieu lan phan dung const
        private const string ORDER_SEARCH = "order_search";
        //số dòng trên trang thị hiển thị danh sách mặt hàng cần tìm thì lập đơn hàng 
        private const int PRODUCT_PAGE_SIZE = 5;
        //Tên biến session lưu điều kiện tìm kiếm mặt hàng thì lập đơn hàng  
        private const string PRODUCT_SEARCH = "product_search";
        //Tên biến session dùng để lưu giỏ hàng 
        private const string SHOPPING_CART = "shopping_cart";
        public IActionResult Index()
        {
            OrderSearchInput? input = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH);
            //Trong hợp trong session chưa có điều kiện thì tạo điều kiện mới
            if (input == null)
            {
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = ORDER_PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    DateRange = string.Format("{0:dd/MM/yyyy}-{1:dd/MM/yyyy}",
                                               DateTime.Today.AddMonths(-1),
                                               DateTime.Today)
                };
            }
            return View(input);
        }

        public ActionResult Search(OrderSearchInput input)
        {
            int rowCount = 0;
            var data = OrderDataService.ListOrders(out rowCount, input.Page, input.PageSize, input.Status, input.FromTime, input.ToTime, input.SearchValue ?? "");
            var model = new OrderSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Status = input.Status,
                TimeRange = input.DateRange ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH, input);
            return View(model);
        }
        /// <summary>
        /// tìm kiếm mặt hàng đưa và giỏ hàng 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult SearchProduct(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SHOPPING_CART, input);
            return View(model);
        }

        /// <summary>
        /// lấy giỏ hàng hiện đang lưu trong session
        /// </summary>
        /// <returns></returns>
        private List<OrderDetails> GetShoppingCart()
        {
            //Giả hàng là danh sách mặt hàng (OrderDetail) được để bán trong đơn hàng 
            //và lưu trong session
            var shoppingCart = ApplicationContext.GetSessionData<List<OrderDetails>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<OrderDetails>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }


        /// <summary>
        /// trang hiện thị danh sách mặt hàng có trong giỏ hàng 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IActionResult ShowShoppingCart()
        {
            var model = GetShoppingCart();
            return View(model);
        }




        /// <summary>
        /// Bộ sung thêm mặt hàng vào giỏ hàng.
        /// Hàm trả về chuỗi khác rỗng không báo lỗi nếu dữ liệu lkhoong hợp lệ 
        /// Hàn trả về chuỗi rỗng nếu thành công 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IActionResult AddToCart(OrderDetails data)
        {
            if (data.SalePrice <= 0 || data.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == data.ProductID);
            if (existsProduct == null) //Nếu mặt hàng chưa có trong thì bộ sung vào giỏ hàng 
            {
                shoppingCart.Add(data);
            }
            else
            {
                existsProduct.Quantity += data.Quantity;
                existsProduct.SalePrice = data.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }


        /// <summary>
        /// Xóa mặt hàng ra khỏi giỏ hàng 
        /// </summary>
        /// <param name="id">Mã mặt hàng cần xóa khỏi giỏ hàng</param> 
        /// <returns></returns>
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }


        /// <summary>
        /// Xóa tắt cả mặt hàng trong giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }



        /// <summary>
        /// khỏi tạo đơn hàng(lập một đơn hàng mới)
        /// Hàm trả về chuỗi khác rỗng thông báo lỗi nếu đầu vào không hợp lệ 
        /// hoặc việc tạo đơn hàng không thành công 
        /// Ngược lại, hàm trả về đơn hàng được tạo (là một giá trị số)
        /// </summary>
        /// <param name="CustomerID">Mã khách hàng</param>
        /// <param name="deliveryProvice">Tỉnh/Thành giao hàng</param>
        /// <param name="deliveryAdderss">Địa chỉ giao hàng</param>
        /// <returns></returns>
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống , không thể lập đơn hàng");
            if (customerID <= 0 || string.IsNullOrWhiteSpace(deliveryProvince)
                               || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin");

            int employeeID = Convert.ToInt32(User.GetUserData()?.UserId);
            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, shoppingCart);

            ClearCart();
            return Json(orderID);
        }

        /// <summary>
        /// xem thông tim và chi tiết của chức năng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id = 0)
        {

            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details

            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        /// <summary>
        /// Giao diện thay đổi chi tiết thông tin đơn hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>

        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            ViewBag.Title = "Cập nhật thông tin chi tiết đơn hàng ";
            var model = OrderDataService.GetOrderDetail(id, productId);

            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateDetail(int OrderID, int ProductID, int Quantity, decimal SalePrice)
        {
            if (Quantity <= 0)
                return Json("số lượng bán không hợp lệ");
            if (SalePrice < 0)
                return Json("Giá bán không hợp lệ");
            bool result = OrderDataService.SaveOrderDetail(OrderID, ProductID, Quantity, SalePrice);
            if (!result)
                return Json("không được phép thay đổi tin của hàng này");
            return Json("");
        }

        /// <summary>
        /// chuyển hàng sang trạng thái được duyệt 
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);
            if (!result)

                TempData["Message"] = "Không thể duyệt đơn hàng này";
            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// chuyển đơn hàng sang trạng thái kết thúc 
        /// </summary>
        /// <param name="id">mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if (!result)
                TempData["Message"] = "Không thể ghi nhận trạng thái kết thú cho đơn hàng này";
            return RedirectToAction("Details", new { id });
        }


        /// <summary>
        /// chuyển đơn hàng sang trạng thái hủy
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
                TempData["Message"] = "Không thể thực hiện thao tác hủy đối với đơn hàng này";
            return RedirectToAction("Details", new { id });
        }


        /// <summary>
        /// chuyển sang trạng thái tự chỗi 
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public ActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id);
            if (!result)
                TempData["Message"] = "Khồng thể thực hiện thao tác tự chỗi đối với đơn hàng này";
            return RedirectToAction("Details", new { id });

        }


        /// <summary>
        /// Xóa đơn hàng 
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
                TempData["Message"] = "không thể xóa đơn hàng này";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// xác nhận chuyển đơn hàng cho người giao hàng 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {
            ViewBag.OrderID = id;
            return View();
        }

        /// <summary>
        /// Ghi nhận người giao hàng cho đơn hàng và chuyển đơn hàng sang trạng thái đang giao hàng
        /// Hàm trả về chuỗi khác rỗng thông báo lỗi nếu đầu vào không hợp lệ hoặc lỗi 
        /// hàm trả về chuỗi rỗng nếu thành công 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shipperID"></param>
        /// <returns></returns>

        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (shipperID <= 0)
                return Json("Vui lòng chọ người giao hàng");
            bool result = OrderDataService.ShipOrder(id, shipperID);
            if (!result)
                return Json("Đơn hàng không cho phép chuyển người giao hàgn");
            return Json("");
        }

    }
}
