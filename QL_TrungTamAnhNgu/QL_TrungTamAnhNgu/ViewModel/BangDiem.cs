using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QL_TrungTamAnhNgu.Models;

namespace QL_TrungTamAnhNgu.ViewModel
{
    public class DiemBaiTap
    {
        public BaiTap bt { get; set; }
        public double Diem { get; set; }

        public DiemBaiTap(BaiTap b, double d)
        {
            bt = b;
            Diem = d;
        }

    }

    public class BangDiem
    {
        public HocVien hv { get; set; }
        public List<DiemBaiTap> ds = new List<DiemBaiTap>();

        public BangDiem(string mahv, string malop, string madk, DataClasses1DataContext data)
        {
            hv = data.HocViens.FirstOrDefault(t => t.MaHocVien == mahv);
            List<BaiTap> bt = data.BaiTaps.Where(t => t.KhoaHoc_BaiTaps.Where(u => u.KhoaHoc.LopHocs.Where(i => i.MaLop == malop).Any()).Any()).ToList();
            foreach (var item in bt)
            {
                DangKy_BaiTap c = data.DangKy_BaiTaps.FirstOrDefault(t => t.MaBaiTap == item.MaBaiTap && t.MaDangKy == madk);

                DiemBaiTap a = new DiemBaiTap(item, c != null ? double.Parse(c.Diem != null ? c.Diem.ToString() : Convert.ToString(-2)) : -1);

                ds.Add(a);
            }
        }
    }
}