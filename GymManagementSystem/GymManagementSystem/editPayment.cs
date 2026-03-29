using OfficeOpenXml.FormulaParsing.Excel.Functions;
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
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WindowsFormsApp1.deletePayment;

namespace WindowsFormsApp1
{
    public partial class editPayment : Form
    {
        string connectionString;
        bool foundOne;
        Payment GPayment;
        public editPayment(List<string> clientNames)
        {
            InitializeComponent();
            comboBox1.Items.AddRange(clientNames.ToArray());
            connectionString = DatabaseConnections.GymDB;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Length==0)
            {
                textBox2.Text = "";
                textBox3.Text = "";
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
                comboBox3.SelectedIndex = -1;
                comboBox4.SelectedIndex = -1;
                foundOne = false;
                return;
            }
            int paymentID = 0;
            int.TryParse(textBox1.Text,out paymentID);
            if (paymentID == 0 && textBox1.Text.Length>0)
            {
                MessageBox.Show("رقم سجل غير صالح");
                foundOne = false;
                return;
            }
            Payment payment= GetPaymentInfo(paymentID);
            if (payment == null)
            {
                foundOne = false;
                return;
            }
            GPayment = payment;
            //now I need to turn the ids into words and fill the comboboxes with these words
            string clientName;
            string section;
            string coach;
            string trainingType;
            DateTime paymentDate;
            int paymentAmount;
            int rpa;


            clientName = GetInfoUsingID("Select ClientName from Clients where ClientID=@ClientID", "@ClientID",payment.clientID);
            FillPaymentData(comboBox1, clientName);


            if(payment.sectionID!=null)
            {
                section = GetInfoUsingID("Select SectionName from Sections where SectionID=@SectionID", "@SectionID", payment.sectionID);
                FillPaymentData(comboBox2, section);
            }
            if (payment.coachID != null)
            {
                coach = GetInfoUsingID("Select CoachName from Coaches where CoachID=@CoachID", "@CoachID", payment.coachID);
                FillPaymentData(comboBox3, coach);
            }

            if(payment.trainingTypeID==1)
            {
                trainingType = "عام";
            } else
            {
                trainingType = "برايفت";
            }

            FillPaymentData(comboBox4, trainingType);


            paymentDate = payment.paymentDate;
            paymentAmount = payment.paymentValue;
            rpa = payment.remainingPamentAmount;

            dateTimePicker1.Value= paymentDate;
            textBox2.Text = paymentAmount.ToString();
            textBox3.Text=rpa.ToString();


        }


        private void PopulateComboBoxWithQuery(System.Windows.Forms.ComboBox comboBox, string query)
        {
          

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Assuming the first column contains the names
                                string name = reader.GetString(0); // Adjust the index if needed
                                comboBox.Items.Add(name);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                    // Handle any errors that occur during the database operation
                }
            }
        }

        private void FillPaymentData(System.Windows.Forms.ComboBox comboBox, string valueToSelect)
        {
            comboBox.SelectedIndex = -1;
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i].ToString() == valueToSelect)
                {
                    comboBox.SelectedIndex = i;
                    break; // Exit the loop once the matching item is found and selected
                }
            }
        }

        private string GetInfoUsingID(string query, string parameterName ,int? id)
        {
            string result = "";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(parameterName, id); // Assuming @ClientID is the parameter name in your query

                        result = command.ExecuteScalar() as string; // ExecuteScalar is used for queries that return a single value
                    }
                }
                catch (Exception ex)
                {
                   MessageBox.Show($"An error occurred: {ex.Message}");
                    result = ""; // Return empty string in case of an error
                }
            }

            return result;
        }


        private class Payment
        {
            public int paymentID { get; set; }
            public int clientID { get; set; } // Assuming clientID needs to be part of the class
            public int paymentValue { get; set; }
            public int? sectionID { get; set; } // Using nullable int to handle null values
            public int? coachID { get; set; } // Using nullable int to handle null values
            public int trainingTypeID { get; set; }
            public DateTime paymentDate { get; set; }
            public int remainingPamentAmount { get; set; }

            // Constructor
            public Payment(int paymentID, int clientID, int paymentValue, int? sectionID, int? coachID, int trainingTypeID, DateTime paymentDate, int remainingPamentAmount)
            {
                this.paymentID = paymentID;
                this.clientID = clientID;
                this.paymentValue = paymentValue;
                this.sectionID = sectionID;
                this.coachID = coachID;
                this.trainingTypeID = trainingTypeID;
                this.paymentDate = paymentDate;
                this.remainingPamentAmount = remainingPamentAmount;
            }
        }

        private Payment GetPaymentInfo(int paymentID)
        {
       
            Payment payment = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT ClientID, PaymentAmount, SectionID, CoachID, TrainingTypeID, PaymentDate, RemainingPaymentAmount FROM PaymentsTable WHERE Id=@PaymentID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaymentID", paymentID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if there are any rows returned by the query
                            if (!reader.HasRows)
                            {
                                MessageBox.Show("لا يوجد دفعة مطابقة لرقم السجل");
                                foundOne = false;
                                return null; // Return null since no payment info could be retrieved
                            }

                            if (reader.Read())
                            {
                                foundOne = true;
                                int clientID = reader.GetInt32(0); // Correctly obtaining clientID from the query
                                payment = new Payment(
                                    paymentID,
                                    clientID, // Now correctly passed to the constructor
                                    reader.GetInt32(1), // PaymentAmount
                                    reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2), // SectionID
                                    reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3), // CoachID
                                    reader.GetInt32(4), // TrainingTypeID
                                    reader.GetDateTime(5),// PaymentDate
                                    reader.GetInt32(6)
                                    ) ; 
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during the database operation
                   MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }

            return payment;
        }

        private void editPayment_Load(object sender, EventArgs e)
        {
            PopulateComboBoxWithQuery(comboBox2, "select SectionName From Sections");
            PopulateComboBoxWithQuery(comboBox3, "select CoachName From Coaches");

        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {

            if(!foundOne)
            {
                MessageBox.Show("لم يتم العثور على سجل الدفعة بعد, تأكد من رقم سجل الدفعة");
                return;
            }
            int? sectionID=null;
            int? coachID=null;

           DialogResult result= MessageBox.Show("هل تريد حفظ التغييرات؟","",MessageBoxButtons.YesNo);
            if(result==DialogResult.No)
            {
                return;
            }


            int clientID = GetIdUsingText("select ClientID from Clients where ClientName=@ClientName", "@ClientName", comboBox1.SelectedItem.ToString());

            if(comboBox2.SelectedIndex>=0)
            {
                 sectionID = GetIdUsingText("select SectionID from Sections where SectionName=@SectionName", "@SectionName", comboBox2?.SelectedItem.ToString());

            }
            if (comboBox3.SelectedIndex >= 0)
            {
                coachID = GetIdUsingText("select CoachID from Coaches where CoachName=@CoachName", "@CoachName", comboBox3?.SelectedItem.ToString());
            }

            int trainingTypeID = (comboBox4.SelectedIndex)+1;

            int paymentamount = 0;
            int.TryParse(textBox2.Text,out paymentamount);
            if(paymentamount<1000)
            {
                MessageBox.Show("قيمة الدفعة غير صالحة");
                return;
            }


            int rpa = 0;
            int.TryParse(textBox3.Text, out rpa);


            DateTime paymentDate = dateTimePicker1.Value;


            AlterRecord(int.Parse(textBox1.Text),clientID,sectionID,coachID,trainingTypeID,paymentamount,rpa,paymentDate);

            //now i need to get the correct sed if the rpa is 0 and the new rpa is not 0

            if(GPayment.remainingPamentAmount==0 && rpa!=0)
            {
                //reset sed
                ResetSED(clientID, sectionID, coachID, trainingTypeID);
            } 


        }

        private void ResetSED(int clientID, int? sectionID, int? coachID, int trainingTypeID)
        {

            string query = @"
        UPDATE Subscriptions
        SET SubscriptionEndDate = DATEADD(day, -31, SubscriptionEndDate)
        WHERE ClientID = @ClientID AND SectionID = @SectionID AND CoachID = @CoachID AND TrainingTypeID = @TrainingTypeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClientID", clientID);

                        // For nullable types, check if they have a value before adding them as parameters
                        if (sectionID.HasValue)
                            command.Parameters.AddWithValue("@SectionID", sectionID.Value);
                        else
                            command.Parameters.AddWithValue("@SectionID", DBNull.Value);

                        if (coachID.HasValue)
                            command.Parameters.AddWithValue("@CoachID", coachID.Value);
                        else
                            command.Parameters.AddWithValue("@CoachID", DBNull.Value);

                        command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                           MessageBox.Show("تمت ازالة 30 يوم من الاشتراك بنجاح");
                        else
                            MessageBox.Show("لا يوجد اشتراك مطابق للمعلومات المدخلة");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    // Handle any errors that occur during the database operation
                }
            }
        }



        private void AlterRecord(int paymentID, int clientID, int? sectionID, int? coachID, int trainingTypeID, int paymentAmount, int remainingPaymentAmount, DateTime paymentDate)
    {
     
        string query = @"
        UPDATE PaymentsTable
        SET ClientID = @ClientID,
            SectionID = @SectionID,
            CoachID = @CoachID,
            TrainingTypeID = @TrainingTypeID,
            PaymentAmount = @PaymentAmount,
            RemainingPaymentAmount = @RemainingPaymentAmount,
            PaymentDate = @PaymentDate
        WHERE Id = @PaymentID";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PaymentID", paymentID);
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    // For nullable types, check if they have a value before adding them as parameters
                    if (sectionID.HasValue)
                        command.Parameters.AddWithValue("@SectionID", sectionID.Value);
                    else
                        command.Parameters.AddWithValue("@SectionID", DBNull.Value);

                    if (coachID.HasValue)
                        command.Parameters.AddWithValue("@CoachID", coachID.Value);
                    else
                        command.Parameters.AddWithValue("@CoachID", DBNull.Value);

                    command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);
                    command.Parameters.AddWithValue("@PaymentAmount", paymentAmount);
                    command.Parameters.AddWithValue("@RemainingPaymentAmount", remainingPaymentAmount);
                    command.Parameters.AddWithValue("@PaymentDate", paymentDate);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                       MessageBox.Show("تم تعديل بيانات الدفعة بنجاح");
                    else
                        MessageBox.Show ("No record found to update.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                // Handle any errors that occur during the database operation
            }
        }
    }





    private int GetIdUsingText(string query, string parameter,string text)
        {
            int id= 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue(parameter, text);
                    id = (int) command.ExecuteScalar();
                }
            }
            return id;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }
    }
}
