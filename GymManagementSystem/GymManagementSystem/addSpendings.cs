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
    public partial class addSpendings : Form
    {
         string connectionString;
        public addSpendings()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("تاكد من ادخال جميع البيانات المطلوبة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int price = 0;
                int.TryParse(textBox2.Text, out price);
                string spendingName = textBox1.Text;
                DateTime spendingDate = dateTimePicker1.Value;
                if (price < 1000)
                {
                    MessageBox.Show("سعر غير صالح", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult result = MessageBox.Show("هل تريد تأكيد الاضافة؟", "", MessageBoxButtons.YesNo);
                if (!(result == DialogResult.Yes))
                {
                    return;
                }

                string addSpendingCommand = "INSERT INTO Spendings (spendingName, spendingAmount,spendingDate) values (@spendingName,@spendingAmount,@spendingDate)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(addSpendingCommand, connection))
                    {
                        command.Parameters.AddWithValue("@spendingName", spendingName);
                        command.Parameters.AddWithValue("@spendingAmount", price);
                        command.Parameters.AddWithValue("@spendingDate", spendingDate);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("تمت الاضافة الى قائمة المشتريات بنجاح");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Enabled= false;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void addSpendings_Load(object sender, EventArgs e)
        {

        }
    }
}
