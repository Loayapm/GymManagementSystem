using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class deleteSection : Form
    {
        string connectionString;
        public deleteSection()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            bunifuImageButton2.Enabled = false;
            this.Close();
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialog = MessageBox.Show("هل تريد تاكيد حذف القسم؟ سيتم حذف القسم عند جميع المشتركين", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.No)
                {
                    return;
                }
                string sectionName = comboBox1.SelectedIndex >= 0 ? comboBox1.SelectedItem.ToString() : "";

                int sectionID = GetSectionIdByName(sectionName);


                MakeTheSectionIdNull(sectionID);

                MessageBox.Show("تم حذف القسم بنجاح");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }



        }

        private void MakeTheSectionIdNull(int sectionId)
        {
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();

                string query1 = "UPDATE Subscriptions SET SectionID = NULL WHERE SectionID = @SectionID";
                string query2 = "UPDATE PaymentsTable SET SectionID = NULL WHERE SectionID = @SectionID";
                string query3 = "update PausedSubscriptions set SectionID = null where SectionID=@SectionID";
                string query4 = "delete from Sections where SectionID=@SectionID";

                using (SqlCommand command1= new SqlCommand(query1,connection))
                {
                    command1.Parameters.AddWithValue("@SectionID", sectionId);
                    command1.ExecuteNonQuery();
                }

                using (SqlCommand command2 = new SqlCommand(query2, connection))
                {
                    command2.Parameters.AddWithValue("@SectionID", sectionId);
                    command2.ExecuteNonQuery();
                }
                using (SqlCommand command3 = new SqlCommand(query3, connection))
                {
                    command3.Parameters.AddWithValue("@SectionID", sectionId);
                    command3.ExecuteNonQuery();
                }
                using (SqlCommand command4 = new SqlCommand(query4, connection))
                {
                    command4.Parameters.AddWithValue("@SectionID", sectionId);
                    command4.ExecuteNonQuery();
                }



            }
        }

        public void SetSectionIdToNull(int deletedSectionId)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Example for Table1
                using (SqlCommand command = new SqlCommand("UPDATE PaymentsTable SET SectionId = NULL WHERE SectionId = @DeletedSectionId", connection))
                {
                    command.Parameters.AddWithValue("@DeletedSectionId", deletedSectionId);
                    command.ExecuteNonQuery();
                }

                // Repeat for each table that has a SectionId foreign key
                // Example for Table2
                using (SqlCommand command = new SqlCommand("UPDATE Subscriptions SET SectionId = NULL WHERE SectionId = @DeletedSectionId", connection))
                {
                    command.Parameters.AddWithValue("@DeletedSectionId", deletedSectionId);
                    command.ExecuteNonQuery();
                }

                // Add more commands for other tables as needed

                using (SqlCommand command = new SqlCommand("DELETE FROM Sections WHERE SectionId = @DeletedSectionId", connection))
                {
                    command.Parameters.AddWithValue("@DeletedSectionId", deletedSectionId);
                    command.ExecuteNonQuery();
                }
            }
        }
        public int GetSectionIdByName(string sectionName)
        {
            string query = "SELECT SectionId FROM Sections WHERE SectionName = @SectionName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SectionName", sectionName);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return 0; // Return null if no section with the given name is found
        }
        private void  removeSection(string name)
        {
            string query = "delete from Sections where SectionName=@SectionName";
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@SectionName", name);
                    command.ExecuteNonQuery();
                }

            }
        }

        private void deleteSection_Load(object sender, EventArgs e)
        {
            // Populate the section combobox
            string sectionQuery = "SELECT DISTINCT SectionName FROM Sections"; // Replace YourTableName with the actual table name
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sectionQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string section = reader["SectionName"].ToString();
                            comboBox1.Items.Add(section);

                        }
                    }
                }
            }
        }
    }
}
