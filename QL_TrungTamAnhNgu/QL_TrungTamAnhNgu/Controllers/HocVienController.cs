using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_TrungTamAnhNgu.Models;

namespace QL_TrungTamAnhNgu.Controllers
{
    public class HocVienController : Controller
    {
        // GET: /HocVien/
        DataClasses1DataContext db = new DataClasses1DataContext();
        public ActionResult TrangChu()
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
            // Lấy mã học viên từ session (lúc đăng nhập)
            var maHocVien = Session["MaHocVien"].ToString();
            if (string.IsNullOrEmpty(maHocVien))
            {
                ViewBag.ThongBao = "Không tìm thấy mã học viên.";
                return RedirectToAction("TrangChu", "HocVien"); // Redirect về trang chủ nếu không có mã học viên
            }

            // Lấy thông tin mã thanh toán từ bảng ThanhToan dựa trên mã học viên
            var maThanhToan = db.ThanhToans
                                .Where(t => t.MaHocVien == maHocVien)
                                .Select(t => t.MaThanhToan)
                                .FirstOrDefault();

            if (string.IsNullOrEmpty(maThanhToan))
            {
                ViewBag.ThongBao = "Không tìm thấy thông tin thanh toán cho học viên này.";
                return View(); // Nếu không có thanh toán, hiển thị thông báo
            }

            // Lấy danh sách lớp học mà sinh viên đã đăng ký từ bảng DangKy thông qua MaThanhToan
            var danhSachLopHoc = db.DangKies
                                   .Where(dk => dk.MaThanhToan == maThanhToan)  // Lọc theo mã thanh toán
                                   .Select(dk => dk.MaLop)  // Lấy danh sách mã lớp
                                   .ToList();

            if (!danhSachLopHoc.Any())
            {
                ViewBag.ThongBao = "Bạn chưa đăng ký lớp học nào.";
                return View(); // Nếu không có lớp học, hiển thị thông báo
            }

            // Lấy thông tin lịch học chi tiết của các lớp mà sinh viên đã đăng ký
            var danhSachLichHoc = db.view_lichhoc_chitiet_cua_lophocs
                                    .Where(lh => danhSachLopHoc.Contains(lh.MaLop)) // Kiểm tra các lớp đã đăng ký
                                    .ToList();

            if (!danhSachLichHoc.Any())
            {
                ViewBag.ThongBao = "Không tìm thấy lịch học cho các lớp đã đăng ký.";
                return View(); // Nếu không có lịch học, hiển thị thông báo
            }

            return View(danhSachLichHoc); // Trả lại view với danh sách lịch học
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
            var user = db.NguoiDungs.FirstOrDefault(u => u.TenTaiKhoan == username && u.MatKhau == password);
            var hocVien = db.HocViens.FirstOrDefault(hv => hv.MaNguoiDung == user.MaNguoiDung);
            if (user != null)
            {
                Session["UserId"] = user.MaNguoiDung;
                Session["MaHocVien"] = hocVien.MaHocVien; 
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
            return RedirectToAction("DangNhap");
        } 
    }
}
