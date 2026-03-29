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
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WindowsFormsApp1.editeSubscription;

namespace WindowsFormsApp1
{
    public partial class removeSubscription : Form
    {
        public string searchQuery;

        public int clientID = 0;
        public int? sectionID = 0;
        public int coachID = 0;
        public int trainingTypeID = 0;
        bool foundOne = false;
        Client clientToDelete = new Client();
        const string connectionString = "data source=DESKTOP-T1STQLB;initial catalog=Database1;trusted_connection=true";
        public removeSubscription(List <string> ClientNames)
        {
            InitializeComponent();
            comboBox1.Items.AddRange(ClientNames.ToArray());
        }

        private void removeForm_Load(object sender, EventArgs e)
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
                    //    AutoCompleteStringCollection autoCompleteCollection3 = new AutoCompleteStringCollection();

                        while (reader.Read())
                        {
                            string sectionName = reader["SectionName"].ToString();
                            comboBox3.Items.Add(sectionName);
                          //  autoCompleteCollection3.Add(sectionName);
                        }

                
                    }
                }
            }
            comboBox3.Items.Add("المشترك غير مسجل في اي قسم");


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string clientQuery = "SELECT TrainingName FROM TrainingTypes";
                using (SqlCommand command = new SqlCommand(clientQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        AutoCompleteStringCollection autoCompleteCollection1 = new AutoCompleteStringCollection();

                        while (reader.Read())
                        {
                            string clientName = reader["TrainingName"].ToString();
                            comboBox2.Items.Add(clientName);
                            autoCompleteCollection1.Add(clientName);
                        }

                      
                    }
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string clientQuery = "SELECT DISTINCT CoachName FROM Coaches";
                using (SqlCommand command = new SqlCommand(clientQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        AutoCompleteStringCollection autoCompleteCollection1 = new AutoCompleteStringCollection();

                        while (reader.Read())
                        {
                            string clientName = reader["CoachName"].ToString();
                            comboBox4.Items.Add(clientName);
                            autoCompleteCollection1.Add(clientName);
                        }

                       
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
         //   clientToDelete.name=comboBox1.SelectedItem.ToString();
            clientID = GetClientId(comboBox1.SelectedItem.ToString());
            searchQuery = "Select count (*) from Subscriptions where ClientID=@ClientID";
            CountSubsRecords();

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
            if (recordsFound == 1)
            {

                foundOne = true;
                MessageBox.Show("يمكنك المتابعة لحذف الاشتراك","تم العثور على الاشتراك",MessageBoxButtons.OK,MessageBoxIcon.Question);
            
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
                //5,3,4,8

           


            }
            return recordsFound > 0;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!foundOne)
                {
                    MessageBox.Show("لم يتم العثور على اشتراك بعد");
                    return;
                }
                DialogResult result = MessageBox.Show("هل تريد نأكيد حذف الاشتراك؟", "confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
                string deleteQuery = "Delete from Subscriptions where ClientID=@ClientID";
                if (sectionID > 0) deleteQuery += " and SectionID=@SectionID";
                if (coachID > 0) deleteQuery += " and CoachID=@CoachID";
                if (trainingTypeID > 0) deleteQuery += " and TrainingTypeID=@TrainingTypeID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ClientID", clientID);
                        if (sectionID > 0) command.Parameters.AddWithValue("@SectionID", sectionID);
                        if (coachID > 0) command.Parameters.AddWithValue("@CoachID", coachID);
                        if (trainingTypeID > 0) command.Parameters.AddWithValue("@TrainingTypeID", trainingTypeID);
                        command.ExecuteNonQuery();
                        MessageBox.Show("تم حذف الاشتراك بنجاح");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }



        }




        private int GetSTypeID(string trainingType)
        {
           
            if(trainingType==null)
            {
                return 0;
            }
            string query = "SELECT Id FROM TrainingTypes WHERE TrainingName = @TrainingName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TrainingName", trainingType);

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

            return 0; // Return a default value if the section ID is not found
        }


        private int GetSectionId(string section)
        {
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int lastIndex = comboBox3.Items.Count - 1;

            if (comboBox3.SelectedIndex == lastIndex)
                searchQuery += " and SectionID is null";
            else
            {
                sectionID = GetSectionId(comboBox3.SelectedItem?.ToString());
                searchQuery += " and SectionID=@SectionID";
            }

            //  MessageBox.Show("the query is:" + searchQuery);


            CountSubsRecords();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchQuery += " and TrainingTypeID=@TrainingTypeID";
            trainingTypeID = GetSTypeID(comboBox2.SelectedItem.ToString());
            CountSubsRecords();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchQuery += " and CoachID=@CoachID";
            coachID = GetCoachId(comboBox4.Text.ToString());
            //  MessageBox.Show($"Coach id is: {coachID}");
            CountSubsRecords();
        }
    }
}
