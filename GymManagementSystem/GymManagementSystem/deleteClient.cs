using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace WindowsFormsApp1
{
    public partial class deleteClient : Form
    {
        string  connectionString;
        public deleteClient(List<string> clientNames)
        {
            InitializeComponent();
            comboBox1.Items.AddRange(clientNames.ToArray());
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(comboBox1.SelectedIndex >= 0))
                {
                    MessageBox.Show("select a client please", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string selectedClient = comboBox1.SelectedItem.ToString();
                int clientID = GetClientId(selectedClient);
                DialogResult result = MessageBox.Show("سيتم حذف المشترك مع سجلات الاشتراكات والدفعات والحضور, هل تريد تأكيد الحذف؟", "تأكيد", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {


                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string deleteSubscriptionsQurey = "DELETE from Subscriptions WHERE ClientID=@ClientID";

                        using (SqlCommand command = new SqlCommand(deleteSubscriptionsQurey, connection))
                        {
                            command.Parameters.AddWithValue("@ClientID", clientID);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                // Display an error message if no subscription is found
                                MessageBox.Show("تم حذف المشترك بنجاح");
                            }
                            else
                            {
                                // Display a success message if the deletion was successful
                                MessageBox.Show("تم حذف المشترك وجميع اشتراكاته ودفعاته بنجاح", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                        }

                        string deletePayments = "delete from PaymentsTable where ClientID=@ClientID";
                        using (SqlCommand command3 = new SqlCommand(deletePayments, connection))
                        {
                            command3.Parameters.AddWithValue("@ClientID", clientID);
                            command3.ExecuteNonQuery();
                        }

                        string deleteAttendance = "delete from Attendance where ClientID=@ClientID";
                        using (SqlCommand command4 = new SqlCommand(deleteAttendance, connection))
                        {
                            command4.Parameters.AddWithValue("@ClientID", clientID);
                            command4.ExecuteNonQuery();
                        }



                        string deleteMedicalRecord = "DELETE FROM MedicalRecords WHERE ClientID=@ClientID";
                        using (SqlCommand command5 = new SqlCommand(deleteMedicalRecord, connection))
                        {
                            command5.Parameters.AddWithValue("@ClientID", clientID);
                            command5.ExecuteNonQuery();
                        }

                        string deletePausedSubscriptoin = "DELETE FROM PausedSubscriptions WHERE ClientID=@ClientID";
                        using (SqlCommand command6 = new SqlCommand(deletePausedSubscriptoin, connection))
                        {
                            command6.Parameters.AddWithValue("@ClientID", clientID);
                            command6.ExecuteNonQuery();
                        }

                        string deleteClientQurey = "DELETE FROM Clients WHERE ClientID=@ClientID";
                        using (SqlCommand command2 = new SqlCommand(deleteClientQurey, connection))
                        {
                            command2.Parameters.AddWithValue("@ClientID", clientID);
                            command2.ExecuteNonQuery();
                        }


                        connection.Close();


                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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

        private  void deleteClient_Load(object sender, EventArgs e)
        {
           
    
        }


        private void UpdateProgressBar(int progressPercentage)
        {
           
        }




        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
       
          
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
