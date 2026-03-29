using Bunifu.Framework.UI;
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
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WindowsFormsApp1.deletePayment;
using static WindowsFormsApp1.editeSubscription;

namespace WindowsFormsApp1
{
    public partial class addPayment : Form
    {
         string connectionString;

        public string searchQuery = "Select count (*) from Subscriptions where 1=1";
        public int clientID = 0;
        public int? sectionID = 0;
        public int coachID = 0;
        public int trainingTypeID = 0;
        bool foundOne = false;



        public addPayment(List<string> clientNames)
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
            comboBox1.Items.AddRange(clientNames.ToArray());

            bunifuCheckbox1.Checked=false;
        }



        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int paymentAmount;
                int remainingPaymentAmount = 0;
                int.TryParse(textBox2.Text, out remainingPaymentAmount);
                bool halfPayment = bunifuCheckbox1.Checked;

                DateTime paymentDate = dateTimePicker1.Value;
                string CoachName = textBox8.Text;

                //if no subscription found class
                if (!foundOne)
                {
                    MessageBox.Show("لن يتم تسجيل الدفعة, تأكد من معلومات الاشتراك", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if (foundOne)
                {
                    string name = comboBox1.SelectedItem.ToString();
                    string section = comboBox2.SelectedItem?.ToString();
                    int clientID = GetClientId(name);
                    int? sectionID = GetSectionId(section);
                    SubscriptionInfo subscription = GetSubscriptionInfo();




                    // DateTime subscriptionDate = GetSubscriptionDate(name, section);

                    DialogResult result = MessageBox.Show("هل تريد تأكيد اضافة الدفعة؟", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        //payment not valid
                        if (string.IsNullOrEmpty(textBox1.Text))
                        {
                            MessageBox.Show("ادخل قيمة الدفعة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        //get the payment info
                        if (int.TryParse(textBox1.Text, out paymentAmount) && paymentAmount > 1000)
                        {
                            if (CheckClientSubscriptionEndDate()) //there is SED and the payment is valid
                            {
                                //monthly subscription
                                if (textBox6.Text == "شهري")
                                {
                                    if (string.IsNullOrEmpty(textBox2.Text) || remainingPaymentAmount == 0)  //without remaining payment amount
                                    {
                                        // Retrieve the current subscription end date
                                        DateTime subscriptionEndDate = GetSubscriptionEndDate();
                                        DateTime newEndDate = subscriptionEndDate.AddMonths(1);




                                        if (newEndDate < DateTime.Today)
                                        {
                                            MessageBox.Show("سيتم احتساب أيام الاشتراك الجديدة بدءاً من اليوم");
                                            if (halfPayment)
                                                newEndDate = DateTime.Today.AddDays(15);
                                            else
                                                newEndDate = DateTime.Today.AddMonths(1);
                                        }

                                        // Update the subscription end date in the database
                                        UpdateSubscriptionEndDate(newEndDate);
                                        //monthly
                                        RecordPaymentInfo(clientID, subscription.subscriptionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);

                                        MessageBox.Show("تمت اضافة الدفعة وتجديد اشتراك المشترك");
                                    }
                                    else if (remainingPaymentAmount > 0) //with remaining paymet amount
                                    {
                                        if (remainingPaymentAmount < 1000)
                                        {
                                            MessageBox.Show("قيمة مبلغ متبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تسجيل الدفعة بنجاح");
                                    }
                                }
                                //half month subscription
                                else if (textBox6.Text == "نصف شهري")
                                {
                                    //without remaining payment amount 
                                    if (string.IsNullOrEmpty(textBox2.Text) || remainingPaymentAmount == 0)
                                    {
                                        // Retrieve the current subscription end date
                                        DateTime subscriptionEndDate = GetSubscriptionEndDate();

                                        // Add one month to the subscription end date
                                        DateTime newEndDate = subscriptionEndDate.AddDays(15);

                                        if (newEndDate < DateTime.Today)
                                            newEndDate = DateTime.Today.AddDays(15);

                                        // Update the subscription end date in the database
                                        UpdateSubscriptionEndDate(newEndDate);
                                        //monthly
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);

                                        MessageBox.Show("تمت اضافة الدفعة وتجديد اشتراك المشترك");
                                    }

                                    //with remaining payment amount
                                    else if (remainingPaymentAmount > 0) //with remaining paymet amount
                                    {
                                        if (remainingPaymentAmount < 1000)
                                        {
                                            MessageBox.Show("قيمة مبلغ متبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تسجيل الدفعة بنجاح");
                                    }


                                }
                                //weekly subscription
                                else if (textBox6.Text == "اسبوعي")

                                {
                                    if (string.IsNullOrEmpty(textBox2.Text) || remainingPaymentAmount == 0)  //without remaining payment amount
                                    {
                                        // Retrieve the current subscription end date
                                        DateTime subscriptionEndDate = GetSubscriptionEndDate();

                                        // Add 7 days to the subscription end date
                                        DateTime newEndDate = subscriptionEndDate.AddDays(7);

                                        if (newEndDate < DateTime.Today)
                                            newEndDate = DateTime.Today.AddDays(7);

                                        // Update the subscription end date in the database
                                        UpdateSubscriptionEndDate(newEndDate);
                                        //record payment info
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);

                                        MessageBox.Show("تمت اضافة الدفعة وتجديد اشتراك المشترك");
                                    }
                                    //with remaining payment amount
                                    if (remainingPaymentAmount > 0)
                                    {
                                        if (remainingPaymentAmount < 1000)
                                        {
                                            MessageBox.Show("قيمة مبلغ متبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }

                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تسجيل الدفعة بنجاح");


                                    }
                                }
                                //yearly subscription
                                else if (textBox6.Text == "سنوي")
                                {
                                    if (string.IsNullOrEmpty(textBox2.Text) || remainingPaymentAmount == 0)  //without remaining payment amount
                                    {
                                        // Retrieve the current subscription end date
                                        DateTime subscriptionEndDate = GetSubscriptionEndDate();

                                        // Add 7 days to the subscription end date
                                        DateTime newEndDate = subscriptionEndDate.AddDays(365);

                                        if (newEndDate < DateTime.Today)
                                            newEndDate = DateTime.Today.AddDays(365);

                                        // Update the subscription end date in the database
                                        UpdateSubscriptionEndDate(newEndDate);
                                        //record payment info
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);

                                        MessageBox.Show("تمت اضافة الدفعة وتجديد اشتراك المشترك");
                                    }
                                    //with remaining payment amount
                                    if (remainingPaymentAmount > 0)
                                    {
                                        if (remainingPaymentAmount < 1000)
                                        {
                                            MessageBox.Show("قيمة مبلغ متبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تسجيل الدفعة بنجاح");


                                    }
                                }


                            }
                            //if there is no SED, the client is new and it is their first payment
                            else if (!CheckClientSubscriptionEndDate())
                            {
                                //monthly subscription
                                if (textBox6.Text == "شهري")
                                {
                                    //without remaining payment amount
                                    if (string.IsNullOrEmpty(textBox2.Text) || remainingPaymentAmount == 0)
                                    {

                                        DateTime newEndDate = CalculateSubscriptionEndDateForMonthlyClients(subscription.SubscriptionDate);
                                        // Update the subscription end date in the database
                                        UpdateSubscriptionEndDate(newEndDate);
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تمت اضافة الدفعة وتجديد اشتراك المشترك");

                                    }
                                    //with remaining payment amount
                                    else if (remainingPaymentAmount > 0)
                                    {
                                        if (remainingPaymentAmount < 1000)
                                        {
                                            MessageBox.Show("قيمة مبلغ متبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تسجيل الدفعة بنجاح");

                                    }

                                }
                                //half monthly subscription
                                else if (textBox6.Text == "نصف شهري")
                                {
                                    //without remaining payment amount
                                    if (string.IsNullOrEmpty(textBox2.Text) || remainingPaymentAmount == 0)
                                    {
                                        DateTime endDate = DateTime.Now.AddDays(15);
                                        UpdateSubscriptionEndDate(endDate);
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تجديد الاشتراك وتسجيل الدفعة بنجاح");

                                    }
                                    //with remaining payment amount
                                    else if (remainingPaymentAmount > 0)
                                    {
                                        if (remainingPaymentAmount < 1000)
                                        {
                                            MessageBox.Show("قيمة مبلغ متبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تسجيل الدفعة بنجاح");

                                    }



                                }
                                //montly subscription
                                else if (textBox6.Text == "اسبوعي")
                                {
                                    //without remaining payment amount
                                    if (string.IsNullOrEmpty(textBox2.Text) || remainingPaymentAmount == 0)
                                    {
                                        DateTime endDate = DateTime.Now.AddDays(7);
                                        UpdateSubscriptionEndDate(endDate);
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تجديد الاشتراك وتسجيل الدفعة بنجاح");
                                    }
                                    //with remaining payment amount
                                    else if (remainingPaymentAmount > 0)
                                    {
                                        if (remainingPaymentAmount < 1000)
                                        {
                                            MessageBox.Show("قيمة مبلغ متبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تسجيل الدفعة بنجاح");

                                    }
                                }
                                //yearly subscription
                                else if (textBox6.Text == "سنوي")
                                {
                                    //without remaining payment amount
                                    if (string.IsNullOrEmpty(textBox2.Text) || remainingPaymentAmount == 0)
                                    {
                                        DateTime endDate = DateTime.Now.AddDays(365);
                                        UpdateSubscriptionEndDate(endDate);
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تجديد الاشتراك وتسجيل الدفعة بنجاح");

                                    }
                                    //with remaining payment amount
                                    else if (remainingPaymentAmount > 0)
                                    {
                                        if (remainingPaymentAmount < 1000)
                                        {
                                            MessageBox.Show("قيمة مبلغ متبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;
                                        }
                                        RecordPaymentInfo(clientID, subscription.SectionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId, subscription.TrainingTypeId);
                                        MessageBox.Show("تم تسجيل الدفعة بنجاح");

                                    }
                                }


                            }


                        }
                        else
                        {
                            MessageBox.Show("قيمة دفعة غير صالحة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }


                    }
                }
                else
                {
                    MessageBox.Show("تأكد من اختيار كافة المعلومات المطلوبة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        //fixed
        public DateTime GetSubscriptionDate()
        {
         

            DateTime subscriptionDate = DateTime.MinValue; // Default value if subscription date is not found

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the database connection
                connection.Open();

                // Create a SQL command to fetch the subscription date based on the client name and section
                string query = "SELECT SubscriptionDate FROM Subscriptions WHERE ClientID = @ClientID";
                if (sectionID != 0)
                {
                    query += " AND SectionID = @SectionID";
                }

                int items = comboBox2.Items.Count - 1;
                if (comboBox2.SelectedIndex == items)
                {
                    query += " and SectionID is null";
                }

                if (coachID != 0)
                {
                    query += " AND CoachID = @CoachID";
                }

                if (trainingTypeID != 0)
                {
                    query += " AND TrainingTypeID = @TrainingTypeID";
                }


                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClientID", clientID);
                if (sectionID > 0) command.Parameters.AddWithValue("@SectionID", sectionID);
                if (coachID > 0) command.Parameters.AddWithValue("@CoachID", coachID);
                if (trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                // Execute the SQL command and retrieve the subscription date
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    subscriptionDate = Convert.ToDateTime(result);
                }

                // Close the database connection
                connection.Close();
            }

            return subscriptionDate;
        }



        //correct
        public DateTime CalculateSubscriptionEndDateForMonthlyClients(DateTime subscriptionDate)
        {
            // Calculate the new subscription end date based on the given criteria
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month + 1;
            int day = subscriptionDate.Day;

            // Adjust the year and month if necessary
            if (month > 12)
            {
                month -= 12;
                year++;
            }

            // Create the new subscription end date
            DateTime endDate = new DateTime(year, month, day);

            return endDate;
        }

        //fixed     RecordPaymentInfo(clientID, subscription.subscriptionId, subscription.SubscriptionTypeId, paymentAmount, remainingPaymentAmount, paymentDate, subscription.CoachId);

        private void RecordPaymentInfo(int clientID, int sectionID, int subscriptionTypeID, decimal paymentAmount, decimal remainingPaymentAmount, DateTime paymentDate, int coachID, int TrainingTypeID)
        {
            

            string query = "INSERT INTO PaymentsTable (ClientID, SectionID, SubscriptionTypeID, PaymentAmount, RemainingPaymentAmount, PaymentDate, CoachID, TrainingTypeID) " +
                           "VALUES (@ClientId, @SectionId, @SubscriptionTypeId, @PaymentAmount, @RemainingPaymentAmount, @PaymentDate, @CoachID, @TrainingTypeID)";


            if(sectionID==0 && coachID>0) query = "INSERT INTO PaymentsTable (ClientID,  SubscriptionTypeID, PaymentAmount, RemainingPaymentAmount, PaymentDate, CoachID, TrainingTypeID) " +
               "VALUES (@ClientId,  @SubscriptionTypeId, @PaymentAmount, @RemainingPaymentAmount, @PaymentDate, @CoachID, @TrainingTypeID)";

            else if(coachID==0 && sectionID>0)
            {
                 query = "INSERT INTO PaymentsTable (ClientID, SectionID, SubscriptionTypeID, PaymentAmount, RemainingPaymentAmount, PaymentDate, TrainingTypeID) " +
               "VALUES (@ClientId, @SectionId, @SubscriptionTypeId, @PaymentAmount, @RemainingPaymentAmount, @PaymentDate, @TrainingTypeID)";

            }
            else if(coachID==0 && sectionID==0)
            {
                 query = "INSERT INTO PaymentsTable (ClientID, SubscriptionTypeID, PaymentAmount, RemainingPaymentAmount, PaymentDate, TrainingTypeID) " +
                               "VALUES (@ClientId, @SubscriptionTypeId, @PaymentAmount, @RemainingPaymentAmount, @PaymentDate, @TrainingTypeID)";

            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientID);
                   if(sectionID!=0) command.Parameters.AddWithValue("@SectionId", sectionID);
                    command.Parameters.AddWithValue("@SubscriptionTypeId", subscriptionTypeID);
                    command.Parameters.AddWithValue("@PaymentAmount", paymentAmount);
                    command.Parameters.AddWithValue("@RemainingPaymentAmount", remainingPaymentAmount);
                    command.Parameters.AddWithValue("@PaymentDate", paymentDate);
                    if(coachID!=0)
                    command.Parameters.AddWithValue("@CoachID", coachID);
                    command.Parameters.AddWithValue("@TrainingTypeID", TrainingTypeID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        //correct
        private int GetCoachId(string coachName)
        {
            string query = "SELECT CoachID FROM Coaches WHERE CoachName = @CoachName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachName", coachName);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (int)result;
                    }
                }
            }

            return 0; // Return a default value if the client ID is not found
        }
        //correct
        private int GetClientId(string clientName)
        {
            string query = "SELECT ClientId FROM Clients WHERE ClientName = @ClientName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientName", clientName);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (int)result;
                    }
                }
            }

            return 0; // Return a default value if the client ID is not found
        }
        //correct
        private int? GetSectionId(string section)
        {
            if (section == null)
            {
                return null;
            }
               
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
        //correct
        private int GetSubscriptionTypeId(string subscriptionType)
        {
            string query = "SELECT SubscriptionID FROM SubscriptionType WHERE SubscriptionName = @SubscriptionType";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubscriptionType", subscriptionType);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (int)result;
                    }
                }
            }

            return 0; // Return a default value if the subscription type ID is not found
        }

        //fixed^2
        private DateTime GetSubscriptionEndDate()
        {
     
            string query = "SELECT SubscriptionEndDate FROM Subscriptions WHERE ClientID = @clientID";

         

            if (sectionID != 0)
            {
                query += " AND SectionID = @SectionID";
            }

            int items = comboBox2.Items.Count - 1;
            if (comboBox2.SelectedIndex == items)
            {
                query += " and SectionID is null";
            }

            if (coachID != 0)
            {
                query += " AND CoachID = @CoachID";
            }

            if (trainingTypeID != 0)
            {
                query += " AND TrainingTypeID = @TrainingTypeID";
            }


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if (sectionID > 0) command.Parameters.AddWithValue("@SectionID", sectionID);
                    if (coachID > 0) command.Parameters.AddWithValue("@CoachID", coachID);
                    if (trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (DateTime)result;
                    }
                }
            }

            return DateTime.MinValue; // Return a default value if the subscription end date is not found
        }

        private void UpdateSubscriptionEndDate( DateTime newEndDate)
        {

          
            string query = "UPDATE Subscriptions SET SubscriptionEndDate = @NewEndDate WHERE ClientID = @clientID";

         

            if (sectionID != 0)
            {
                query += " AND SectionID = @SectionID";
            }

            int items = comboBox2.Items.Count - 1;
            if (comboBox2.SelectedIndex == items)
            {
                query += " and SectionID is null";
            }

            if (coachID != 0)
            {
                query += " AND CoachID = @CoachID";
            }

            if (trainingTypeID != 0)
            {
                query += " AND TrainingTypeID = @TrainingTypeID";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NewEndDate", newEndDate);
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if (sectionID > 0) command.Parameters.AddWithValue("@SectionID", sectionID);
                    if (coachID > 0) command.Parameters.AddWithValue("@CoachID", coachID);
                    if (trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        //fixed
        public bool CheckClientSubscriptionEndDate()
        {
            
           
            string query = "SELECT COUNT(*) FROM Subscriptions WHERE ClientID = @ClientID AND SubscriptionEndDate IS NOT NULL";

           

            if (sectionID != 0)
            {
                query += " AND SectionID = @SectionID";
            }

            int items = comboBox2.Items.Count - 1;
            if (comboBox2.SelectedIndex == items)
            {
                query += " and SectionID is null";
            }

            if (coachID != 0)
            {
                query += " AND CoachID = @CoachID";
            }

            if (trainingTypeID != 0)
            {
                query += " AND TrainingTypeID = @TrainingTypeID";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if (sectionID > 0) command.Parameters.AddWithValue("@SectionID", sectionID);
                    if (coachID > 0) command.Parameters.AddWithValue("@CoachID", coachID);
                    if (trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
      
        //checked

        private void addPayment_Load(object sender, EventArgs e)
        {
            FillSectionCombobox();
            FillCoachesNames();
            FillTrainingNames();
        }

        private void FillTrainingNames()
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
                            comboBox4.Items.Add(trainingName);

                        }
                    }
                }
            }

            comboBox2.Items.Add("المشترك غير مسجل في اي قسم");
        }
        private void FillCoachesNames()
        {
            // Populate the coach combobox
            string coachQuery = "SELECT DISTINCT CoachName FROM Coaches"; // Replace YourTableName with the actual table name
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(coachQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string coach = reader["CoachName"].ToString();
                            comboBox3.Items.Add(coach);
                        }
                    }
                }
            }
        }

       private void FillClientsNames()
        {

            string query = "SELECT DISTINCT ClientName FROM Clients"; // Replace YourTableName with the actual table name

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string clientName = reader["ClientName"].ToString();
                            comboBox1.Items.Add(clientName);
                        }
                    }
                }
            }
        } 

        private void FillSectionCombobox()
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
                        AutoCompleteStringCollection autoCompleteCollection3 = new AutoCompleteStringCollection();

                        while (reader.Read())
                        {
                            string sectionName = reader["SectionName"].ToString();
                            comboBox2.Items.Add(sectionName);
                            autoCompleteCollection3.Add(sectionName);
                        }

               
                    }
                }
            }
        }


        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuImageButton1.Enabled = false;
            this.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            sectionID = GetSectionId(comboBox2.SelectedItem.ToString());
            int lastIndex = comboBox2.Items.Count - 1;

            if (comboBox2.SelectedIndex == lastIndex)
                searchQuery += " and SectionID is null";
            else
            {
                searchQuery += " and SectionID=@SectionID";
            }

            CountSubsRecords();


        }



        public class CoachAndSubscriptionTypeInfo
        {
            public string CoachName { get; set; }
            public string SubscriptionTypeName { get; set; }
        }


        public string GetCoachName(int coachID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query to get coach name
                string coachQuery = @"
            SELECT CoachName
            FROM Coaches
            WHERE CoachID = @CoachID";

                using (SqlCommand coachCommand = new SqlCommand(coachQuery, connection))
                {
                    coachCommand.Parameters.AddWithValue("@CoachID", coachID);

                    using (SqlDataReader coachReader = coachCommand.ExecuteReader())
                    {
                        if (coachReader.Read())
                        {
                            return coachReader.GetString(0);
                        }
                    }
                }
            }
            return string.Empty;

        }

        public string GetSubscriptionType(int stID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString) )
            {
                connection.Open();
                // Query to get subscription type name
                string subscriptionTypeQuery = @"
            SELECT SubscriptionName
            FROM SubscriptionType
            WHERE SubscriptionID = @SubscriptionTypeID";

                using (SqlCommand subscriptionTypeCommand = new SqlCommand(subscriptionTypeQuery, connection))
                {
                    subscriptionTypeCommand.Parameters.AddWithValue("@SubscriptionTypeID", stID);

                    using (SqlDataReader subscriptionTypeReader = subscriptionTypeCommand.ExecuteReader())
                    {
                        if (subscriptionTypeReader.Read())
                        {
                            return subscriptionTypeReader.GetString(0);
                        }
                    }
                }
            }
            return string.Empty;

            
        }
        
      


        public void FillData(SubscriptionAndPaymentInfo sapi)
        {
            textBox7.Text = sapi.SubscriptionDate.ToString("dd/MM/yyyy");
            string trainingType="";
            if (sapi.trainingTypeID == 1)
                trainingType = "عام";
            else if (sapi.trainingTypeID == 2)
                trainingType = "برايفت";

            textBox9.Text = trainingType;

            string subscriptionType = GetSubscriptionType(sapi.SubscriptionTypeID);
            

            textBox6.Text= subscriptionType;
            textBox3.Text=sapi.TotalSubscriptionAmount.ToString();
            string coachName;
           
                coachName = GetCoachName(sapi.CoachID);

            
            textBox8.Text = coachName;
            textBox10.Text = GetSectionName(sapi.sectionID);

            textBox5.Text=sapi.LastPaymentDate.ToString("dd/MM/yyyy");
            textBox4.Text=sapi.RemainingPaymentAmount.ToString();

            if(sapi.LastPaymentDate==DateTime.MinValue)
            {
                textBox5.TextAlign = HorizontalAlignment.Right;
                textBox4.TextAlign = HorizontalAlignment.Right; 
                textBox5.Text = "لا يوجد دفعات سابقة";
                textBox4.Text = "لا يوجد دفعات سابقة";
            }
        }

        private string GetSectionName(int secionID)
        {
            string sectionName = "";
            string query="select SectionName from Sections where SectionID=@SectionID";
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection)) 
                {
                    command.Parameters.AddWithValue("@SectionID",secionID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            sectionName = reader.GetString(0);
                        }
                    }


                }
                
            }
            return sectionName;
        }

    


        //did not use it
        public string GetSubscriptionTypeByID(int subscriptionTypeId)
        {
            CoachAndSubscriptionTypeInfo info = new CoachAndSubscriptionTypeInfo();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

               

                // Query to get subscription type name
                string subscriptionTypeQuery = @"
            SELECT SubscriptionName
            FROM SubscriptionType
            WHERE SubscriptionID = @SubscriptionTypeID";

                using (SqlCommand subscriptionTypeCommand = new SqlCommand(subscriptionTypeQuery, connection))
                {
                    subscriptionTypeCommand.Parameters.AddWithValue("@SubscriptionTypeID", subscriptionTypeId);

                    using (SqlDataReader subscriptionTypeReader = subscriptionTypeCommand.ExecuteReader())
                    {
                        if (subscriptionTypeReader.Read())
                        {
                            info.SubscriptionTypeName = subscriptionTypeReader.GetString(0);
                        }
                    }
                }
            }

            return info.SubscriptionTypeName;
        }

        public class SubscriptionAndPaymentInfo
        {
            public int SubscriptionTypeID { get; set; }
            public int CoachID { get; set; }
            public int TotalSubscriptionAmount { get; set; }
            public DateTime SubscriptionDate { get; set; }
            public int RemainingPaymentAmount { get; set; }
            public DateTime LastPaymentDate { get; set; }
            public int trainingTypeID { get; set; }
            public int sectionID { get; set; }
        }


        //Now I have the Ids of the subscription and the info of the last payment
        public SubscriptionAndPaymentInfo GetSubscriptionAndPaymentInfo()
        {
          //  string connectionString = "YourConnectionStringHere"; // Replace with your actual connection string
            SubscriptionAndPaymentInfo info = new SubscriptionAndPaymentInfo();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query to get subscription info
                string subscriptionQuery = @"
            SELECT SubscriptionTypeID, CoachID, TotalSubscriptionAmount, SubscriptionDate, TrainingTypeID, SectionID
            FROM Subscriptions
            WHERE ClientID = @ClientID";


                if (sectionID != 0)
                {
                    subscriptionQuery += " AND SectionID = @SectionID";
                }

                int items = comboBox2.Items.Count - 1;
                if (comboBox2.SelectedIndex == items)
                {
                    subscriptionQuery += " and SectionID is null";
                }

                if (coachID != 0)
                {
                    subscriptionQuery += " AND CoachID = @CoachID";
                }

                if (trainingTypeID != 0)
                {
                    subscriptionQuery += " AND TrainingTypeID = @TrainingTypeID";
                }

                using (SqlCommand subscriptionCommand = new SqlCommand(subscriptionQuery, connection))
                {
                    subscriptionCommand.Parameters.AddWithValue("@ClientID", clientID);
                    if (sectionID > 0) subscriptionCommand.Parameters.AddWithValue("@SectionID", sectionID);
                    if (coachID > 0) subscriptionCommand.Parameters.AddWithValue("@CoachID", coachID);
                    if (trainingTypeID > 0) subscriptionCommand.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);


                    using (SqlDataReader subscriptionReader = subscriptionCommand.ExecuteReader())
                    {
                        if (subscriptionReader.Read())
                        {
                            // Check for DBNull before assigning SubscriptionTypeID
                            if (!subscriptionReader.IsDBNull(0))
                            {
                                info.SubscriptionTypeID = subscriptionReader.GetInt32(0);
                            }

                            // Check for DBNull before assigning CoachID
                            if (!subscriptionReader.IsDBNull(1))
                            {
                                info.CoachID = subscriptionReader.GetInt32(1);
                            }

                            // Check for DBNull before assigning TotalSubscriptionAmount
                            if (!subscriptionReader.IsDBNull(2))
                            {
                                info.TotalSubscriptionAmount = subscriptionReader.GetInt32(2);
                            }

                            // Check for DBNull before assigning SubscriptionDate
                            if (!subscriptionReader.IsDBNull(3))
                            {
                                info.SubscriptionDate = subscriptionReader.GetDateTime(3);
                            }

                            if (!subscriptionReader.IsDBNull(4))
                            {
                                info.trainingTypeID = subscriptionReader.GetInt32(4);
                            }

                            if (!subscriptionReader.IsDBNull(5))
                            {
                                info.sectionID = subscriptionReader.GetInt32(5);
                            }
                        }
                    }

                }

                // Query to get the last payment info
                string paymentQuery = @"
                SELECT TOP 1 RemainingPaymentAmount, PaymentDate
                FROM PaymentsTable
                WHERE ClientID = @ClientID";
                if (coachID != 0) paymentQuery += " and CoachID=@CoachID";
                if (trainingTypeID != 0) paymentQuery += " and TrainingTypeID=@TrainingTypeID";
                if (sectionID != 0) paymentQuery += " and SectionID=@SectionID";


                paymentQuery+= " ORDER BY Id DESC";




                using (SqlCommand paymentCommand = new SqlCommand(paymentQuery, connection))
                {
                    paymentCommand.Parameters.AddWithValue("@ClientID", clientID);
                    if(coachID>0)   paymentCommand.Parameters.AddWithValue("@CoachID", coachID);
                    if(trainingTypeID>0) paymentCommand.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);
                    if(sectionID>0) paymentCommand.Parameters.AddWithValue("@SectionID", sectionID);

                    using (SqlDataReader paymentReader = paymentCommand.ExecuteReader())
                    {
                        if (paymentReader.Read())
                        {
                            info.RemainingPaymentAmount = paymentReader.GetInt32(0);
                            info.LastPaymentDate = paymentReader.GetDateTime(1);
                        }
                    }
                }
            }

            return info;
        }





        private bool CountSubsRecords()
        {

            int recordsFound = 8;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(searchQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if (sectionID > 0) command.Parameters.AddWithValue("@SectionID", sectionID);
                    if (coachID > 0) command.Parameters.AddWithValue("@CoachID", coachID);
                    if (trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);
                    recordsFound = (int)command.ExecuteScalar();
                }
            }
            //I need to get the info then fill it to the textBoxes
            if (recordsFound == 1)
            {

                foundOne = true;
                SubscriptionAndPaymentInfo subscription = GetSubscriptionAndPaymentInfo();
                //now i need to fill the data
                FillData(subscription);

             
            }
            else if (recordsFound > 1)
            {
                MessageBox.Show("يوجد أكثر من اشتراك بالمعلومات التي اخترتها, جرب تحديد معلومات أكثر أو اختر خيار 'المشترك غير مسجل في أي قسم' اذا كان حقل قسم التسجل فارغاً في سجل الاشتراك", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Text = string.Empty;
                textBox4.Text = string.Empty;
                textBox5.Text = string.Empty;
                textBox6.Text = string.Empty;
                textBox7.Text = string.Empty;
                textBox8.Text = string.Empty;
                textBox9.Text = string.Empty;
                textBox10.Text = string.Empty;


            }
            else if (recordsFound == 0)
            {
                foundOne = false;
                MessageBox.Show("لا يوجد سجلات بالمعلومات المدخلة", "", MessageBoxButtons.OK, MessageBoxIcon.Question);
                //5,3,4,8
                textBox3.Text = string.Empty;
                textBox4.Text = string.Empty;
                textBox5.Text = string.Empty;
                textBox6.Text = string.Empty;
                textBox7.Text = string.Empty;
                textBox8.Text = string.Empty;
                textBox9.Text = string.Empty;
                textBox10.Text = string.Empty;


            }
            return recordsFound > 0;
        }


        /// <summary>
        /// this function gets the subscriptions info, assign the ids to the attributes of the Subscription. It uses the Ids in the form constructor
        /// </summary>
        /// <returns> SubscripionInfo object with the subscripiton info</returns>
        public SubscriptionInfo GetSubscriptionInfo()
        {
            SubscriptionInfo subscriptionInfo = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT SectionID, CoachID, TrainingTypeID, SubscriptionTypeID, TotalSubscriptionAmount , SubscriptionDate, SubscriptionEndDate, TrainingTypeID " +
                               "FROM Subscriptions " +
                               "WHERE 1 = 1";

                if (clientID != 0)
                {
                    query += " AND ClientID = @ClientID";
                }

                if (sectionID != 0)
                {
                    query += " AND SectionID = @SectionID";
                }

                int items = comboBox2.Items.Count - 1;
                if (comboBox2.SelectedIndex == items)
                {
                    query += " and SectionID is null";
                }

                if (coachID != 0)
                {
                    query += " AND CoachID = @CoachID";
                }

                if (trainingTypeID != 0)
                {
                    query += " AND TrainingTypeID = @TrainingTypeID";
                }
                // int retrievedSectionId;
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    command.Parameters.AddWithValue("@SectionID", sectionID);
                    command.Parameters.AddWithValue("@CoachID", coachID);
                    command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            int retrievedSectionId = !reader.IsDBNull(0) ? reader.GetInt32(0) : 0;
                            int retrievedCoachId = !reader.IsDBNull(1) ? reader.GetInt32(1) : 0;
                            int retrievedTrainingTypeId = reader.GetInt32(2);
                            int retrievedSubscriptionTypeId = reader.GetInt32(3);
                            int retrievedSubscriptionFee = reader.GetInt32(4);
                            DateTime retrievedSubscriptionDate = reader.GetDateTime(5);
                            DateTime retrievedSubscriptionEndDate = !reader.IsDBNull(6) ? reader.GetDateTime(6) : DateTime.Now;
                            int retrievedSubscriptionID = reader.GetInt32(7);

                            subscriptionInfo = new SubscriptionInfo(retrievedSectionId, retrievedCoachId, retrievedTrainingTypeId, retrievedSubscriptionTypeId, retrievedSubscriptionFee, retrievedSubscriptionDate, retrievedSubscriptionEndDate, retrievedSubscriptionID);
                        }
                    }
                }
            }

            return subscriptionInfo;
        }




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
                clientID = GetClientId(comboBox1.SelectedItem.ToString());
                searchQuery = "Select count (*) from Subscriptions where ClientID=@ClientID";
                CountSubsRecords();
            


        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                coachID = GetCoachId(comboBox3.SelectedItem.ToString());
                searchQuery += " and CoachID=@CoachID";
                CountSubsRecords();
            }
         
            catch (ArgumentNullException ex) // Catching ArgumentNullException if comboBox7.SelectedItem is null
            {
                // Handle the case where comboBox7.SelectedItem is null
                MessageBox.Show($"Please select a training type from the dropdown. \n {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidCastException ex) // Catching InvalidCastException if conversion fails
            {
                // Handle the case where conversion fails
                MessageBox.Show($"Failed to convert the selected item to a training type. \n {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException ex) // Catching SqlException if there's an issue with database operations
            {
                // Handle the case where there's an issue with database operations
                MessageBox.Show($"Database operation failed: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ex) // Catching InvalidOperationException for other unexpected issues
            {
                // Handle other unexpected issues
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // Catch-all for any other unexpected exceptions
            {
                // Handle any other unexpected exceptions
                MessageBox.Show($"An unknown error occurred: {ex.Message}", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                trainingTypeID = GetTrainingTypeID(comboBox4.SelectedItem.ToString());
                searchQuery += " and TrainingTypeID=@TrainingTypeID";
                CountSubsRecords();
            }
            catch (ArgumentNullException ex) // Catching ArgumentNullException if comboBox7.SelectedItem is null
            {
                // Handle the case where comboBox7.SelectedItem is null
                MessageBox.Show($"Please select a training type from the dropdown. \n {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidCastException ex) // Catching InvalidCastException if conversion fails
            {
                // Handle the case where conversion fails
                MessageBox.Show($"Failed to convert the selected item to a training type. \n {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException ex) // Catching SqlException if there's an issue with database operations
            {
                // Handle the case where there's an issue with database operations
                MessageBox.Show($"Database operation failed: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ex) // Catching InvalidOperationException for other unexpected issues
            {
                // Handle other unexpected issues
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // Catch-all for any other unexpected exceptions
            {
                // Handle any other unexpected exceptions
                MessageBox.Show($"An unknown error occurred: {ex.Message}", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
