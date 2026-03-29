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
    public partial class deleteCoach : Form
    {
        string connectionString;
        public deleteCoach()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(comboBox1.SelectedIndex >= 0))
                {
                    MessageBox.Show("اختر مدربا من القائمة");
                    return;
                }

                DialogResult result = MessageBox.Show("سيتم حذف المدرب مع سجلات السحوبات المالية الخاصة به. هل تريد تأكيد الحذف؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string name = comboBox1.SelectedItem.ToString();
                    int coachID = GetCoachIDByName(name);
                    DeleteCoachFromPayments(coachID);
                    DeleteCoachFromSubscriptions(coachID);
                    DeleteCoachFromPausedSubscriptions(coachID);
                    DeleteWithdrawls(coachID);
                    DeleteCoach(coachID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void DeleteCoachFromPausedSubscriptions(int coachID)
        {
            string query = "update PausedSubscriptions set CoachID=NULL where CoachID=@CoachID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachID", coachID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteCoachFromSubscriptions(int coachID)
        {
            string query = "update Subscriptions set CoachID=NULL where CoachID=@CoachID";
            using (SqlConnection connection=new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@CoachID", coachID);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteCoachFromPayments(int coachID)
        {
            string query = "update PaymentsTable set CoachID=NULL where CoachID=@CoachID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachID", coachID);
                    command.ExecuteNonQuery();
                }
            }
        }


        private void DeleteCoach(int coachID)
        {
            string query = "delete from Coaches where CoachID=@CoachID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@CoachID", coachID);
                    int rowsAffected= command.ExecuteNonQuery();
                    if(rowsAffected>0)
                    MessageBox.Show("تم حذف المدرب مع سجلات السحوبات الماليةالخاصة به");
                }
            }
        }

        private int GetCoachIDByName(string coachName)
        {
            string query = "SELECT CoachID FROM Coaches WHERE CoachName = @CoachName";
            int coachID = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachName", coachName);
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        coachID = Convert.ToInt32(result);
                    }
                }
            }

            return coachID;
        }


        private void DeleteWithdrawls(int CoachID)
        {
            string query = "delete from Withdrawals where CoachID=@CoachID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand (query,connection))
                {
                    command.Parameters.AddWithValue("@CoachID", CoachID);
                    int affectedRecords= command.ExecuteNonQuery();
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }

        private void deleteCoach_Load(object sender, EventArgs e)
        {
            // Populate the coaches combobox
            string sectionQuery = "SELECT  CoachName FROM Coaches"; 
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sectionQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string coach = reader["CoachName"].ToString();
                            comboBox1.Items.Add(coach);

                        }
                    }
                }
            }

            // Enable autocomplete for the ComboBox
        }
    }
}
