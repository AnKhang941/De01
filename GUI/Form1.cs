using BUS;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GUI
{
    public partial class FrmSinhVien : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();

        public FrmSinhVien()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            List<Sinhvien> sinhviens = studentService.GetAll();
            BindGrid(sinhviens);
        }

        private void BindGrid(List<Sinhvien> sinhViens)
        {
            dgvDSSV.Rows.Clear();
            foreach (var s in sinhViens)
            {
                int index = dgvDSSV.Rows.Add();
                dgvDSSV.Rows[index].Cells[0].Value = s.MaSV;
                dgvDSSV.Rows[index].Cells[1].Value = s.HotenSV;
                dgvDSSV.Rows[index].Cells[2].Value = s.Ngaysinh;
                dgvDSSV.Rows[index].Cells[3].Value = s.Lop?.TenLop;
            }
        }

        private void FillFacultyCombobox(List<Lop> lops)
        {
            cmbLop.DataSource = lops;
            cmbLop.DisplayMember = "TenLop";
            cmbLop.ValueMember = "MaLop";
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSinhVien_Load(object sender, EventArgs e)
        {
            LoadData();
            List<Lop> lops = facultyService.GetAllLop();
            FillFacultyCombobox(lops);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtMSSV.Text) ||
                    string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                    cmbLop.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đủ thông tin: MSSV, Họ tên và Lớp.");
                    return;
                }

                string studentId = txtMSSV.Text;
                if (studentService.FindByID(studentId) != null)
                {
                    MessageBox.Show("Mã số sinh viên đã tồn tại. Vui lòng nhập mã khác.");
                    return;
                }

                Sinhvien newStudent = new Sinhvien
                {
                    MaSV = studentId,
                    HotenSV = txtHoTen.Text,
                    Ngaysinh = dtpNgaySinh.Value,
                    MaLop = cmbLop.SelectedValue.ToString(),
                    Lop = new Lop { TenLop = cmbLop.Text } // Tạo đối tượng lớp
                };

                studentService.AddSinhVien(newStudent);
                studentService.SaveChanges();

                MessageBox.Show("Thêm sinh viên thành công.");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            
                try
                {
                    if (dgvDSSV.SelectedRows.Count == 0)
                    {
                        MessageBox.Show("Vui lòng chọn một sinh viên để sửa thông tin.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                        cmbLop.SelectedValue == null)
                    {
                        MessageBox.Show("Vui lòng nhập đủ thông tin: Họ tên và Lớp.");
                        return;
                    }

                    var selectedRow = dgvDSSV.SelectedRows[0];
                    string studentId = selectedRow.Cells[0].Value.ToString();

                    var student = studentService.FindByID(studentId);
                    if (student != null)
                    {
                        student.HotenSV = txtHoTen.Text;
                        student.Ngaysinh = dtpNgaySinh.Value;
                        student.MaLop = cmbLop.SelectedValue.ToString(); // Cập nhật mã lớp

                        // Nếu đối tượng Lop là null, hãy khởi tạo lại
                        if (student.Lop == null)
                        {
                            student.Lop = new Lop();
                        }
                        student.Lop.TenLop = cmbLop.Text; // Cập nhật tên lớp

                        studentService.SaveChanges();
                        MessageBox.Show("Sửa thông tin sinh viên thành công.");
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi: {ex.Message}");
                }
            

        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvDSSV.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn một sinh viên để xóa.");
                    return;
                }

                var selectedRow = dgvDSSV.SelectedRows[0];
                string studentId = selectedRow.Cells[0].Value.ToString();

                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này không?",
                                                     "Xác nhận xóa",
                                                     MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    var student = studentService.FindByID(studentId);
                    if (student != null)
                    {
                        studentService.DeleteSinhVien(studentId);
                        studentService.SaveChanges();
                        MessageBox.Show("Xóa sinh viên thành công.");
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchText = txtTim.Text.ToLower();
            List<Sinhvien> allStudents = studentService.GetAll();
            var filteredStudents = allStudents.Where(s => s.HotenSV.ToLower().Contains(searchText)).ToList();
            BindGrid(filteredStudents);
        }

        private void dgvDSSV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                var selectedRow = dgvDSSV.Rows[e.RowIndex];
                txtMSSV.Text = selectedRow.Cells[0].Value.ToString();
                txtHoTen.Text = selectedRow.Cells[1].Value.ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(selectedRow.Cells[2].Value);
                cmbLop.SelectedValue = selectedRow.Cells[3].Value.ToString();
            }
        }
    }
}
