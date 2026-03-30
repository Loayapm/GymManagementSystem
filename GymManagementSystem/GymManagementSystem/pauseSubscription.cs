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
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WindowsFormsApp1.editeSubscription;

namespace WindowsFormsApp1
{
    public partial class pauseSubscription : Form
    {
        string connectionString;// = "data source=DESKTOP-T1STQLB;initial catalog=Database1;trusted_connection=true";
        public string searchQuery = "Select count (*) from Subscriptions where 1=1";

        public int clientID = 0;
        public int? sectionID = 0;
        public int coachID = 0;
        public int trainingTypeID = 0;
        bool foundOne = false;
        
        
        public pauseSubscription(List<string > clientNames)
        {
            InitializeComponent();
            comboBox1.Items.AddRange(clientNames.ToArray());
            connectionString = DatabaseConnections.GymDB;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Enabled= false;
            this.Close();
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {

          try
            {
                //Deactivate subscriptions
                if (comboBox3.SelectedIndex == 0)
                {
                    DialogResult = MessageBox.Show("هل تريد تأكيد تجميد الاشتراك؟", "confirm", MessageBoxButtons.YesNo);
                    if (!(DialogResult == DialogResult.Yes))
                    {
                        return;
                    }
                    if (foundOne == false)
                    {
                        MessageBox.Show("لم يتم العثور على الاشتراك بعد", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }






                    DateTime PauseDate = dateTimePicker1.Value;
                    string pauseQurey = "update Subscriptions set ActiveState=0 , PauseDate=@PauseDate where ClientID=@ClientID AND ActiveState=1";
                    if (sectionID > 0) pauseQurey += " and SectionID=@SectionID";
                    if (comboBox2.SelectedIndex == comboBox2.Items.Count - 1) pauseQurey += " and SectionID is null";
                    if (coachID > 0) pauseQurey += " And CoachID=@CoachID";
                    if (trainingTypeID > 0) pauseQurey += " and TrainingTypeID=@TrainingTypeID";
                    int rowsAffected = 0;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(pauseQurey, connection))
                        {
                            command.Parameters.AddWithValue("@ClientID", clientID);
                            if (sectionID > 0)
                                command.Parameters.AddWithValue("@SectionID", sectionID);
                            if (coachID > 0)
                                command.Parameters.AddWithValue("@CoachID", coachID);
                            if (trainingTypeID > 0)
                                command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);


                            command.Parameters.AddWithValue("@PauseDate", PauseDate);
                            rowsAffected = command.ExecuteNonQuery();
                        }
                        connection.Close();

                    }
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم تجميد الاشتراك بنجاح");
                        InsertIntoPaused();

                    }
                    else
                    {
                        MessageBox.Show("لم يتم العثور على اشنراك فعّال بالمعلومات المدخلة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                //activate subscription
                else if (comboBox3.SelectedIndex == 1)
                {
                    DialogResult = MessageBox.Show("هل تريد تأكيد تفعيل الاشتراك؟", "confirm", MessageBoxButtons.YesNo);
                    if (!(DialogResult == DialogResult.Yes))
                    {
                        return;
                    }
                    if (foundOne == false)
                    {
                        MessageBox.Show("لم يتم العثور على الاشتراك بعد", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    //  string name = comboBox1.SelectedItem.ToString();
                    //  string section = comboBox2.SelectedItem.ToString();

                    int rowsAffected = 0;
                    // int clientID = GetClientId(name);
                    //  int sectionId = GetSectionId(section);


                    //set active sate to 1 in subscriptions
                    string activeateQuery = "update Subscriptions set ActiveState=1 , PauseDate=NULL where ClientID=@ClientID and ActiveState = 0";
                    if (sectionID > 0) activeateQuery += " and SectionID=@SectionID";
                    if (comboBox2.SelectedIndex == comboBox2.Items.Count - 1) activeateQuery += " and SectionID is null";
                    if (coachID > 0) activeateQuery += " And CoachID=@CoachID";
                    if (trainingTypeID > 0) activeateQuery += " and TrainingTypeID=@TrainingTypeID";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(activeateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@ClientID", clientID);
                            if (sectionID > 0) command.Parameters.AddWithValue("@SectionID", sectionID);
                            if (coachID > 0) command.Parameters.AddWithValue("@CoachID", coachID);
                            if (trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                            rowsAffected = command.ExecuteNonQuery();
                        }
                        connection.Close();

                    }
                    // MessageBox.Show($"{rowsAffected} row/rows affected");
                    if (rowsAffected > 0)
                    {
                        //I activated the subscripiton in subscriptoins table, Now I need to calculate if the pause days >45 days
                        Subscription subscriptionToActivate = GetSubscripitonInfo();
                        // MessageBox.Show($"SInfo: {coachID}, {subscriptionToActivate.sectionID}, {subscriptionToActivate.coachID}, {subscriptionToActivate.trainingTypeID}");
                        DateTime pauseDate = GetSubscriptionPauseDate(subscriptionToActivate);
                        TimeSpan timeSpan = DateTime.Today - pauseDate;
                        double daysFromPause = timeSpan.TotalDays;

                        //now I have the pause days, I need to know if pausDays>45 

                        //  MessageBox.Show($"Pause date: {pauseDate} and DaysFromPause: {daysFromPause}");



                        if (daysFromPause <= 45)
                        {
                            int remainingSubscriptionDays = GetRemainingDays(subscriptionToActivate);
                            if (remainingSubscriptionDays > 0)
                            {
                                UpdateSED(remainingSubscriptionDays, subscriptionToActivate);
                                DeletePausedSubscription(subscriptionToActivate);
                                MessageBox.Show($"تم تفعيل الاشتراك بنجاح, فترة تجميد الاشتراك أقل من شهر ونصف لذلك تمت استعادة {remainingSubscriptionDays} من ايام الاشتراك");
                            }
                            else
                            {
                                UpdateSED(0, subscriptionToActivate);
                                DeletePausedSubscription(subscriptionToActivate);
                                MessageBox.Show("تم تفعيل الاشتراك بنجاح");
                            }
                        }
                        else if (daysFromPause > 45)
                        {
                            UpdateSED(0, subscriptionToActivate);
                            DeletePausedSubscription(subscriptionToActivate);
                            MessageBox.Show("تم تفعيل الاشتراك بنجاح, فترة تجميد الاشتراك اكثر من شهر ونصف");
                        }

                    }
                    else
                    {
                        MessageBox.Show("لم يتم العثور على اشنراك متوقف بالمعلومات المدخلة ", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                searchQuery = "Select count (*) from Subscriptions where 1=1";
                FillDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        public void DeletePausedSubscription(Subscription subscription)
        {
            string query = "DELETE FROM PausedSubscriptions WHERE ClientID = @ClientID";
            if (subscription.sectionID > 0) query += " and SectionID=@SectionID";
            if (subscription.coachID > 0) query += " And CoachID=@CoachID";
            if (subscription.trainingTypeID > 0) query += " and TrainingTypeID=@TrainingTypeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if (subscription.sectionID > 0) command.Parameters.AddWithValue("@SectionID", subscription.sectionID);
                    if (subscription.coachID > 0) command.Parameters.AddWithValue("@CoachID", subscription.coachID);
                    if (subscription.trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", subscription.trainingTypeID);


                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateSED(int days, Subscription subscription)
        {
            DateTime newDate = DateTime.Today.AddDays(days);
            string query = "update Subscriptions set SubscriptionEndDate=@NewDate where ClientID=@ClientID";
            if (subscription.sectionID > 0) query += " and SectionID=@SectionID";
            if (subscription.coachID > 0) query += " And CoachID=@CoachID";
            if (subscription.trainingTypeID > 0) query += " and TrainingTypeID=@TrainingTypeID";

            using (SqlConnection connection=new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command=new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@NewDate",newDate);
                    command.Parameters.AddWithValue("@ClientID",clientID);

                    if (subscription.sectionID > 0) command.Parameters.AddWithValue("@SectionID", subscription.sectionID);
                    if (subscription.coachID > 0) command.Parameters.AddWithValue("@CoachID", subscription.coachID);
                    if (subscription.trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", subscription.trainingTypeID);


                    command.ExecuteNonQuery();

                }
            }
        }

        private DateTime GetSubscriptionPauseDate(Subscription subscription)
        {
            string query = @"
        SELECT PauseDate
        FROM PausedSubscriptions
        WHERE ClientID = @ClientID";

            if (subscription.sectionID > 0) query += " and SectionID=@SectionID";
            if (subscription.coachID > 0) query += " And CoachID=@CoachID";
            if (subscription.trainingTypeID > 0) query += " and TrainingTypeID=@TrainingTypeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if(subscription.sectionID>0)  command.Parameters.AddWithValue("@SectionID", subscription.sectionID);
                    if(subscription.coachID>0)  command.Parameters.AddWithValue("@CoachID", subscription.coachID);
                    if(subscription.trainingTypeID>0)  command.Parameters.AddWithValue("@TrainingTypeID", subscription.trainingTypeID);


                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDateTime(result);
                    }
                }
            }

            return DateTime.MinValue; // Return null if the subscription end date is not found
        }

        private int GetRemainingDays(Subscription subscription)
        {
            string query = @"
        SELECT RemainingSubscriptionDays
        FROM PausedSubscriptions
        WHERE ClientID = @ClientID";

            if (subscription.sectionID > 0) query += " and SectionID=@SectionID";
            if (subscription.coachID > 0) query += " And CoachID=@CoachID";
            if (subscription.trainingTypeID > 0) query += " and TrainingTypeID=@TrainingTypeID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if (subscription.sectionID > 0) command.Parameters.AddWithValue("@SectionID", subscription.sectionID);
                    if (subscription.coachID > 0) command.Parameters.AddWithValue("@CoachID", subscription.coachID);
                    if (subscription.trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", subscription.trainingTypeID);



                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }

            return 0; // Return null if the subscription end date is not found
        }



        private int GetSectionId(string section)
        {
            string query = "SELECT SectionID FROM Sections WHERE SectionName = @SectionName";

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

        private string GetInsertQuery(Subscription subscription)
        {
            StringBuilder queryBuilder = new StringBuilder("INSERT INTO PausedSubscriptions (ClientID , PauseDate, RemainingSubscriptionDays");

            if (subscription.sectionID > 0)
            {
                queryBuilder.Append(", SectionID");
            }

            if (subscription.coachID > 0)
            {
                queryBuilder.Append(", CoachID");
            }

            if (subscription.trainingTypeID > 0)
            {
                queryBuilder.Append(", TrainingTypeID");
            }

            // Close the query with the VALUES part
            queryBuilder.AppendLine(") VALUES (@ClientID , @PauseDate, @RemainingSubscriptionDays");

            // Add conditions for optional parameters
            if (subscription.sectionID > 0)
            {
                queryBuilder.AppendLine(", @SectionID");
            }
            if (subscription.coachID > 0)
            {
                queryBuilder.AppendLine(", @CoachID");
            }
            if (subscription.trainingTypeID > 0)
            {
                queryBuilder.AppendLine(", @TrainingTypeID");
            }

            queryBuilder.AppendLine(");"); // Close the query

            string query2 = queryBuilder.ToString();
            return query2;

        }

        public  class Subscription
        {
            public int sectionID { get; set; }
            public int coachID { get; set; }
            public int trainingTypeID { get; set; }
            public  Subscription(int sectionID, int coachID, int trainingTypeID)
            {
                this.sectionID = sectionID;
                this.coachID = coachID;
                this.trainingTypeID = trainingTypeID;
            }
        }

        private Subscription GetSubscripitonInfo()
        {
            string query = "select SectionID, CoachID, TrainingTypeID from Subscriptions where ClientID=@ClientID";
            if (sectionID > 0) query += " and SectionID=@SectionID";
            if (comboBox2.SelectedIndex == comboBox2.Items.Count - 1) query += " and SectionID is null";
            if (coachID > 0) query += " And CoachID=@CoachID";
            if (trainingTypeID > 0) query += " and TrainingTypeID=@TrainingTypeID";

            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if (sectionID > 0) command.Parameters.AddWithValue("@SectionID", sectionID);
                    if (coachID > 0) command.Parameters.AddWithValue("@CoachID", coachID);
                    if (trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            int retrievedSectionId = !reader.IsDBNull(0) ? reader.GetInt32(0) : 0;
                            int retrievedCoachId = !reader.IsDBNull(1) ? reader.GetInt32(1) : 0;
                            int retrievedTrainingTypeId = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0;
                          

                            Subscription subscription = new Subscription(retrievedSectionId, retrievedCoachId, retrievedTrainingTypeId);
                            return subscription;
                        }
                       
                    }
                }

            }
            Subscription s2 = new Subscription(0, 0, 0);
            return s2;


        }

        private void InsertIntoPaused()
        {
            //needs edit
            DateTime  SED = GetSubscriptionEndDate();
            TimeSpan timeSpan = SED - DateTime.Today;
            double remainingSubscriptionDate = timeSpan.TotalDays;


            if(remainingSubscriptionDate < 0)
            {
                remainingSubscriptionDate = 0;
            }
            // MessageBox.Show("remaining days :" + remainingSubscriptionDate);

            //needs edit
            //  string query = "insert into PausedSubscriptions (ClientID,SectionID,PauseDate,RemainingSubscriptionDays) values (@ClientID,@SectionID,@PauseDate,@RemainingSubscriptionDays)";

           
            Subscription subscription = GetSubscripitonInfo();
            string query2 = GetInsertQuery(subscription);

          //  MessageBox.Show(query2);
          //  MessageBox.Show(subscription.sectionID + " " + subscription.coachID + " " + subscription.trainingTypeID);
         //   return;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query2,connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    if(subscription.sectionID>0) command.Parameters.AddWithValue("@SectionID", subscription.sectionID);
                    command.Parameters.AddWithValue("@PauseDate", dateTimePicker1.Value);
                    command.Parameters.AddWithValue("@RemainingSubscriptionDays", remainingSubscriptionDate);
                    if (subscription.coachID > 0) command.Parameters.AddWithValue("@CoachID", subscription.coachID);
                    if (subscription.trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", subscription.trainingTypeID);


                    command.ExecuteNonQuery();

                }
                connection.Close();
            }
        }

        private DateTime GetSubscriptionEndDate()
        {
            string query = @"
        SELECT SubscriptionEndDate
        FROM Subscriptions
        WHERE ClientID = @ClientID";

            if (sectionID > 0) query += " and SectionID=@SectionID";
            if (coachID > 0) query += " and CoachID=@CoachID";
            if (trainingTypeID > 0) query += " and TrainingTypeID=@TrainingTypeID";


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
                        return Convert.ToDateTime(result);
                    }
                }
            }

            return DateTime.MinValue; // Return null if the subscription end date is not found  تم العثور
        }




        private int GetClientId(string clientName)
        {
            string query = "SELECT ClientID FROM Clients WHERE ClientName = @ClientName";

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
        private void fillCoachesNames()
        {
            string query = "SELECT CoachName FROM Coaches";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string coachName = reader["CoachName"].ToString();
                        comboBox4.Items.Add(coachName);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading coach names: " + ex.Message);
                }
            }
        }

        private void fillTrainingTypes()
        {
            string query = "SELECT TrainingName FROM TrainingTypes";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string TrainingName = reader["TrainingName"].ToString();
                        comboBox5.Items.Add(TrainingName);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading training types : " + ex.Message);
                }
            }
        }

        private void pauseSubscription_Load(object sender, EventArgs e)
        {
            fillCoachesNames();
            fillTrainingTypes();
       

            string query3 = "SELECT SectionName FROM Sections"; 

            using (SqlConnection connection2 = new SqlConnection(connectionString))
            {
                connection2.Open();

                SqlCommand command = new SqlCommand(query3, connection2);
                SqlDataReader reader = command.ExecuteReader();

                AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();

                while (reader.Read())
                {
                    string clientName = reader["SectionName"].ToString();
                    comboBox2.Items.Add(clientName);
                    autoCompleteCollection.Add(clientName);
                }
                reader.Close();

            }

            comboBox2.Items.Add("المشترك غير مسجل في أي قسم");
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

        private void FillDataGridView()
        {
            string query = @"
        SELECT 
            Clients.ClientName as 'اسم المشترك', 
            Sections.SectionName as 'القسم', 
            Coaches.CoachName as 'المدرب المسؤول',
            TrainingTypes.TrainingName as 'نوع التدريب',
            PausedSubscriptions.PauseDate as 'تاريخ ايقاف الاشتراك', 
            PausedSubscriptions.RemainingSubscriptionDays as 'الايام المتبقية من الاشتراك'
        FROM 
            PausedSubscriptions
        INNER JOIN 
            Clients ON PausedSubscriptions.ClientID = Clients.ClientID
        left JOIN 
            Sections ON PausedSubscriptions.SectionID = Sections.SectionID
         left join
            Coaches on PausedSubscriptions.CoachID=Coaches.CoachID
          left join
            TrainingTypes on PausedSubscriptions.TrainingTypeID=TrainingTypes.Id
          ORDER BY 
    PausedSubscriptions.PauseDate DESC; 
    ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
        }

        private bool CountSubsRecords()
        {

            int recordsFound = 0;
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

                MessageBox.Show("تم العثور على الاشتراك, يمكنك اختيار الاجراء المطلوب لتغيير حالة الاشتراك الآن");
            }
            else if (recordsFound > 1)
            {
                foundOne = false;
                MessageBox.Show("يوجد أكثر من اشتراك بالمعلومات التي اخترتها, جرب تحديد معلومات أكثر أو اختر خيار 'المشترك غير مسجل في أي قسم' اذا كان حقل قسم التسجل فارغاً في سجل الاشتراك", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (recordsFound == 0)
            {
                foundOne = false;
                MessageBox.Show("لا يوجد سجلات بالمعلومات المدخلة", "", MessageBoxButtons.OK, MessageBoxIcon.Question);
               
            }
            return recordsFound > 0;
        }


        //section comboBox3
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int lastIndex = comboBox2.Items.Count - 1;

                if (comboBox2.SelectedIndex == lastIndex)
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pauseSubscription_Activated(object sender, EventArgs e)
        {
            FillDataGridView();
        }

        private int GetCoachId(string coachName)
        {
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

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                searchQuery += " and CoachID=@CoachID";
                coachID = GetCoachId(comboBox4.Text.ToString());
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

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                searchQuery += " and TrainingTypeID=@TrainingTypeID";
                trainingTypeID = GetTrainingTypeID(comboBox5.SelectedItem.ToString());
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

        private void comboBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

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
