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
    public partial class EditClientForm : Form
    {
        string connectionString;

        public EditClientForm(List<string> clientNames)
        {

            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
            comboBox1.Items.AddRange(clientNames.ToArray());

        }

        private void EditClientForm_Load(object sender, EventArgs e)
        {
          
            
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            bunifuFlatButton1.Enabled = false;
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected client from the ComboBox
            string selectedClient = comboBox1.SelectedItem.ToString();

            // Retrieve the current client information from the database
            // and populate the TextBox controls with the client info
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT ClientName, PhoneNumber FROM Clients WHERE ClientName = @ClientName";
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                selectCommand.Parameters.AddWithValue("@ClientName", selectedClient);

                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        textBox1.Text = reader.GetString(0);
                        string phoneNumber = reader.GetString(1);
                        textBox2.Text = phoneNumber;
                    }
                }
            }
        }

       
            private void bunifuFlatButton1_Click(object sender, EventArgs e)
            {
            try
            {
                string selectedClient = comboBox1.SelectedItem.ToString();
                string newClientName = textBox1.Text;
                string newClientPhoneNumber = textBox2.Text;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string updateQuery = "UPDATE Clients SET ClientName = @NewClientName, PhoneNumber = @NewClientPhoneNumber WHERE ClientName = @SelectedClient";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@NewClientName", newClientName);
                    updateCommand.Parameters.AddWithValue("@NewClientPhoneNumber", newClientPhoneNumber);
                    updateCommand.Parameters.AddWithValue("@SelectedClient", selectedClient);

                    int rowsAffected = updateCommand.ExecuteNonQuery();


                }
                string updatedClientName = newClientName; // The new client name after modification
                MessageBox.Show("تم تحديث معلومات المشترك بنجاح");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }



        }

     }

 }