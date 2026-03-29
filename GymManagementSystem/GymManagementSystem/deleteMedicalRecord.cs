using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class deleteMedicalRecord : Form
    {
        string connectionString;
        public deleteMedicalRecord(List<string> clientNames)
        {

            InitializeComponent();
            comboBox1.Items.AddRange(clientNames.ToArray());
            connectionString = DatabaseConnections.GymDB;
        }

        private void deleteMedicalRecord_Load(object sender, EventArgs e)
        {



        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Wanna delete record?");
                //if the user selects a client
                if (comboBox1.SelectedIndex >= 0)
                {
                    string name = comboBox1.SelectedItem.ToString();
                    int clientID = GetClientId(name);
                    DeleteMedicalRecord(clientID);


                }
                //if the user enteres a record number
                else if (textBox1.Text != string.Empty)
                {
                    int recordID = 0;
                    int.TryParse(textBox1.Text, out recordID);
                    DeleteMedicalRecordWithItsID(recordID);
                }
                else
                    MessageBox.Show("Select a client or enter a record number please", "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }


        }
        private void DeleteMedicalRecordWithItsID(int recordID)
        {
            int rowsAffected = 0;
            string query = "Delete from MedicalRecords where ID=@RecordID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RecordID", recordID);
                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            if (rowsAffected == 0)
            {
                MessageBox.Show("No record has found for this client");
            }
            else
            {
                MessageBox.Show("Record deleted successfully");
            }
        }
        private void DeleteMedicalRecord(int clientID)
        {
            int rowsAffected = 0;
            string query = "Delete from MedicalRecords where ClientID=@ClientID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    rowsAffected=  command.ExecuteNonQuery();
                }
            }
            if(rowsAffected==0)
            {
                MessageBox.Show("No record has found for this client");
            }
            else
            {
                MessageBox.Show("Record deleted successfully");
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
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


        }
    }
}
