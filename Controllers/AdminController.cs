using MvcBookStore.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;
using PagedList.Mvc;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.ComTypes;
using System.Reflection;

namespace MvcBookStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly dbQLBansachDataContext db;

        public AdminController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["QLBANSACHConnectionString"].ConnectionString;
            db = new dbQLBansachDataContext(connectionString);
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["name"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                // Gán giá trị cho đối tượng được tạo mới (ad)   
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);

                if (ad != null)
                {
                    // ViewBag.Thongbao = "Chúc mừng đăng nhập thành công";   
                    Session["Taikhoanadmin"] = ad;
                    Session["tendn"] = tendn;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

   
        public ActionResult Sach(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 4;
            //var sachmoi = Laysachmoi();
            //return View(sachmoi.ToPagedList(pageNumber, pageSize));
            return View(db.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize));
            //return View(books);
        }
        public ActionResult Themmoisach()
        {
            //Dữa dư liệu vào dropdownList  
            //Lấy ds từ table chú để, sắp xếp tăng dần theo tên chủ đề, chọn lày giá trị Ma CD, hiện thì Tên chủ đề  
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisach(SACH sach, HttpPostedFileBase fileupload)
        {
           
            string mota = sach.Mota;

            // Loại bỏ các thẻ HTML
            string plainText = Regex.Replace(mota, "<.*?>", String.Empty);
            Debug.WriteLine($"Tên sách: {sach.Tensach}");
            Debug.WriteLine($"Giá bán: {sach.Giaban}");
            Debug.WriteLine($"File tải lên: {fileupload?.FileName}");
            ViewBag.TenSach = sach.Tensach;
            ViewBag.GiaBan = sach.Giaban;
            ViewBag.FileName = sach.Anhbia;
            ViewBag.Mota = plainText;
            // Đưa dữ liệu vào dropdownload  
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            // Kiểm tra đường dẫn file  
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            // Thêm vào CSDL  
            else
            {
                if (ModelState.IsValid)
                {
                    // Lưu tên file, lưu ý bổ sung thư viện using System.IO;  
                    var fileName = Path.GetFileName(fileupload.FileName);
                    // Lưu đường dẫn của file  
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    // Kiểm tra hình ảnh tồn tại chưa?  
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // Lưu hình ảnh vào đường dẫn  
                        fileupload.SaveAs(path);

                    }

                    sach.Mota = plainText;
                    sach.Anhbia = fileName;
                    //// Lưu vào CSDL  
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();
                }
                return RedirectToAction("Sach");
            }
        }

        public ActionResult Chitietsach(int id)
        {
            //Lay ra doi tuong sach theo ma  
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        //Xóa sản phẩm  
        [HttpGet]
        public ActionResult Xoasach(int id)
        {
            //Lấy ra đối tượng sách cần xóa theo mã  
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Xoasach")]
        public ActionResult Xacnhanxoa(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma  
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");
        }
        //Chinh sua san pham  
        [HttpGet]
        public ActionResult Suasach(int id)
        {
            //Lay ra doi tuong sach theo ma  
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe",sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB",sach.MaNXB);
            return View(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasach(SACH sach, HttpPostedFileBase fileupload)
        {
            string mota = sach.Mota;

            // Loại bỏ các thẻ HTML
            string plainText = Regex.Replace(mota, "<.*?>", String.Empty);
            string decodedText = HttpUtility.HtmlDecode(plainText);
            //ViewBag.Mota = decodedText;
            // Dua du lieu vao dropdownlist  
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            ViewBag.TenSach = sach.Tensach;
            ViewBag.GiaBan = sach.Giaban;
            //ViewBag.FileName = sach.Anhbia;
            ViewBag.Soluongton = sach.Soluongton;
            ViewBag.Ngaycapnhat = sach.Ngaycapnhat;
            Debug.WriteLine($"Tên sách: {sach.Tensach}, Giá bán: {sach.Giaban}, Mô tả: {sach.Mota}, File: {fileupload?.FileName}");
            // Kiem tra duong dan file  
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(sach);
            }
            // Them vao CSDL  
            else
            {
                if (ModelState.IsValid)
                {
                    // Luu ten file, luu y bo sung thu vien using System.IO;  
                    var fileName = Path.GetFileName(fileupload.FileName);
                    // Luu duong dan file  
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    // Kiem tra hinh anh ton tai chua?  
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại!";
                    }
                    else
                    {
                        // Luu hinh anh vao duong dan  
                        fileupload.SaveAs(path);
                    }
                
                    ViewBag.FileName = fileName;
             
                    var sachToUpdate = db.SACHes.SingleOrDefault(s => s.Masach == sach.Masach);
                    string motaconver = sachToUpdate.Mota;
                    string plainTextConvert = Regex.Replace(motaconver, "<.*?>", String.Empty);
                    sachToUpdate.Mota = plainText;
                    if (sachToUpdate != null)
                    {


                        //Loại bỏ các thẻ HTML


                        sachToUpdate.Tensach = sach.Tensach;
                        sachToUpdate.Giaban = sach.Giaban;
                        sachToUpdate.Mota = decodedText;
                        sachToUpdate.Anhbia = fileName;

                        db.SubmitChanges();
                    }
                 
            }
                return RedirectToAction("Sach");
                //return View(sach);
            }
       
        }




    }
}