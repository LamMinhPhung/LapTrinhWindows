using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Kiemtraa
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<Lop> listLop = context.Lop.ToList(); 
                List<Sinhvien> listSinhvien = context.Sinhvien.ToList(); 
                FillLopCombobox(listLop);
                BindGrid(listSinhvien);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillLopCombobox(List<Lop> listLop)
        {
            this.cmbLop.DataSource = listLop;
            this.cmbLop.DisplayMember = "TenLop";
            this.cmbLop.ValueMember = "MaLop";
        }
        private void BindGrid(List<Sinhvien> listSinhvien)
        {
            dgvSinhvien.Rows.Clear();
            foreach (var item in listSinhvien)
            {
                int index = dgvSinhvien.Rows.Add();
                dgvSinhvien.Rows[index].Cells[0].Value = item.MaSV;
                dgvSinhvien.Rows[index].Cells[1].Value = item.HotenSV;
                dgvSinhvien.Rows[index].Cells[2].Value = item.NgaySinh;
                dgvSinhvien.Rows[index].Cells[3].Value = item.Lop.TenLop;
            }
        }

        private void btThoat_Click(object sender, EventArgs e)
        {

            DialogResult res = MessageBox.Show("Ban co muon thoat khong ", "Thong bao",
                              MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMSSV.Text) ||
               string.IsNullOrWhiteSpace(txtHoten.Text) ||
               cmbLop.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {

                Model1 context = new Model1();

                List<Sinhvien> SinhvientList = context.Sinhvien.ToList();

                if (SinhvientList.Any(s => s.MaSV == txtMSSV.Text))
                {
                    MessageBox.Show("Mã SV đã tồn tại. Vui lòng nhập một mã khác.",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                var newStudent = new Sinhvien
                {
                    MaSV = txtMSSV.Text,
                    HotenSV = txtHoten.Text,
                    NgaySinh = dtNgay.Value,
                    MaLop = cmbLop.SelectedValue.ToString(),

                };

                context.Sinhvien.Add(newStudent);
                context.SaveChanges();
                BindGrid(context.Sinhvien.ToList());

                MessageBox.Show("Thêm sinh viên thành công!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Ban co muon xoa khong ", "Thong bao",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                try
                {
                    using (Model1 context = new Model1())
                    {
                        // Lấy mã sinh viên từ dòng được chọn trong DataGridView
                        if (dgvSinhvien.CurrentRow != null)
                        {
                            string maSinhVien = dgvSinhvien.CurrentRow.Cells[0].Value.ToString();

                            // Tìm sinh viên trong database
                            var student = context.Sinhvien.FirstOrDefault(s => s.MaSV == maSinhVien);

                            if (student != null)
                            {
                                // Xóa sinh viên
                                context.Sinhvien.Remove(student);
                                context.SaveChanges();

                                // Load lại dữ liệu vào DataGridView
                                BindGrid(context.Sinhvien.ToList());
                                MessageBox.Show("Sinh viên đã được xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Sinh viên không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Vui lòng chọn sinh viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvSinhvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectRow = dgvSinhvien.Rows[e.RowIndex];
                txtMSSV.Text = selectRow.Cells[0].Value?.ToString();
                txtHoten.Text = selectRow.Cells[1].Value?.ToString();

                // Kiểm tra và chuyển đổi ngày tháng an toàn
                if (DateTime.TryParse(selectRow.Cells[2].Value?.ToString(), out DateTime ngaySinh))
                {
                    dtNgay.Value = ngaySinh; // Gán cho DateTimePicker
                }
                else
                {
                    MessageBox.Show("Ngày sinh không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                cmbLop.Text = selectRow.Cells[3].Value?.ToString();
            }
        }

        private void btTim_Click(object sender, EventArgs e)
        {
            try
            {
                string searchMSSV = textBox2.Text.Trim();

                if (string.IsNullOrWhiteSpace(searchMSSV))
                {
                    MessageBox.Show("Vui lòng nhập MSSV cần tìm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Model1 context = new Model1();
                var sinhvien = context.Sinhvien.FirstOrDefault(s => s.MaSV == searchMSSV);

                if (sinhvien != null)
                {
                    txtMSSV.Text = sinhvien.MaSV;
                    txtHoten.Text = sinhvien.HotenSV;
                    dtNgay.Value = (DateTime)sinhvien.NgaySinh;
                    cmbLop.SelectedValue = sinhvien.MaLop;
                    BindGrid(new List<Sinhvien> { sinhvien });

                    MessageBox.Show("Tìm thấy sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên với MSSV này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm sinh viên: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<Sinhvien> students = context.Sinhvien.ToList();
                var sinhvien = students.FirstOrDefault(s => s.MaSV == txtMSSV.Text);

                if (sinhvien != null)
                {

                    if (students.Any(s => s.MaSV == txtMSSV.Text && s.MaSV != sinhvien.MaSV))
                    {
                        MessageBox.Show("Mã SV đã tồn tại! BẠN VUI LONG NHẬP LẠI!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    sinhvien.HotenSV = txtHoten.Text;

                    sinhvien.NgaySinh = dtNgay.Value;
                    sinhvien.MaLop = cmbLop.SelectedValue.ToString();

                    context.SaveChanges();

                    BindGrid(context.Sinhvien.ToList());

                    MessageBox.Show("Chỉnh sửa thông tin SV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {

                    MessageBox.Show("Không tìm thấy sinh viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
