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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class editeSectionPrice : Form
    {
        string connectionString;
        public editeSectionPrice()
        {
            InitializeComponent();
            bunifuCheckbox1.Checked = false;
            connectionString = DatabaseConnections.GymDB;
        }
        private void UpdateRedPrice( int newPrice)
        {
            string query= "update Subscriptions set TotalSubscriptionAmount=@NewPrice where IsRed=1";
            using (SqlConnection connection=new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@NewPrice",newPrice);
                    command.ExecuteNonQuery();
                }

            }
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if(bunifuCheckbox1.Checked)
                {
                   DialogResult result2= MessageBox.Show("سيتم تعديل سعر الاشتراك عند جميع مشتركي الهلال الاحمر, هل تريد المتابعى","",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                    if(result2==DialogResult.No)
                    {
                        return;
                    }
                    int sectionPrice = 0;
                    int.TryParse(textBox1.Text, out sectionPrice);
                    if(sectionPrice==0)
                    {
                        MessageBox.Show("سعر اشتراك غير صالح");
                        return;
                    }
                    UpdateRedPrice(sectionPrice);
                    MessageBox.Show("تم تعديل سعر الاشتراك عند مشتركي الهلال الأحمر بنجاح");
                    return;
                }

                if (!(comboBox1.SelectedIndex >= 0 && textBox1.Text.Length > 0))
                {
                    MessageBox.Show("اختر قسماً وادخل قيمة الاشتراك الجديد", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string section = comboBox1.SelectedItem.ToString();
                string message = $"سيتم تعديل قيمة الاشتراك عند جميع المشتركين المسجلين في {comboBox1.SelectedItem} ";
                if (comboBox2.SelectedIndex >= 0)
                {
                    message += $"والذين نوع تدريبهم {comboBox2.SelectedItem}";
                }
                message += ".";
                message += " هل تريد المتابعة؟";
                DialogResult result = MessageBox.Show(message, "تنبيه", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                int newFee = 0;
                int.TryParse(textBox1.Text, out newFee);

                if (newFee < 1000)
                {
                    MessageBox.Show("سعر الاشتراك الجديد غير صالح", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    int sectionID = GetSectionId(section);
                    UpdateFee(sectionID, newFee);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private int GetSectionId(string section)
        {
            string query = "SELECT SectionId FROM Sections WHERE SectionName = @SectionName";
            

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SectionName", section);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (int)result;
                    }
                }
            }

            return 0; // Return a default value if the section ID is not found
        }

        private void UpdateFee(int sectionID, int newFee)
        {
            string query1 = "UPDATE Subscriptions SET TotalSubscriptionAmount = @TotalSubscriptionAmount WHERE SectionID = @SectionID and SubscriptionTypeID=1 and IsRed=0";
            if(comboBox2.SelectedIndex==0)
            {
                query1 += " and TrainingTypeID=1";

            }  
            else if(comboBox2.SelectedIndex==1)
            {
                query1 += " and TrainingTypeID=2";
            }
            string query2 = "Update Sections set SectionMonthlyPrice=@SectionMonthlyPrice Where SectionID=@SectionID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query1, connection))
                    {
                        command.Parameters.AddWithValue("@TotalSubscriptionAmount", newFee);
                        command.Parameters.AddWithValue("@SectionID", sectionID);
                        int rowsAffected = command.ExecuteNonQuery();
                        MessageBox.Show($"تم تحديث {rowsAffected} من السجلات");
                    }

                    using (SqlCommand command2= new SqlCommand(query2,connection))
                    {
                        command2.Parameters.AddWithValue("@SectionID", sectionID);
                        command2.Parameters.AddWithValue("@SectionMonthlyPrice", newFee);
                        if(comboBox2.SelectedIndex==0)
                        command2.ExecuteNonQuery();
                    }


                }
                catch (Exception ex)
                {
                   MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }



        private void fillCombobox()
        {

            // Populate comboBox3 with section names and enable autocomplete
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sectionQuery = "SELECT DISTINCT SectionName FROM Sections";
                using (SqlCommand command = new SqlCommand(sectionQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                       // AutoCompleteStringCollection autoCompleteCollection3 = new AutoCompleteStringCollection();

                        while (reader.Read())
                        {
                            string sectionName = reader["SectionName"].ToString();
                            comboBox1.Items.Add(sectionName);
                          //  autoCompleteCollection3.Add(sectionName);
                        }

                      
                    }
                }
            }
        }
        private void editeSectionPrice_Load(object sender, EventArgs e)
        {
            fillCombobox();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }
    }
}
