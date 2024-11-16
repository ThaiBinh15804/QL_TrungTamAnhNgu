using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using QL_TrungTamAnhNgu.Models;

namespace QL_TrungTamAnhNgu.Controllers
{
    public class QuanTriVienController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public int GetSoLuongNguoiDung_KhoaHoc_PhongHoc(string loaiNguoiDung)
        {
            using (var db = new DataClasses1DataContext())
            {
                // Xây dựng câu lệnh SQL động
                var sql = "EXEC soluong_hocvien_giangvien_khoahoc_phonghoc @LoaiNguoiDung = '" + loaiNguoiDung + "'";

                // Thực thi câu lệnh SQL với tham số và trả về kết quả
                var result = db.ExecuteQuery<int>(sql).FirstOrDefault();
                return result;
            }
        }

        public ActionResult GetDangKyTheoThang()
        {
            using (var db = new DataClasses1DataContext())
            {
                // Gọi stored procedure thông qua DataContext
                var sql = "EXEC soluong_dangky_theo_thang";
                List<SoluongDangKyTheoThangResult> result = db.ExecuteQuery<SoluongDangKyTheoThangResult>(sql).ToList();
                return PartialView(result);
            }
        }

        public ActionResult GetLopHocTrungTam()
        {
            using (var db = new DataClasses1DataContext())
            {
                // Gọi stored procedure thông qua DataContext
                var sql = "select * from thongke_caclop_trungtam";
                var result = db.ExecuteQuery<LopHocTrungTam>(sql).ToList();
                return PartialView(result);
            }
        }

        public ActionResult GetDoanhThuTheoThang()
        {
            using (var db = new DataClasses1DataContext())
            {
                // Gọi stored procedure thông qua DataContext
                var sql = "EXEC doanhthu_theo_thang";
                List<DoanhThuTheoThang> result = db.ExecuteQuery<DoanhThuTheoThang>(sql).ToList();

                // Chuyển dữ liệu sang chuỗi JSON
                var jsonResult = JsonConvert.SerializeObject(result);
                ViewBag.DoanhThuData = jsonResult;

                return PartialView(result);
            }
        }

        public ActionResult GetThongKeSoTuoi()
        {
            using (var db = new DataClasses1DataContext())
            {
                // Gọi stored procedure thông qua DataContext
                var sql = "select * from thongke_tuoi_hocvien";
                var result = db.ExecuteQuery<ThongKeSoTuoi>(sql).ToList();

                // Chuyển dữ liệu sang chuỗi JSON
                var jsonResult = JsonConvert.SerializeObject(result);
                ViewBag.SoTuoi = jsonResult;

                return PartialView(result);
            }
        }

        public ActionResult GetThongKeGioiTinh()
        {
            using (var db = new DataClasses1DataContext())
            {
                // Gọi stored procedure thông qua DataContext
                var sql = "select * from thongke_gioitinh_hocvien";
                var result = db.ExecuteQuery<ThongKeGioiTinh>(sql).ToList();

                // Chuyển dữ liệu sang chuỗi JSON
                var jsonResult = JsonConvert.SerializeObject(result);
                ViewBag.GioiTinh = jsonResult;

                return PartialView(result);
            }
        }

        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            // Gọi stored procedure để lấy số lượng học viên và giảng viên
            var soLuongHocVien = GetSoLuongNguoiDung_KhoaHoc_PhongHoc("HocVien");
            var soLuongGiangVien = GetSoLuongNguoiDung_KhoaHoc_PhongHoc("GiangVien");
            var soLuongKhoaHoc = GetSoLuongNguoiDung_KhoaHoc_PhongHoc("KhoaHoc");
            var soLuongPhongHoc = GetSoLuongNguoiDung_KhoaHoc_PhongHoc("PhongHoc");

            // Trả về kết quả vào view
            ViewBag.SoLuongHocVien = soLuongHocVien;
            ViewBag.SoLuongGiangVien = soLuongGiangVien;
            ViewBag.SoLuongKhoaHoc = soLuongKhoaHoc;
            ViewBag.SoLuongPhongHoc = soLuongPhongHoc;

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
            NguoiDung user = db.NguoiDungs.FirstOrDefault(k => k.TenTaiKhoan == username && k.MatKhau == password);
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
                string newConnectionString="Data Source=PHAMTHUAN\\MSSQLSERVER01;Initial Catalog=QL_TrungTamAnhNgu;Persist Security Info=True;User ID=" + username + ";Password=" + password;
                db = new DataClasses1DataContext(newConnectionString);


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
            } catch (Exception ex)
            {
                // Nếu có lỗi khi kết nối hoặc truy vấn, thông báo cho người dùng
                ViewBag.text = "Không thể kết nối đến cơ sở dữ liệu hoặc quyền truy cập bị từ chối. Lỗi: " + ex.Message;
                return View();
            }
            return View();
        }

        public ActionResult DangXuat()
        {
            if (Session["user"] != null)
            {
                Session["user"] = null;
            }
            return RedirectToAction("DangNhap");
        }

        public ActionResult DanhSachKhoaHoc(int page = 1, int pageSize = 4)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<KhoaHoc> listKH = db.KhoaHocs.ToList();
            var totalRecords = listKH.Count();

            var dsKHList = listKH.OrderBy(q => q.MaKhoaHoc).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var model = new KhoaHocPagedList
            {
                DsKhoaHoc = dsKHList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult TimKiemKhoaHoc(string search, int page = 1, int pageSize = 4)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            using (var db = new DataClasses1DataContext())
            {
                // Gọi stored procedure thông qua DataContext
                var sql = "select * from fn_timKiemKhoaHoc(N'" + search + "')";
                IEnumerable<KhoaHoc> listKH = db.ExecuteQuery<KhoaHoc>(sql).ToList();

                var totalRecords = listKH.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                var khoahocList = listKH.OrderBy(q => q.MaKhoaHoc).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new KhoaHocPagedList
                {
                    DsKhoaHoc = khoahocList,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    SearchQuery = search
                };

                return View(model);
            }
        }

        public ActionResult DanhSachKhoaHocThamKhao()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<KhoaHoc> listKH = db.KhoaHocs.ToList();
            var dsKHList = listKH.OrderBy(q => q.MaKhoaHoc).Take(4).ToList();
            return PartialView(dsKHList);
        }


        public ActionResult ChiTietKhoaHoc(string makh)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            using (var db = new DataClasses1DataContext())
            {
                // Gọi stored procedure thông qua DataContext
                var sql = "select * from fn_chiTietKhoaHoc('" + makh + "')";
                KhoaHoc KhoaHocDetail = db.ExecuteQuery<KhoaHoc>(sql).FirstOrDefault();

                return View(KhoaHocDetail);
            }
        }
    }
}
