using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class searchForPayments : Form
    {
        const string connectionString = "data source=DESKTOP-T1STQLB;initial catalog=Database1;trusted_connection=true";

        public searchForPayments(List<string> clientNames)
        {
            InitializeComponent();
            comboBox6.Items.AddRange(clientNames.ToArray());

        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // Safely retrieve selected values from ComboBoxes and TextBoxes
                string clientName = comboBox6.SelectedItem?.ToString();
                string coachName = comboBox3.SelectedItem?.ToString();
                string sectionName = comboBox4.SelectedItem?.ToString();
                string trainingType = comboBox1.SelectedItem?.ToString();
                string paymentYear = dateTimePicker1.Value.Year.ToString();
                string paymentMonth = dateTimePicker1.Value.Month.ToString();


                // Retrieve IDs based on selected names, if applicable
                int clientID = string.IsNullOrEmpty(clientName) ? -1 : GetClientID(clientName);
                int sectionID = string.IsNullOrEmpty(sectionName) ? -1 : GetSectionID(sectionName);
                int coachID = string.IsNullOrEmpty(coachName) ? -1 : GetCoachID(coachName);
                int trainingTypeID = string.IsNullOrEmpty(trainingType) ? -1 : GetTrainingTypeID(trainingType);

                PopulatePaymentsDataGridView(clientID, sectionID, coachID, paymentYear, paymentMonth, trainingTypeID);


                FindSumOfPayments(clientID, sectionID, coachID, paymentYear, paymentMonth, trainingTypeID);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }


        }

        public void FindSumOfPayments(int clientID, int sectionID, int coachID, string paymentYear, string paymentMonth, int trainingTypeID)
        {
            string query = @"SELECT SUM(PaymentAmount) FROM PaymentsTable WHERE 1 = 1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (clientID != -1)
            {
                query += " AND ClientID = @ClientID";
                parameters.Add(new SqlParameter("@ClientID", clientID));
            }
            if (sectionID != -1)
            {
                query += " AND SectionID = @SectionID";
                parameters.Add(new SqlParameter("@SectionID", sectionID));
            }
            if (coachID != -1)
            {
                query += " AND CoachID = @CoachID";
                parameters.Add(new SqlParameter("@CoachID", coachID));
            }
            if (!string.IsNullOrEmpty(paymentYear))
            {
                query += " AND YEAR(PaymentDate) = @PaymentYear";
                parameters.Add(new SqlParameter("@PaymentYear", paymentYear));
            }
            if (!string.IsNullOrEmpty(paymentMonth))
            {
                query += " AND MONTH(PaymentDate) = @PaymentMonth";
                parameters.Add(new SqlParameter("@PaymentMonth", paymentMonth));
            }
            if (trainingTypeID != -1)
            {
                query += " AND TrainingTypeID=@TrainingTypeID";
                parameters.Add(new SqlParameter("@TrainingTypeID", trainingTypeID));
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddRange(parameters.ToArray());
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        // Display the total payment amount in the TextBox
                        textBox3.RightToLeft = RightToLeft.No;
                        textBox3.Text = result.ToString();
                    }
                    else
                    {
                        // No payments found, clear the TextBox or set a default message
                        textBox3.RightToLeft = RightToLeft.Yes;
                        textBox3.Text = "لم يتم العثور على دفعات";
                    }
                }
            }
            query = @"SELECT SUM(PaymentAmount) FROM PaymentsTable WHERE 1 = 1";
        }
        //Load the data
        private void searchForPayments_Load(object sender, EventArgs e)
        {
          //  FillComboBoxClients(comboBox6);
            FillComboBoxSections(comboBox4);
            FillComboBoxCoaches(comboBox3);
            FillComboBoxTrainingTypes(comboBox1);

        }

       



        //fill client names
        private void FillComboBox(ComboBox comboBox, string query, string columnName)
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


        public void FillComboBoxClients(ComboBox comboBox)
        {
            string query = "SELECT ClientName FROM Clients";
            FillComboBox(comboBox, query, "ClientName");
        }

        public void FillComboBoxSections(ComboBox comboBox)
        {
            string query = "SELECT SectionName FROM Sections";
            FillComboBox(comboBox, query, "SectionName");
        }

        public void FillComboBoxCoaches(ComboBox comboBox)
        {
            string query = "SELECT CoachName FROM Coaches";
            FillComboBox(comboBox, query, "CoachName");
        }


        public void FillComboBoxTrainingTypes(ComboBox comboBox)
        {
            string query = "SELECT TrainingName FROM TrainingTypes";
            FillComboBox(comboBox, query, "TrainingName");
        }


        //get ids
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

        private int GetTrainingTypeID(string trainingType)
        {
            string query = "SELECT Id FROM TrainingTypes WHERE TrainingName = @TrainingName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TrainingName", trainingType);

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
        private void PopulatePaymentsDataGridView(int clientID, int sectionID, int coachID, string paymentYear, string paymentMonth, int trainingTypeID)
        {
            string query = @"
        SELECT 
            Clients.ClientName AS 'اسم المشترك', 
            Sections.SectionName AS 'قسم التسجيل', 
            SubscriptionType.SubscriptionName AS 'نوع الاشتراك',
            Coaches.CoachName AS 'اسم المدرب', 
            TrainingTypes.TrainingName as 'نوع التدريب',
            PaymentsTable.PaymentAmount AS 'قيمة الدفعة', 
            FORMAT(PaymentsTable.PaymentDate, 'dd/MM/yyyy') AS 'تاريخ الدفعة',
        PaymentsTable.RemainingPaymentAmount AS 'المبلغ المتبقي'
        FROM PaymentsTable
      left  JOIN Clients ON PaymentsTable.ClientID = Clients.ClientId
      left  JOIN Sections ON PaymentsTable.SectionID = Sections.SectionID
      left  JOIN SubscriptionType ON PaymentsTable.SubscriptionTypeID = SubscriptionType.SubscriptionID
      left  JOIN Coaches ON PaymentsTable.CoachID = Coaches.CoachID
      left  join TrainingTypes on PaymentsTable.TrainingTypeID=TrainingTypes.Id
        WHERE 1 = 1";

            string query2 = query;

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (clientID != -1)
            {
                query += " AND Clients.ClientID = @ClientID";
                parameters.Add(new SqlParameter("@ClientID", clientID));
            }
            if (sectionID != -1)
            {
                query += " AND Sections.SectionID = @SectionID";
                parameters.Add(new SqlParameter("@SectionID", sectionID));
            }
            if (coachID != -1)
            {
                query += " AND Coaches.CoachID = @CoachID";
                parameters.Add(new SqlParameter("@CoachID", coachID));
            }
            if (!string.IsNullOrEmpty(paymentYear))
            {
                query += " AND YEAR(PaymentsTable.PaymentDate) = @PaymentYear";
                parameters.Add(new SqlParameter("@PaymentYear", paymentYear));
            }
            if (!string.IsNullOrEmpty(paymentMonth))
            {
                query += " AND MONTH(PaymentsTable.PaymentDate) = @PaymentMonth";
                parameters.Add(new SqlParameter("@PaymentMonth", paymentMonth));
            }
            if (trainingTypeID!= -1)
            {
                query += " AND TrainingTypeID=@TrainingTypeID";
                parameters.Add(new SqlParameter("@TrainingTypeID", trainingTypeID));
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddRange(parameters.ToArray());

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
            }
            query = query2;
        }







    }
}