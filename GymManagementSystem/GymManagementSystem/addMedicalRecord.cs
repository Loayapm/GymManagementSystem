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
    public partial class addMedicalRecord : Form
    {
         string connectionString;
        public addMedicalRecord(List<string> clientNames)
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
            comboBox1.Items.AddRange(clientNames.ToArray());

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex>=0)
            {
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = MessageBox.Show("هل تريد اضافة السجل؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult == DialogResult.No)
                {
                    return;
                }

                string address = textBox3.Text;
                DateTime birthDate = dateTimePicker1.Value;
                string work = textBox4.Text;
                string diseases = textBox5.Text;
                string bloodType = textBox6.Text;
                string purposeOfTraining = textBox7.Text;
                //if the user selects a client from the combobox
                if (comboBox1.SelectedIndex >= 0)
                {
                    string clientName = comboBox1.SelectedItem.ToString();
                    int cientID = GetClientId(clientName);
                    if (CheckMedicalRecordExists(cientID))
                    {
                        MessageBox.Show("تم العثور على سجل طبي لنفس المشترك", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    InsertRecord(cientID, address, birthDate, work, diseases, bloodType, purposeOfTraining);



                }
                //if the user does not select a client and enters a new client info
                else if (comboBox1.SelectedIndex == -1 && textBox1.Text != string.Empty && textBox2.Text != string.Empty)
                {
                    string name = textBox1.Text.Trim();
                    string phoneNumber = textBox2.Text;
                    if (name == string.Empty || phoneNumber == string.Empty)
                    {
                        MessageBox.Show("لا يمكن ان يكون الاسم او الرقم فارغا", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    double pn = 0;
                    double.TryParse(phoneNumber, out pn);

                    if (pn == 0)
                    {
                        MessageBox.Show("رقم هاتف غير صالح", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (CheckClientExists(name))
                    {
                        MessageBox.Show("المشترك الذي تحاول اضافته موجود مسبقا", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    InsertintoClientsTable(name, phoneNumber);
                    int clientID = GetClientId(name);
                    InsertRecord(clientID, address, birthDate, work, diseases, bloodType, purposeOfTraining);

                }
                //if the user does not select a client, niether enters a new client info
                else if (textBox1.Text == string.Empty && textBox2.Text == string.Empty)
                {
                    MessageBox.Show("اختر مشتركاً من القائمة او ادخل بيانات مشترك جديد", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void InsertintoClientsTable(string clientName,string phoneNumber)
        {
            string query = "insert into Clients (ClientName, PhoneNumber) values (@ClientName,@PhoneNumber)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@ClientName", clientName);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    command.ExecuteNonQuery();
                }
            }
            MessageBox.Show("تمت اضافة المشترك بنجاح");
        }

        private bool CheckClientExists(string name)
        {
            string checkClientQuery = "SELECT COUNT(*) FROM Clients WHERE ClientName = @ClientName";

            int existingClientsCount;

            using (SqlConnection connection = new SqlConnection("Data Source=DESKTOP-4J8VJT3;Initial Catalog=TempDBforCST;Integrated Security=True"))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(checkClientQuery, connection))
                {
                    command.Parameters.AddWithValue("@ClientName", name);

                    existingClientsCount = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            bool isExist = true ? existingClientsCount>0 : false;
            return isExist;
        }



        private void InsertRecord(int clientID, string address, DateTime birthdate, string work, string diseases, string bloodType, string purposeOfTraining)
        {
            string query = "insert into MedicalRecords (ClientID, Address, BirthDate, Work, Diseases, BloodType, Purpos) values (@ClientID, @Address, @BirthDate, @Work, @Diseases, @BloodType, @Purpos)";
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@ClientID",clientID);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@BirthDate", birthdate);
                    command.Parameters.AddWithValue("@Work", work);
                    command.Parameters.AddWithValue("@Diseases", diseases);
                    command.Parameters.AddWithValue("@BloodType", bloodType);
                    command.Parameters.AddWithValue("@Purpos", purposeOfTraining);
                    command.ExecuteNonQuery();
                   
                }
            }
            MessageBox.Show("تمت اضافة السجل الطبي بنجاح");

        }

        private bool CheckMedicalRecordExists(int clientID)
        {
            string query = " select count (*) from MedicalRecords where ClientID=@ClientID";
            int recordsFound = 0;
            using (SqlConnection connection = new SqlConnection (connectionString))
            {
                connection.Open ();
                using (SqlCommand command = new SqlCommand(query,connection))
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }

        private void addMedicalRecord_Load(object sender, EventArgs e)
        {
          //  fillClientsNames();
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
