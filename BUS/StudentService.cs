using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class StudentService
    {
        
        
            QlySV context = new QlySV();

            public List<Sinhvien> GetAll()
            {

                return context.Sinhviens.ToList();
            }
            public void AddSinhVien(Sinhvien sinhVien)
            {
                context.Sinhviens.Add(sinhVien);
                context.SaveChanges();
            }
             public void SaveChanges()
             {
                context.SaveChanges(); 
             }

        public void UpdateSinhVien(Sinhvien updatedSinhVien)
            {
                var existingSinhVien = context.Sinhviens.Find(updatedSinhVien.MaSV);
                if (existingSinhVien != null)
                {
                    existingSinhVien.HotenSV = updatedSinhVien.HotenSV;
                    existingSinhVien.Ngaysinh = updatedSinhVien.Ngaysinh;
                    existingSinhVien.MaLop = updatedSinhVien.MaLop;

                    context.SaveChanges();
                }
            }
            public Sinhvien FindByID(String MaSV)
            {
                QlySV context = new QlySV();
                return context.Sinhviens.FirstOrDefault(p => p.MaSV == MaSV);
            }
            public void DeleteSinhVien(string studentID)
            {
                var sinhVien = context.Sinhviens.Find(studentID);
                if (sinhVien != null)
                {
                    context.Sinhviens.Remove(sinhVien);
                    context.SaveChanges();
                }
            }

            public void InsertUpdate(Sinhvien sinhvien, bool isEdit)
            {
                using (var context = new QlySV())
                {
                    if (isEdit)
                    {

                        var existingStudent = context.Sinhviens.Find(sinhvien.MaSV);
                        if (existingStudent != null)
                        {
                            context.Entry(existingStudent).CurrentValues.SetValues(sinhvien);
                        }
                    }
                    else
                    {

                        context.Sinhviens.Add(sinhvien);
                    }

                    context.SaveChanges();
                }
            }
        }
    }

