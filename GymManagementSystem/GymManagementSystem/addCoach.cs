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

namespace WindowsFormsApp1
{
    public partial class addCoach : Form
    {
        string connectionString;
        public addCoach()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        //fixed
        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("هل تريد تأكيد اضافة المدرب؟", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }

                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("لا يمكن ان يكون الاسم فارغا", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string name = textBox1.Text.Trim();
                int phoneNumber = 0;
                int.TryParse(textBox3.Text, out phoneNumber);
                int salary = 0;
                int.TryParse(textBox2.Text, out salary);

                if (CoachExists(name))
                {
                    MessageBox.Show("المدرب موجود مسبقا", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Construct the SQL query
                string query = "INSERT INTO Coaches (CoachName, CoachPhoneNumber, Salary) VALUES (@Name, @PhoneNumber, @Salary)";

                // Create a SqlCommand object with the query and connection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the query to prevent SQL injection
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@Salary", salary);

                        // Open the database connection
                        connection.Open();

                        // Execute the query
                        command.ExecuteNonQuery();

                        connection.Close();
                    }
                }

                // Display a success message or perform any other desired actions
                MessageBox.Show("تمت إضافة المدرب بنجاح!");
            } catch (Exception ex)
            {
                MessageBox.Show("Error: "+ex.Message);
            }
           
        }

        private bool CoachExists(string coachName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Coaches WHERE CoachName = @CoachName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachName", coachName);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuImageButton1.Enabled = false;
            this.Close();
        }

        private void addCoach_Load(object sender, EventArgs e)
        {

        }
    }
}
