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
    public partial class editeMedicalRecord : Form
    {
        string connectionString;
        public editeMedicalRecord(List<string> clientNames)
        {

            InitializeComponent();
            comboBox1.Items.AddRange(clientNames.ToArray());
            connectionString = DatabaseConnections.GymDB;
        }

        private void editeMedicalRecord_Load(object sender, EventArgs e)
        {
          //  fillClientsNames();
        }

        class medicalRecord
        {
            public string address ;
            public DateTime birthDate;
            public string work;
            public string diseases;
            public string bloodType;
            public string purpose;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name=comboBox1.SelectedItem.ToString();
            int clientID = GetClientId(name);
            if(!CheckMedicalRecordExists(clientID))
            {
                MessageBox.Show("لم يتم العثور على سجل طبي لهذا المشترك", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            medicalRecord record = GetRecordInfo(clientID);
            textBox3.Text = record.address;
            dateTimePicker1.Value= record.birthDate;
            textBox4.Text = record.work;
            textBox5.Text = record.diseases;
            textBox6.Text = record.bloodType;
            textBox7.Text = record.purpose;


        }

        private medicalRecord GetRecordInfo(int clientID)
        {
            medicalRecord recordToReturn= new medicalRecord();
            string query = "Select Address, BirthDate, Work, Diseases, BloodType, Purpos from MedicalRecords where ClientID=@ClientID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            recordToReturn.address = reader.GetString(0);
                            recordToReturn.birthDate= reader.GetDateTime(1);
                            recordToReturn.work = reader.GetString(2);
                            recordToReturn.diseases=reader.GetString(3);
                            recordToReturn.bloodType=reader.GetString(4);
                            recordToReturn.purpose=reader.GetString(5);


                        }
                    }
                }
            }

            return recordToReturn;
        }


        private bool CheckMedicalRecordExists(int clientID)
        {
            string query = " select count (*) from MedicalRecords where ClientID=@ClientID";
            int recordsFound = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientID);
                    recordsFound = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            if (recordsFound > 0)
            {
                return true;
            }
            else return false;

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

        
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuImageButton1.Enabled = false;
            this.Close();
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("يرجى اختيار مشترك أولا", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!CheckMedicalRecordExists(GetClientId(comboBox1.SelectedItem.ToString())))
                {
                    MessageBox.Show("لم يتم العثور على سجل طبي لهذا المشترك", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult result = MessageBox.Show("هل تريد حفظ التغييرات؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


                if (result == DialogResult.Yes)
                {
                    int clientID = GetClientId(comboBox1.SelectedItem.ToString());
                    string address = textBox3.Text;
                    DateTime birthDate = dateTimePicker1.Value;
                    string work = textBox4.Text;
                    string diseases = textBox5.Text;
                    string bloodType = textBox6.Text;
                    string purpos = textBox7.Text;
                    updateMedicalRecord(clientID, address, birthDate, work, diseases, bloodType, purpos);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }



        }

        private void updateMedicalRecord(int clientID, string address, DateTime birthDate,string work, string diseases, string bloodType, string purpos)
        {
            string query = @"UPDATE MedicalRecords
                     SET Address = @Address, BirthDate = @BirthDate, Work = @Work, Diseases=@Diseases, BloodType=@BloodType, Purpos=@Purpos
                     WHERE ClientID = @ClientID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@BirthDate", birthDate);
                    command.Parameters.AddWithValue("@Work", work);
                    command.Parameters.AddWithValue("@Diseases", diseases);
                    command.Parameters.AddWithValue("@BloodType", bloodType);
                    command.Parameters.AddWithValue("@Purpos", purpos);
                    command.Parameters.AddWithValue("@ClientID", clientID);

                    command.ExecuteNonQuery();
                    
                }
            }

            MessageBox.Show("تم حفظ التغييرات بنجاح");


        }
    }
}
