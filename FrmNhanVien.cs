using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace QLBanHang
{
    public partial class FrmNhanVien : Form
    {
        private DatabaseHelper dbHelper;
        private Label selectLabel;

        public FrmNhanVien()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper();
            selectLabel = new Label();
            selectLabel.Text = " select";
            LoadNhanVien();
            dgvNhanVien.CellClick += DgvNhanVien_CellClick;
        }

        // Phương thức tải danh sách nhân viên
        private void LoadNhanVien()
        {
            try
            {
                string query = "SELECT * FROM NhanVien";
                SqlDataReader reader = dbHelper.ExecuteReader(query);

                dgvNhanVien.Rows.Clear(); // Xóa các dòng cũ trong DataGridView
                while (reader.Read())
                {
                    dgvNhanVien.Rows.Add(
                        reader["MaNhanVien"].ToString(),
                        reader["TenNhanVien"].ToString(),
                        reader["GioiTinh"].ToString(),
                        reader["NgaySinh"].ToString(),
                        reader["Luong"].ToString()
                    );
                }

                reader.Close();
                dbHelper.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách nhân viên: " + ex.Message);
            }
        }


        public void GetNhanVienByName(string tenNhanVien)
        {
            try
            {
                // Xóa dữ liệu cũ trong DataGridView
                dgvNhanVien.Rows.Clear();

                // Kiểm tra tên nhân viên nhập vào có hợp lệ không
                if (string.IsNullOrEmpty(tenNhanVien))
                {
                    MessageBox.Show("Vui lòng nhập tên nhân viên để tìm kiếm.");
                    return;
                }

                string query = "SELECT TOP 1000 * FROM NhanVien " +
                               "WHERE TenNhanVien LIKE '%' + @TenNhanVien + '%' " +
                               "ORDER BY create_time DESC";

                using (SqlCommand cmd = new SqlCommand(query, dbHelper.GetConnection()))
                {
                    // Thêm tham số tìm kiếm
                    cmd.Parameters.AddWithValue("@TenNhanVien", tenNhanVien);

                    // Thực hiện truy vấn và lấy dữ liệu
                    using (SqlDataReader resultSet = cmd.ExecuteReader())
                    {
                        while (resultSet.Read())
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = resultSet["MaNhanVien"].ToString() });
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = resultSet["TenNhanVien"].ToString() });
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = resultSet["GioiTinh"].ToString() });
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = resultSet["NgaySinh"].ToString() });
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = resultSet["Luong"].ToString() });

                            dgvNhanVien.Rows.Add(row); // Thêm dữ liệu vào DataGridView
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Lỗi khi tìm kiếm nhân viên: " + e.Message);
            }
        }



        private void InsertNhanVien()
        {
            try
            {
                string query = "INSERT INTO NhanVien (MaNhanVien, TenNhanVien, GioiTinh, NgaySinh, Luong) " +
                               "VALUES (@MaNhanVien, @TenNhanVien, @GioiTinh, @NgaySinh, @Luong)";

                SqlParameter[] parameters = {
            new SqlParameter("@MaNhanVien", txtID.Text),
            new SqlParameter("@TenNhanVien", txtName.Text),
            new SqlParameter("@GioiTinh", selectLabel.Text),  
            new SqlParameter("@NgaySinh", txtNgaySinh.Text),
            new SqlParameter("@Luong", txtLuong.Text)
        };

                int rowsAffected = dbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Thêm mới thành công!");
                    ClearText();
                    LoadNhanVien();
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu được thêm vào.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm mới: " + ex.Message);
            }
        }


        private void UpdateNhanVien()
        {
            try
            {
                string query = "UPDATE NhanVien SET TenNhanVien = @TenNhanVien, GioiTinh = @GioiTinh, NgaySinh = @NgaySinh, Luong = @Luong WHERE MaNhanVien = @MaNhanVien";
                SqlParameter[] parameters = {
                    new SqlParameter("@TenNhanVien", txtName.Text),  // Use txtName instead of txtID for the name field
                    new SqlParameter("@GioiTinh", selectLabel.Text),
                    new SqlParameter("@NgaySinh", txtNgaySinh.Text),
                    new SqlParameter("@Luong", txtLuong.Text),
                    new SqlParameter("@MaNhanVien", int.Parse(txtID.Text))
                };

                int rowsAffected = dbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Cập nhật thành công!");
                    ClearText();
                    LoadNhanVien();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message);
            }
        }

        // Xóa nhân viên
        private void DeleteNhanVien()
        {
            try
            {
                string query = "DELETE FROM NhanVien WHERE MaNhanVien = @MaNhanVien";
                SqlParameter[] parameters = {
                    new SqlParameter("@MaNhanVien", int.Parse(txtID.Text))
                };

                int rowsAffected = dbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa thành công!");
                    ClearText();
                    LoadNhanVien();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên với mã này.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
            }
        }

        private void ClearText()
        {
            txtID.Clear();
            txtName.Clear(); 
            selectLabel.Text = "";
            txtNgaySinh.Clear();
            txtLuong.Clear();
        }
        private void RbnGT_CheckedChanged(object sender, EventArgs e)
        {
            if (RbnGT.Checked)
            {
                selectLabel.Text = "Nam";
            }
            else if (RbnGT2.Checked)
            {
                selectLabel.Text = "Nu";
            }
        }

        private void RbnGT2_CheckedChanged(object sender, EventArgs e)
        {
            if (RbnGT2.Checked)
            {
                selectLabel.Text = "Nu"; 
            }
            else if (RbnGT.Checked)
            {
                selectLabel.Text = "Nam"; 
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            LoadNhanVien();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            InsertNhanVien();
            LoadNhanVien();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            UpdateNhanVien();
            LoadNhanVien();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DeleteNhanVien();
                LoadNhanVien(); 
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow row = dgvNhanVien.Rows[e.RowIndex];

            
                txtID.Text = row.Cells["MaNhanVien"].Value.ToString();
                txtName.Text = row.Cells["TenNhanVien"].Value.ToString();
                selectLabel.Text = row.Cells["GioiTinh"].Value.ToString();
                txtNgaySinh.Text = row.Cells["NgaySinh"].Value.ToString();
                txtLuong.Text = row.Cells["Luong"].Value.ToString();

                // Chọn RadioButton tương ứng
                if (selectLabel.Text == "Nam")
                {
                    RbnGT.Checked = true;
                    RbnGT2.Checked = false;
                }
                else
                {
                    RbnGT.Checked = false;
                    RbnGT2.Checked = true;
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string tenNhanVien = txtSearch.Text.Trim(); // Loại bỏ khoảng trắng thừa
            if (!string.IsNullOrEmpty(tenNhanVien))
            {
                GetNhanVienByName(tenNhanVien);
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tên nhân viên để tìm kiếm.");
            }
        }

    }
}
