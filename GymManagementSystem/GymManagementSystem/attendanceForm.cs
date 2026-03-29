using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;


namespace WindowsFormsApp1
{
    public partial class attendanceForm : Form
    {

        string connectionString;
     
        public attendanceForm(List<string > clientNames)
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
            comboBox1.Items.AddRange(clientNames.ToArray());

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Enabled = false;
            this.Close();
        }

        private void attendanceForm_Load(object sender, EventArgs e)
        {
          //  fillClientsNames();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            
            

        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("اختر مشترك لتسجيل حضوره");
                    return;
                }

                string name = comboBox1.SelectedItem.ToString();
                int clientID = GetClientId(name);
                ShowEndedSubscriptionsMessage(clientID);
                InsertIntoAttendance(clientID, dateTimePicker1.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }






        }

        private void InsertIntoAttendance(int clientID, DateTime attendanceDate)
        {
            string query = "Insert into Attendance (ClientID,AttendanceDate) Values (@ClientID,@AttendanceDate)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    command.Parameters.AddWithValue("@AttendanceDate", attendanceDate);
                    command.ExecuteNonQuery();
                }
            }
            MessageBox.Show("تم تسجيل الحضور بنجاح");
        }

        public class Subscription
        {
            public int ClientID { get; set; }
            public int SectionID { get; set; }
            public int CoachID { get; set; }
            public DateTime SubscriptionDate { get; set; }
            public DateTime SubscriptionEndDate { get; set; }

            public Subscription(int clientID, int sectionID, int coachID, DateTime subscriptionDate, DateTime subscriptionEndDate)
            {
                ClientID = clientID;
                SectionID = sectionID;
                CoachID = coachID;
                SubscriptionDate = subscriptionDate;
                SubscriptionEndDate = subscriptionEndDate;
            }
        }

        //this function takes a client id and returns a list of all of their subscriptions
        public List<Subscription> GetSubscriptionsForClient(int clientID)
        {
            List<Subscription> subscriptions = new List<Subscription>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to select subscriptions for the specified clientID           
                string query = "SELECT ClientID, SectionID, CoachID, SubscriptionDate, SubscriptionEndDate FROM Subscriptions WHERE ClientID = @ClientID And ActiveState = 1";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int clientId = reader.GetInt32(0);
                            int sectionId = 0;
                            int coachId=0;
                            if (!reader.IsDBNull(1))
                            {
                                sectionId = reader.GetInt32(1);
                                // continue with your logic here
                            }
                            if (!reader.IsDBNull(2))
                            {
                                 coachId = reader.GetInt32(2);
                                // continue with your logic here
                            }
                            DateTime subscriptionDate = reader.GetDateTime(3);


                            DateTime subscriptionEndDate = DateTime.MinValue;
                            if (!reader.IsDBNull(4))
                            {
                                 subscriptionEndDate = reader.GetDateTime(4);
                            }
                            Subscription subscription = new Subscription(clientId, sectionId, coachId, subscriptionDate, subscriptionEndDate);
                            subscriptions.Add(subscription);
                        }
                    }
                }
            }

            return subscriptions;
        }

        public void ShowEndedSubscriptionsMessage(int clientID)
        {
            //this list contains all the ended subscription a client has, now I need to filter the private subscriptions
            List<Subscription> endedSubscriptions = CheckEndedSubscriptions(clientID);

            List<Subscription> privateSubscriptions = endedSubscriptions
            .Where(subscription => subscription.SectionID ==0)
            .ToList();

            List<Subscription> generalSubscriptions = endedSubscriptions
            .Where(subscription => subscription.SectionID > 0)
            .ToList();

            string message="";

            if (generalSubscriptions.Count == 1)
            {
                string sectionName = GetSectionName(generalSubscriptions[0].SectionID);
                 message= $"يوجد اشتراك منتهي لهذا المشترك في قسم {sectionName} ";
            }
            else if(generalSubscriptions.Count>0)
            {
                string sections = string.Join(", ", generalSubscriptions.Select(sub => GetSectionName(sub.SectionID)));
                message = $"هذا المشترك لديه {generalSubscriptions.Count} اشتراكات منتهية في الاقسام التالية: {sections} ";
            }

            bool allPrivate=false;
            if(endedSubscriptions.Count==privateSubscriptions.Count)
            {
                allPrivate = true;
            }

            if(privateSubscriptions.Count==1)
            {
                string coachName = GetCoachNameById(privateSubscriptions[0].CoachID);
                message += $"واشتراك منتهي مع المدرب {coachName} ";

                if(allPrivate)
                    message = $"هذا المشترك لديه اشتراك منتهي مع المدرب  {coachName} ";


            }
            if (privateSubscriptions.Count>1)
            {
                string coaches = string.Join(", ", privateSubscriptions.Select(coache => GetCoachNameById(coache.CoachID)));
                message += $"و {privateSubscriptions.Count}اشتراكات برايفت منتهية مع المدربين :{coaches}";
                if(allPrivate)
                {
                    message =$"هذا المشترك لديه {privateSubscriptions.Count}اشتراكات منتهية مع المدربين : {coaches} ";
                }
            }
            if (message == "")
                return;
            MessageBox.Show(message,"تم العثور على اشتراكات منتهية",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }

        //this function uses the get GetSubscriptionsForClient() method to get all the subscriptions, then it filters them and addes the ended ones to a new list
        public List<Subscription> CheckEndedSubscriptions(int clientID)
        {
            // Placeholder for retrieving subscriptions from a data source
            List<Subscription> allSubscriptions = GetSubscriptionsForClient(clientID);
            if(allSubscriptions.Count == 0)
            {
                MessageBox.Show("لا يوجد اي اشتراك فعال لهذا المشترك","",MessageBoxButtons.OK,MessageBoxIcon.Error);
                
            }
            // Filter the list to only include subscriptions that have ended
            List<Subscription> endedSubscriptions = allSubscriptions
                .Where(subscription => subscription.SubscriptionEndDate < DateTime.Now)
                .ToList();

            return endedSubscriptions;
        }

        private string GetSectionName(int sectionID)
        {
            string sectionName;
            string query = "select SectionName from Sections where SectionID=@SectionID";
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@SectionID", sectionID);
                    using (SqlDataReader reader= command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sectionName= reader.GetString(0);
                            return sectionName;
                        }
                    }
                }
            }
            return "";
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

        //fill the combobox
        private void fillClientsNames()
        {
            string query = "SELECT ClientName FROM Clients";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);


                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string ClientName = reader["ClientName"].ToString();
                    comboBox1.Items.Add(ClientName);
                }

                reader.Close();
            }

            // Enable autocomplete for each combobox
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
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

        private string GetCoachNameById(int coachId)
        {
            string query = "SELECT CoachName FROM Coaches WHERE CoachID = @CoachId";
            if (coachId == 0)
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
    }
}
