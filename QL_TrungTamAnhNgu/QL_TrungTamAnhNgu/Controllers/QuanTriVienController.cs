using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_TrungTamAnhNgu.Models;

namespace QL_TrungTamAnhNgu.Controllers
{
    public class QuanTriVienController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection col)
        {
            string username = col["inputTenTK"];
            string password = col["inputMatKhau"];

            ViewBag.tk = username;
            ViewBag.mk = password;
            string newConnectionString="Data Source=PHAMTHUAN\MSSQLSERVER01;Initial Catalog=QL_TrungTamAnhNgu;Persist Security Info=True;User ID=" + username + ";Password=" + password;
            db = new DataClasses1DataContext(newConnectionString);

            NguoiDung user = db.NguoiDungs.FirstOrDefault(k => k.TenTaiKhoan == username && k.MatKhau == password);

            if (user == null)
            {
                ViewBag.text = "Tài khoản hoặc mật khẩu không chính xác!";
                return View();
            }
            if (!string.IsNullOrEmpty(username) && username.Contains(" "))
            {
                ViewBag.text = "Tên đăng nhập không chứa khoảng trắng!";
                return View();
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.text = "Vui lòng điền đầy đủ thông tin!";
                return View();
            }

            if (user.TrangThai == "Dừng hoạt động" && (user.MaNhomND == "ND002" || user.MaNhomND == "ND004" || user.MaNhomND == "ND005" || user.MaNhomND == "ND006"))
            {
                ViewBag.text = "Tài khoản đã bị đình chỉ!";
                return View();
            }

            if (user != null && user.TrangThai == "Đang hoạt động" && (user.MaNhomND == "ND001" || user.MaNhomND == "ND004" || user.MaNhomND == "ND005" || user.MaNhomND == "ND006"))
            {
                Session["user"] = user;
                return RedirectToAction("Index", "QuanTriVien");
            }
            else if (user != null && user.TrangThai == "Đang hoạt động" && user.MaNhomND == "ND002")
            {
                Session["user"] = user;
                return RedirectToAction("Index", "GiangVien");
            }
            else if (user != null && user.TrangThai == "Đang hoạt động" && user.MaNhomND == "ND003")
            {
                ViewBag.text = "Tài khoản không đủ quyền truy cập!";
            }
            return View();
        }
    }
}
