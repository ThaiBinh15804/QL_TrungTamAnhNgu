﻿// xử handleClick user
const user = document.querySelector(".user")
const iconEye = document.querySelector(".iconEye")
const inputPassWord = document.getElementById("inputMatKhau")

const form1 = document.querySelector(".myForm1");
const form2 = document.querySelector(".myForm2");
const form3 = document.querySelector(".myForm3");
const form4 = document.querySelector(".myForm4");

if (form1) {
    form1.addEventListener("submit", handleForm1)
}

if (form2) {
    form2.addEventListener("submit", handleForm2)
}

if (form3) {
    form3.addEventListener("submit", handleForm3)
}

if (form4) {
    form4.addEventListener("submit", handleForm4)
}

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

const errTenDangNhap = document.querySelector(".errTenDangNhap");
const errMatKhau = document.querySelector(".errMatKhau");
const errEmail = document.querySelector(".errEmail");
const errHoTen = document.querySelector(".errHoTen");
const errNhomNguoiDung = document.querySelector(".errNhomNguoiDung");
const errAvatar = document.querySelector(".errAvatar")

const inputTenDangNhap = document.getElementById("TenDangNhap");
const inputMatKhau = document.getElementById("MatKhau");
const inputEmail = document.getElementById("Email");
const inputHoTen = document.getElementById("HoTen");
const inputMaNhom = document.getElementById("MaNhom");

function handleForm1(event) {
    if (errTenDangNhap) {
        errTenDangNhap.innerText = "";
    }
    errMatKhau.innerText = "";
    errEmail.innerText = "";
    errHoTen.innerText = "";
    if (errNhomNguoiDung) {
        errNhomNguoiDung.innerText = "";
    }
    errAvatar.innerText = "";

    let isValid = true;
    
    if (inputMaNhom.value === "") {
        errNhomNguoiDung.innerText = "Nhóm người dùng bắt buộc!"
        isValid = false;
    }
    
    if (inputTenDangNhap.value.trim() === "") {
         errTenDangNhap.innerText = "Tên đăng nhập bắt buộc!";
         isValid = false;
    } else if (inputTenDangNhap.value.length < 5) {
        errTenDangNhap.innerText = "Tên đăng nhập ít nhất 5 kí tự!";
    } else if (inputTenDangNhap.value.trim().includes(" ")) {
        errTenDangNhap.innerText = "Tên đăng nhập không chứa khoảng trắng!";
        isValid = false;
    }

    if (inputMatKhau.value.trim() === "") {
         errMatKhau.innerText = "Mật khẩu bắt buộc!";
         isValid = false;
    } else if (inputMatKhau.value.length < 8) {
        errMatKhau.innerText = "Mật khẩu ít nhất 8 kí tự!";
        isValid = false;
    }

     let regexEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
     if (inputEmail.value.trim() === "") {
          errEmail.innerText = "Email bắt buộc!";
         isValid = false;
     } else if (!regexEmail.test(inputEmail.value)) {
          errEmail.innerText = "Email không hợp lệ!";
          isValid = false;
     }

     let regexHoTen = /\d/; // \d đại diện kí tự số
     if (inputHoTen.value.trim() === "") {
          errHoTen.innerText = "Họ tên bắt buộc!";
          isValid = false;
     } else if (regexHoTen.test(inputHoTen.value)) {
         errHoTen.innerText = "Họ tên không chứa kí tự số!";
         isValid = false;
     } else if (inputHoTen.value.length < 6) {
         errHoTen.innerText = "Họ tên ít nhất 6 kí tự!";
         isValid = false;
     }

     var avatarPreview = document.getElementById("avatarPreview").getAttribute("src");
     if(avatarPreview.includes("avatar_default.jpg")) {
         errAvatar.innerText = "Avatar bắt buộc!";
         isValid = false;
     }

     if (!isValid) {
          event.preventDefault();  // Ngăn submit nếu có lỗi
     }
}

const selectMaNhom = document.getElementById("MaNhom");
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

const errSoDienThoai = document.querySelector(".errSoDienThoai");
const errDiaChi = document.querySelector(".errDiaChi");
const errChuyenNganh = document.querySelector(".errChuyenNganh");
const inputSoDienThoai = document.getElementById("SoDienThoai");
const inputDiaChi = document.getElementById("DiaChi");
const inputChuyenNganh = document.getElementById("ChuyenNganh");

function handleForm2(event) {
    if (errTenDangNhap) {
        errTenDangNhap.innerText = "";
    }
    errMatKhau.innerText = "";
    errEmail.innerText = "";
    errHoTen.innerText = "";
    errSoDienThoai.innerText = "";
    errDiaChi.innerText = "";
    errChuyenNganh.innerText = "";
    errAvatar.innerText = "";

    let isValid = true;

    if (inputTenDangNhap.value.trim() === "") {
        errTenDangNhap.innerText = "Tên đăng nhập bắt buộc!";
        isValid = false;
    } else if (inputTenDangNhap.value.length < 5) {
        errTenDangNhap.innerText = "Tên đăng nhập ít nhất 5 kí tự!";
    } else if (inputTenDangNhap.value.trim().includes(" ")) {
        errTenDangNhap.innerText = "Tên đăng nhập không chứa khoảng trắng!";
        isValid = false;
    }

    if (inputMatKhau.value.trim() === "") {
        errMatKhau.innerText = "Mật khẩu bắt buộc!";
        isValid = false;
    } else if (inputMatKhau.value.length < 8) {
        errMatKhau.innerText = "Mật khẩu ít nhất 8 kí tự!";
        isValid = false;
    }

    let regexEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (inputEmail.value.trim() === "") {
        errEmail.innerText = "Email bắt buộc!";
        isValid = false;
    } else if (!regexEmail.test(inputEmail.value)) {
        errEmail.innerText = "Email không hợp lệ!";
        isValid = false;
    }

    let regexHoTen = /\d/; // \d đại diện kí tự số
    if (inputHoTen.value.trim() === "") {
        errHoTen.innerText = "Họ tên bắt buộc!";
        isValid = false;
    } else if (regexHoTen.test(inputHoTen.value)) {
        errHoTen.innerText = "Họ tên không chứa kí tự số!";
        isValid = false;
    } else if (inputHoTen.value.length < 6) {
        errHoTen.innerText = "Họ tên ít nhất 6 kí tự!";
        isValid = false;
    }

    let regexChuCai = /[a-zA-Z]/; // \d đại diện kí tự chữ
    if (inputSoDienThoai.value.trim() === "") {
        errSoDienThoai.innerText = "Số điện thoại bắt buộc!";
        isValid = false;
    } else if (regexChuCai.test(inputSoDienThoai.value)) {
        errSoDienThoai.innerText = "Số điện thoại không chứa kí tự chữ!";
        isValid = false;
    }

    if (inputDiaChi.value.trim() === "") {
        errDiaChi.innerText = "Địa chỉ bắt buộc!";
        isValid = false;
    }

    if (inputChuyenNganh.value.trim() === "") {
        errChuyenNganh.innerText = "Chuyên ngành bắt buộc!";
        isValid = false;
    }

    var avatarPreview = document.getElementById("avatarPreview").getAttribute("src");
    if (avatarPreview.includes("avatar_default.jpg")) {
        errAvatar.innerText = "Avatar bắt buộc!";
        isValid = false;
    }

    if (!isValid) {
        event.preventDefault();  // Ngăn submit nếu có lỗi
    }
}

const btnClearForm2 = document.getElementById("btnClearForm2")
if (btnClearForm2) {
    btnClearForm2.addEventListener("click", function (event) {
        event.preventDefault();
        inputTenDangNhap1.value = "";
        inputMatKhau1.value = "";
        inputEmail1.value = "";
        inputHoTen1.value = "";
        inputSoDienThoai1.value = "";
        inputDiaChi1.value = "";
        inputChuyenNganh1.value = "";
    })
}

const errNgaySinh = document.querySelector(".errNgaySinh")
const errGioiTinh = document.querySelector(".errGioiTinh")
const inputNgaySinh = document.getElementById("NgaySinh");
const inputGioiTinh = document.getElementById("GioiTinh")

function handleForm3(event) {
    if (errTenDangNhap) {
        errTenDangNhap.innerText = "";
    }
    errMatKhau.innerText = "";
    errEmail.innerText = "";
    errHoTen.innerText = "";
    errSoDienThoai.innerText = "";
    errDiaChi.innerText = "";
    if (errNgaySinh) {
        errNgaySinh.innerText = "";
    }

    let isValid = true;

    if (inputTenDangNhap.value.trim() === "") {
        errTenDangNhap.innerText = "Tên đăng nhập bắt buộc!";
        isValid = false;
    } else if (inputTenDangNhap.value.length < 5) {
        errTenDangNhap.innerText = "Tên đăng nhập ít nhất 5 kí tự!";
    } else if (inputTenDangNhap.value.trim().includes(" ")) {
        errTenDangNhap.innerText = "Tên đăng nhập không chứa khoảng trắng!";
        isValid = false;
    }

    if (inputMatKhau.value.trim() === "") {
        errMatKhau.innerText = "Mật khẩu bắt buộc!";
        isValid = false;
    } else if (inputMatKhau.value.length < 8) {
        errMatKhau.innerText = "Mật khẩu ít nhất 8 kí tự!";
        isValid = false;
    }

    let regexEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (inputEmail.value.trim() === "") {
        errEmail.innerText = "Email bắt buộc!";
        isValid = false;
    } else if (!regexEmail.test(inputEmail.value)) {
        errEmail.innerText = "Email không hợp lệ!";
        isValid = false;
    }

    let regexHoTen = /\d/; // \d đại diện kí tự số
    if (inputHoTen.value.trim() === "") {
        errHoTen.innerText = "Họ tên bắt buộc!";
        isValid = false;
    } else if (regexHoTen.test(inputHoTen.value)) {
        errHoTen.innerText = "Họ tên không chứa kí tự số!";
        isValid = false;
    } else if (inputHoTen.value.length < 6) {
        errHoTen.innerText = "Họ tên ít nhất 6 kí tự!";
        isValid = false;
    }

    let regexChuCai = /[a-zA-Z]/;
    if (inputSoDienThoai.value.trim() === "") {
        errSoDienThoai.innerText = "Số điện thoại bắt buộc!";
        isValid = false;
    } else if (regexChuCai.test(inputSoDienThoai.value)) {
        errSoDienThoai.innerText = "Số điện thoại không chứa kí tự chữ!";
        isValid = false;
    }

    if (inputDiaChi.value.trim() === "") {
        errDiaChi.innerText = "Địa chỉ bắt buộc!";
        isValid = false;
    }

    if (inputNgaySinh.value.trim() === "") {
        errNgaySinh.innerText = "Ngày sinh bắt buộc!";
        isValid = false;
    }

    var avatarPreview = document.getElementById("avatarPreview").getAttribute("src");
    if (avatarPreview.includes("avatar_default.jpg")) {
        errAvatar.innerText = "Avatar bắt buộc!";
        isValid = false;
    }

    if (!isValid) {
        event.preventDefault();  // Ngăn submit nếu có lỗi
    }
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

const btnHuy = document.getElementById("btnHuy")
const btnSua = document.getElementById("btnSua")
const btnLuu = document.getElementById("btnLuu")

const label = document.getElementById("label")
const matKhau = document.getElementById("MatKhau")
const email = document.getElementById("Email")
const hoTen = document.getElementById("HoTen")
const trangThai = document.getElementById("TrangThai")
const maNhom = document.getElementById("MaNhom")
const soDienThoai = document.getElementById("SoDienThoai")
const chuyenNganh = document.getElementById("ChuyenNganh")
const diaChi = document.getElementById("DiaChi")
const gioiTinh = document.getElementById("GioiTinh")
const ngaySinh = document.getElementById("NgaySinh")

const tenLoai = document.getElementById("TenLoai")
const moTa = document.getElementById("MoTa")

let saveMatKhau = "";
let saveEmail = "";
let saveHoten = "";
let saveTrangThai = "";
let saveMaNhom = "";
let saveSoDienThoai = "";
let saveChuyenNganh = "";
let saveDiaChi = "";
let saveGioiTinh = "";
let saveNgaySinh = "";
let saveTenLoai = "";
let saveMoTa = "";

if (matKhau) {
    saveMatKhau = matKhau.value;
}
if (email) {
    saveEmail = email.value;
}
if (hoTen) {
    saveHoten = hoTen.value;
}
if (trangThai) {
    saveTrangThai = trangThai.value;
}
if (maNhom) {
    saveMaNhom = maNhom.value;
}
if (soDienThoai) {
    saveSoDienThoai = soDienThoai.value;
}
if (chuyenNganh) {
    saveChuyenNganh = chuyenNganh.value;
}
if (diaChi) {
    saveDiaChi = diaChi.value;
}
if (gioiTinh) {
    saveGioiTinh = gioiTinh.value;
}
if (ngaySinh) {
    saveNgaySinh = ngaySinh.value;
}
if (tenLoai) {
    saveTenLoai = tenLoai.value;
}
if (moTa) {
    saveMoTa = moTa.value;
}
    
if (btnSua) {
    btnSua.addEventListener("click", function () {
        btnHuy.disabled = false;
        btnLuu.disabled = false;
        btnSua.disabled = true;
        if (matKhau) {
            matKhau.removeAttribute("readonly");
        }
        if (email) {
            email.removeAttribute("readonly");
        }
        if (hoTen) {
            hoTen.removeAttribute("readonly");
        }
        if (trangThai) {
            trangThai.disabled = false;
        }
        if (maNhom) {
            maNhom.disabled = false;
        }
        if (label) {
            label.style.cursor = "pointer";
            label.style.pointerEvents = "auto";
        }

        if (soDienThoai) {
            soDienThoai.removeAttribute("readonly");
        }
        if(chuyenNganh) {
            chuyenNganh.removeAttribute("readonly");
        }
        if (diaChi) {
            diaChi.removeAttribute("readonly");
        }
        if (gioiTinh) {
            gioiTinh.disabled = false;
        }
        if (ngaySinh) {
            ngaySinh.removeAttribute("readonly");
        }
        if (tenLoai) {
            tenLoai.removeAttribute("readonly");
        }
        if (moTa) {
            moTa.removeAttribute("readonly");
        }
    })
    btnHuy.addEventListener("click", function () {
        if (matKhau) {
            matKhau.value = saveMatKhau;
        }
        if (email) {
            email.value = saveEmail;
        }
        if (hoTen) {
            hoTen.value = saveHoten;
        }
        if (trangThai) {
            trangThai.value = saveTrangThai;
        }
        if (maNhom) {
            maNhom.value = saveMaNhom;
        }
        if (soDienThoai) {
            soDienThoai.value = saveSoDienThoai;
        }
        if (chuyenNganh) {
            chuyenNganh.value = saveChuyenNganh;
        }
        if (diaChi) {
            diaChi.value = saveDiaChi;
        }
        if (gioiTinh) {
            gioiTinh.value = saveGioiTinh;
        }
        if (ngaySinh) {
            ngaySinh.save = saveNgaySinh;
        }
        if (tenLoai) {
            tenLoai.value = saveTenLoai;
        }
        if (moTa) {
            moTa.value = saveMoTa;
        }
    })
}

function handleDeleteItemTemporary(button) {
    var item = button.parentElement;
    var row = item.parentElement;
    row.remove();
}

const errTenLoai = document.querySelector(".errTenLoai")
const errMoTa = document.querySelector(".errMoTa")
const inputTenLoai = document.getElementById("TenLoai")
const inputMoTa = document.getElementById("MoTa")
function handleForm4() {
    errTenLoai.innerText = "";
    errMoTa.innerText = "";
    let isValid = true;
    let regexHoTen = /\d/; // \d đại diện kí tự số
    if (inputTenLoai.value.trim() === "") {
        errTenLoai.innerText = "Tên loại bắt buộc!";
        isValid = false;
    } else if (regexHoTen.test(inputTenLoai.value)) {
        errTenLoai.innerText = "Tên loại không chứa kí tự số!";
        isValid = false;
    }

    if (inputMoTa.value.trim() === "") {
        errMoTa.innerText = "Mô tả bắt buộc!";
        isValid = false;
    }

    if (!isValid) {
        event.preventDefault();  // Ngăn submit nếu có lỗi
    }
}