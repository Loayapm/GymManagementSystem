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
    public partial class deleteSpending : Form
    {
        string connectionString;
        public deleteSpending()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            bunifuImageButton2.Enabled= false;
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int spendingid = 0;
                int.TryParse(textBox1.Text, out spendingid);
                if (spendingid == 0)
                {
                    MessageBox.Show("رقم المنتج غير صالح", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
                DialogResult result = MessageBox.Show("هل تريد تأكيد حذف المنتج من قائمة المشتريات؟", "تأكيد", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    DeleteSpending(spendingid);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }


        private void DeleteSpending(int spendingid)
        {
            string query = "DELETE FROM Spendings where Id=@SpendingID";
            using (SqlConnection connection=new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command=new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@SpendingID", spendingid);
                    int rowsAffected=command.ExecuteNonQuery();
                    if(rowsAffected==0)
                    {
                        MessageBox.Show("لم يتم العثور على منتج بالرقم المُدخل","",MessageBoxButtons.OK,MessageBoxIcon.Question);
                    }
                    else
                        MessageBox.Show("تم حذف المنتج بنجاح");
                }
            }
        }

        private void deleteSpending_Load(object sender, EventArgs e)
        {

        }
    }
}
