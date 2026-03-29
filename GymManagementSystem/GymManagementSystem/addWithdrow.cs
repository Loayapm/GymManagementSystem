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
using System.Xml.Linq;

namespace WindowsFormsApp1
{
    public partial class addWithdrow : Form
    {
         string connectionString;
        public addWithdrow()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.Enabled= false;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int withdrowAmount = 0;
                int.TryParse(textBox1.Text, out withdrowAmount);
                if (!(comboBox1.SelectedIndex >= 0))
                {
                    MessageBox.Show("اختر مدرب من القائمة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string name = comboBox1.SelectedItem.ToString();
                DateTime dateOfWithdral = dateTimePicker1.Value;
                DialogResult dialogResult = MessageBox.Show("هل تريد تأكيد اضافة السحب؟", "Confirm", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (withdrowAmount < 1500)
                    {
                        MessageBox.Show("قيمة السحب غير صالحة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    RecordWithdrawal(name, withdrowAmount, dateOfWithdral);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name;
            
            name=comboBox1.SelectedItem.ToString();
            
           float salary =getSalry(name);
            getTotalWithdrawAmount( name);
            textBox2.Text = salary.ToString();
            

        }

        private void RecordWithdrawal(string coachName, decimal withdrawalAmount, DateTime withdrawalDate)
        {
            int coachId = GetCoachId(coachName);
            string query = @"INSERT INTO Withdrawals (CoachId, WithdrawalAmount, WithdrawalDate)
                     VALUES (@CoachId, @WithdrawalAmount, @WithdrawalDate)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CoachId", coachId);
                command.Parameters.AddWithValue("@WithdrawalAmount", withdrawalAmount);
                command.Parameters.AddWithValue("@WithdrawalDate", withdrawalDate);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تمت اضافة السحب بنجاح");
                    }
                    else
                    {
                        MessageBox.Show("لم تتم اضافة السحب");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error recording withdrawal: " + ex.Message);
                }
            }
        }

        //working
        private int GetCoachId(string coachName)
        {
            int coachId = 0;
            string query = "SELECT CoachId FROM Coaches WHERE CoachName = @CoachName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CoachName", coachName);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        coachId = Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving coach ID: " + ex.Message);
                }
            }

            return coachId;
        }

        //fixed
        private void getTotalWithdrawAmount(string name)
        {
            int clientID = GetCoachId(name);
            // Assuming you have the coach's ID stored in a variable named 'selectedCoachId'
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            string query = @"SELECT SUM(WithdrawalAmount) 
                 FROM Withdrawals 
                 WHERE CoachID = @CoachID AND MONTH(WithdrawalDate) = @CurrentMonth AND YEAR(WithdrawalDate) = @CurrentYear";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CoachID", clientID);
                command.Parameters.AddWithValue("@CurrentMonth", currentMonth);
                command.Parameters.AddWithValue("@CurrentYear", currentYear);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        decimal totalWithdrawals = Convert.ToDecimal(result);
                        textBox3.Text = totalWithdrawals.ToString();
                    }
                    else
                    {
                        textBox3.Text = "0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving total withdrawals: " + ex.Message);
                }
            }
        }

        //fixed
       private float getSalry(string name)
        {
            int coachID = GetCoachId(name);
            float coachSalary;
            string salaryQurey = "SELECT Salary FROM Coaches WHERE CoachID=@CoachID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(salaryQurey, connection);
                command.Parameters.AddWithValue("@CoachID", coachID);
               

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                         coachSalary = float.Parse(result.ToString());
                        return coachSalary;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving coach salary: " + ex.Message);
                }
                return 0;

            }
        }

        //correct
        private void addWithdrow_Load(object sender, EventArgs e)
        {
            fillCoachesNames();
        }

        //fill combobox1 with coach names
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
                        comboBox1.Items.Add(coachName);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading coach names: " + ex.Message);
                }
            }
        }
    }
}
