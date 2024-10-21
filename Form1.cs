using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Entity_Framework_Data_Binding
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); // Khởi tạo các thành phần giao diện

            // Kết nối sự kiện Load của Form với phương thức Form1_Load
            this.Load += Form1_Load;
            dataGridView1.CellClick += dataGridView1_CellContentClick;

        }

        //
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureDataGridView();

                using (var context = new SchoolContext()) // Sử dụng SchoolContext để kết nối cơ sở dữ liệu
                {
                    // Giả sử bạn có một bảng ngành học (Major) để lấy danh sách cho ComboBox
                    List<string> majors = context.Students.Select(s => s.Major).Distinct().ToList(); // Lấy danh sách ngành học duy nhất

                    // Điền dữ liệu vào ComboBox
                    FillMajorComboBox(majors);

                    // Tải dữ liệu sinh viên vào DataGridView
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FillMajorComboBox(List<string> majors)
        {
            cmbKhoa.DataSource = majors; // Điền danh sách ngành học vào ComboBox
            cmbKhoa.SelectedItem = null;  // Đặt giá trị mặc định không chọn gì
        }
        private void BindGrid(List<Student> listStudents)
        {
            dataGridView1.AutoGenerateColumns = false; // Không tự động tạo cột
            dataGridView1.DataSource = null; // Đặt DataSource về null trước
            dataGridView1.DataSource = listStudents; // Đặt DataSource cho DataGridView
        }
        /// <summary>
        /// /
        /// </summary>
        private void ConfigureDataGridView()
        {
            // Xóa các cột cũ
            dataGridView1.Columns.Clear();

            // Cột StudentId
            DataGridViewTextBoxColumn colStudentId = new DataGridViewTextBoxColumn();
            colStudentId.Name = "StudentId";
            colStudentId.HeaderText = "Mã Sinh Viên";
            colStudentId.DataPropertyName = "StudentId";
            dataGridView1.Columns.Add(colStudentId);

            // Cột FullName
            DataGridViewTextBoxColumn colFullName = new DataGridViewTextBoxColumn();
            colFullName.Name = "FullName";
            colFullName.HeaderText = "Tên Sinh Viên";
            colFullName.DataPropertyName = "FullName";
            dataGridView1.Columns.Add(colFullName);

            // Cột Age
            DataGridViewTextBoxColumn colAge = new DataGridViewTextBoxColumn();
            colAge.Name = "Age";
            colAge.HeaderText = "Tuổi";
            colAge.DataPropertyName = "Age";
            dataGridView1.Columns.Add(colAge);

            // Cột Major
            DataGridViewTextBoxColumn colMajor = new DataGridViewTextBoxColumn();
            colMajor.Name = "Major";
            colMajor.HeaderText = "Ngành Học";
            colMajor.DataPropertyName = "Major";
            dataGridView1.Columns.Add(colMajor);
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                // Lấy giá trị từ các cột tương ứng
                txtMa.Text = dataGridView1.CurrentRow.Cells["StudentId"].Value.ToString();
                txtTen.Text = dataGridView1.CurrentRow.Cells["FullName"].Value.ToString();
                txtTuoi.Text = dataGridView1.CurrentRow.Cells["Age"].Value.ToString();
                cmbKhoa.SelectedItem = dataGridView1.CurrentRow.Cells["Major"].Value.ToString(); // Nếu Major là kiểu dữ liệu trong ComboBox
            }
        }


        private void Them_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolContext())
            {
                // Kiểm tra các trường nhập liệu
                if (string.IsNullOrWhiteSpace(txtMa.Text) || string.IsNullOrWhiteSpace(txtTen.Text) || string.IsNullOrWhiteSpace(txtTuoi.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên!");
                    return;
                }

                if (!int.TryParse(txtTuoi.Text, out int age))
                {
                    MessageBox.Show("Tuổi không hợp lệ!");
                    return;
                }

                // Kiểm tra nếu sinh viên đã tồn tại
                var existingStudent = context.Students.FirstOrDefault(s => s.StudentId.ToString() == txtMa.Text);
                if (existingStudent == null)
                {
                    // Tạo một đối tượng sinh viên mới
                    Student newStudent = new Student()
                    {
                        StudentId = int.Parse(txtMa.Text),
                        FullName = txtTen.Text,
                        Age = age,
                        Major = cmbKhoa.SelectedItem.ToString()
                    };

                    // Thêm sinh viên mới vào cơ sở dữ liệu
                    context.Students.Add(newStudent);
                    context.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu

                    // Làm mới dữ liệu trong DataGridView
                    LoadData();

                    // Thông báo thành công
                    MessageBox.Show("Thêm mới dữ liệu thành công!");

                    // Xóa các trường nhập liệu sau khi thêm
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng nhập mã khác.");
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolContext())
            {
                if (dataGridView1.CurrentRow != null)
                {
                    // Lấy StudentId của sinh viên đang chọn
                    int studentId = (int)dataGridView1.CurrentRow.Cells["StudentId"].Value;

                    // Tìm sinh viên trong cơ sở dữ liệu theo StudentId
                    var studentToUpdate = context.Students.Find(studentId);

                    if (studentToUpdate != null)
                    {
                        try
                        {
                            // Kiểm tra xem dữ liệu nhập vào có hợp lệ không
                            if (string.IsNullOrWhiteSpace(txtMa.Text) || string.IsNullOrWhiteSpace(txtTen.Text) || string.IsNullOrWhiteSpace(txtTuoi.Text))
                            {
                                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sinh viên!");
                                return;
                            }

                            if (!int.TryParse(txtTuoi.Text, out int age))
                            {
                                MessageBox.Show("Tuổi không hợp lệ!");
                                return;
                            }

                            // Cập nhật thông tin sinh viên
                            studentToUpdate.FullName = txtTen.Text;
                            studentToUpdate.Age = age;
                            studentToUpdate.Major = cmbKhoa.SelectedItem.ToString();

                            // Lưu thay đổi vào cơ sở dữ liệu
                            context.SaveChanges();

                            // Làm mới lại DataGridView để hiển thị thông tin mới cập nhật
                            LoadData();

                            // Xóa các trường nhập liệu sau khi cập nhật
                            ClearInputFields();

                            MessageBox.Show("Cập nhật sinh viên thành công!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi cập nhật sinh viên: " + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên để cập nhật.");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần sửa.");
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (var context = new SchoolContext())
            {
                if (dataGridView1.CurrentRow != null)
                {
                    int selectedStudentId = (int)dataGridView1.CurrentRow.Cells["StudentId"].Value;

                    // Kiểm tra sinh viên có tồn tại không
                    Student dbDelete = context.Students.FirstOrDefault(s => s.StudentId == selectedStudentId);
                    if (dbDelete != null)
                    {
                        // Hiển thị cảnh báo YES/NO
                        var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                        if (confirmResult == DialogResult.Yes)
                        {
                            context.Students.Remove(dbDelete);
                            context.SaveChanges(); // Lưu thay đổi sau khi xóa

                            // Làm mới lại DataGridView để hiển thị thông tin sau khi xóa
                            LoadData();

                            // Thông báo thành công
                            MessageBox.Show("Xóa thành công!");

                            // Xóa các trường nhập liệu sau khi xóa
                            ClearInputFields();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Sinh viên cần xóa không tồn tại!");
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên cần xóa.");
                }
            }
        }
        private void LoadData()
        {
            using (var context = new SchoolContext())
            {
                // Lấy tất cả sinh viên từ cơ sở dữ liệu
                var students = context.Students.ToList();
                dataGridView1.DataSource = students; // Đặt DataSource cho DataGridView
            }
        }
        private void ClearInputFields()
        {
            if (txtMa != null) txtMa.Clear(); // Xóa nội dung của TextBox Ma Sinh Viên
            if (txtTen != null) txtTen.Clear(); // Xóa nội dung của TextBox Tên Sinh Viên
            if (txtTuoi != null) txtTuoi.Clear(); // Xóa nội dung của TextBox Tuổi
            if (cmbKhoa != null) cmbKhoa.SelectedIndex = -1; // Đặt ComboBox về trạng thái không chọn gì
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu có dữ liệu hợp lệ trong hàng được chọn
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells[0].Value != null)
            {
                // Lấy dòng hiện tại
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Cập nhật dữ liệu từ DataGridView sang các TextBox hoặc Button
                txtMa.Text = row.Cells["StudentId"].Value.ToString();
                txtTen.Text = row.Cells["FullName"].Value.ToString();
                txtTuoi.Text = row.Cells["Age"].Value.ToString();
                cmbKhoa.SelectedItem = row.Cells["Major"].Value.ToString();
            }
        }
    }
}
