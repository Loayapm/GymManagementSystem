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
    public partial class deletePayment : Form
    {
        string connectionString;

        public deletePayment()
        {

            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int paymentID = 0;
                int.TryParse(textBox1.Text, out paymentID);
                if (paymentID == 0)
                {
                    MessageBox.Show("payment id not valid");
                    return;
                }
                //i have info about RPA, SubsID, and SectionID
                Payment payment = GetPaymentInfo(paymentID);

                if (payment == null)
                {
                    MessageBox.Show("No payment found with this id");
                    return;
                }



                //if the subscription has been renewed
                if (payment.RemainingPaymentAmount == 0)
                {

                    DialogResult result = MessageBox.Show("هل تريد تأكيد حذف الدفعة؟ ستتم ازالة ايام من الاشتراك", "تحذير", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                    string subscriptionType = GetSubscriptionName(payment.SubscriptionTypeID);
                    DateTime endDate = GetSubscriptionEndDate(payment.ClientID, payment.SectionID);

                    //monthly
                    if (subscriptionType == "شهري")
                    {
                        DateTime newEndDate = endDate.AddDays(-30);
                        UpdateSubscriptionEndDate(payment.ClientID, payment.SectionID, newEndDate, 30);
                        deletePaymentRecord(paymentID);
                    }
                    if (subscriptionType == "نصف شهري")
                    {
                        DateTime newEndDate = endDate.AddDays(-15);
                        UpdateSubscriptionEndDate(payment.ClientID, payment.SectionID, newEndDate, 15);
                        deletePaymentRecord(paymentID);
                    }
                    if (subscriptionType == "اسبوعي")
                    {
                        DateTime newEndDate = endDate.AddDays(-7);
                        UpdateSubscriptionEndDate(payment.ClientID, payment.SectionID, newEndDate, 7);
                        deletePaymentRecord(paymentID);
                    }
                    if (subscriptionType == "سنوي")
                    {
                        DateTime newEndDate = endDate.AddDays(-365);
                        UpdateSubscriptionEndDate(payment.ClientID, payment.SectionID, newEndDate, 365);
                        deletePaymentRecord(paymentID);
                    }

                }
                else
                {
                    deletePaymentRecord(paymentID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }



        }

        private void UpdateSubscriptionEndDate(int clientID, int sectionID, DateTime newEndDate, int daysRemoved)
        {
            string query = "update Subscriptions set SubscriptionEndDate=@SubscriptionEndDate where ClientID=@ClientID and SectionID=@SectionID";
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command=new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@SubscriptionEndDate", newEndDate);
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    command.Parameters.AddWithValue("@SectionID",sectionID);
                    command.ExecuteNonQuery();
                }
            }
            MessageBox.Show($"تمت ازالة {daysRemoved} من الايام من الاشتراك بنجاح");
        }

        public  DateTime GetSubscriptionEndDate(int clientId, int sectionId)
        {
            string query = @"
            SELECT SubscriptionEndDate 
            FROM Subscriptions 
            WHERE ClientID = @ClientId AND SectionID = @SectionId
            ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@SectionId", sectionId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Check if there is a row returned
                        {
                            return reader.GetDateTime(0); // Assuming EndDate is a DateTime
                        }
                    }
                }
            }
            return DateTime.MinValue; // Return a default value if no end date is found
        }



        private void deletePaymentRecord(int paymentID)
        {
            int rowsAffected = 0;
            string query = "delete from PaymentsTable where Id=@PaymentID";
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@PaymentID", paymentID);
                    rowsAffected=command.ExecuteNonQuery();
                }
            }

            if(rowsAffected==0)
            {
                MessageBox.Show("No payment found with this ID");
            }
            else
            {
                MessageBox.Show("Payment deleted succesfully");
            }
        }


        public  string GetSubscriptionName(int subscriptionTypeID)
        {
            string query = "SELECT SubscriptionName FROM SubscriptionType WHERE SubscriptionID=@SubscriptionTypeID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubscriptionTypeID", subscriptionTypeID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Check if there is a row returned
                        {
                            return reader.GetString(0); // Assuming SubscriptionName is a string
                        }
                    }
                }
            }
            return null; // Return null if no subscription name is found
        }




        public class Payment
        {
            public int RemainingPaymentAmount { get; set; }
            public int SubscriptionTypeID { get; set; }

            public int SectionID { get; set; }

            public int ClientID { get; set; }
        }


        public class SubscriptionType
        {
            public int SubscriptionTypeID { get; set; }
            public string SubscriptionName { get; set; }

        }

        private Payment GetPaymentInfo(int paymentID)
        {
            string query = "SELECT RemainingPaymentAmount, SubscriptionTypeID, SectionID, ClientID FROM PaymentsTable WHERE Id=@PaymentID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PaymentID", paymentID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Check if there is a row returned
                        {
                            Payment payment = new Payment
                            {
                                RemainingPaymentAmount = reader.GetInt32(0),
                                SubscriptionTypeID = reader.GetInt32(1),
                                SectionID = reader.GetInt32(2),
                                ClientID = reader.GetInt32(3) // Initialize ClientID
                            };
                            return payment;
                        }
                    }
                }
            }
            return null; // Return null if no payment info is found
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }

        private void deletePayment_Load(object sender, EventArgs e)
        {

        }
    }
}
