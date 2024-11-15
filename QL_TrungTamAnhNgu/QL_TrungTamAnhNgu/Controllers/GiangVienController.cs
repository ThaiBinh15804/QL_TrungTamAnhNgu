using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

    }
}