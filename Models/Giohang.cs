using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MvcBookStore.Models
{
    public class Giohang
    {
        //Tạo đối tượng data chứa dữ liệu từ model dbBansach đã tạo.  
        //dbQlbansachDataContext data = new dbQlbansachDataContext();
        //private readonly dbQLBansachDataContext data;
        //dbQLBansachDataContext data = new dbQLBansachDataContext();
        //private readonly dbQLBansachDataContext data;
        //public Giohang()
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["QLBANSACHConnectionString"].ConnectionString;
        //    db = new dbQLBansachDataContext(connectionString);
        //}
        //string connectionString = ConfigurationManager.ConnectionStrings["QLBANSACHConnectionString"].ConnectionString;

        dbQLBansachDataContext db = new dbQLBansachDataContext(ConfigurationManager.ConnectionStrings["QLBANSACHConnectionString"].ConnectionString);
        public int iMasach { get; set; }
        public string sTenSach { get; set; }
        public string sAnhbia { get; set; }
        public Double dDongia { get; set; }
        public int iSoluong { get; set; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }

        //Khởi tạo giỏ hàng theo Masach được truyền vào với Soluong mặc định là 1  
        public Giohang(int Masach)
        {
            iMasach = Masach;

            // Tìm sách bằng SingleOrDefault để tránh lỗi nếu không tìm thấy
            SACH sach = db.SACHes.Single(n => n.Masach == iMasach);
            if (sach != null)
            {
                sTenSach = sach.Tensach;
                sAnhbia = sach.Anhbia;
                dDongia = double.Parse(sach.Giaban.ToString());
                iSoluong = 1; // Mặc định số lượng là 1
            }
            else
            {
                // Xử lý nếu không tìm thấy sách
                throw new Exception("Không tìm thấy sách với mã: " + iMasach);
            }
        }
    }
}