using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Collections.Specialized.BitVector32;
using OfficeOpenXml.Style.XmlAccess;

namespace WindowsFormsApp1
{
    public partial class addForm : Form
    {

          string connectionString;



        public addForm(List<string> ClientNames)
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
            bunifuCheckbox1.Checked=false;
            comboBox6.Items.AddRange(ClientNames.ToArray());


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("هل تريد تأكيد اضافة الاشتراك؟", "confirmation", MessageBoxButtons.YesNo);
                if (!(result == DialogResult.Yes))
                {
                    return;
                }


                if (comboBox6.SelectedIndex == -1 && (textBox1.Text == string.Empty && textBox2.Text == string.Empty))
                {
                    MessageBox.Show("اختر مشتركا من القائمة او ادخل بيانات مشترك جديد", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //if the user selects a client 
                if (comboBox6.SelectedIndex >= 0)
                {

                    string selectedClient = comboBox6.SelectedItem.ToString();
                    if (!(comboBox5.SelectedIndex >= 0) || !(comboBox2.SelectedIndex >= 0))
                    {
                        MessageBox.Show("تأكد من اختيار كافة بيانات الاشتراك", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }

                    string selectedSection = comboBox4?.SelectedItem?.ToString();

                    int clientID = GetClientId(selectedClient);
                    int? sectionID = GetSectionId(selectedSection);

                    // MessageBox.Show($"SectionID is : {sectionID}");
                    int trainingTypeID = GetTrainingType(comboBox5.SelectedItem.ToString());

                    //  MessageBox.Show($"TrainingTypeID is : {trainingTypeID}");

                    int subscriptionTypeID = GetSubscriptionTypeId(comboBox2.SelectedItem.ToString());
                    int? coachID = GetCoachID(comboBox1.SelectedItem?.ToString());
                    DateTime subscriptionDate = dateTimePicker1.Value;

                    if (sectionID > 0 && clientID > 0)
                        if (CheckSubscriptionExistUsingSectionID(clientID, sectionID))
                        {
                            MessageBox.Show("يوجد اشتراك لنفس المشترك في نفس القسم, احذف الاشتراك القديم او قم بتعديله", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    if (sectionID == null)
                    {
                        if (CheckSubscriptionExistsUsingCoachID(coachID, trainingTypeID, clientID))
                        {
                            MessageBox.Show("يوجد اشتراك لنفس المشترك مع نفس المدرب مسبقاً, جرب تخصيص قسم تدريب جديد");
                            return;
                        }
                    }

                    int subscriptionFee = 0;
                    int.TryParse(textBox3.Text, out subscriptionFee);
                    if (subscriptionFee < 1000)
                    {
                        MessageBox.Show("قيمة الاشتراك غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    bool isRed = bunifuCheckbox1.Checked;

                    RecordSubscription(clientID, sectionID, coachID, subscriptionTypeID, subscriptionDate, subscriptionFee, trainingTypeID, isRed);

                    //Now I want to check if the user wants to add a payment
                    int paymentAmount = 0;
                    int.TryParse(textBox4.Text, out paymentAmount);

                    int remainingPaymentAmount = 0;
                    int.TryParse(textBox5.Text, out remainingPaymentAmount);

                    //if there is a payment
                    if (paymentAmount > 0)
                    {
                        //That means that there is a payment
                        if (paymentAmount < 1000)
                        {
                            MessageBox.Show("قيمة دفعة غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        //this means that there is a remaining payment amount
                        if (remainingPaymentAmount > 0)
                        {
                            //RPA is not valid
                            if (remainingPaymentAmount < 1000)
                            {
                                MessageBox.Show("قيمة المبلغ المتبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            //RPA is valid, now i can record the payment without updating SED
                            RecordPaymentInfo(clientID, sectionID, coachID, subscriptionTypeID, paymentAmount, remainingPaymentAmount, subscriptionDate, trainingTypeID);
                            MessageBox.Show("تمت اضافة الدفعة بنجاح");


                        }
                        else if (remainingPaymentAmount == 0)
                        {
                            //here there is a payment and the RPA is 0 or null, i need to record the payment then update the SED
                            RecordPaymentInfo(clientID, sectionID, coachID, subscriptionTypeID, paymentAmount, 0, subscriptionDate, trainingTypeID);
                            UpdateSubscriptionEndDate(clientID, sectionID, subscriptionTypeID, coachID, trainingTypeID);
                            MessageBox.Show("تمت اضافة الدفعة وتجديد تاريخ انتهاء الاشتراك");
                        }


                    }




                }
                //if no client selected
                else if (!(comboBox6.SelectedIndex >= 0))
                {
                    //now i want to add the client first
                    string clientName = textBox1.Text.Trim();
                    string phoneNumber = textBox2.Text.Trim();
                    double pn;
                    double.TryParse(phoneNumber, out pn);
                    if (!(pn > 0))
                    {
                        MessageBox.Show("رقم هاتف غير صالح");
                        return;
                    }

                    if (CheckIfClientExists(clientName))
                    {
                        MessageBox.Show("." + $"الاسم {clientName} موجود مسبقاً, جرب استخدام الاسم الثلاثي", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }




                    //now i need to check for the subscription info 

                    if (!(comboBox4.SelectedIndex >= 0) || !(comboBox2.SelectedIndex >= 0))
                    {
                        MessageBox.Show("تأكد من اختيار كافة بيانات الاشتراك", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }

                    //now i need to check for the subscription fee
                    int subscriptionFee = 0;
                    int.TryParse(textBox3.Text, out subscriptionFee);
                    if (subscriptionFee < 1000)
                    {
                        MessageBox.Show("قيمة الاشتراك غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //now we can record the client
                    RecordClient(clientName, phoneNumber);

                    // now i can record the subscription
                    int clientID = GetClientId(clientName);
                    int? sectionID = GetSectionId(comboBox4.SelectedItem.ToString());
                    int subscriptionTypeID = GetSubscriptionTypeId(comboBox2.SelectedItem.ToString());
                    int? coachID = null;
                    if (comboBox1.SelectedIndex >= 0)
                    {
                        coachID = GetCoachID(comboBox1.SelectedItem?.ToString());

                    }
                    int trainingTypeID = GetTrainingType(comboBox5.SelectedItem.ToString());
                    bool isRed = bunifuCheckbox1.Checked;

                    RecordSubscription(clientID, sectionID, coachID, subscriptionTypeID, dateTimePicker1.Value, subscriptionFee, trainingTypeID, isRed);

                    //now i need to check for the payment
                    int paymentAmount = 0;
                    int.TryParse(textBox4.Text, out paymentAmount);

                    int remainingPaymentAmount = 0;
                    int.TryParse(textBox5.Text, out remainingPaymentAmount);
                    DateTime subscriptionDate = dateTimePicker1.Value;

                    if (paymentAmount > 0)
                    {
                        //That means that there is a payment
                        if (paymentAmount < 1000)
                        {
                            MessageBox.Show("قيمة الدفعة غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        //this means that there is a remaining payment amount
                        if (remainingPaymentAmount > 0)
                        {
                            //RPA is not valid
                            if (remainingPaymentAmount < 1000)
                            {
                                MessageBox.Show("قيمة المبلغ المتبقي غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            //RPA is valid, now i can record the payment without updating SED
                            RecordPaymentInfo(clientID, sectionID, coachID, subscriptionTypeID, paymentAmount, remainingPaymentAmount, subscriptionDate, trainingTypeID);
                            MessageBox.Show("تمت اضافة الدفعة");


                        }
                        else if (remainingPaymentAmount == 0)
                        {
                            //here there is a payment and the RPA is 0 or null, i need to record the payment then update the SED
                            RecordPaymentInfo(clientID, sectionID, coachID, subscriptionTypeID, paymentAmount, 0, subscriptionDate, trainingTypeID);
                            UpdateSubscriptionEndDate(clientID, sectionID, subscriptionTypeID, coachID, trainingTypeID);
                            MessageBox.Show("تمت اضافة الدفعة وتجديد تاريخ انتهاء الاشتراك");
                        }


                    }

                }
            } catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
          
        }
        private bool CheckSubscriptionExistsUsingCoachID(int? coachID,int trainingType, int clientID)
        {
            int result = 0;
            string query = "Select Count (*) from Subscriptions where CoachID=@CoachID and TrainingTypeID=@TrainingTypeID and ClientID=@ClientID";

            if(coachID==null)
            {
               query = "Select Count (*) from Subscriptions where TrainingTypeID=@TrainingTypeID and ClientID=@ClientID";

            }
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    if(coachID!=null)
                    command.Parameters.AddWithValue("@CoachID", coachID);
                    command.Parameters.AddWithValue("@TrainingTypeID", trainingType);
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    result = (int)command.ExecuteScalar();
                }
            }
            return result > 0;

        }


        private void RecordClient(string clientName, string phoneNumber)
        {
            string query = "INSERT INTO Clients (ClientName, PhoneNumber) VALUES (@ClientName, @PhoneNumber)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientName", clientName);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("تمت اضافة المشترك بنجاح", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool CheckIfClientExists(string clientName)
        {
            string query = "SELECT COUNT(*) FROM Clients WHERE ClientName = @ClientName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientName", clientName);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }


        //this function needs to be fixed, i need to pass the coachName and the trainingType
        private void UpdateSubscriptionEndDate(int clientID, int? sectionID, int subscriptionTypeId, int ? coachID,int triningTypeID)
        {

            string subscriptionType = GetSubscriptionType(subscriptionTypeId);

            DateTime newEndDate =DateTime.Today;
            if(subscriptionType=="شهري")
            {
                newEndDate = DateTime.Today.AddMonths(1);
            }
            if(subscriptionType=="نصف شهري")
            {
                newEndDate = DateTime.Today.AddDays(15);

            }
            if (subscriptionType=="اسبوعي")
            {
                newEndDate = DateTime.Today.AddDays(7);

            }
            if (subscriptionType=="سنوي")
            {
                newEndDate = DateTime.Today.AddDays(365);

            }
            string query = "UPDATE Subscriptions SET SubscriptionEndDate = @NewEndDate WHERE ClientID = @ClientID AND sectionID = @sectionID";
            if(sectionID==null && coachID!=null)
                query = "UPDATE Subscriptions SET SubscriptionEndDate = @NewEndDate WHERE ClientID = @ClientID AND CoachID = @CoachID and TrainingTypeID=@TrainingTypeID";

            if(sectionID==null && coachID==null)
                query = "UPDATE Subscriptions SET SubscriptionEndDate = @NewEndDate WHERE ClientID = @ClientID AND  TrainingTypeID=@TrainingTypeID";
            if (sectionID != null && coachID == null)
                query = "UPDATE Subscriptions SET SubscriptionEndDate = @NewEndDate WHERE ClientID = @ClientID AND  TrainingTypeID=@TrainingTypeID and SectionID=@SectionID";



            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@NewEndDate", newEndDate);
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if(sectionID!=null)
                    command.Parameters.AddWithValue("@SectionID", sectionID);

                    if(sectionID==null)
                    {
                        if(coachID!=null)
                        command.Parameters.AddWithValue("@CoachID", coachID);
                    }
                    command.Parameters.AddWithValue("@TrainingTypeID", triningTypeID);



                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private string GetSubscriptionType(int subscriptionTypeId)
        {
            string subscriptionType = string.Empty;

            string query = "SELECT SubscriptionName FROM SubscriptionType WHERE SubscriptionID = @SubscriptionTypeId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubscriptionTypeId", subscriptionTypeId);

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        subscriptionType = result.ToString();
                    }
                }
            }

            return subscriptionType;
        }


        //wow
        private void RecordSubscription(int clientID, int? sectionID, int? coachID, int subscriptionTypeID, DateTime subscriptionDate, int subscriptionFee, int trainingTypeID, bool isRed)
        {
            try
            {

                string query = "INSERT INTO Subscriptions (ClientID, CoachID, SectionID, SubscriptionTypeID, SubscriptionDate, TotalSubscriptionAmount, TrainingTypeID, isRed) VALUES (@ClientID, @CoachID, @SectionID, @SubscriptionTypeID, @SubscriptionDate, @TotalSubscriptionAmount, @TrainingTypeID, @isRed)";
                if(sectionID==null && coachID!=null)
                 query = "INSERT INTO Subscriptions (ClientID, CoachID, SubscriptionTypeID, SubscriptionDate, TotalSubscriptionAmount, TrainingTypeID, isRed) VALUES (@ClientID, @CoachID, @SubscriptionTypeID, @SubscriptionDate, @TotalSubscriptionAmount, @TrainingTypeID, @isRed)";
                if (sectionID != null && coachID == null)
                    query = "INSERT INTO Subscriptions (ClientID, SectionID, SubscriptionTypeID, SubscriptionDate, TotalSubscriptionAmount, TrainingTypeID, isRed) VALUES (@ClientID, @SectionID, @SubscriptionTypeID, @SubscriptionDate, @TotalSubscriptionAmount, @TrainingTypeID, @isRed)";

                if(sectionID==null && coachID==null)
                    query = "INSERT INTO Subscriptions (ClientID,  SubscriptionTypeID, SubscriptionDate, TotalSubscriptionAmount, TrainingTypeID, isRed) VALUES (@ClientID,  @SubscriptionTypeID, @SubscriptionDate, @TotalSubscriptionAmount, @TrainingTypeID, @isRed)";


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        command.Parameters.AddWithValue("@ClientID", clientID);
                        if (coachID != null) 
                        command.Parameters.AddWithValue("@CoachID", coachID);
                        if (sectionID != null)
                        command.Parameters.AddWithValue("@SectionID", sectionID);
                        command.Parameters.AddWithValue("@SubscriptionTypeID", subscriptionTypeID);
                        command.Parameters.AddWithValue("@SubscriptionDate", subscriptionDate);
                        command.Parameters.AddWithValue("@TotalSubscriptionAmount", subscriptionFee);
                        command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);
                        command.Parameters.AddWithValue("@isRed", isRed);

                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                MessageBox.Show("تمت اضافة الاشتراك بنجاح");
            }
            catch (SqlException ex)
            {
                // Log the error or show a message to the user
                MessageBox.Show("An error occurred while inserting data: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }
        }

        private int GetTrainingType(string trainingType)
        {
            string qurey = "select Id from TrainingTypes where TrainingName=@TrainingName";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(qurey, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@TrainingName", trainingType);
                    int trainingid = (int)command.ExecuteScalar();
                    return trainingid;
                }
            }
        }

        private int? GetCoachID (string coachName)
        {
            if(coachName==null)
            {
                return null;
            }
            string qurey = "select CoachID from Coaches where CoachName=@CoachName";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command =new SqlCommand(qurey,connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@CoachName", coachName);
                    int coachID= (int)command.ExecuteScalar();
                    return coachID;
                }
            }
        }


        private bool CheckSubscriptionExistUsingSectionID(int clientID, int? sectionID)
        {
            string query = "SELECT COUNT(*) FROM Subscriptions WHERE ClientID = @ClientID AND SectionID = @SectionID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    command.Parameters.AddWithValue("@SectionID", sectionID);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        //
        private void RecordPaymentInfo(int clientID, int? sectionID, int? coachID, int subscriptionTypeID, decimal paymentAmount, decimal remainingPaymentAmount, DateTime paymentDate, int trainingTypeID)
        {
           

            string query = "INSERT INTO PaymentsTable (ClientID,  SectionID, CoachID, SubscriptionTypeID,  PaymentAmount, RemainingPaymentAmount, PaymentDate, TrainingTypeID) " +
                           "VALUES (@ClientId,  @SectionId, @CoachID, @SubscriptionTypeId, @PaymentAmount, @RemainingPaymentAmount, @PaymentDate, @TrainingTypeID)";
            if(sectionID==null && coachID!=null)
            {

                 query = "INSERT INTO PaymentsTable (ClientID, CoachID, SubscriptionTypeID,  PaymentAmount, RemainingPaymentAmount, PaymentDate, TrainingTypeID) " +
                               "VALUES (@ClientId,  @CoachID, @SubscriptionTypeId, @PaymentAmount, @RemainingPaymentAmount, @PaymentDate, @TrainingTypeID)";
            }

            if(sectionID!=null && coachID==null)
            {
                query = "INSERT INTO PaymentsTable (ClientID, SectionID, SubscriptionTypeID,  PaymentAmount, RemainingPaymentAmount, PaymentDate, TrainingTypeID) " +
                                          "VALUES (@ClientId,  @SectionID, @SubscriptionTypeId, @PaymentAmount, @RemainingPaymentAmount, @PaymentDate, @TrainingTypeID)";
            }

            if (sectionID==null && coachID==null)
            {
                query = "INSERT INTO PaymentsTable (ClientID,  SubscriptionTypeID,  PaymentAmount, RemainingPaymentAmount, PaymentDate, TrainingTypeID) " +
                                          "VALUES (@ClientId,  @SubscriptionTypeId, @PaymentAmount, @RemainingPaymentAmount, @PaymentDate, @TrainingTypeID)";
            }


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientID);
                    if (sectionID != null)
                        command.Parameters.AddWithValue("@SectionId", sectionID);
                    if(coachID!=null)
                    command.Parameters.AddWithValue("@CoachID", coachID);
                   
                    command.Parameters.AddWithValue("@SubscriptionTypeId", subscriptionTypeID);
                    command.Parameters.AddWithValue("@PaymentAmount", paymentAmount);
                    command.Parameters.AddWithValue("@RemainingPaymentAmount", remainingPaymentAmount);
                    command.Parameters.AddWithValue("@PaymentDate", paymentDate);
                    command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

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

        private int? GetSectionId(string section)
        {
            if (section == null) return null;
            string query = "SELECT SectionID FROM Sections WHERE SectionName = @SectionName";
           
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SectionName", section);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    
                        return (int?)result;
                    
                }
            }

        }

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



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void addForm_Load(object sender, EventArgs e)
        {

          //  PopulateComboBox("comboBox6","select ClientName from Clients");
            PopulateComboBox("comboBox4","select SectionName from Sections");
            PopulateComboBox("comboBox2","select SubscriptionName from SubscriptionType");
            PopulateComboBox("comboBox5","select TrainingName from TrainingTypes");
            PopulateComboBox("comboBox1","select CoachName from Coaches");

            if (comboBox4.Items.Count > 0)
            {
                comboBox4.SelectedIndex = 0;
            }
            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }
            if (comboBox5.Items.Count > 0)
            {
                comboBox5.SelectedIndex = 0;
            }


        }



        public void PopulateComboBox(string comboBoxName, string query)
        {
         

            // Open the database connection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create a command with the provided query
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Execute the query and get the result
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Clear the ComboBox items to avoid duplicates
                        var comboBox = this.Controls.Find(comboBoxName, true).FirstOrDefault() as System.Windows.Forms.ComboBox; // Specify the correct ComboBox type
                        comboBox.Items.Clear();

                        // Populate the ComboBox with data from the database
                        while (reader.Read())
                        {
                            comboBox.Items.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }

      
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            


        }

        private void addForm_Activated(object sender, EventArgs e)
        {
          
        }

        private void bunifuSeparator1_Load(object sender, EventArgs e)
        {

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox6.SelectedIndex >= 0)
            {
                // Disable the text boxes for client name and client phone number
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
            else
            {

                // Enable the text boxes for client name and client phone number
                textBox1.Enabled = true;
                textBox2.Enabled = true;

            }
        }
    }
}
