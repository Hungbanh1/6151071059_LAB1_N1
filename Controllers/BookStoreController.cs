using MvcBookStore.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class BookStoreController : Controller
    {
      
        private readonly dbQLBansachDataContext data;

        public BookStoreController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["QLBANSACHConnectionString"].ConnectionString;
            data = new dbQLBansachDataContext(connectionString);
        }
        // Tao 1 doi tuong chua tona bo CSDL tu dbQLBansach  
        


        private List<SACH> Laysachmoi(int count)
        {
            // Sap xep giam dan theo Ngaycapnhat, lay count dong dau  
            return data.SACHes.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }

        public ActionResult Index()
        {
            // Lay 5 quyen sach moi nhat  
            var sachmoi = Laysachmoi(5);
            return View(sachmoi);
        }
        public ActionResult ChuDe() {
            var chude = from cd in data.CHUDEs select cd;
            return PartialView(chude);
        }
     
        public ActionResult SPTheochude(int id)
        {
            var sach = from s in data.SACHes where s.MaCD == id select s;
            return View(sach);
        }
        public ActionResult NhaXuatBan()
        {
            var nxb = from cd in data.NHAXUATBANs select cd;
            return PartialView(nxb);
        }
        public ActionResult SPTheoNXB(int id)
        {
            var sach = from s in data.SACHes where s.MaNXB == id select s;
            return View(sach);
        }
        public ActionResult Details(int id)
        {
            var sach = from s in data.SACHes
                       where s.Masach == id
                       select s;
            return View(sach.Single());
        }
    }
}