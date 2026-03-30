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
using WindowsFormsApp1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class searchForClient : Form
    {
      string connectionString;
        public searchForClient(List <string> clientNames)
        {
            InitializeComponent();
            comboBox1.Items.AddRange(clientNames.ToArray());
            // dateTimePicker1.Value = new DateTime(2000, 1, 1);
            connectionString = DatabaseConnections.GymDB;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Enabled= false;
            this.Close();
        }

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            try
            {
                string clientName = comboBox1.SelectedItem?.ToString();
                string section = comboBox2.SelectedItem?.ToString();
                string coach = comboBox3.SelectedItem?.ToString();
                string sType = comboBox4.SelectedItem?.ToString();
                string activeState = comboBox5.SelectedItem?.ToString();
                string trainingType = comboBox6.SelectedItem?.ToString();
                //  DateTime PauseDate = dateTimePicker1.Value;
                int year = textBox1.Text != string.Empty ? int.Parse(textBox1.Text) : 0;
                int month = textBox2.Text != string.Empty ? int.Parse(textBox2.Text) : 0;
                int day = textBox3.Text != string.Empty ? int.Parse(textBox3.Text) : 0;


                bool aState = activeState == "فعال";
                if (activeState == "غير فعال")
                {
                    aState = false;
                }



                // Retrieve IDs based on selected names, if applicable
                int clientID = string.IsNullOrEmpty(clientName) ? -1 : GetClientID(clientName);
                int sectionID = string.IsNullOrEmpty(section) ? -1 : GetSectionID(section);
                int coachID = string.IsNullOrEmpty(coach) ? -1 : GetCoachID(coach);
                int sTypeID = string.IsNullOrEmpty(sType) ? -1 : GetSTypeID(sType);
                int trainingTypeID = string.IsNullOrEmpty(trainingType) ? -1 : GetTrainingTypeID(trainingType);


                filterSubscriptions(clientID, sectionID, coachID, sTypeID, aState, trainingTypeID, year, month, day);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }







        }

        private void FillTrainingName()
        {
            string trainingTypeQuery = "Select TrainingName from TrainingTypes";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(trainingTypeQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string trainingName = reader["TrainingName"].ToString();
                            comboBox6.Items.Add(trainingName);
                            

                        }
                    }
                }
            }
        }

        private void filterSubscriptions(int clientID, int sectionID, int coachID, int sTypeID, bool aState,int trainingTypeID, int year,int month, int day)
        {

            // Start with a base query
            string query = @"
        SELECT c.ClientName as 'اسم المشترك',  
               se.SectionName as 'القسم',  
               co.CoachName as 'اسم المدرب', 
               tr.TrainingName as 'نوع التدريب',
               st.SubscriptionName as 'نوع الاشتراك', 
               s.TotalSubscriptionAmount as 'قيمة الاشتراك', 
               FORMAT(s.SubscriptionDate, 'dd/MM/yyyy') AS 'تاريخ الاشتراك', 
               FORMAT(s.SubscriptionEndDate, 'dd/MM/yyyy') AS 'تاربخ انتهاء الاشتراك',    
               s.ActiveState as 'حالة الاشتراك' 
        FROM Subscriptions s 
        left  JOIN Clients c ON s.ClientID = c.ClientID 
        left  JOIN Coaches co ON s.CoachID = co.CoachID 
        left  JOIN Sections se ON s.SectionID = se.SectionID 
        left  JOIN SubscriptionType st ON s.SubscriptionTypeID = st.SubscriptionID
        left  join TrainingTypes tr on s.TrainingTypeID=tr.Id
        WHERE 1 = 1"; // This ensures the query is always valid

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (clientID != -1)
            {
                query += " AND s.ClientID = @ClientID";
                parameters.Add(new SqlParameter("@ClientID", clientID));
            }
            if (sectionID != -1)
            {
                query += " AND s.SectionID = @SectionID";
                parameters.Add(new SqlParameter("@SectionID", sectionID));
            }
            if (coachID != -1)
            {
                query += " AND s.CoachID = @CoachID";
                parameters.Add(new SqlParameter("@CoachID", coachID));
            }
            if(trainingTypeID!=-1)
            {
                query += " and s.TrainingTypeID=@TrainingTypeID";
                parameters.Add(new SqlParameter("@TrainingTypeID", trainingTypeID));
            }
            if (sTypeID != -1)
            {
                query += " AND s.SubscriptionTypeID = @sTypeID";
                parameters.Add(new SqlParameter("@sTypeID", sTypeID));
            }
            if (comboBox5.SelectedIndex==0)
            {
                query += " AND s.ActiveState = 1";
              //  parameters.Add(new SqlParameter("@aState", 1)); // Assuming ActiveState is stored as 1 (true)
            }
             if (comboBox5.SelectedIndex==1)
            {
                query += " AND s.ActiveState = 0";
              //  parameters.Add(new SqlParameter("@aState", 0)); // Assuming ActiveState is stored as 0 (false)
            }

             if(!(comboBox5.SelectedIndex>=0))
            {
                query += " AND (s.ActiveState=1 or s.ActiveState=0)";
            }
             if(textBox1.Text!=string.Empty)
            {
                query += " and Year(SubscriptionDate)=@SYear";
                parameters.Add(new SqlParameter("@SYear", year));

            }

            if (textBox2.Text != string.Empty)
            {
                query += " and Month(SubscriptionDate)=@SMonth";
                parameters.Add(new SqlParameter("@SMonth", month));

            }

            if (textBox3.Text != string.Empty)
            {
                query += " and Day(SubscriptionDate)=@SDay";
                parameters.Add(new SqlParameter("@SDay", day));

            }
             




            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    connection.Open();

                    // Load the filtered data into a DataTable
                    DataTable filteredData = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(filteredData);
                    }

                    // Bind the DataTable to the DataGridView
                    dataGridView1.DataSource = filteredData;
                }
            }
        }


        private void FillComboBox(System.Windows.Forms.ComboBox comboBox, string query, string columnName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Clear the ComboBox and add the items from the DataTable
                        comboBox.Items.Clear();
                        foreach (DataRow row in dataTable.Rows)
                        {
                            comboBox.Items.Add(row[columnName]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error filling ComboBox: " + ex.Message);
            }
        }


        public void FillComboBoxClients(System.Windows.Forms.ComboBox comboBox)
        {
            string query = "SELECT ClientName FROM Clients";
            FillComboBox(comboBox, query, "ClientName");
        }

        public void FillComboBoxSections(System.Windows.Forms.ComboBox comboBox)
        {
            string query = "SELECT SectionName FROM Sections";
            FillComboBox(comboBox, query, "SectionName");
        }

        public void FillComboBoxCoaches(System.Windows.Forms.ComboBox comboBox)
        {
            string query = "SELECT CoachName FROM Coaches";
            FillComboBox(comboBox, query, "CoachName");
        }

        private int GetClientID(string clientName)
        {
            string query = "SELECT clientID FROM Clients WHERE clientName = @ClientName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClientName", clientName);

                connection.Open();
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private int GetSectionID(string sectionName)
        {
            string query = "SELECT SectionID FROM Sections WHERE SectionName = @SectionName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SectionName", sectionName);

                connection.Open();
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private int GetCoachID(string coachName)
        {
            string query = "SELECT CoachID FROM Coaches WHERE CoachName = @CoachName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CoachName", coachName);

                connection.Open();
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private int GetSTypeID(string sType)
        {
            string query = "SELECT SubscriptionID FROM SubscriptionType WHERE SubscriptionName = @SubscriptionName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SubscriptionName", sType);

                connection.Open();
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private int GetTrainingTypeID(string trainingType)
        {
            string query = "SELECT Id FROM TrainingTypes WHERE TrainingName = @TrainingName";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TrainingName", trainingType);

                    return (int)command.ExecuteScalar();
                }
            }
        }

        private void searchForClient_Load(object sender, EventArgs e)
        {
         //   FillComboBoxClients(comboBox1);
            FillComboBoxSections(comboBox2);
            FillComboBoxCoaches(comboBox3);
            FillTrainingName();

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
