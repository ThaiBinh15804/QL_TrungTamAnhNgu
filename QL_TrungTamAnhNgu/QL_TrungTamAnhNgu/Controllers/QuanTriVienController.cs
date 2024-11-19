using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using System.Web.Security;
using Newtonsoft.Json;
using QL_TrungTamAnhNgu.Models;
using QL_TrungTamAnhNgu.ViewModel;
using System.IO;

namespace QL_TrungTamAnhNgu.Controllers
{
    [Authorize]
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
                string newConnectionString="Data Source=PHAMTHUAN\\MSSQLSERVER01;Initial Catalog=QL_TrungTamAnhNgu;Persist Security Info=True;User ID=" + username + ";Password=" + password;
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
                FormsAuthentication.SignOut();
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
        public ActionResult ThemKhoaHoc()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var model = new KhoaHoc();
            return View(model);
        }
        [HttpPost]
        public ActionResult ThemKhoaHoc(KhoaHoc kh)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            try
            {
                string maKH = db.KhoaHocs.OrderByDescending(t => t.MaKhoaHoc).FirstOrDefault().MaKhoaHoc;
                int k = maKH != null ? (int.Parse(maKH.Substring(2)) + 1) : 1;
                string maKHMoi = "KH" + k.ToString("D3");
                // Thêm người dùng vào bảng NguoiDung
                var khoahoc = new KhoaHoc
                {
                    MaKhoaHoc = maKHMoi,
                    TenKhoaHoc = kh.TenKhoaHoc,
                    MoTa = kh.MoTa,
                    HocPhi = kh.HocPhi,
                    AnhBia = kh.AnhBia,
                    NguoiTao = kh.NguoiTao,
                    CapDo = kh.CapDo,
                    NgayTao = DateTime.Now,
                    TrangThai = "Đang hoạt động",
                };

                db.KhoaHocs.InsertOnSubmit(khoahoc);
                db.SubmitChanges();

                return RedirectToAction("DanhSachKhoaHoc");
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi từ trigger: " + sqlEx.Message;
                return View(kh);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(kh);
            }
        }
        public ActionResult ViewDanhSachTaiLieu(string makh)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<KhoaHoc_TaiLieu> dsTaiLieuHienThi = db.KhoaHoc_TaiLieus.Where(t => t.MaKhoaHoc == makh).ToList();
            return PartialView(dsTaiLieuHienThi);
        }

        public ActionResult ChiTietKhoaHoc(string makh)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var sql = "select * from fn_chiTietKhoaHoc('" + makh + "')";
            KhoaHoc KhoaHocDetail = db.ExecuteQuery<KhoaHoc>(sql).FirstOrDefault();

            return View(KhoaHocDetail);
        }

        [HttpPost]
        public ActionResult ChiTietKhoaHoc(KhoaHoc kh)
        {
            var khoaHocExists = db.KhoaHocs.FirstOrDefault(k => k.MaKhoaHoc == kh.MaKhoaHoc);
            if(khoaHocExists != null)
            {
                if (string.IsNullOrEmpty(kh.AnhBia))
                {
                    kh.AnhBia = khoaHocExists.AnhBia;
                }
                khoaHocExists.TenKhoaHoc = kh.TenKhoaHoc;
                khoaHocExists.MoTa = kh.MoTa;
                khoaHocExists.HocPhi = kh.HocPhi;
                khoaHocExists.CapDo = kh.CapDo;
                khoaHocExists.AnhBia = kh.AnhBia;
                khoaHocExists.TrangThai = kh.TrangThai;

                db.SubmitChanges();
                return RedirectToAction("DanhSachKhoaHoc");
            }
            return View(kh);
        }

        public ActionResult XoaKhoaHoc(string makh)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            KhoaHoc khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.MaKhoaHoc == makh);
            if (khoaHoc.LopHocs.Any())
            {
                TempData["ErrorMessageKhoaHoc"] = "Khóa học này đang được sử dụng, nên không thể xóa ngay lúc này!";
                return RedirectToAction("ChiTietKhoaHoc", new { makh = makh });
            }
            db.KhoaHocs.DeleteOnSubmit(khoaHoc);
            db.SubmitChanges();
            return RedirectToAction("DanhSachKhoaHoc");
        }

        public ActionResult QuanTriVien(int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<QuanTriVien> dsAdmin = db.QuanTriViens.ToList();
            var totalRecords = dsAdmin.Count();

            var adminList = dsAdmin.OrderBy(q => q.MaQTV).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var model = new QuanTriVienPagedList
            {
                AdminList = adminList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult TimKiemQuanTriVien(string search, int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var sql = "select * from fn_timKiemQuanTriVien(N'" + search + "')";
            List<QuanTriVien> listQTV = db.ExecuteQuery<QuanTriVien>(sql).ToList();

            var totalRecords = listQTV.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var QuanTriVienList = listQTV.OrderBy(q => q.MaQTV).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new QuanTriVienPagedList
            {
                AdminList = QuanTriVienList,
                PageSize = pageSize,    
                TotalPages = totalPages,
                CurrentPage = page,
                SearchQuery = search
            };

            return View(model);
        }
        public string getChucVu(string MaNhomND) {
            if (MaNhomND == "NND001")
            {
                return "Quản trị viên";
            }
            else if (MaNhomND == "NND004")
            {
                return "Kĩ thuật viên";
            } else if (MaNhomND == "NND005")
            {
                return "Thu ngân";
            } 
            return "Quản lý cơ sở vật chất";
        }
        public ActionResult ThemQuanTriVien()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var model = new QuanTriVien();
            return View(model);
        }
        [HttpPost]
        public ActionResult ThemQuanTriVien(QuanTriVien qtv)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            try
            {
                string maND = db.NguoiDungs.OrderByDescending(t => t.MaNguoiDung).FirstOrDefault().MaNguoiDung;
                int k = maND != null ? (int.Parse(maND.Substring(2)) + 1) : 1;
                string maMoi = "ND" + k.ToString("D3");
                // Thêm người dùng vào bảng NguoiDung
                var nguoiDung = new NguoiDung
                {
                    MaNguoiDung = maMoi,
                    TenTaiKhoan = qtv.NguoiDung.TenTaiKhoan,
                    MatKhau = qtv.NguoiDung.MatKhau,
                    AnhDaiDien = qtv.NguoiDung.AnhDaiDien,
                    NgayTao = DateTime.Now,
                    TrangThai = "Đang hoạt động",
                    MaNhomND = qtv.NguoiDung.MaNhomND
                };

                db.NguoiDungs.InsertOnSubmit(nguoiDung);
                db.SubmitChanges();

                db.CreateUserAccount(nguoiDung.TenTaiKhoan, nguoiDung.MatKhau, nguoiDung.MaNhomND);

                var quanTriVien = new QuanTriVien
                {
                    MaQTV = nguoiDung.MaNguoiDung,
                    HoTen = qtv.HoTen,
                    SoDienThoai = qtv.SoDienThoai,
                    Email = qtv.Email,
                    ChucVu = getChucVu(qtv.NguoiDung.MaNhomND),
                };

                db.QuanTriViens.InsertOnSubmit(quanTriVien);
                db.SubmitChanges();

                return RedirectToAction("QuanTriVien");
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi từ trigger: " + sqlEx.Message;
                return View(qtv);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(qtv);
            }
        }

        public ActionResult ChiTietQuanTriVien(string maQTV)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var sql = "select * from fn_chiTietQuanTriVien('" + maQTV + "')";
            QuanTriVien quanTriVienDetail = db.ExecuteQuery<QuanTriVien>(sql).FirstOrDefault();

            return View(quanTriVienDetail);
        }
        [HttpPost]
        public ActionResult ChiTietQuanTriVien(QuanTriVien qtv)
        {
            var quanTriVienExists = db.QuanTriViens.FirstOrDefault(k => k.MaQTV == qtv.MaQTV);
            if (quanTriVienExists != null)
            {
                if (string.IsNullOrEmpty(qtv.NguoiDung.AnhDaiDien))
                {
                    qtv.NguoiDung.AnhDaiDien = quanTriVienExists.NguoiDung.AnhDaiDien;
                }
                quanTriVienExists.HoTen = qtv.HoTen;
                quanTriVienExists.Email = qtv.Email;
                quanTriVienExists.SoDienThoai = qtv.SoDienThoai;
                quanTriVienExists.NguoiDung.TrangThai = qtv.NguoiDung.TrangThai;
                quanTriVienExists.NguoiDung.AnhDaiDien = qtv.NguoiDung.AnhDaiDien;
                db.SubmitChanges();
                return RedirectToAction("QuanTriVien");
            }
            return View(qtv);
        }

        public ActionResult GiangVien(int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<GiangVien> dsTeacher = db.GiangViens.ToList();
            var totalRecords = dsTeacher.Count();

            var teacherList = dsTeacher.OrderBy(q => q.MaGiangVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var model = new GiangVienPagedList
            {
                TeacherList = teacherList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }

        public ActionResult TimKiemGiangVien(string search, int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var sql = "select * from fn_timKiemGiangVien(N'" + search + "')";
            List<GiangVien> listGV = db.ExecuteQuery<GiangVien>(sql).ToList();

            var totalRecords = listGV.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var GiangVienList = listGV.OrderBy(q => q.MaGiangVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new GiangVienPagedList
            {
                TeacherList = GiangVienList,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentPage = page,
                SearchQuery = search
            };

            return View(model);
        }

        public ActionResult ThemGiangVien()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var model = new GiangVien();
            return View(model);
        }
        [HttpPost]
        public ActionResult ThemGiangVien(GiangVien gv)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
                try
                {
                    string maND = db.NguoiDungs.OrderByDescending(t => t.MaNguoiDung).FirstOrDefault().MaNguoiDung;
                    int k = maND != null ? (int.Parse(maND.Substring(2)) + 1) : 1;
                    string maMoi = "ND" + k.ToString("D3");
                    // Thêm người dùng vào bảng NguoiDung
                    var nguoiDung = new NguoiDung
                    {
                        MaNguoiDung = maMoi,
                        TenTaiKhoan = gv.NguoiDung.TenTaiKhoan,
                        MatKhau = gv.NguoiDung.MatKhau,
                        AnhDaiDien = gv.NguoiDung.AnhDaiDien,
                        NgayTao = DateTime.Now,
                        TrangThai = "Đang hoạt động",
                        MaNhomND = "NND002"
                    };

                    db.NguoiDungs.InsertOnSubmit(nguoiDung);
                    db.SubmitChanges(); // Lưu thay đổi vào bảng NguoiDung

                    // Gọi procedure tạo tài khoản người dùng
                    db.CreateUserAccount(nguoiDung.TenTaiKhoan, nguoiDung.MatKhau, nguoiDung.MaNhomND);

                    var giangVien = new GiangVien
                    {
                        MaGiangVien = nguoiDung.MaNguoiDung,
                        HoTen = gv.HoTen,
                        GioiTinh = gv.GioiTinh,
                        NgaySinh = gv.NgaySinh,
                        DiaChi = gv.DiaChi,
                        SoDienThoai = gv.SoDienThoai,
                        Email = gv.Email,
                        MucLuong = gv.MucLuong,
                        TrinhDo = gv.TrinhDo,
                        ChuyenMon = gv.ChuyenMon,
                        NgayVaoLam = gv.NgayVaoLam
                    };

                    db.GiangViens.InsertOnSubmit(giangVien);
                    db.SubmitChanges(); // Lưu thay đổi vào bảng GiangVien

                    return RedirectToAction("GiangVien");
                }
                catch (SqlException sqlEx)
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi từ trigger: " + sqlEx.Message;
                    return View(gv);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                    return View(gv);
                }
        }

        public ActionResult ChiTietGiangVien(string maGV)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var sql = "select * from fn_chiTietGiangVien('" + maGV + "')";
            GiangVien giangVienDetail = db.ExecuteQuery<GiangVien>(sql).FirstOrDefault();
            return View(giangVienDetail);
        }
        [HttpPost]
        public ActionResult ChiTietGiangVien(GiangVien gv)
        {
            var giangVienExists = db.GiangViens.FirstOrDefault(k => k.MaGiangVien == gv.MaGiangVien);
            if (giangVienExists != null)
            {
                if (string.IsNullOrEmpty(gv.NguoiDung.AnhDaiDien))
                {
                    gv.NguoiDung.AnhDaiDien = giangVienExists.NguoiDung.AnhDaiDien;
                }
                giangVienExists.HoTen = gv.HoTen;
                giangVienExists.Email = gv.Email;
                giangVienExists.SoDienThoai = gv.SoDienThoai;
                giangVienExists.GioiTinh = gv.GioiTinh;
                giangVienExists.NgaySinh = gv.NgaySinh;
                giangVienExists.ChuyenMon = gv.ChuyenMon;
                giangVienExists.TrinhDo = gv.TrinhDo;
                giangVienExists.MucLuong = gv.MucLuong;
                giangVienExists.DiaChi = gv.DiaChi;

                giangVienExists.NguoiDung.AnhDaiDien = gv.NguoiDung.AnhDaiDien;
                giangVienExists.NguoiDung.TrangThai = gv.NguoiDung.TrangThai;

                db.SubmitChanges();
                return RedirectToAction("GiangVien");
            }
            return View(gv);
        }

        public ActionResult XoaGiangVien(string maGV)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var giangVien = db.GiangViens.FirstOrDefault(k => k.MaGiangVien == maGV);
            if (giangVien != null)
            {
                db.GiangViens.DeleteOnSubmit(giangVien);
                db.SubmitChanges();
            }
            return RedirectToAction("GiangVien");
        }

        public ActionResult HocVien(int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<HocVien> dsStudent = db.HocViens.ToList();
            var totalRecords = dsStudent.Count();

            var studentList = dsStudent.OrderByDescending(q => q.NguoiDung.NgayTao).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var model = new HocVienPagedList
            {
                StudentList = studentList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }

        public ActionResult TimKiemHocVien(string search, int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var sql = "select * from fn_timKiemHocVien(N'" + search + "')";
            List<HocVien> listGV = db.ExecuteQuery<HocVien>(sql).ToList();

            var totalRecords = listGV.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var GiangVienList = listGV.OrderBy(q => q.MaHocVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new HocVienPagedList
            {
                StudentList = GiangVienList,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentPage = page,
                SearchQuery = search
            };

            return View(model);
        }

        public ActionResult ThemHocVien()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var model = new HocVien();
            return View(model);
        }
        [HttpPost]
        public ActionResult ThemHocVien(HocVien hv)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            try
            {
                    string maND = db.NguoiDungs.OrderByDescending(t => t.MaNguoiDung).FirstOrDefault().MaNguoiDung;
                    int k = maND != null ? (int.Parse(maND.Substring(2)) + 1) : 1;
                    string maMoi = "ND" + k.ToString("D3");
                    // Thêm người dùng vào bảng NguoiDung
                    var nguoiDung = new NguoiDung
                    {
                        MaNguoiDung = maMoi,
                        TenTaiKhoan = hv.NguoiDung.TenTaiKhoan,
                        MatKhau = hv.NguoiDung.MatKhau,
                        AnhDaiDien = hv.NguoiDung.AnhDaiDien,
                        NgayTao = DateTime.Now,
                        TrangThai = "Đang hoạt động",
                        MaNhomND = "NND003"
                    };

                    db.NguoiDungs.InsertOnSubmit(nguoiDung);
                    db.SubmitChanges(); // Lưu thay đổi vào bảng NguoiDung

                    db.CreateUserAccount(nguoiDung.TenTaiKhoan, nguoiDung.MatKhau, nguoiDung.MaNhomND);

                    var hocVien = new HocVien
                    {
                        MaHocVien = nguoiDung.MaNguoiDung,
                        HoTen = hv.HoTen,
                        GioiTinh = hv.GioiTinh,
                        NgaySinh = hv.NgaySinh,
                        DiaChi = hv.DiaChi,
                        SoDienThoai = hv.SoDienThoai,
                        Email = hv.Email,
                        MucTieuHocTap = hv.MucTieuHocTap,
                        TrinhDo = hv.TrinhDo,
                        NgayThamGia = hv.NgayThamGia
                    };

                    db.HocViens.InsertOnSubmit(hocVien);
                    db.SubmitChanges(); // Lưu thay đổi vào bảng GiangVien


                    return RedirectToAction("HocVien");
                }
                catch (SqlException sqlEx)
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi từ trigger: " + sqlEx.Message;
                    return View(hv);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                    return View(hv);
                }
        }

        public ActionResult ChiTietHocVien(string maHV)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var sql = "select * from fn_chiTietHocVien('" + maHV + "')";
            HocVien hocVienDetail = db.ExecuteQuery<HocVien>(sql).FirstOrDefault();
            return View(hocVienDetail);
        }
        [HttpPost]
        public ActionResult ChiTietHocVien(HocVien hv)
        {
            var hocVienExists = db.HocViens.FirstOrDefault(k => k.MaHocVien == hv.MaHocVien);
            if (hocVienExists != null)
            {
                if (string.IsNullOrEmpty(hv.NguoiDung.AnhDaiDien))
                {
                    hv.NguoiDung.AnhDaiDien = hocVienExists.NguoiDung.AnhDaiDien;
                }
                hocVienExists.HoTen = hv.HoTen;
                hocVienExists.Email = hv.Email;
                hocVienExists.SoDienThoai = hv.SoDienThoai;
                hocVienExists.GioiTinh = hv.GioiTinh;
                hocVienExists.NgaySinh = hv.NgaySinh;
                hocVienExists.MucTieuHocTap = hv.MucTieuHocTap;
                hocVienExists.TrinhDo = hv.TrinhDo;
                hocVienExists.DiaChi = hv.DiaChi;

                hocVienExists.NguoiDung.AnhDaiDien = hv.NguoiDung.AnhDaiDien;
                hocVienExists.NguoiDung.TrangThai = hv.NguoiDung.TrangThai;

                db.SubmitChanges();
                return RedirectToAction("HocVien");
            }
            return View(hv);
        }

        public ActionResult XoaHocVien(string maHV)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var hocVien = db.HocViens.FirstOrDefault(k => k.MaHocVien == maHV);
            if (hocVien != null)
            {
                db.HocViens.DeleteOnSubmit(hocVien);
                db.SubmitChanges();
            }
            return RedirectToAction("HocVien");
        }

        public ActionResult DanhSachMaGiamGia(int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<GiamGia> giamGiaList = db.GiamGias.ToList();
            var totalRecords = giamGiaList.Count();

            var dsGiamGiaList = giamGiaList.OrderBy(q => q.MaGiamGia).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var model = new GiamGiaPagedList
            {
                giamGiaList = dsGiamGiaList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };
            return View(model);
        }
        [HttpGet]
        public ActionResult TimKiemGiamGia(string search, int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var sql = "select * from fn_timKiemMaGiamGia(N'" + search + "')";
            List<GiamGia> giamGiaList = db.ExecuteQuery<GiamGia>(sql).ToList();
            var totalRecords = giamGiaList.Count();

            var dsGiamGiaList = giamGiaList.OrderBy(q => q.MaGiamGia).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var model = new GiamGiaPagedList
            {
                giamGiaList = dsGiamGiaList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = search
            };
            return View(model);
        }

        public ActionResult ThemGiamGia()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var giamGia = new GiamGia();
            return View(giamGia);
        }
        [HttpPost]
        public ActionResult ThemGiamGia(GiamGia gg)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            try
            {
                string maGiamGia = db.GiamGias.OrderByDescending(t => t.MaGiamGia).FirstOrDefault().MaGiamGia;
                int k = maGiamGia != null ? (int.Parse(maGiamGia.Substring(2)) + 1) : 1;
                string maGiamGiaMoi = "GG" + k.ToString("D3"); 
                var giamGia = new GiamGia
                {
                    MaGiamGia = maGiamGiaMoi,
                    TenGiamGia = gg.TenGiamGia,
                    MoTa = gg.MoTa,
                    TiLeGiam = gg.TiLeGiam,
                    NgayBatDau = gg.NgayBatDau,
                    NgayKetThuc = gg.NgayKetThuc,
                    NgayTao = DateTime.Now,
                    TrangThai = "Đang hoạt động",
                };

                db.GiamGias.InsertOnSubmit(giamGia);
                db.SubmitChanges();

                return RedirectToAction("DanhSachMaGiamGia");
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi từ trigger: " + sqlEx.Message;
                return View(gg);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(gg);
            }
        }

        public ActionResult XoaGiamGia(string maGG)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            GiamGia giamGia = db.GiamGias.FirstOrDefault(k => k.MaGiamGia == maGG);
            if (giamGia.KhoaHoc_GiamGias.Any())
            {
                TempData["ErrorMessageGiamGia"] = "Mã Giảm giá đang được sử dụng, nên không thể xóa!";
                return RedirectToAction("ChiTietGiamGia", new { maGG = maGG });
            }
            db.GiamGias.DeleteOnSubmit(giamGia);
            db.SubmitChanges();
            return RedirectToAction("DanhSachMaGiamGia");
        }

        public ActionResult ChiTietGiamGia(string maGG)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            GiamGia giamGia = db.GiamGias.FirstOrDefault(t => t.MaGiamGia == maGG);
            return View(giamGia);
        }
        [HttpPost]
        public ActionResult ChiTietGiamGia(GiamGia giamgia)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var giamGiaExists = db.GiamGias.FirstOrDefault(k => k.MaGiamGia == giamgia.MaGiamGia);
            if (giamGiaExists != null)
            {
                giamGiaExists.TenGiamGia = giamgia.TenGiamGia;
                giamGiaExists.MoTa = giamgia.MoTa;
                giamGiaExists.TrangThai = giamgia.TrangThai;
                giamGiaExists.NgayBatDau = giamgia.NgayBatDau;
                giamGiaExists.NgayKetThuc = giamgia.NgayKetThuc;
                giamGiaExists.TiLeGiam = giamgia.TiLeGiam;
                db.SubmitChanges();
                return RedirectToAction("DanhSachMaGiamGia");
            }
            return View(giamgia);
        }

        public ActionResult DanhSachGiamGiaThuocKhoaHoc(string maGG)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            ViewBag.MaGiamGia = maGG;
            GiamGia checkTrangThaiGiamGia = db.GiamGias.FirstOrDefault(t => t.MaGiamGia == maGG);
            if (checkTrangThaiGiamGia.TrangThai == "Hết hạn")
            {
                ViewBag.TrangThaiKhoaHoc = "Mã giảm giá hết hạn, nên không thể thêm khóa học";
            }
            List<KhoaHoc_GiamGia> dsGiamGiaThuocKhoaHoc = db.KhoaHoc_GiamGias.Where(k => k.MaGiamGia == maGG).ToList();
            return View(dsGiamGiaThuocKhoaHoc);
        }
        public ActionResult XoaKhoaHoc_GiamGia(string maKH, string maGG)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            KhoaHoc_GiamGia khoaHoc_giamGia = db.KhoaHoc_GiamGias.FirstOrDefault(t => t.MaGiamGia == maGG && t.MaKhoaHoc == maKH);
            db.KhoaHoc_GiamGias.DeleteOnSubmit(khoaHoc_giamGia);
            db.SubmitChanges();
            return RedirectToAction("DanhSachGiamGiaThuocKhoaHoc", new { maGG = maGG });
        }
        public ActionResult ThemKhoaHoc_GiamGia(string maGG)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            ViewBag.MaGiamGia = maGG;
            var khoahoc_giamgia = new KhoaHoc_GiamGia();
            return View(khoahoc_giamgia);
        }
        [HttpPost]
        public ActionResult ThemKhoaHoc_GiamGia(KhoaHoc_GiamGia khgg)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var khoahoc_giamgia = new KhoaHoc_GiamGia
            {
                MaKhoaHoc = khgg.MaKhoaHoc,
                MaGiamGia = khgg.MaGiamGia
            };
            KhoaHoc_GiamGia findKhoaHocGiamGia = db.KhoaHoc_GiamGias.FirstOrDefault(t => t.MaGiamGia == khoahoc_giamgia.MaGiamGia && t.MaKhoaHoc == khoahoc_giamgia.MaKhoaHoc);
            if (findKhoaHocGiamGia != null)
            {
                TempData["ErrorMessageKhoaHocGiamGia"] = "Khóa học đã tồn tại mã giảm giá này!";
                return RedirectToAction("ThemKhoaHoc_GiamGia", new { maGG = khoahoc_giamgia.MaGiamGia });
            }
            db.KhoaHoc_GiamGias.InsertOnSubmit(khoahoc_giamgia);
            db.SubmitChanges();
            return RedirectToAction("DanhSachGiamGiaThuocKhoaHoc", new { maGG = khoahoc_giamgia.MaGiamGia });
        }

        public ActionResult DanhSachTaiLieu(int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<TaiLieu> taiLieuList = db.TaiLieus.ToList();
            var totalRecords = taiLieuList.Count();

            var dsTaiLieuList = taiLieuList.OrderBy(q => q.MaTaiLieu).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var model = new TaiLieuPagedList
            {
                taiLieuList = dsTaiLieuList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };
            return View(model);
        }
        [HttpGet]
        public ActionResult TimKiemTaiLieu(string search, int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var sql = "select * from fn_timKiemTaiLieu(N'" + search + "')";
            List<TaiLieu> taiLieuList = db.ExecuteQuery<TaiLieu>(sql).ToList();
            var totalRecords = taiLieuList.Count();

            var dsTaiLieuList = taiLieuList.OrderBy(q => q.MaTaiLieu).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var model = new TaiLieuPagedList
            {
                taiLieuList = dsTaiLieuList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                SearchQuery = search
            };
            return View(model);
        }

        public ActionResult ThemTaiLieu()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var tailieu = new TaiLieu();
            return View(tailieu);
        }
        [HttpPost]
        public ActionResult ThemTaiLieu(TaiLieu tl)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            try
            {
                string maTaiLieu = db.TaiLieus.OrderByDescending(t => t.MaTaiLieu).FirstOrDefault().MaTaiLieu;
                int k = maTaiLieu != null ? (int.Parse(maTaiLieu.Substring(2)) + 1) : 1;
                string maTaiLieuMoi = "TL" + k.ToString("D3"); 
                var tailieu = new TaiLieu
                {
                    MaTaiLieu = maTaiLieuMoi,
                    TenTaiLieu = tl.TenTaiLieu,
                    MoTa = tl.MoTa,
                    FileUpload = tl.FileUpload,
                    NgayTao = DateTime.Now,
                    TrangThai = "Đang hoạt động",
                };

                db.TaiLieus.InsertOnSubmit(tailieu);
                db.SubmitChanges();

                return RedirectToAction("DanhSachTaiLieu");
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi từ trigger: " + sqlEx.Message;
                return View(tl);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(tl);
            }
        }
        public ActionResult XoaTaiLieu(string maTL)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            TaiLieu taiLieu = db.TaiLieus.FirstOrDefault(k => k.MaTaiLieu == maTL);
            if (taiLieu.KhoaHoc_TaiLieus.Any())
            {
                TempData["ErrorMessageTaiLieu"] = "Tài liệu này đang được sử dụng, nên không thể xóa!";
                return RedirectToAction("ChiTietTaiLieu", new { maTL = maTL });
            }
            db.TaiLieus.DeleteOnSubmit(taiLieu);
            db.SubmitChanges();
            return RedirectToAction("DanhSachTaiLieu");
        }

        public ActionResult ChiTietTaiLieu(string maTL)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            TaiLieu tailieu = db.TaiLieus.FirstOrDefault(t => t.MaTaiLieu == maTL);
            return View(tailieu);
        }
        [HttpPost]
        public ActionResult ChiTietTaiLieu(TaiLieu tailieu)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var taiLieuExists = db.TaiLieus.FirstOrDefault(k => k.MaTaiLieu == tailieu.MaTaiLieu);
            if (taiLieuExists != null)
            {
                if (string.IsNullOrEmpty(tailieu.FileUpload))
                {
                    tailieu.FileUpload = taiLieuExists.FileUpload;
                }
                taiLieuExists.TenTaiLieu = tailieu.TenTaiLieu;
                taiLieuExists.MoTa = tailieu.MoTa;
                taiLieuExists.TrangThai = tailieu.TrangThai;
                taiLieuExists.FileUpload = tailieu.FileUpload;
                db.SubmitChanges();
                return RedirectToAction("DanhSachTaiLieu");
            }
            return View(tailieu);
        }

        public ActionResult DanhSachTaiLieuThuocKhoaHoc(string maTL)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            ViewBag.MaTaiLieu = maTL;
            TaiLieu checkTrangThaiTaiLieu = db.TaiLieus.FirstOrDefault(t => t.MaTaiLieu == maTL);
           if (checkTrangThaiTaiLieu.TrangThai == "Không sử dụng")
            {
                ViewBag.TrangThaiKhoaHoc = "Tài liệu không sử dụng, nên không thể thêm khóa học";
            }
            List<KhoaHoc_TaiLieu> dsTaiLieuThuocKhoaHoc = db.KhoaHoc_TaiLieus.Where(k => k.MaTaiLieu == maTL).ToList();
            return View(dsTaiLieuThuocKhoaHoc);
        }

        public ActionResult XoaKhoaHoc_TaiLieu(string maKH, string maTL)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            KhoaHoc_TaiLieu khoaHoc_taiLieu = db.KhoaHoc_TaiLieus.FirstOrDefault(t => t.MaTaiLieu == maTL && t.MaKhoaHoc == maKH);
            db.KhoaHoc_TaiLieus.DeleteOnSubmit(khoaHoc_taiLieu);
            db.SubmitChanges();
            return RedirectToAction("DanhSachTaiLieuThuocKhoaHoc", new { maTL = maTL });
        }

        public ActionResult ThemKhoaHoc_TaiLieu(string maTL)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            ViewBag.MaTaiLieu = maTL;
            var khoahoc_tailieu = new KhoaHoc_TaiLieu();
            return View(khoahoc_tailieu);
        }
        [HttpPost]
        public ActionResult ThemKhoaHoc_TaiLieu(KhoaHoc_TaiLieu khtl)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var khoahoc_tailieu = new KhoaHoc_TaiLieu
            {
                MaKhoaHoc = khtl.MaKhoaHoc,
                MaTaiLieu = khtl.MaTaiLieu
            };
            KhoaHoc_TaiLieu findKhoaHocTaiLieu = db.KhoaHoc_TaiLieus.FirstOrDefault(t => t.MaTaiLieu == khoahoc_tailieu.MaTaiLieu && t.MaKhoaHoc == khoahoc_tailieu.MaKhoaHoc);
            if (findKhoaHocTaiLieu != null)
            {
                TempData["ErrorMessageKhoaHocTaiLieu"] = "Khóa học đã tồn tại tài liệu này!";
                return RedirectToAction("ThemKhoaHoc_TaiLieu", new { maTL = khoahoc_tailieu.MaTaiLieu });
            }
            db.KhoaHoc_TaiLieus.InsertOnSubmit(khoahoc_tailieu);
            db.SubmitChanges();
            return RedirectToAction("DanhSachTaiLieuThuocKhoaHoc", new { maTL = khoahoc_tailieu.MaTaiLieu });
        }
        public ActionResult DanhSachPhongHoc(int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<PhongHoc> listPH = db.PhongHocs.ToList();
            var totalRecords = listPH.Count();

            var dsPHList = listPH.OrderBy(q => q.MaPhong).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var model = new PhongHocPagedList
            {
                DsPhongHoc = dsPHList,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult TimKiemPhongHoc(string search, int page = 1, int pageSize = 5)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            using (var db = new DataClasses1DataContext())
            {
                // Gọi stored procedure thông qua DataContext
                var sql = "select * from fn_timKiemPhongHoc(N'" + search + "')";
                IEnumerable<PhongHoc> listPH = db.ExecuteQuery<PhongHoc>(sql).ToList();

                var totalRecords = listPH.Count();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                var phongHocList = listPH.OrderBy(q => q.MaPhong).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var model = new PhongHocPagedList
                {
                    DsPhongHoc = phongHocList,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    SearchQuery = search
                };

                return View(model);
            }
        }

        public ActionResult ThemPhongHoc()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var model = new PhongHoc();
            return View(model);
        }
        [HttpPost]
        public ActionResult ThemPhongHoc(PhongHoc phong)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            try
            {
                string maPH = db.PhongHocs.OrderByDescending(t => t.MaPhong).FirstOrDefault().MaPhong;
                int k = maPH != null ? (int.Parse(maPH.Substring(2)) + 1) : 1;
                string maPHMoi = "PH" + k.ToString("D3");
                var phongHoc = new PhongHoc
                {
                    MaPhong = maPHMoi,
                    TenPhong = phong.TenPhong,
                    SucChua = phong.SucChua,
                    ThietBi = phong.ThietBi,
                    ViTri = phong.ViTri,
                    TrangThai = "Đang hoạt động",
                };

                db.PhongHocs.InsertOnSubmit(phongHoc);
                db.SubmitChanges(); 

                return RedirectToAction("DanhSachPhongHoc");
            }
            catch (SqlException sqlEx)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi từ trigger: " + sqlEx.Message;
                return View(phong);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi: " + ex.Message;
                return View(phong);
            }
        }

        public ActionResult XoaPhongHoc(string maPH)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            PhongHoc phong = db.PhongHocs.FirstOrDefault(t => t.MaPhong == maPH);
            if (phong.LopHocs.Any())
            {
                TempData["ErrorMessagePhongHoc"] = "Phòng học đang được sử dụng, nên không thể xóa ngay lúc này!";
                return RedirectToAction("DanhSachPhongHoc");
            }
            db.PhongHocs.DeleteOnSubmit(phong);
            db.SubmitChanges();
            return RedirectToAction("DanhSachPhongHoc");
        }

        public ActionResult ChiTietPhongHoc(string maPH)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }

            var sql = "select * from fn_chiTietPhongHoc('" + maPH + "')";
            PhongHoc phongHocDetail = db.ExecuteQuery<PhongHoc>(sql).FirstOrDefault();
            return View(phongHocDetail);
        }
        [HttpPost]
        public ActionResult ChiTietPhongHoc(PhongHoc ph)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var phongHocExists = db.PhongHocs.FirstOrDefault(k => k.MaPhong == ph.MaPhong);
            if (phongHocExists != null)
            {
                phongHocExists.TenPhong = ph.TenPhong;
                phongHocExists.SucChua = ph.SucChua;
                phongHocExists.ThietBi =ph.ThietBi;
                phongHocExists.ViTri = ph.ViTri;
                phongHocExists.TrangThai = ph.TrangThai;
                db.SubmitChanges();
                return RedirectToAction("DanhSachPhongHoc");
            }
            return View(ph);
        }

        public ActionResult DanhSachLopHoc(string maKH)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            List<LopHoc> dsLopHoc = db.LopHocs.Where(k => k.MaKhoaHoc == maKH).ToList();
            return View(dsLopHoc);
        }

        public ActionResult ChiTietLopHoc(string malop)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            LopHoc lopHocDetail = db.LopHocs.FirstOrDefault(k => k.MaLop == malop);
            return View(lopHocDetail);
        }

        public ActionResult XuLyDieuHuong(string malop, string page)
        {
            TempData["DieuHuong"] = page;
            return RedirectToAction("ChiTietLopHoc", new { malop = malop });
        }

        public ActionResult ThongTinLopHoc(string malop)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            ViewBag.SlgHVHT = db.fn_DemSoLuongHocVienTrongLop(malop);
            LopHoc lopHocDetail = db.LopHocs.FirstOrDefault(k => k.MaLop == malop);
            return PartialView(lopHocDetail);
        }

        public ActionResult HocVienLopHoc(string malop)
        {
            return PartialView(db.fn_DSHocVienCuaLop(malop).ToList());
        }

        public ActionResult DiemDanh(string malop)
        {
            var lst = db.HocViens.Where(t => t.ThanhToans.Where(u => u.DangKies.Where(i => i.MaLop == malop).Any()).Any());

            List<HocVien_DiemDanh> ds = new List<HocVien_DiemDanh>();

            foreach (HocVien item in lst)
            {
                HocVien_DiemDanh a = new HocVien_DiemDanh(item.MaHocVien, malop, db);
                ds.Add(a);
            }

            Session["dsDiemDanh"] = ds;

            return PartialView(ds);
        }

        public ActionResult QuanLyNhomND()
        {
            return View(db.NhomNguoiDungs.ToList());
        }

        public ActionResult SuaQuyen(string MaNhomND)
        {
            TempData["roleNhomND"] = db.NhomNguoiDungs.FirstOrDefault(t => t.MaNhomND == MaNhomND).VaiTro_NhomNguoiDungs.ToList() as List<VaiTro_NhomNguoiDung>;
            TempData["role"] = db.fn_VaiTro_KhongThuocNND(MaNhomND).ToList();
            
            return View(db.NhomNguoiDungs.FirstOrDefault(t => t.MaNhomND == MaNhomND));
        }

        public ActionResult ThemNhomNguoiDung()
        {
            return View();
        }

        [HttpPost]
        public ActionResult XuLyThemNhomNguoiDung(NhomNguoiDung ng)
        {
            if (ng != null)
            {
                db.NhomNguoiDungs.InsertOnSubmit(ng);
                db.CreateRoleAndGrantPermissions(ng.TenNhomND);
                db.SubmitChanges();
            }

            return RedirectToAction("QuanLyNhomND");
        }

        [HttpPost]
        public ActionResult XuLySuaQuyen(string MaNhomND, string new_permissions, string removed_permissions)
        {
            //var lst = TempData["roleNhomND"] as List<VaiTro_NhomNguoiDung>;
            var newPermissionsList = string.IsNullOrEmpty(new_permissions) ? new List<string>() : new_permissions.Split(',').ToList();
            var removedPermissionsList = string.IsNullOrEmpty(removed_permissions) ? new List<string>() : removed_permissions.Split(',').ToList();

            if (newPermissionsList.Any())
            {
                foreach (var item in newPermissionsList)
                {
                    if (!db.VaiTro_NhomNguoiDungs.Any(t => t.MaVaiTro == item && t.MaNhomND == MaNhomND))
                    {
                        db.GrantPermissionsToUserGroup(item, MaNhomND);
                        
                    }
                }
            }


            if (removedPermissionsList.Any())
            {
                foreach (var item in removedPermissionsList)
                {
                    var existingRole = db.VaiTro_NhomNguoiDungs.FirstOrDefault(t => t.MaVaiTro == item && t.MaNhomND == MaNhomND);
                    if (existingRole != null)
                    {
                        db.RevokePermissionsFromUserGroup(item, MaNhomND);
                    }
                }
            }

            return Json(new { success = true });
        }

        public ActionResult QuanLyThanhToan()
        {
            return View(db.ThanhToans.OrderByDescending(t => t.NgayThucHien).ToList());
        }


        public ActionResult TaoThanhToan()
        {

            string ma = db.ThanhToans.OrderByDescending(t => t.MaThanhToan).FirstOrDefault().MaThanhToan;

            int k;

            if (ma != null)
            {
                k = int.Parse(ma.Substring(2, 3)) + 1;
            }
            else
            {
                k = 1;
            }

            string mamoi = "TT" + k.ToString("D3");

            ThanhToan tt = new ThanhToan()
            {
                MaThanhToan = mamoi,
                NgayThucHien = DateTime.Now,
                TongTien = 0 // Mặc định ban đầu
            };

            Session["ThanhToan"] = tt;

            return RedirectToAction("TaoDangKy");
        }

        public ActionResult TaoDangKy()
        {
            // Lấy danh sách khóa học
            var khoaHocs = db.KhoaHocs.ToList();
            return View(khoaHocs);
        }


        [HttpPost]
        public JsonResult LayDanhSachLopHoc(string MaKH)
        {
            var lopHocs = db.LopHocs.Where(t => t.MaKhoaHoc == MaKH).ToList();

            List<LopHoc> lh = new List<LopHoc>();

            foreach (var item in lopHocs)
            {
                LopHoc a = new LopHoc()
                {
                    MaLop = item.MaLop,
                    TenLop = item.TenLop,
                    MaKhoaHoc = item.MaKhoaHoc,
                    MaPhong = item.MaPhong,
                    MaGiangVien = item.MaGiangVien,
                    NgayBatDau = item.NgayBatDau,
                    NgayKetThuc = item.NgayKetThuc,
                    TrangThai = item.TrangThai,
                    SoLuongToiDa = item.SoLuongToiDa,
                    SoLuongToiThieu = item.SoLuongToiThieu,
                    ThoiLuong = item.ThoiLuong
                };
                lh.Add(a);
            }    


            if (lh.Any())
            {
                return Json(new { success = true, lh});
            }
            else
            {
                return Json(new { success = false, message = "Không có lớp học nào." });
            }
        }

        [HttpPost]
        public JsonResult LayDanhSachGiamGia(string MaKH)
        {
            var giamGias = db.GiamGias.Where(t => t.KhoaHoc_GiamGias.Where(u => u.MaKhoaHoc == MaKH).Any()).ToList();

            List<GiamGia> gg = new List<GiamGia>();

            foreach (var item in giamGias)
            {
                GiamGia a = new GiamGia()
                {
                    MaGiamGia = item.MaGiamGia,
                    TenGiamGia = item.TenGiamGia,
                    MoTa = item.MoTa,
                    TiLeGiam = item.TiLeGiam,
                    NgayBatDau = item.NgayBatDau,
                    NgayKetThuc = item.NgayKetThuc,
                    NgayTao = item.NgayTao,
                    TrangThai = item.TrangThai
                };
                gg.Add(a);
            }

            return Json(new { success = true, gg });
        }

        [HttpPost]
        public JsonResult ThemDangKy(string LopHocId, string GiamGiaId)
        {
            var lopHoc = db.LopHocs.FirstOrDefault(t => t.MaLop == LopHocId);
            var giamGia = db.GiamGias.FirstOrDefault(t => t.MaGiamGia == GiamGiaId);


            if (lopHoc == null)
            {
                return Json(new { success = false });
            }

            // Tính toán tiền gốc và giảm giá
            var soTien = lopHoc.KhoaHoc.HocPhi;
            var tienGiam = giamGia != null ? soTien * giamGia.TiLeGiam / 100 : 0;
            var thucTra = soTien - tienGiam;

            string ma = db.DangKies.OrderByDescending(t => t.MaDangKy).FirstOrDefault().MaDangKy;

            int k;

            if (ma != null)
            {
                k = int.Parse(ma.Substring(2, 3)) + 1;
            }
            else
            {
                k = 1;
            }

            string mamoi = "DK" + k.ToString("D3");
            var thanhToan = Session["ThanhToan"] as ThanhToan;

            // Tạo đăng ký mới
            var dangKy = new DangKy
            {
                MaDangKy = mamoi,
                MaLop = lopHoc.MaLop,
                MaThanhToan = thanhToan.MaThanhToan,
                MaGiamGia = giamGia == null ? "" : giamGia.MaGiamGia,
                SoTien = soTien,
                ThucTra = thucTra,
                LopHoc = lopHoc,
                GiamGia = giamGia
            };

            thanhToan.TongTien += thucTra;
            // Thêm vào session
            thanhToan.DangKies.Add(dangKy);
            Session["ThanhToan"] = thanhToan;


            // Trả về danh sách đăng ký cập nhật
            return Json(new
            {
                success = true,
                danhSachDangKy = thanhToan.DangKies.Select((dk, index) => new
                {
                    STT = index + 1,
                    KhoaHoc = dk.LopHoc.KhoaHoc.TenKhoaHoc,
                    LopHoc = dk.LopHoc == null ? "" : dk.LopHoc.TenLop,
                    GiamGia = dk.GiamGia == null ? 0 : dk.GiamGia.TiLeGiam,
                    SoTien = dk.SoTien,
                    Giam = tienGiam,
                    ThucTra = dk.ThucTra
                }).ToList()
            });
        }

        public ActionResult XacNhanThanhToan()
        {
            ThanhToan tt = Session["ThanhToan"] as ThanhToan;
            return View(tt);
        }

        public ActionResult TienHanhThanhToan(FormCollection c, HttpPostedFileBase AnhHoaDon)
        {
            ThanhToan tt = Session["ThanhToan"] as ThanhToan;

            if (tt != null)
            {
                tt.MaHocVien = c["MaHocVien"];
                tt.HinhThuc = c["HinhThuc"];

                if (AnhHoaDon != null)
                {
                    string fileName = Path.GetFileName(AnhHoaDon.FileName);
                    string duongdan = Path.Combine(Server.MapPath("~/Content/HinhAnh/ThanhToan"), fileName);
                    AnhHoaDon.SaveAs(duongdan);
                    tt.AnhHoaDon = fileName;
                }

                ThanhToan n = new ThanhToan()
                {
                    MaThanhToan = tt.MaThanhToan,
                    MaHocVien = tt.MaHocVien,
                    TongTien = tt.TongTien,
                    HinhThuc = tt.HinhThuc,
                    NgayThucHien = DateTime.Now,
                    TrangThai = "Hoàn tất",
                    AnhHoaDon = tt.AnhHoaDon
                };
                

                db.ThanhToans.InsertOnSubmit(n);
                
                foreach (var i in tt.DangKies.ToList())
                {
                    DangKy d = new DangKy()
                    {
                        MaDangKy = i.MaDangKy,
                        MaLop = i.MaLop,
                        MaThanhToan = i.MaThanhToan,
                        MaGiamGia = i.MaGiamGia,
                        SoTien = i.SoTien,
                        ThucTra = i.ThucTra
                    };

                    db.DangKies.InsertOnSubmit(d);
                }

                db.SubmitChanges();

                Session["ThanhToan"] = null;
            }
            
            return RedirectToAction("QuanLyThanhToan");
        }

        public ActionResult HuyThanhToan()
        {
            Session["ThanhToan"] = null;
            return RedirectToAction("QuanLyThanhToan");
        }
    }
    
}
