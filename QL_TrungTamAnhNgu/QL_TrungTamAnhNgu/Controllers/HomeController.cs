using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using QL_TrungTamAnhNgu.Models;


namespace QL_TrungTamAnhNgu.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        [AllowAnonymous]
        public ActionResult DangNhap()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult DangNhap(FormCollection col)
        {
            string username = col["inputTenTK"];
            string password = col["inputMatKhau"];

            ViewBag.tk = username;
            ViewBag.mk = password;
            NguoiDung user = db.NguoiDungs.FirstOrDefault(k => k.MaNguoiDung == db.AuthenticateUser(username, password));
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
            if (user == null)
            {
                ViewBag.text = "Tài khoản hoặc mật khẩu không chính xác!";
                return View();
            }
            try
            {
                string newConnectionString = "Data Source=PHAMTHUAN\\MSSQLSERVER01;Initial Catalog=QL_TrungTamAnhNgu;Persist Security Info=True;User ID=" + username + ";Password=" + password;
                db = new DataClasses1DataContext(newConnectionString);


                if (user.TrangThai == "Đã khóa" && (user.MaNhomND == "NND002" || user.MaNhomND == "NND004" || user.MaNhomND == "NND005" || user.MaNhomND == "NND006"))
                {
                    ViewBag.text = "Tài khoản đã bị đình chỉ!";
                    return View();
                }

                if (user != null && user.TrangThai == "Đang hoạt động" && (user.MaNhomND == "NND001" || user.MaNhomND == "NND004" || user.MaNhomND == "NND005" || user.MaNhomND == "NND006"))
                {
                    Session["user"] = user;
                    FormsAuthentication.SetAuthCookie(user.TenTaiKhoan, false);
                    return RedirectToAction("Index", "QuanTriVien");
                }
                else if (user != null && user.TrangThai == "Đang hoạt động" && user.MaNhomND == "NND002")
                {
                    Session["user"] = user;
                    FormsAuthentication.SetAuthCookie(user.TenTaiKhoan, false);
                    return RedirectToAction("Index", "GiangVien");
                }
                else if (user != null && user.TrangThai == "Đang hoạt động" && user.MaNhomND == "NND003")
                {
                    ViewBag.text = "Tài khoản không đủ quyền truy cập!";
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi khi kết nối hoặc truy vấn, thông báo cho người dùng
                ViewBag.text = "Không thể kết nối đến cơ sở dữ liệu hoặc quyền truy cập bị từ chối. Lỗi: " + ex.Message;
                return View();
            }
            return View();
        }


    }
}
