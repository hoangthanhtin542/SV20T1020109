/*using Microsoft.AspNetCore.Mvc;
using SV20T1020109.BusinessLayers;
using System.Globalization;

namespace SV20T1020109.web.Controllers
{
    public IActionResult Create()
    {
        var model = new Models.Person()
        {
            Name = "Hoàng Thanh Tín",
            BirthDate = DateTime.Now,
            Salarey = 10.25m
        };
        return View(model);
    }
    public IActionResult Save(Models.Person model, string briDateInput = "")
    {
        //chuyen birthDateInput sang gia tri kieu ngay
        DateTime? dValue = StringToDatetime(briDateInput);
        if (dValue.HasValue)
        {
            model.BirthDate = dValue.Value;

        }

        return Json(model);
    }

    private DateTime? StringToDatetime(string s, string formats = "d/M/yyyy;d-M-yyyy;d.M.yyy")
    {
        try
        {
            return DateTime.ParseExact(s, formats.Split(';'), CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }
}*/