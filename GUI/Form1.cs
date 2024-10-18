using BUS;
using System.Windows.Forms;
using System;
using DAL.Model;
using System.Collections.Generic;
using System.Linq;



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

        // Load dữ liệu từ cơ sở dữ liệu
        private void LoadData()
        {
            List<Sinhvien> sinhviens = studentService.GetAll();
            BindGrid(sinhviens);
        }

        // Hiển thị dữ liệu sinh viên lên DataGridView
        private void BindGrid(List<Sinhvien> sinhViens)
        {
            dgvDSSV.Rows.Clear();
            foreach (var s in sinhViens)
            {
                int index = dgvDSSV.Rows.Add();
                dgvDSSV.Rows[index].Cells[0].Value = s.MaSV;
                dgvDSSV.Rows[index].Cells[1].Value = s.HotenSV;
                dgvDSSV.Rows[index].Cells[2].Value = s.Ngaysinh;
                dgvDSSV.Rows[index].Cells[3].Value = s.Lop.TenLop; 
            }
        }

        // Điền dữ liệu vào ComboBox các lớp
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

        // Thêm sinh viên mới
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
                    MaLop = cmbLop.SelectedValue.ToString() 
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
            student.MaLop = cmbLop.SelectedValue.ToString(); 

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

// Xóa sinh viên
private void btnXoa_Click(object sender, EventArgs e)
{
    try
    {
        // Kiểm tra xem người dùng đã chọn hàng nào chưa
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

        private void dgvDSSV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Kiểm tra nếu nhấp vào hàng hợp lệ
            {
                var selectedRow = dgvDSSV.Rows[e.RowIndex];
                txtMSSV.Text = selectedRow.Cells[0].Value.ToString();
                txtHoTen.Text = selectedRow.Cells[1].Value.ToString();
                dtpNgaySinh.Value = Convert.ToDateTime(selectedRow.Cells[2].Value);
                cmbLop.SelectedValue = selectedRow.Cells[3].Value.ToString();
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string searchText = txtTim.Text.ToLower();
            List<Sinhvien> allStudents = studentService.GetAll();
            var filteredStudents = allStudents.Where(s => s.HotenSV.ToLower().Contains(searchText)).ToList();
            BindGrid(filteredStudents);
        }

        private void FrmSinhVien_FormClosing(object sender, FormClosingEventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn thoát không?",
                                                 "Xác nhận thoát",
                                                 MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}