using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_TrungTamAnhNgu.Models;
using System.Text.RegularExpressions;
using System.IO;  

namespace QL_TrungTamAnhNgu.Controllers
{
    public class HocVienController : Controller
    {
        // GET: /HocVien/
        public static string conn = "Data Source=THAIBINH-LAPTOP;Initial Catalog=QL_TrungTamAnhNgu;User ID=sa;Password=sa123";
        DataClasses1DataContext db = new DataClasses1DataContext(conn);
        public ActionResult TrangChu()
        {
            var kh = db.KhoaHocs.ToList();
            return View(kh);
        }
        public ActionResult LopHocCuaToi()
        {
            if (Session["MaHocVien"] == null)
            {
                return RedirectToAction("DangNhap", "HocVien");
            }
            var maHocVien = Session["MaHocVien"].ToString();
            var danhSachLopHoc = db.view_lophoc_dangkies.Where(v => v.MaHocVien == maHocVien).ToList();
            return View(danhSachLopHoc);
        }

        public ActionResult LopHoc(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("TrangChu", "HocVien");
            }
            var lopHoc = db.LopHocs.FirstOrDefault(lh => lh.MaLop == id);
            if (lopHoc == null)
            {
                return RedirectToAction("TrangChu", "HocVien");
            }
            Session["MaLop"] = lopHoc.MaLop;
            Session["MaKhoaHoc"] = lopHoc.MaKhoaHoc;

            return View(lopHoc);  
        }


        public ActionResult ChuyenCan()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("DangNhap", "HocVien");
            }
            var maHocVien = Session["MaHocVien"].ToString();
            var danhSachChuyenCan = db.view_dshocvien_diemdanhs.Where(cc => cc.MaHocVien == maHocVien);
            if (danhSachChuyenCan == null || !danhSachChuyenCan.Any())
            {
                return RedirectToAction("TrangChu", "HocVien");
            }
            return View(danhSachChuyenCan);
        }

        public ActionResult TaiLieu()
        {
            var maKhoaHoc = Session["MaKhoaHoc"].ToString();
            if (string.IsNullOrEmpty(maKhoaHoc))
            {
                return RedirectToAction("TrangChu", "HocVien");
            }
            var danhSachTaiLieu = db.view_tailieu_theokhoahocs.Where(v => v.MaKhoaHoc == maKhoaHoc).ToList();
            if (danhSachTaiLieu == null || !danhSachTaiLieu.Any())
            {
                ViewBag.ThongBao = "Không tìm thấy tài liệu nào cho khóa học này.";
                return View();
            }
            return View(danhSachTaiLieu);
        }

        public ActionResult ChiTietTaiLieu(string maTaiLieu)
        {
            if (string.IsNullOrEmpty(maTaiLieu))
            {
                ViewBag.ThongBao = "Không tìm thấy mã tài liệu.";
                return RedirectToAction("TaiLieu", "HocVien");
            }
            var chiTietTaiLieu = db.view_tailieu_theokhoahocs.FirstOrDefault(v => v.MaTaiLieu == maTaiLieu);
            if (chiTietTaiLieu == null)
            {
                ViewBag.ThongBao = "Không tìm thấy thông tin tài liệu.";
                return View();
            }
            return View(chiTietTaiLieu);
        }

        public ActionResult BaiTap()
        {
            var maKhoaHoc = Session["MaKhoaHoc"].ToString();
            if (string.IsNullOrEmpty(maKhoaHoc))
            {
                ViewBag.ThongBao = "Không tìm thấy mã bài tập.";
                return RedirectToAction("BaiTap", "HocVien");  
            }

            var chiTietBaiTap = db.view_baitap_theokhoahocs
                                  .FirstOrDefault(bt => bt.MaKhoaHoc == maKhoaHoc);

            // Kiểm tra nếu không tìm thấy bài tập
            if (chiTietBaiTap == null)
            {
                ViewBag.ThongBao = "Không tìm thấy thông tin bài tập.";
                return View();
            }

            // Truyền dữ liệu vào View
            return View(chiTietBaiTap);
        }

        public ActionResult ChiTietBaiTap(string maBaiTap)
        {
            if (string.IsNullOrEmpty(maBaiTap))
            {
                ViewBag.ThongBao = "Không tìm thấy bài tập.";
                return RedirectToAction("BaiTap", "HocVien");
            }

            // Lấy chi tiết bài tập từ view_baitap_theodangky
            var chiTietBaiTap = db.view_baitap_theodangkies.FirstOrDefault(v => v.MaBaiTap == maBaiTap);

            if (chiTietBaiTap == null)
            {
                ViewBag.ThongBao = "Không tìm thấy thông tin bài tập.";
                return View();
            }

            return View(chiTietBaiTap);
        }

        public ActionResult ChiTietLichHoc()
        {
            var maHocVien = Session["MaHocVien"].ToString();
            var maThanhToan = db.ThanhToans.Where(t => t.MaHocVien == maHocVien).Select(t => t.MaThanhToan).FirstOrDefault();
            if (string.IsNullOrEmpty(maThanhToan))
            {
                ViewBag.ThongBao = "Không tìm thấy thông tin thanh toán cho học viên này.";
                return View(); 
            }
            var danhSachLopHoc = db.DangKies.Where(dk => dk.MaThanhToan == maThanhToan).Select(dk => dk.MaLop) .ToList();
            var danhSachLichHoc = db.view_lichhoc_chitiet_cua_lophocs.Where(lh => danhSachLopHoc.Contains(lh.MaLop)).ToList();
            if (!danhSachLichHoc.Any())
            {
                ViewBag.ThongBao = "Không tìm thấy lịch học cho các lớp đã đăng ký.";
                return View(); 
            }
            return View(danhSachLichHoc); 
        }

        // THONG TIN NGUOI DUNG
        public ActionResult ThongTinNguoiDung()
        {
            var userId = Session["UserId"];
            if (userId != null)
            {
                var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == userId);
                var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaHocVien == nguoiDung.MaNguoiDung);
                if (nguoiDung != null)
                {
                    ViewBag.TenDangNhap = nguoiDung.TenTaiKhoan ?? string.Empty;
                    ViewBag.Email = hocVien.Email ?? string.Empty;

                    if (hocVien != null)
                    {
                        ViewBag.MaHocVien = hocVien.MaHocVien ?? string.Empty;
                        ViewBag.HoTen = hocVien.HoTen ?? string.Empty;
                        ViewBag.GioiTinh = hocVien.GioiTinh ?? string.Empty;
                        ViewBag.SoDienThoai = hocVien.SoDienThoai ?? string.Empty;
                        ViewBag.DiaChi = hocVien.DiaChi ?? string.Empty;
                        ViewBag.NgaySinh = hocVien.NgaySinh.HasValue ? hocVien.NgaySinh.Value.ToString("dd/MM/yyyy") : "Chưa xác định";
                    }
                    else
                    {
                        ViewBag.HoTen = string.Empty;
                        ViewBag.GioiTinh = string.Empty;
                        ViewBag.SoDienThoai = string.Empty;
                        ViewBag.DiaChi = string.Empty;
                        ViewBag.NgaySinh = "Chưa xác định";
                    }
                    ViewBag.AnhBia = Session["AnhBia"] ?? "~/Content/HocVien/Images/Default_Avatar.png";
                    return View();
                }
            }
            return RedirectToAction("DangNhap", "HocVien");
        }

        [HttpPost]
        public ActionResult UploadAvatar(HttpPostedFileBase avatar)
        {
            if (avatar != null && avatar.ContentLength > 0)
            {
                // Đường dẫn lưu ảnh
                string fileName = Path.GetFileName(avatar.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/HinhAnh/Avatar/"), fileName);
                Session["AnhBia"] = fileName;
                // Lưu ảnh vào thư mục
                avatar.SaveAs(path);

                // Cập nhật đường dẫn ảnh vào cơ sở dữ liệu
                var userId = Session["UserId"];
                var user = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == userId);
                if (user != null)
                {
                    user.AnhDaiDien = fileName; // Lưu tên file ảnh vào cơ sở dữ liệu
                    db.SubmitChanges();
                }

                return Json(new { success = true, filename = fileName });
            }
            return Json(new { success = false, message = "Có lỗi xảy ra khi tải lên ảnh." });
        }


        [HttpPost]
        public ActionResult ChinhSua(string email, string hoTen, DateTime? ngaySinh, string gioiTinh, string soDienThoai, string diaChi)
        {
            try
            {
                var userId = Session["UserId"];
                if (userId != null)
                {
                    var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == userId);
                    var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaHocVien == nguoiDung.MaNguoiDung);

                    if (nguoiDung != null && hocVien != null)
                    {
                        hocVien.Email = email ?? hocVien.Email;
                        hocVien.HoTen = hoTen ?? hocVien.HoTen;
                        hocVien.NgaySinh = ngaySinh ?? hocVien.NgaySinh;
                        hocVien.GioiTinh = gioiTinh ?? hocVien.GioiTinh;
                        hocVien.SoDienThoai = soDienThoai ?? hocVien.SoDienThoai;
                        hocVien.DiaChi = diaChi ?? hocVien.DiaChi;
                        db.SubmitChanges();
                        return Json(new { success = true, message = "Thông tin đã được cập nhật thành công!" });
                    }
                }
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật thông tin. Người dùng không tồn tại." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Cập nhật thất bại: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ChinhSuaEmail(string email)
        {
            try
            {
                var userId = Session["UserId"];
                if (userId != null)
                {
                    var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == userId);
                    var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaHocVien == nguoiDung.MaNguoiDung);

                    if (hocVien != null)
                    {
                        hocVien.Email = email;
                        db.SubmitChanges();
                        return Json(new { success = true, message = "Email đã được cập nhật thành công!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Người dùng không tồn tại." });
                    }
                }
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện chức năng này." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Cập nhật thất bại: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ChinhSuaSoDienThoai(string soDienThoai)
        {
            try
            {
                var userId = Session["UserId"];
                if (userId != null)
                {
                    var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaHocVien == userId);
                    if (hocVien != null)
                    {
                        hocVien.SoDienThoai = soDienThoai;
                        db.SubmitChanges();
                        return Json(new { success = true, message = "Số điện thoại đã được cập nhật thành công!" });
                    }
                }
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật số điện thoại. Người dùng không tồn tại." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Cập nhật thất bại: " + ex.Message });
            }
        }

        //[HttpPost]
        //public ActionResult DoiMatKhau(string currentPassword, string newPassword)
        //{
        //    var userId = Session["UserId"];
        //    if (userId == null)
        //    {
        //        return Json(new { success = false, message = "Bạn cần đăng nhập để đổi mật khẩu." });
        //    }
        //    var nguoiDung = db.NguoiDungs.FirstOrDefault(nd => nd.MaNguoiDung == userId);
        //    if (nguoiDung == null || nguoiDung.MatKhau != currentPassword)
        //    {
        //        return Json(new { success = false, message = "Mật khẩu hiện tại không đúng." });
        //    }
        //    if (!Regex.IsMatch(newPassword, @"[A-Z]")) 
        //    {
        //        return Json(new { success = false, message = "Mật khẩu mới phải chứa ít nhất một ký tự in hoa." });
        //    }

        //    if (!Regex.IsMatch(newPassword, @"[0-9]")) 
        //    {
        //        return Json(new { success = false, message = "Mật khẩu mới phải chứa ít nhất một chữ số." });
        //    }
        //    if (!Regex.IsMatch(newPassword,"[!@#$%^&*(),.?\":{}|<>]"))
        //    {
        //        return Json(new { success = false, message = "Mật khẩu mới phải chứa ít nhất một ký tự đặc biệt." });
        //    }
        //    nguoiDung.MatKhau = newPassword;
        //    db.SubmitChanges();  
        //    return Json(new { success = true, message = "Mật khẩu đã được cập nhật thành công!" });
        //}


       public ActionResult ThanhToan()
        {
            var maHocVien = Session["MaHocVien"].ToString();
            var thanhToanList = db.ThongTinThanhToans.Where(tt => tt.MaHocVien == maHocVien) .ToList(); 

            if (thanhToanList == null || !thanhToanList.Any())
            {
                ViewBag.ThongBao = "Không có dữ liệu thanh toán.";
                return View();
            }
            return View(thanhToanList);
        }

       public ActionResult NopBaiTap(string maBaiTap)
       {
           if (string.IsNullOrEmpty(maBaiTap))
           {
               return RedirectToAction("BaiTap", "HocVien");
           }

           ViewBag.MaBaiTap = maBaiTap;
           return View();
       }

       [HttpPost]
       public ActionResult XuLyNopBaiTap(string maBaiTap, HttpPostedFileBase fileUpload)
       {
           if (fileUpload != null && fileUpload.ContentLength > 0)
           {
               try
               {
                   // Tạo thư mục nếu chưa tồn tại
                   string uploadFolder = Server.MapPath("~/Uploads");
                   if (!Directory.Exists(uploadFolder))
                   {
                       Directory.CreateDirectory(uploadFolder);
                   }

                   // Kiểm tra kích thước file
                   if (fileUpload.ContentLength > 5 * 1024 * 1024) // Giới hạn 5MB
                   {
                       TempData["Error"] = "Dung lượng file vượt quá giới hạn (5MB).";
                       TempData["Success"] = null; // Xóa thông báo thành công nếu có
                       return RedirectToAction("NopBaiTap", "HocVien", new { maBaiTap });
                   }

                   // Lưu file
                   string fileName = Path.GetFileName(fileUpload.FileName);
                   string path = Path.Combine(uploadFolder, fileName);
                   fileUpload.SaveAs(path);

                   // Cập nhật cơ sở dữ liệu
                   var baiTap = db.DangKy_BaiTaps.FirstOrDefault(bt => bt.MaBaiTap == maBaiTap);
                   if (baiTap != null)
                   {
                       baiTap.FileUpload = fileName;
                       baiTap.TrangThai = "Đã nộp";
                       baiTap.NgayNop = DateTime.Now;
                       db.SubmitChanges();
                   }

                   // Xóa thông báo lỗi nếu có và hiển thị thông báo thành công
                   TempData["Error"] = null;
                   TempData["Success"] = "Bài tập đã được nộp thành công!";
               }
               catch (Exception ex)
               {
                   // Xóa thông báo thành công nếu có và hiển thị thông báo lỗi
                   TempData["Success"] = null;
                   TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                   return RedirectToAction("NopBaiTap", "HocVien", new { maBaiTap });
               }
           }
           else
           {
               // Hiển thị lỗi khi chưa chọn file
               TempData["Success"] = null; // Xóa thông báo thành công nếu có
               TempData["Error"] = "Bạn chưa chọn file!";
           }

           return RedirectToAction("NopBaiTap", "HocVien", new { maBaiTap });
       }



        // DANG NHAP
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string username, string password)
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.MaNguoiDung == db.AuthenticateUser(username, password));
            conn = "Data Source=THAIBINH-LAPTOP;Initial Catalog=QL_TrungTamAnhNgu;User ID=" + username + ";Password=" + password + "";
            db = new DataClasses1DataContext(conn);
            if (user != null)
            {
                Session["UserId"] = user.MaNguoiDung;
                Session["MaHocVien"] = user.HocVien.MaHocVien; 
                Session["User"] = user;
                Session["UserName"] = user.TenTaiKhoan;
                
                return RedirectToAction("TrangChu", "HocVien");
            }
            else
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            conn = "Data Source=THAIBINH-LAPTOP;Initial Catalog=QL_TrungTamAnhNgu;User ID=sa;Password=sa123";
            return RedirectToAction("DangNhap");
        }

        [HttpPost]
        public JsonResult DoiMatKhau(string username, string currentPassword, string newPassword)
        {
            try
            {
                // Kiểm tra username và mật khẩu hiện tại
                var user = db.NguoiDungs.SingleOrDefault(u => u.MaNguoiDung == db.AuthenticateUser(username, currentPassword));
                if (user == null)
                {
                    return Json(new { success = false, message = "Mật khẩu hiện tại không đúng hoặc tài khoản không tồn tại." });
                }
                using (db = new DataClasses1DataContext("Data Source=THAIBINH-LAPTOP;Initial Catalog=QL_TrungTamAnhNgu;User ID=sa;Password=sa123"))
                {
                    // Đổi mật khẩu
                    db.CapNhatMatKhau(username, currentPassword, newPassword);

                }

                conn = "Data Source=THAIBINH-LAPTOP;Initial Catalog=QL_TrungTamAnhNgu;User ID=" + username + ";Password=" + newPassword + "";
                db = new DataClasses1DataContext(conn);

                return Json(new { success = true, message = "Đổi mật khẩu thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

    }
}
