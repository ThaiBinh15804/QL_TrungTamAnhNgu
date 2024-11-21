﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QL_TrungTamAnhNgu.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult DieuHuong()
        {
            return View();
        }

        
        public ActionResult XuLyDieuHuong(string ng)
        {
            if (ng == "HocVien")
            {
                return RedirectToAction("DangNhap", "HocVien");
            }
            else if (ng == "GiangVien")
            {
                return RedirectToAction("DangNhap", "GiangVien");
            }
            else if (ng == "QTV")
            {
                return RedirectToAction("DangNhap", "QuanTriVien");
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
