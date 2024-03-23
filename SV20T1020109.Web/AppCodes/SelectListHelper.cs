using Microsoft.AspNetCore.Mvc.Rendering;
using SV20T1020109.BusinessLayers;

namespace SV20T1020109.Web
{
    public static class SelectListHelper
    {
        public static List<SelectListItem> Provinces()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn tỉnh\thành --"
            });
            foreach (var item in CommonDataService.ListOfProvinces())
            {
                list.Add(new SelectListItem()
                {

                    Value = item.ProvinceName.ToString(),
                    Text = item.ProvinceName
                });
            }
            return list;
        }

        /// <summary>
        /// Danh sách loại hàng.
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> ListOfCategorys()
        {
            List<SelectListItem> List = new List<SelectListItem>();

            List.Add(new SelectListItem()
            {
                Value = "0",
                Text = "---Chọn Loại Hàng---"

            });

            string searchvalue = "";
            foreach (var item in CommonDataService.ListOfCategories(searchvalue))
            {
                List.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }

            return List;
        }

        /// <summary>
        /// Danh sách nhà cung cấp.
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> ListOfSuppliers()
        {
            List<SelectListItem> List = new List<SelectListItem>();

            List.Add(new SelectListItem()
            {
                Value = "0",
                Text = "---Chọn Nhà Cung Cấp---"

            });

            string searchvalue = "";
            foreach (var item in CommonDataService.ListOfSuppliers(searchvalue))
            {
                List.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }

            return List;
        }

    }
}
