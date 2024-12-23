﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_TrungTamAnhNgu.Models
{
    public class SoluongDangKyTheoThangResult
    {
        public string ThangNam { get; set; }
        public int SoLuongDangKy { get; set; }
    }

    public class DoanhThuTheoThang
    {
        public string ThangNam { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class ThongKeSoTuoi
    {
        public int Tuoi { get; set; }
        public int SoLuong { get; set; }
    }

    public class ThongKeGioiTinh
    {
        public string GioiTinh { get; set; }
        public int SoLuong { get; set; }
    }

    public class LopHocTrungTam
    {
        public string TenLop { get; set; }
        public string HoTen { get; set; }
        public string TenPhong { get; set; }
        public string TrangThai { get; set; }
    }

    public class KhoaHocPagedList
    {
        public List<KhoaHoc> DsKhoaHoc { get; set; } // Danh sách loai khoa hoc
        public int CurrentPage { get; set; }  // Trang hiện tại
        public int TotalPages { get; set; }   // Tổng số trang
        public int PageSize { get; set; }     // Số lượng bản ghi mỗi trang
        public string SearchQuery { get; set; }
    }
}