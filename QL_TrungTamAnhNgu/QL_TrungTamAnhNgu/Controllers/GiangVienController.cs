using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using QL_TrungTamAnhNgu.Models;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Security;
using QL_TrungTamAnhNgu.ViewModel;

namespace QL_TrungTamAnhNgu.Controllers
{
    [Authorize]
    public class GiangVienController : Controller
    {
        //
        // GET: /GiangVien/

        DataClasses1DataContext data = new DataClasses1DataContext();

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult DangNhap()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult XuLyDangNhap(FormCollection c)
        {
            string username = c["username"];
            string password = c["password"];

            string newConnectionString = "Data Source=THAIBINH-LAPTOP;Initial Catalog=QL_TrungTamAnhNgu;User ID=" + username + ";Password=" + password + ";";
            data = new DataClasses1DataContext(newConnectionString);

            //// Lấy đối tượng Configuration hiện tại từ web.config
            //Configuration config = WebConfigurationManager.OpenWebConfiguration("~");

            //// Lấy phần tử connectionStrings từ cấu hình
            //ConnectionStringsSection connectionStringsSection = config.GetSection("connectionStrings") as ConnectionStringsSection;

            //if (connectionStringsSection != null)
            //{
            //    // Tìm chuỗi kết nối theo tên trong cấu hình
            //    ConnectionStringSettings connectionString = connectionStringsSection.ConnectionStrings["QL_TrungTamAnhNguConnectionString"];

            //    if (connectionString != null)
            //    {
            //        // Cập nhật chuỗi kết nối với giá trị mới
            //        connectionString.ConnectionString = newConnectionString;

            //        // Lưu lại các thay đổi vào file web.config
            //        config.Save(ConfigurationSaveMode.Modified);

            //        // Đảm bảo các thay đổi được áp dụng ngay lập tức
            //        ConfigurationManager.RefreshSection("connectionStrings");
            //    }
            //}


            var user = data.NguoiDungs.FirstOrDefault(u => u.TenTaiKhoan == username && u.MatKhau == password);
            if (user != null)
            {
                GiangVien gv = user.GiangViens.SingleOrDefault();
                Session["User"] = gv;

                FormsAuthentication.SetAuthCookie(user.TenTaiKhoan, false);
            }
            return RedirectToAction("Index", "GiangVien");
        }

        public ActionResult DangXuat()
        {
            // Xóa session của người dùng
            Session["User"] = null;

            // Hủy cookie xác thực của FormsAuthentication (nếu sử dụng FormsAuthentication)
            FormsAuthentication.SignOut();

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("DangNhap", "GiangVien");
        }


        public ActionResult Test()
        {
            GiangVien gv = (GiangVien)Session["User"];
            return View(data.LopHocs.Where(t => t.MaGiangVien == gv.MaGiangVien).ToList());

            //return View(data.NhomNguoiDungs.ToList());
        }

        public ActionResult LopHocCuaToi()
        {
            GiangVien gv = (GiangVien)Session["User"];
            return View(data.LopHocs.Where(t => t.MaGiangVien == gv.MaGiangVien).ToList());
        }


        public ActionResult TimKiem(FormCollection c)
        {
            GiangVien gv = (GiangVien)Session["User"];
            string tenlop = c["TenLop"];
            return View(data.LopHocs.Where(t => t.MaGiangVien == gv.MaGiangVien && t.TenLop.Contains(tenlop)).ToList());
        }

        public ActionResult ChiTietLopHoc(string malop)
        {
            LopHoc l = data.LopHocs.FirstOrDefault(t => t.MaLop == malop);
            return View(l);
        }

        public ActionResult XuLyDieuHuong(string malop, string page)
        {
            TempData["DieuHuong"] = page;
            return RedirectToAction("ChiTietLopHoc", new { malop = malop });
        }

        public ActionResult HocVien(string malop)
        {
            return PartialView(data.fn_DSHocVienCuaLop(malop).ToList());
        }

        public ActionResult ThongTin(string malop)
        {
            ViewBag.SlgHVHT = data.fn_DemSoLuongHocVienTrongLop(malop);
            return PartialView(data.LopHocs.FirstOrDefault(t => t.MaLop == malop));
        }

        public ActionResult DiemDanh(string malop)
        {
            var lst = data.HocViens.Where(t => t.ThanhToans.Where(u => u.DangKies.Where(i => i.MaLop == malop).Any()).Any());

            List<HocVien_DiemDanh> ds = new List<HocVien_DiemDanh>();

            foreach (HocVien item in lst)
            {
                HocVien_DiemDanh a = new HocVien_DiemDanh(item.MaHocVien, malop, data);
                ds.Add(a);
            }

            Session["dsDiemDanh"] = ds;

            return PartialView(ds);
        }


        [HttpPost]
        public ActionResult LuuDiemDanh()
        {
            var dsDiemDanh = Session["dsDiemDanh"] as List<HocVien_DiemDanh>;
            string malop = "";

            foreach (var hocVien in dsDiemDanh)
            {
                foreach (var diemDanh in hocVien.ds)
                {
                    malop = diemDanh.LichH.MaLop;

                    // Lấy thông tin điểm danh hiện tại từ bảng ChuyenCans
                    var chuyenCan = data.ChuyenCans.FirstOrDefault(cc =>
                        cc.MaLichHoc == diemDanh.LichH.MaLichHoc &&
                        cc.DangKy.ThanhToan.MaHocVien == hocVien.hv.MaHocVien);

                    // Chuyển đổi trạng thái mới từ input
                    string trangThaiMoi = diemDanh.TrangThai;

                    if (chuyenCan != null)
                    {
                        // Chỉ cập nhật khi trạng thái mới khác với trạng thái cũ
                        if (chuyenCan.TrangThai != trangThaiMoi)
                        {
                            chuyenCan.TrangThai = trangThaiMoi;
                        }
                    }

                    if (chuyenCan == null && trangThaiMoi != "Chưa điểm danh")
                    {
                        // Trường hợp không tìm thấy bản ghi trong bảng ChuyenCans
                        var dk = data.DangKies.FirstOrDefault(d => d.ThanhToan.MaHocVien == hocVien.hv.MaHocVien);
                        if (dk != null)
                        {
                            var newChuyenCan = new ChuyenCan
                            {
                                MaLichHoc = diemDanh.LichH.MaLichHoc,
                                MaDangKy = dk.MaDangKy,
                                TrangThai = trangThaiMoi,
                                NgayDiemDanh = DateTime.Now
                            };
                            data.ChuyenCans.InsertOnSubmit(newChuyenCan);
                        }
                    }
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            data.SubmitChanges();

            // Chuyển hướng lại trang chi tiết lớp học
            return RedirectToAction("ChiTietLopHoc", new { malop = malop });
        }

        [HttpPost]
        public ActionResult CapNhatDiemDanh(int hocVienIndex, int diemDanhIndex, string trangThai)
        {
            // Lấy dữ liệu dsDiemDanh từ session
            var dsDiemDanh = Session["dsDiemDanh"] as List<HocVien_DiemDanh>;
            if (dsDiemDanh != null)
            {
                // Tìm và cập nhật trạng thái cho học viên và điểm danh tương ứng
                var hocVien = dsDiemDanh[hocVienIndex];
                var diemDanh = hocVien.ds[diemDanhIndex];
                diemDanh.TrangThai = trangThai; // Cập nhật trạng thái điểm danh

                // Lưu lại dsDiemDanh vào session
                Session["dsDiemDanh"] = dsDiemDanh;
            }

            return Json(new { success = true });
        }

        public ActionResult ChamDiem(string malop)
        {
            return PartialView(data.DangKy_BaiTaps.Where(t => t.DangKy.MaLop == malop && t.Diem == null).OrderBy(u => u.NgayNop));
        }

        public ActionResult ChamDiemBaiTap(string madk, string mabt)
        {
            return View(data.DangKy_BaiTaps.FirstOrDefault(t => t.MaBaiTap == mabt && t.MaDangKy == madk));
        }

        public ActionResult XuLyChamDiemBaiTap(DangKy_BaiTap k)
        {
            DangKy_BaiTap moi = data.DangKy_BaiTaps.FirstOrDefault(t => t.MaDangKy == k.MaDangKy && t.MaBaiTap == k.MaBaiTap);
            
            if (k.Diem != null)
            {
                moi.Diem = k.Diem;
                data.SubmitChanges();
            }

            TempData["DieuHuong"] = "ChamDiem";
            return RedirectToAction("ChiTietLopHoc", new { malop = moi.DangKy.MaLop });
        }

        public ActionResult BangDiem(string malop)
        {
            var lst = data.HocViens.Where(t => t.ThanhToans.Where(u => u.DangKies.Where(i => i.MaLop == malop).Any()).Any());

            List<BangDiem> ds = new List<BangDiem>();

            foreach (HocVien item in lst)
            {
                BangDiem a = new BangDiem(item.MaHocVien, malop, data);
                ds.Add(a);
            }

            Session["dsBangDiem"] = ds;
            TempData["MaLop"] = malop;

            return PartialView(ds);

        }


        [HttpPost]
        public ActionResult LuuBangDiem()
        {
            var dsBangDiem = Session["dsBangDiem"] as List<BangDiem>;

            foreach (var i in dsBangDiem)
            {
                foreach (var j in i.ds)
                {
                    // Lấy thông tin điểm danh hiện tại từ bảng ChuyenCans
                    var dkbt = data.DangKy_BaiTaps.FirstOrDefault(t => t.BaiTap.MaBaiTap == j.bt.MaBaiTap && t.DangKy.ThanhToan.HocVien.MaHocVien == i.hv.MaHocVien);

                    // Chuyển đổi trạng thái mới từ input
                    var diemMoi = double.Parse(j.Diem.ToString());

                    if (dkbt != null && diemMoi >= 0 && diemMoi <= 10 && dkbt.Diem != null)
                    {
                        // Chỉ cập nhật khi trạng thái mới khác với trạng thái cũ
                        if (double.Parse(dkbt.Diem.ToString()) != diemMoi)
                        {
                            dkbt.Diem = decimal.Parse(diemMoi.ToString());
                        }
                    }

                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            data.SubmitChanges();

            // Chuyển hướng lại trang chi tiết lớp học
            return RedirectToAction("ChiTietLopHoc", new { malop = TempData["MaLop"] });
        }

        [HttpPost]
        public ActionResult CapNhatBangDiem(int hocVienIndex, int baiTapIndex, double diem)
        {
            // Lấy dữ liệu dsDiemDanh từ session
            var dsBangDiem = Session["dsBangDiem"] as List<BangDiem>;
            if (dsBangDiem != null)
            {
                // Tìm và cập nhật trạng thái cho học viên và điểm danh tương ứng
                var hocVien = dsBangDiem[hocVienIndex];
                var baitap = hocVien.ds[baiTapIndex];
                baitap.Diem = diem;

                // Lưu lại dsDiemDanh vào session
                Session["dsDiemDanh"] = dsBangDiem;
            }

            return Json(new { success = true });
        }

        public ActionResult ThongTinNguoiDung()
        {
            GiangVien gv = Session["User"] as GiangVien;
            return View(gv);
        }

        [HttpPost]
        public ActionResult SuaThongTinNguoiDung(FormCollection c, HttpPostedFileBase anh)
        {
            GiangVien gv = (GiangVien)Session["User"];
            GiangVien cu = data.GiangViens.FirstOrDefault(t => t.MaGiangVien == gv.MaGiangVien);

            cu.SoDienThoai = c["sdt"];
            cu.Email = c["email"];
            cu.HoTen = c["hoten"];

            if (anh != null)
            {
                string fileName = Path.GetFileName(anh.FileName);
                string duongdan = Path.Combine(Server.MapPath("~/Content/GiangVien/HinhAnh/Avatar"), fileName);
                anh.SaveAs(duongdan);
                cu.NguoiDung.AnhDaiDien = fileName;
            }

            Session["User"] = cu;

            data.SubmitChanges();

            return RedirectToAction("ThongTinNguoiDung");
        }

        public ActionResult LichGiangDay()
        {
            GiangVien gv = Session["User"] as GiangVien;
            return View(data.LichHocs.Where(t => t.LopHoc.MaGiangVien == gv.MaGiangVien && t.NgayHoc >= DateTime.Now).OrderBy(u => u.NgayHoc).Take(10).ToList());
        }

        public ActionResult LichHoc(string malop)
        {
            return PartialView(data.LichHocs.Where(t => t.MaLop == malop).OrderBy(t => t.NgayHoc).ToList());
        }
    }
}