using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
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


                if (user.TrangThai == "Đã khóa" && (user.MaNhomND == "ND002" || user.MaNhomND == "ND004" || user.MaNhomND == "ND005" || user.MaNhomND == "ND006"))
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
        public ActionResult ThemKhoaHoc()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            var model = new KhoaHoc();
            return View(model);
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
            var khoaHoc = db.KhoaHocs.FirstOrDefault(k => k.MaKhoaHoc == makh);
            if (khoaHoc != null)
            {
                db.KhoaHocs.DeleteOnSubmit(khoaHoc);
                db.SubmitChanges();
            }
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
                    // Thêm người dùng vào bảng NguoiDung
                    var nguoiDung = new NguoiDung
                    {
                        MaNguoiDung = gv.NguoiDung.MaNguoiDung,
                        TenTaiKhoan = gv.NguoiDung.TenTaiKhoan,
                        MatKhau = gv.NguoiDung.MatKhau,
                        AnhDaiDien = gv.NguoiDung.AnhDaiDien,
                        NgayTao = DateTime.Now,
                        TrangThai = "Đang hoạt động",
                        MaNhomND = "ND002"
                    };

                    db.NguoiDungs.InsertOnSubmit(nguoiDung);
                    db.SubmitChanges(); // Lưu thay đổi vào bảng NguoiDung

                    // Gọi procedure tạo tài khoản người dùng
                    int k = db.CreateUserAccount(nguoiDung.TenTaiKhoan, nguoiDung.MatKhau, nguoiDung.MaNhomND);

                    // Thêm giảng viên vào bảng GiangVien
                    var giangVien = new GiangVien
                    {
                        MaNguoiDung = nguoiDung.MaNguoiDung,
                        MaGiangVien = gv.MaGiangVien,
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

            var studentList = dsStudent.OrderBy(q => q.MaHocVien).Skip((page - 1) * pageSize).Take(pageSize).ToList();
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
                    // Thêm người dùng vào bảng NguoiDung
                    var nguoiDung = new NguoiDung
                    {
                        MaNguoiDung = hv.NguoiDung.MaNguoiDung,
                        TenTaiKhoan = hv.NguoiDung.TenTaiKhoan,
                        MatKhau = hv.NguoiDung.MatKhau,
                        AnhDaiDien = hv.NguoiDung.AnhDaiDien,
                        NgayTao = DateTime.Now,
                        TrangThai = "Đang hoạt động",
                        MaNhomND = "ND003"
                    };

                    db.NguoiDungs.InsertOnSubmit(nguoiDung);
                    db.SubmitChanges(); // Lưu thay đổi vào bảng NguoiDung

                    db.CreateUserAccount(nguoiDung.TenTaiKhoan, nguoiDung.MatKhau, nguoiDung.MaNhomND);

                    var hocVien = new HocVien
                    {
                        MaNguoiDung = nguoiDung.MaNguoiDung,
                        MaHocVien = hv.MaHocVien,
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
            var phongHocExists = db.PhongHocs.FirstOrDefault(k => k.MaPhong == ph.MaPhong);
            if (phongHocExists != null)
            {
                phongHocExists.TenPhong = ph.TenPhong;
                phongHocExists.SucChua = ph.SucChua;
                phongHocExists.ThietBi =ph.ThietBi;
                phongHocExists.ViTri = ph.ViTri;
                db.SubmitChanges();
                return RedirectToAction("DanhSachPhongHoc");
            }
            return View(ph);
        }
    }
}
