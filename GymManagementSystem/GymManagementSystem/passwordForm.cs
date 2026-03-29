using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Konscious.Security.Cryptography;


namespace WindowsFormsApp1
{
    public partial class passwordForm : Form
    {
        string connectionString;
        public passwordForm()
        {

             connectionString = DatabaseConnections.UsersDB;
            InitializeComponent();



            textBox1.Text = " Username";
            textBox1.ForeColor = Color.FromArgb(192, 64, 0);

            textBox2.Text = " Password";
            textBox2.ForeColor = Color.FromArgb(192, 64, 0);


            textBox3.Text = " Username";
            textBox3.ForeColor = Color.FromArgb(192, 64, 0);

            textBox4.Text = "Old Password";
            textBox4.ForeColor = Color.FromArgb(192, 64, 0);

            textBox5.Text = "New Password";
            textBox5.ForeColor = Color.FromArgb(192, 64, 0);

            textBox6.Text = "Confirm New Password";
            textBox6.ForeColor = Color.FromArgb(192, 64, 0);



        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuImageButton1.Enabled= false;
            this.Close();
        }

        private void bunifuTextbox1_OnTextChange(object sender, EventArgs e)
        {
          
           
        }

        private void bunifuTextbox2_OnTextChange(object sender, EventArgs e)
        {
          
        }

      

        private void bunifuTextbox1_Enter(object sender, EventArgs e)
        {
            bunifuTextbox1.text = "";
        }

        private void bunifuTextbox2_Enter(object sender, EventArgs e)
        {
            bunifuTextbox2.text = "";
            
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
           
            try
            {
                //first get the stored hash for the user from the database, then use PasswordHelper.VerifyPassword to check if the provided password matches the stored hash
                if (textBox1.Text.Length == 0 || textBox2.Text.Length == 0)
                {
                    MessageBox.Show("ادخل اسم المستخدم وكلمة المرور","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string userName = textBox1.Text.Trim();
                string password = textBox2.Text.Trim();

                if (userName == "Username")
                {
                    return;
                }

              

                //username found, now get the stored hash
                string storedHash = GetStoredHash(userName);
                if(storedHash == null)
                {
                    MessageBox.Show("Incorrect login info", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (PasswordHelper.VerifyPassword(password, storedHash))
                {
                    textBox2.Clear();
                    DialogResult = DialogResult.OK; // Set the DialogResult to OK

                    bunifuFlatButton1.Enabled = false;
                    this.Close(); // Close the password form after showing Form1

                } else
                {
                    MessageBox.Show("Incorrect login info", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"General error: {ex.Message}");
            }
        }

        private string GetStoredHash(string userName)
        {
            string storedHash = null;
            string query = "SELECT Password FROM Users WHERE Username=@UserName";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    storedHash = command.ExecuteScalar() as string; // get the stored hash for the user
                }
            }
            return storedHash;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox2.PasswordChar = '*';
        }

        private void passwordForm_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

  

        private void textBox3_Enter(object sender, EventArgs e)
        {
            textBox3.Text = "";
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            textBox4.Text = "";
            textBox4.PasswordChar = '*';
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            textBox5.Text = "";
            textBox5.PasswordChar = '*';
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            textBox6.Text = "";
            textBox6.PasswordChar = '*';
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;

        }


        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input fields
                if (string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) ||
                    string.IsNullOrWhiteSpace(textBox5.Text) || string.IsNullOrWhiteSpace(textBox6.Text))
                {
                    MessageBox.Show("تأكد من ادخال كافة البيانات");
                    return;
                }

                string userName = textBox3.Text.Trim();
                string password = textBox4.Text.Trim(); // Consider validating this against a hashed version
                string newPassword = textBox6.Text.Trim();

               

                if (textBox5.Text != textBox6.Text)
                {
                    MessageBox.Show("كلمة المرور الجديدة وإعادة كتابتها غير متطابقتين. يرجى التأكد من إدخال نفس كلمة المرور في الحقول المتطابقة");
                    return;
                }

                // 1. Get stored hash
                string storedHash = GetStoredHash(userName);

                if (storedHash == null)
                {
                    MessageBox.Show("User not found");
                    return;
                }

                // 2. Verify OLD password
                if (!PasswordHelper.VerifyPassword(password, storedHash))
                {
                    MessageBox.Show("Old password is incorrect");
                    return;
                }

                // 3. Hash NEW password
                string newHashedPassword = PasswordHelper.HashPassword(newPassword);

                // 4. Update DB
                ChangePassword(userName, newHashedPassword);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General error: {ex.Message}");
            }
        }

        private void ChangePassword(string userName, string newHashedPassword)
        {
            string query = "UPDATE Users SET Password = @Password WHERE Username = @Username";  // Note: Username, not UserName

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", userName);   // Be consistent
                command.Parameters.AddWithValue("@Password", newHashedPassword);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    MessageBox.Show("User not found");
                else
                    MessageBox.Show("تم تغيير كلمة المرور بنجاح");
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
            }
        }


        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Execute the button click event or any other action
                bunifuFlatButton1_Click(sender, e);

                // Prevent the Enter key from being processed further
                e.Handled = true;
            }
        }
    }
}
