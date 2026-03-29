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
    public partial class addClientForm : Form
    {
        string connectionString;
        public addClientForm()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("هل تريد تأكيد اضافة المشترك؟", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
                // Get the client's name and phone number
                string clientName = textBox1.Text.Trim(); // Assuming the client's name is entered in textBox1
                string phoneNumber; // Assuming the client's phone number is entered in textBox2
                phoneNumber = textBox2.Text.Trim();
                double pn = 0;
                double.TryParse(textBox2.Text, out pn);
                if (!(pn > 0))
                {
                    MessageBox.Show("رقم هاتف غير صالح", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }




                if (textBox1.Text.Length == 0)
                {
                    MessageBox.Show("لا يمكن ان يكون اسم المشترك قيمة فارغة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                // Check if the client already exists
                string checkClientQuery = "SELECT COUNT(*) FROM Clients WHERE ClientName = @ClientName";

                int existingClientsCount;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(checkClientQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ClientName", clientName);

                        existingClientsCount = Convert.ToInt32(command.ExecuteScalar());
                    }
                }

                if (existingClientsCount > 0)
                {
                    // Client already exists
                    MessageBox.Show("."+$"المشترك {clientName} موجود مسبقا, جرب استخدام الاسم الثلاثي", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int newClientId;
                // Insert the client into the Clients table
                string insertClientQuery = "INSERT INTO Clients (ClientName, PhoneNumber) VALUES (@ClientName, @PhoneNumber); SELECT SCOPE_IDENTITY();";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(insertClientQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ClientName", clientName);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        newClientId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }

                MessageBox.Show("تمت اضافة المشترك.");
            } catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
          
    
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuImageButton1.Enabled = false;
            this.Close();
        }

        private void addClientForm_Load(object sender, EventArgs e)
        {

        }
    }
}
 