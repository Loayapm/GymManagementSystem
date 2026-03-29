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
    public partial class addSection : Form
    {
         string connectionString;

        public addSection()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialog = MessageBox.Show("هل تريد تأكيد اضافة القسم؟", "تاكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.No)
                {
                    return;
                }
                if (textBox1.Text == string.Empty)
                {
                    MessageBox.Show("لا يمكن ان يكون الاسم فارغاً", " Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string sname = textBox1.Text.Trim();
                int price = 0;
                int.TryParse(textBox2.Text, out price);
                if (price < 1000)
                {
                    MessageBox.Show("سعر الاشتراك غير صالح", " Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (isDeleted(sname))
                {

                    //the user is trying to add a deleted section, i need to activate the section
                    ActivateSection(sname);
                    return;
                }

                if (subscriptionExists(sname))
                {
                    MessageBox.Show($"القسم{sname} موجود مسبقاً");
                    return;
                }
                else
                    insertIntoSections(sname, price);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }



        }

        private bool subscriptionExists(string SectionName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Sections WHERE SectionName = @SectionName and IsDeleted=0";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SectionName", SectionName);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        private void ActivateSection(string sectionName)
        {
            string query = "alter table Sections Set IsDeleted=0 where SectionName=@SectionName";
            using (SqlConnection connection=new SqlConnection(connectionString))
            {
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@SectionName", sectionName);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void insertIntoSections(string name, int price)
        {
            using (SqlConnection connection=new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "insert into Sections (SectionName,SectionMonthlyPrice) values (@SectionName,@SectionMonthlyPrice)";
                using (SqlCommand command=new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@SectionName", name);
                    command.Parameters.AddWithValue("@SectionMonthlyPrice", price);
                    command.ExecuteNonQuery();

                }
            }
            MessageBox.Show("Section Added Successfully");
        }

        private bool isDeleted(string SectionName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Sections WHERE SectionName = @SectionName and IsDeleted=1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SectionName", SectionName);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }

        private void addSection_Load(object sender, EventArgs e)
        {

        }
    }
}
