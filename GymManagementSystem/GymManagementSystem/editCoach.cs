using System;
using System.Collections;
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
    public partial class editCoach : Form
    {
        string connectionString;
        int CoachID;
        public editCoach()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string coachName = comboBox1.SelectedItem.ToString();
            int coachID = GetCoachID(coachName);
            CoachID = coachID;
            FillCoachName(coachID);
        }

        private void FillCoachName(int coachID)
        {
            try
            {
                // Define the SQL query with a parameter
                string query = "SELECT CoachName, CoachPhoneNumber, Salary FROM Coaches WHERE CoachID=@CoachID";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create a command with the connection, query, and parameters
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CoachID", coachID);

                        // Open the connection if not already open
                        if (!(connection.State == ConnectionState.Open))
                            connection.Open();

                        // Execute the reader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Check if there's at least one row returned
                            {
                                // Assuming textBox1, textBox2, and textBox3 are the names of your TextBox controls
                                textBox1.Text = reader["CoachName"].ToString();
                                textBox2.Text = reader["CoachPhoneNumber"].ToString();
                                textBox3.Text = reader["Salary"].ToString();
                            }
                            else
                            {
                                // Handle the case where no coach was found
                                MessageBox.Show("No coach found with the provided ID.");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., database connection issues)
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }

        private void FillComboBox(ComboBox comboBox, string query) // Adjust the method signature as needed
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {


                    // Execute the reader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // Iterate through the rows returned
                        {
                            // Add each CoachName to the ComboBox
                            comboBox.Items.Add(reader["CoachName"].ToString());
                        }
                    }
                }

            }
        }


        private int GetCoachID(string coachName)
        {
            string query = "Select CoachID from Coaches where CoachName=@CoachName";
            int coachID = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachName", coachName);
                    coachID = (int)command.ExecuteScalar();
                }
            }
            return coachID;

        }

        private void editCoach_Load(object sender, EventArgs e)
        {
            FillComboBox(comboBox1, "Select CoachName from Coaches");
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("هل تريد تأكيد حفظ التغييرات", "تأكيد الحفظ", MessageBoxButtons.YesNo);
                if (result == DialogResult.No) { return; }

                string name = textBox1.Text;
                string phoneNumber = textBox2.Text;
                int salary = 0;
                int.TryParse(textBox3.Text, out salary);
                AlterCoachRecord(name, phoneNumber, salary);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void AlterCoachRecord(string name, string phoneNumber, int salary)
        {
            try
            {
                // Define the SQL query with parameters
                string query = "UPDATE Coaches SET CoachName=@CoachName, CoachPhoneNumber=@CoachPhoneNumber, Salary=@Salary WHERE CoachID=@CoachID";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Create a command with the connection, query, and parameters
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CoachName", name);
                        command.Parameters.AddWithValue("@CoachPhoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@Salary", salary);
                        command.Parameters.AddWithValue("@CoachID", CoachID);

    

                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("تم تحديث بيانات المدرب بنجاح");
                        }
                        else
                        {
                            MessageBox.Show("No coach found with the provided ID.");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., database connection issues)
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }
    }
}
