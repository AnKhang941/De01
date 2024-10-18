using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class FacultyService
    {
        QlySV context = new QlySV();


        public List<Lop> GetAllLop()
        {
            return context.Lops.ToList();
        }
        


        public void AddLop(Lop lop)
        {
            context.Lops.Add(lop);
            context.SaveChanges();
        }


        public void UpdateLop(Lop updatedLop)
        {
            var existingKhoa = context.Lops.Find(updatedLop);
            if (existingKhoa != null)
            {
                existingKhoa.TenLop = updatedLop.TenLop;
                context.SaveChanges();
            }
        }


        public void DeleteLop(int khoaID)
        {
            var khoa = context.Lops.Find(khoaID);
            if (khoa != null)
            {
                context.Lops.Remove(khoa);
                context.SaveChanges();
            }
        }
    }
}

