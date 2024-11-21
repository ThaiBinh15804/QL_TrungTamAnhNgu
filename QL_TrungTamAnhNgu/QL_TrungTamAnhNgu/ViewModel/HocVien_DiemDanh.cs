using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QL_TrungTamAnhNgu.Models;

namespace QL_TrungTamAnhNgu.ViewModel
{
    public class DiemDanh
    {
        public LichHoc LichH { get; set; }
        public string TrangThai { get; set; }

        public DiemDanh(LichHoc lh, string dd)
        {
            LichH = lh;
            TrangThai = dd;
        }
    }

    public class HocVien_DiemDanh
    {
        public HocVien hv { get; set; }
        public List<DiemDanh> ds = new List<DiemDanh>();

        public HocVien_DiemDanh(string mahv, string malop, DataClasses1DataContext data)
        {
            hv = data.HocViens.FirstOrDefault(t => t.MaHocVien == mahv);
            List<LichHoc> lh = data.LichHocs.Where(t => t.MaLop == malop).OrderBy(t => t.NgayHoc).ToList();
            foreach (var item in lh)
            {
                ChuyenCan c = data.ChuyenCans.FirstOrDefault(t => t.MaLichHoc == item.MaLichHoc && t.DangKy.ThanhToan.MaHocVien == mahv);
                DiemDanh dd = new DiemDanh(item, c != null ? c.TrangThai : "Chưa điểm danh");

                ds.Add(dd);
            }
        }
    }
}