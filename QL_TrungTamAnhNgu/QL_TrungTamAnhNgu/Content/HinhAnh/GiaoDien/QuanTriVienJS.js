// xử handleClick user
const user = document.querySelector(".user")
const iconEye = document.querySelector(".iconEye")
const inputPassWord = document.getElementById("inputMatKhau")

function handleClickUser(event) {
    event.stopPropagation(); // Ngăn sự kiện nhấp lan truyền ra ngoài
    if (user.classList.contains("visible")) {
        user.classList.remove("visible")
    } else {
        user.classList.add("visible")
    }
}

document.addEventListener("click", function (e) {
    if (user && !user.contains(e.target)) {
        user.classList.remove("visible")
    }
})

function handleClickChangeEye(idInputPass) {
    const inputMatKhau = document.getElementById(idInputPass);
    if (iconEye.classList.contains("fa-eye-slash")) {
        iconEye.classList.add("fa-eye");
        iconEye.classList.remove("fa-eye-slash");
        inputMatKhau.type = "text";
    } else {
        iconEye.classList.remove("fa-eye");
        iconEye.classList.add("fa-eye-slash");
        inputMatKhau.type = "password";
    }
}

// xử lý toggle menu
function toggleSubMenu(submenuId) {
    var allSubMenus = document.querySelectorAll('.subMenu');

    for (var i = 0; i < allSubMenus.length; i++) {
        var menu = allSubMenus[i];
        if (menu.id !== submenuId) {
            menu.classList.remove("open");
            menu.parentElement.querySelector(".icon_1").classList.add("icon__visible");
            menu.parentElement.querySelector(".icon_1").classList.remove("icon__hidden");
            menu.parentElement.querySelector(".icon_2").classList.add("icon__hidden");
            menu.parentElement.querySelector(".icon_2").classList.remove("icon__visible");
        }
    }

    var subMenu = document.getElementById(submenuId);

    if (!subMenu.classList.contains("open")) {
        subMenu.classList.add("open"); // Mở menu được nhấp vào
        subMenu.parentElement.querySelector(".icon_1").classList.add("icon__hidden");
        subMenu.parentElement.querySelector(".icon_1").classList.remove("icon__visible");
        subMenu.parentElement.querySelector(".icon_2").classList.add("icon__visible");
        subMenu.parentElement.querySelector(".icon_2").classList.remove("icon__hidden");
    } else {
        subMenu.classList.remove("open"); // Đóng menu được nhấp vào
        subMenu.parentElement.querySelector(".icon_1").classList.add("icon__visible");
        subMenu.parentElement.querySelector(".icon_1").classList.remove("icon__hidden");
        subMenu.parentElement.querySelector(".icon_2").classList.add("icon__hidden");
        subMenu.parentElement.querySelector(".icon_2").classList.remove("icon__visible");
    }
}


const tenKhoaHocInput = document.getElementById("TenKhoaHoc")
const moTaInput = document.getElementById("MoTa")
const hocPhiInput = document.getElementById("HocPhi")
const capDoInput = document.getElementById("CapDo")
const label = document.getElementById("label")
const trangThai = document.getElementById("TrangThai")
const hoTenInput = document.getElementById("HoTen")
const emailInput = document.getElementById("Email")
const soDienThoaiInput = document.getElementById("SoDienThoai")
const trangThaiInput = document.getElementById("TrangThai")
const chuyenMonInput = document.getElementById("ChuyenMon")
const trinhDoInput = document.getElementById("TrinhDo")
const diaChiInput = document.getElementById("DiaChi")
const mucLuongInput = document.getElementById("MucLuong")
const gioiTinhInput = document.getElementById("GioiTinh")
const ngaySinhInput = document.getElementById("NgaySinh")
const ngayBatDauInput = document.getElementById("NgayBatDau")
const ngayKetThucInput = document.getElementById("NgayKetThuc")
const tiLeGiamInput = document.getElementById("TiLeGiam")
const soLuongToiDaInput = document.getElementById("SoLuongToiDa")
const soLuongToiThieuInput = document.getElementById("SoLuongToiThieu")
const phongHocInput = document.getElementById("PhongHoc")
const giangVienInput = document.getElementById("GiangVien")
const thoiLuongInput = document.getElementById("ThoiLuong")

const tenPhongInput = document.getElementById("TenPhong")
const sucChuaInput = document.getElementById("SucChua")
const thietBiInput = document.getElementById("ThietBi")
const viTriInput = document.getElementById("ViTri")

const btnSua = document.getElementById("btnSua")
const btnLuu = document.getElementById("btnLuu")
    
if (btnSua) {
    btnSua.addEventListener("click", function () {
        btnLuu.disabled = false;
        btnSua.disabled = true;
        if (tenKhoaHocInput) {
            tenKhoaHocInput.removeAttribute("readonly");
        }
        if (moTaInput) {
            moTaInput.removeAttribute("readonly")
        }
        if (hocPhiInput) {
            hocPhiInput.removeAttribute("readonly")
        }
        if (capDoInput) {
            capDoInput.removeAttribute("readonly")
        }
        if (label) {
            label.style.cursor = "pointer";
            label.style.pointerEvents = "auto";
        }
        if (hoTenInput) {
            hoTenInput.removeAttribute("readonly")
        }
        if (emailInput) {
            emailInput.removeAttribute("readonly")
        }
        if (soDienThoaiInput) {
            soDienThoaiInput.removeAttribute("readonly")
        }
        if (trangThaiInput) {
            trangThaiInput.disabled = false;
        }
        if (chuyenMonInput) {
            chuyenMonInput.removeAttribute("readonly")
        }
        if (trinhDoInput) {
            trinhDoInput.removeAttribute("readonly")
        }
        if (diaChiInput) {
            diaChiInput.removeAttribute("readonly")
        }
        if (mucLuongInput) {
            mucLuongInput.removeAttribute("readonly")
        }
        if (gioiTinhInput) {
            gioiTinhInput.disabled = false;
        }
        if (ngaySinhInput) {
            ngaySinhInput.removeAttribute("readonly")
        }
        if (tenPhongInput) {
            tenPhongInput.removeAttribute("readonly")
        }
        if (sucChuaInput) {
            sucChuaInput.removeAttribute("readonly")
        }
        if (viTriInput) {
            viTriInput.removeAttribute("readonly")
        }
        if (thietBiInput) {
            thietBiInput.removeAttribute("readonly")
        }

        if (ngayBatDauInput) {
            ngayBatDauInput.removeAttribute("readonly")
        }
        if (ngayKetThucInput) {
            ngayKetThucInput.removeAttribute("readonly")
        }
        if (tiLeGiamInput) {
            tiLeGiamInput.removeAttribute("readonly")
        }
        if (soLuongToiThieuInput) {
            soLuongToiThieuInput.removeAttribute("readonly")
        }
        if (soLuongToiDaInput) {
            soLuongToiDaInput.removeAttribute("readonly")
        }
        if (phongHocInput) {
            phongHocInput.removeAttribute("readonly")
        }
        if (giangVienInput) {
            giangVienInput.removeAttribute("readonly")
        }
        if (thoiLuongInput) {
            thoiLuongInput.removeAttribute("readonly")
        }
    })
}

const avatar = document.getElementById("Avatar")
if (avatar) {
    avatar.addEventListener("change", function (event) {
        var reader = new FileReader();
        var file = event.target.files[0];

        if (file) {
            reader.onload = function (e) {
                // Hiển thị hình ảnh trong thẻ img
                var img = document.getElementById('avatarPreview');
                img.src = e.target.result;
                img.style.display = "block"; // Hiển thị thẻ img
            }
            reader.readAsDataURL(file);
        }
    })
}

const myForm = document.querySelector(".myForm")
//if (myForm) {
//    myForm.addEventListener("submit", function (event) {
//        event.preventDefault();
//        console.log(tenKhoaHocInput.value)
//        console.log(moTaInput.value)
//        console.log(hocPhiInput.value)
//    })
//}

const btnClearForm = document.getElementById("btnClearForm")
if (btnClearForm) {
    btnClearForm.addEventListener("click", function (event) {
        event.preventDefault();
        inputTenDangNhap.value = "";
        inputMatKhau.value = "";
        inputEmail.value = "";
        inputHoTen.value = "";
        selectMaNhom.value = "";
    })
}