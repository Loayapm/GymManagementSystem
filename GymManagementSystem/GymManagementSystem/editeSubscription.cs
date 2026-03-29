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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class editeSubscription : Form
    {
        string connectionString;
        public string searchQuery;

        public int clientID = 0;
        public int? sectionID = 0;
        public int? coachID = 0;
        public int trainingTypeID = 0;
        bool foundOne = false;
   

        public editeSubscription(List<string> clientNames)
        {
            InitializeComponent();
            comboBox1.Items.AddRange(clientNames.ToArray());
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled= false;
            this.Close();
        }


        //count the records using the search qurey
        private bool CountSubsRecords()
        {

            int recordsFound = 8;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(searchQuery,connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if (sectionID>0) command.Parameters.AddWithValue("@SectionID", sectionID);
                    if (coachID>0) command.Parameters.AddWithValue("@CoachID", coachID);
                    if (trainingTypeID>0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);
                    recordsFound = (int)command.ExecuteScalar();
                }
            }
            //I need to get the info then fill it to the textBoxes
            if(recordsFound==1) 
            {
                
                foundOne = true;
                SubscriptionInfo subscription = GetSubscriptionInfo();

                // MessageBox.Show($"Coach id is : {subscription.CoachId}"); it is 0
                FillData(subscription);
            }
            else if(recordsFound>1)
            {
                MessageBox.Show("يوجد أكثر من اشتراك بالمعلومات التي اخترتها, جرب تحديد معلومات أكثر أو اختر خيار 'المشترك غير مسجل في أي قسم' اذا كان حقل قسم التسجل فارغاً في سجل الاشتراك", "",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else if(recordsFound==0)
            {
                foundOne = false;
                MessageBox.Show("لا يوجد سجلات بالمعلومات المدخلة","",MessageBoxButtons.OK,MessageBoxIcon.Question);
                //5,3,4,8
                
                comboBox5.SelectedIndex = -1;
                comboBox3.SelectedIndex = -1;
                comboBox8.SelectedIndex = -1;
                comboBox4.SelectedIndex = -1;
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;

                
            }
            return recordsFound > 0;
       }

        //fill the data in the comboboBoxes and text boxes
        private void FillData(SubscriptionInfo subscription)
        {
            if (subscription == null) return;
                
            string clientSection=GetSectionNameByID(subscription.SectionId);
            string coachName = GetCoachNameById(subscription.CoachId);
            string trainingName = GetTrainingTypeById(subscription.TrainingTypeId);
            string subscriptionType = GetSubscriptionTypeNameById(subscription.SubscriptionTypeId);
            //  MessageBox.Show($"Section is {clientSection} and coach is {coachName} and trainngType is{trainingName}");

            SelectItemInComboBox(comboBox5, clientSection);
            SelectItemInComboBox(comboBox3, coachName);
            SelectItemInComboBox(comboBox4, subscriptionType);
            SelectItemInComboBox(comboBox8, trainingName);
            textBox1.Text=subscription.SubscriptionFee.ToString();
            dateTimePicker1.Value = subscription.SubscriptionDate;
            dateTimePicker2.Value = subscription.SubscriptionEndDate;
            textBox2.Text = subscription.subscriptionId.ToString();
            
          


        }

        private static void SelectItemInComboBox(System.Windows.Forms.ComboBox comboBox, string itemText)
        {
            foreach (var item in comboBox.Items)
            {
                if (item.ToString() == itemText)
                {
                    comboBox.SelectedItem = item;
                    break; // Exit the loop once the item is found and selected
                }
            }
        }
        //SectionID is null

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

           try
            {
                int lastIndex = comboBox2.Items.Count - 1;

                if (comboBox2.SelectedIndex==lastIndex)
                    searchQuery += " and SectionID is null";
                else
                {
                    sectionID = GetSectionId(comboBox2.SelectedItem?.ToString());
                    searchQuery += " and SectionID=@SectionID";
                }

              //  MessageBox.Show("the query is:" + searchQuery);

      
                CountSubsRecords();
            }
            catch (ArgumentNullException ex) // Catching ArgumentNullException if comboBox7.SelectedItem is null
            {
                // Handle the case where comboBox7.SelectedItem is null
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidCastException ex) // Catching InvalidCastException if conversion fails
            {
                // Handle the case where conversion fails
                MessageBox.Show($"Error : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }



        public class SubscriptionInfo
        {
            public int SectionId { get; set; }
            public int CoachId { get; set; }
            public int TrainingTypeId { get; set; }
            public int SubscriptionTypeId { get; set; }
            public decimal SubscriptionFee { get; set; }
            public DateTime SubscriptionDate { get; set; }
            public DateTime SubscriptionEndDate { get; set; }
            public int subscriptionId { get; set; }

            // Constructor to initialize the object
            public SubscriptionInfo(int sectionId, int coachId, int trainingTypeId, int subscriptionTypeId, decimal subscriptionFee, DateTime subscriptionDate, DateTime subscriptionEndDate, int subscriptionID)
            {
                SectionId = sectionId;
                CoachId = coachId;
                TrainingTypeId = trainingTypeId;
                SubscriptionTypeId = subscriptionTypeId;
                SubscriptionFee = subscriptionFee;
                SubscriptionDate = subscriptionDate;
                SubscriptionEndDate = subscriptionEndDate;
                subscriptionId = subscriptionID;
            }


        }



        // Method to get subscription info by various possible identifiers
        public SubscriptionInfo GetSubscriptionInfo()
        {
            SubscriptionInfo subscriptionInfo = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT SectionID, CoachID, TrainingTypeID, SubscriptionTypeID, TotalSubscriptionAmount , SubscriptionDate, SubscriptionEndDate, SubscriptionID " +
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
                if(comboBox2.SelectedIndex==items)
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
                  if(sectionID>0)  command.Parameters.AddWithValue("@SectionID", sectionID);
                  if(coachID>0)  command.Parameters.AddWithValue("@CoachID", coachID);
                  if(trainingTypeID>0)  command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            int retrievedSectionId = !reader.IsDBNull(0)? reader.GetInt32(0):0;
                            int retrievedCoachId = !reader.IsDBNull(1) ? reader.GetInt32(1) : 0;
                            int retrievedTrainingTypeId = reader.GetInt32(2);
                            int retrievedSubscriptionTypeId = reader.GetInt32(3);
                            int retrievedSubscriptionFee = reader.GetInt32(4);
                            DateTime retrievedSubscriptionDate = reader.GetDateTime(5);
                            DateTime retrievedSubscriptionEndDate = !reader.IsDBNull(6)?reader.GetDateTime(6): DateTime.Now;
                            int retrievedSubscriptionID=reader.GetInt32(7);

                            subscriptionInfo = new SubscriptionInfo(retrievedSectionId, retrievedCoachId, retrievedTrainingTypeId, retrievedSubscriptionTypeId, retrievedSubscriptionFee, retrievedSubscriptionDate, retrievedSubscriptionEndDate, retrievedSubscriptionID);
                        }
                    }
                }
            }

            return subscriptionInfo;
        }



        private string GetSubscriptionTypeNameById(int subscriptionTypeId)
        {
            if (subscriptionTypeId == 0)
                return string.Empty;
            string query = @"
        SELECT SubscriptionName
        FROM SubscriptionType
        WHERE SubscriptionID = @SubscriptionTypeId";

            string subscriptionTypeName = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubscriptionTypeId", subscriptionTypeId);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        subscriptionTypeName = result.ToString();
                    }
                }
            }

            return subscriptionTypeName;
        }

        //get section name
        private string GetSectionNameByID(int sectionID )
        {
            if (sectionID == 0)
                return string.Empty;
            string query = "Select SectionName from Sections where SectionID=@SectionID";
            string sectionName;

            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@SectionID", sectionID);
                    sectionName = (string)command.ExecuteScalar();
                }
            }
            return sectionName;
        }


        //need this when inserting data
        private int GetSubscriptionTypeId(int clientId, int sectionId)
        {
            string query = @"
        SELECT SubscriptionTypeID
        FROM Subscriptions
        WHERE ClientID = @ClientId AND SectionID = @SectionId";

            int subscriptionTypeId = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@SectionId", sectionId);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        subscriptionTypeId = Convert.ToInt32(result);
                    }
                }
            }

            return subscriptionTypeId;
        }


        //get coach name
        private string GetCoachNameById(int coachId)
        {
            string query = "SELECT CoachName FROM Coaches WHERE CoachID = @CoachId";
            if(coachId==0)
            {
              //  MessageBox.Show("It is 0");
                return string.Empty;
            }
            string coachName = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachId", coachId);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        coachName = result.ToString();
                    }
                }
            }

            return coachName;
        }

        //get training type name
        private string GetTrainingTypeById(int trainingID)
        {
            string query = "SELECT TrainingName FROM TrainingTypes WHERE Id = @Id";
            if (trainingID == 0)
            {
                return string.Empty;
            }
            string trainingName = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", trainingID);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        trainingName = result.ToString();
                    }
                }
            }

            return trainingName;
        }




        //load
        private void editeSubscription_Load(object sender, EventArgs e)
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
                                comboBox2.Items.Add(section);
                                comboBox5.Items.Add(section);

                            }
                        }
                    }
                }

                // Populate the subscription type combobox
                string subscriptionTypeQuery = "SELECT DISTINCT SubscriptionName FROM SubscriptionType"; // Replace YourTableName with the actual table name
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(subscriptionTypeQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string subscriptionType = reader["SubscriptionName"].ToString();
                                comboBox4.Items.Add(subscriptionType);

                            }
                        }
                    }
                }

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
                                comboBox6.Items.Add(coach);
                            }
                        }
                    }
                }

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
                            comboBox7.Items.Add(trainingName);
                            comboBox8.Items.Add(trainingName);
                            
                        }
                    }
                }
            }




            comboBox2.Items.Add("المشترك غير مسجل في أي قسم");

        }


        // Method to update the subscription information in the database
        private bool RecordNewSubscription(int clientId, int? newSectionId,  int ? coachId, int subscriptionTypeId, int subscriptionFee, DateTime subscriptionDate, DateTime SED, int newtraingTypeID , int subscriptionID)
        {
            string query = "SET IDENTITY_INSERT Subscriptions ON INSERT INTO Subscriptions (ClientID, SectionID, CoachID, SubscriptionTypeId, TotalSubscriptionAmount, SubscriptionDate, SubscriptionEndDate, TrainingTypeID, SubscriptionID) VALUES (@ClientId, @NewSectionId, @CoachId, @SubscriptionTypeId, @subscriptionFee, @SubscriptionDate, @SubscriptionEndDate, @TrainingTypeID, @SubscriptionID)";


            if (newSectionId==null && coachId!=null)
             query = "SET IDENTITY_INSERT Subscriptions ON INSERT INTO Subscriptions (ClientID, CoachID, SubscriptionTypeId, TotalSubscriptionAmount, SubscriptionDate, SubscriptionEndDate, TrainingTypeID, SubscriptionID) VALUES (@ClientId, @CoachId, @SubscriptionTypeId, @subscriptionFee, @SubscriptionDate, @SubscriptionEndDate, @TrainingTypeID, @SubscriptionID)";

          if(coachId==null && newSectionId!=null)
            
                query = @"
    SET IDENTITY_INSERT Subscriptions ON 
    INSERT INTO Subscriptions (ClientID, SectionID, SubscriptionTypeId, TotalSubscriptionAmount, SubscriptionDate, SubscriptionEndDate, TrainingTypeID, SubscriptionID) 
    VALUES (@ClientId, @NewSectionId, @SubscriptionTypeId, @subscriptionFee, @SubscriptionDate, @SubscriptionEndDate, @TrainingTypeID, @SubscriptionID)";

            

          if(newSectionId==null && coachId==null)

              query=  @"
    SET IDENTITY_INSERT Subscriptions ON 
    INSERT INTO Subscriptions (ClientID, SubscriptionTypeId, TotalSubscriptionAmount, SubscriptionDate, SubscriptionEndDate, TrainingTypeID, SubscriptionID) 
    VALUES (@ClientId, @SubscriptionTypeId, @subscriptionFee, @SubscriptionDate, @SubscriptionEndDate, @TrainingTypeID, @SubscriptionID)";
            //  string connectionString = "Data Source=DESKTOP-4J8VJT3;Initial Catalog=TempDBforCST;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientId", clientId);
                  //  command.Parameters.AddWithValue("@OldSectionId", oldSectionId);
                  if(newSectionId!=null)
                    command.Parameters.AddWithValue("@NewSectionId", newSectionId);

                  if(coachId!=null)
                    command.Parameters.AddWithValue("@CoachId", coachId);
                    command.Parameters.AddWithValue("@SubscriptionTypeId", subscriptionTypeId);
                    command.Parameters.AddWithValue("@subscriptionFee", subscriptionFee);
                    command.Parameters.AddWithValue("@SubscriptionDate", subscriptionDate);
                    command.Parameters.AddWithValue("@SubscriptionEndDate", SED);
                    command.Parameters.AddWithValue("@TrainingTypeID", newtraingTypeID);
                    command.Parameters.AddWithValue("@SubscriptionID", subscriptionID);



                    int rowCount = command.ExecuteNonQuery();

                    return rowCount > 0;
                }
            }
        }

        private void MaakeTheSEDNull(int subscriptionID)
        {
            
            string query = "update Subscriptions set SubscriptionEndDate=null where SubscriptionID=@SubscriptionID";
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand comman= new SqlCommand(query,connection))
                {
                    comman.Parameters.AddWithValue("@SubscriptionID", subscriptionID);
                    comman.ExecuteNonQuery();
                }
            }
        }

        //insert
        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(
               "هل تريد حفظ التغييرات؟",
               "confirm",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }

                if (!foundOne)
                {
                    MessageBox.Show(
                        "لم يتم العثور على أي اشتراك بعد, لن يتم حفظ التغييرات",
                        "",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (comboBox4.SelectedIndex == -1 || comboBox8.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "يجب تحديد نوع الاشتراك ونوع التدريب قبل حفظ التغييرات");
                    return;
                }

                if(comboBox1.SelectedIndex==-1)
                {
                    MessageBox.Show("لم يتم اختيار مشترك"); 
                    return;
                }

                // Get the selected client ID
                int clientId = GetClientId(comboBox1.SelectedItem.ToString());

                // Get the selected section IDs
                int? newSectionId = GetSectionId(comboBox5.SelectedItem?.ToString());

                int subscriptionFee = 0;
                if (!int.TryParse(textBox1.Text, out subscriptionFee) || subscriptionFee < 1000)
                {
                    MessageBox.Show(
                        "قيمة الاشتراك غير صالحة, لم يتم حفظ التغييرات",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Get the selected coach ID
                int? newcoachId = GetCoachId(comboBox3.SelectedItem?.ToString());

                // Get the selected subscription type ID
                int subscriptionTypeId = GetSubscriptionTypeId(comboBox4.SelectedItem.ToString());

                // Get the new training id
                int newTrainingTypeID = GetTrainingTypeID(comboBox8.SelectedItem.ToString());

                // Check if the subscription already exists
                int subscriptionID;
                if (!int.TryParse(textBox2.Text, out subscriptionID))
                {
                    MessageBox.Show(
                        "لم يتم العثور على اشتراك بعد",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (foundOne == false)
                {
                    MessageBox.Show(
                        "لم يتم العثور على اشتراك بعد",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Check if the new subscription already exists
                if (CheckSubscriptionExists(clientId, newSectionId, newcoachId, newTrainingTypeID, subscriptionID))
                {
                    MessageBox.Show(
                        "يوجد اشتراك بنفس المعلومات الجديدة",
                        "لا يمكن حفظ التغييرات");
                    return;
                }

                DeleteOldRecord();

                DateTime sd = dateTimePicker1.Value;
                DateTime sed = dateTimePicker2.Value;
               

                // Update the subscription information in the database
                bool updateSuccessful = RecordNewSubscription(clientId, newSectionId, newcoachId, subscriptionTypeId, subscriptionFee, sd, sed, newTrainingTypeID, subscriptionID);
                if(sed.Date==DateTime.Today.Date)
                MaakeTheSEDNull(subscriptionID);
                // Display appropriate message based on the update result
                if (updateSuccessful)
                {
                    // Update successful
                    MessageBox.Show(
                        "تم تحديث معلومات الاشتراك بنجاح",
                        "Update Status",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    // Update failed
                    MessageBox.Show(
                        "لم يتم العثور على اشتراك بالمعلومات المدخلة",
                        "خطأ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                } 
            }
            catch (Exception ex)
            {
                MessageBox.Show("an Error accoured : " + ex.Message);
            }
           
        }

        //fixed for null coachID
        private bool CheckSubscriptionExists(int clientId, int ? newSectionId, int ?  coachId, int  trainngTypeId, int subscriptionID)
        {
            string query = "Select count (*) from Subscriptions where ClientID=@ClientID and TrainingTypeID=@TrainingTypeID AND SubscriptionID <> @SubscriptionID";
            if (newSectionId != null)
                query += " and SectionID=@SectionID";
            if (coachId != null)
                query += " and CoachID=@CoachID";

            int rowsFound = 0;
            using (SqlConnection connection=new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientId);
                    if(coachId != null)
                    command.Parameters.AddWithValue("@CoachID", coachId);
                    command.Parameters.AddWithValue("@TrainingTypeID", trainngTypeId);
                    if(newSectionId!= null)
                    command.Parameters.AddWithValue("@SectionID", newSectionId);
                    command.Parameters.AddWithValue("@SubscriptionID", subscriptionID);

                    rowsFound = (int)command.ExecuteScalar();
                }
            }
            return rowsFound > 0;
        }

        private void DeleteOldRecord()
        {
            string query = "Delete from Subscriptions where ClientID=@ClientID";
            if (sectionID > 0) query += " and SectionID=@SectionID";
            if (coachID > 0) query += " and CoachID=@CoachID";
            if (trainingTypeID > 0) query += " and TrainingTypeID=@TrainingTypeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if (sectionID > 0) command.Parameters.AddWithValue("@SectionID", sectionID);
                    if (coachID > 0) command.Parameters.AddWithValue("@CoachID", coachID);
                    if (trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);
                    command.ExecuteNonQuery();
                }
            }
          
        }

        //need this when inserting data and when finding records
        private int? GetSectionId(string sectionName)
        {
            if (sectionName == null)
                return null;
            string query = "SELECT SectionID FROM Sections WHERE SectionName = @SectionName";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SectionName", sectionName);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return -1; // Return -1 if section ID is not found or an error occurs
        }


        //need this when inserting data
        private int? GetCoachId(string coachName)
        {
            if(coachName==null)
            {
                return null;
            }
            string query = "SELECT CoachID FROM Coaches WHERE CoachName = @CoachName";

          //  string connectionString = "Data Source=DESKTOP-4J8VJT3;Initial Catalog=TempDBforCST;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachName", coachName);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return -1; // Return -1 if coach ID is not found or an error occurs
        }

        //need this when inserting data
        private int GetSubscriptionTypeId(string subscriptionTypeName)
        {
            string query = "SELECT SubscriptionID FROM SubscriptionType WHERE SubscriptionName = @SubscriptionTypeName";

          //  string connectionString = "Data Source=DESKTOP-4J8VJT3;Initial Catalog=TempDBforCST;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SubscriptionTypeName", subscriptionTypeName);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return -1; // Return -1 if subscription type ID is not found or an error occurs
        }


        private int GetClientId(string clientName)
        {
            string query = "SELECT ClientID FROM Clients WHERE ClientName = @ClientName";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientName", clientName);

                    return (int)command.ExecuteScalar();
                }
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

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                searchQuery += " and CoachID=@CoachID";
                coachID = GetCoachId(comboBox6.Text.ToString());
                //  MessageBox.Show($"Coach id is: {coachID}");
                CountSubsRecords();
            }
            catch (ArgumentNullException ex) // Catching ArgumentNullException if comboBox7.SelectedItem is null
            {
                // Handle the case where comboBox7.SelectedItem is null
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidCastException ex) // Catching InvalidCastException if conversion fails
            {
                // Handle the case where conversion fails
                MessageBox.Show($"Error : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                searchQuery += " and TrainingTypeID=@TrainingTypeID";
                trainingTypeID = GetTrainingTypeID(comboBox7.SelectedItem.ToString());
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                clientID = GetClientId(comboBox1.SelectedItem.ToString());
                searchQuery = "Select count (*) from Subscriptions where ClientID=@ClientID";
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
