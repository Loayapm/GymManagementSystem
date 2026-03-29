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
    public partial class editSubscriptions : Form
    {
        string connectionString;
        public editSubscriptions()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }



        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == -1 && comboBox2.SelectedIndex == -1 && comboBox3.SelectedIndex == -1)
                {
                    MessageBox.Show("اختر احدى معلّمات الاشتراكات");
                    return;
                }

                if (comboBox4.SelectedIndex == -1)
                {
                    MessageBox.Show("تأكد من اختيار المدرب الجديد", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string oldCoach = comboBox1.SelectedItem?.ToString();
                string section = comboBox2.SelectedItem?.ToString();
                string trainingType = comboBox3.SelectedItem?.ToString().Trim();
                string newCoach = comboBox4.SelectedItem?.ToString();
                string message = ":" + "هل تريد تأكيد نقل جميع المشتركين الذين معلومات اشتراكاتهم كالتالي" + "\n";


                // string message = $"هل تريد تأكيد نقل جميع المشتركين المسجلين ";
                if (section != null)
                {
                    message += $"المسجلين في القسم : {section} \n";
                }

                if (oldCoach != null)
                {
                    message += $"المدرب المسؤال عنهم : {oldCoach} \n";
                }


                if (trainingType != null)
                {
                    message += $"والذين نوع تدريبهم : {trainingType} \n";
                }

                message += $"الى المدرب الجديد: {newCoach}";


                DialogResult result = MessageBox.Show(message, "تأكيد النقل", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }

                MoveSubscriptionsToNewCoach(oldCoach, section, trainingType, newCoach);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }



        }

        public void FillComboBoxWithTableNames(ComboBox comboBoxName, string query,string tableName)
        {
            // Assuming connectionString is accessible in this context
          //  string connectionString = "YourConnectionStringHere"; // Replace with your actual connection string

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Constructing the query to select all unique column names from the specified table
                  //  string query = $"SELECT DISTINCT ClientName FROM {tableName}";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Assuming you want to display column names; adjust as needed
                                comboBoxName.Items.Add(reader[tableName].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here, e.g., log the error
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void MoveSubscriptionsToNewCoach(string oldCoach, string sectionName, string trainingType, string newCoach)
        {
           


            string query = "update Subscriptions set CoachID=@CoachID where 1=1";


            int sectionID=0;
            int trainingID=0;
            int oldCoachID = 0;
            
            if(oldCoach!=null)
            {
                oldCoachID = getID("CoachID", "Coaches", "CoachName", oldCoach);
                query += " and OldCoachID=@OldCoachID";

            }


            if (sectionName!=null)
            {
                 sectionID = getID("SectionID", "Sections", "SectionName", sectionName);
                query += " and SectionID=@SectionID";
            }
            if(trainingType!=null)
            {
                trainingID = getID("Id", "TrainingTypes", "TrainingName", trainingType);
                query += " and TrainingTypeID=@TrainingTypeID";
            }
            //new coach cannot be null
            int newCoachID = getID("CoachID", "Coaches", "CoachName", newCoach);


            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachID", newCoachID); // Use newCoachID instead of newCoach
                    if(oldCoach!=null) command.Parameters.AddWithValue("@OldCoachID", oldCoachID);
                    if (sectionName != null) command.Parameters.AddWithValue("@SectionID", sectionID);
                    if (trainingType != null) command.Parameters.AddWithValue("@TrainingTypeID", trainingID);
                    command.ExecuteNonQuery();
                }
            }
            MessageBox.Show($"تم نقل المشتركين الى المدرب {newCoach}");

        }

        private int getID(string str1, string str2, string str3, string name)
        {
            string query = $"select {str1} from {str2} where {str3}=@value";
            int id;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command=new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@value", name);
                    id =(int) command.ExecuteScalar();
                }
            }

            return id;

        }

        private void editSubscriptions_Load(object sender, EventArgs e)
        {
            FillComboBoxWithTableNames(comboBox1, "Select CoachName from Coaches","CoachName");
            FillComboBoxWithTableNames(comboBox2, "Select SectionName from Sections","SectionName");
            FillComboBoxWithTableNames(comboBox3, "Select TrainingName from TrainingTypes","TrainingName");
            FillComboBoxWithTableNames(comboBox4, "Select CoachName from Coaches","CoachName");
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }
    }
}
