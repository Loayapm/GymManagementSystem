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
    public partial class deleteAttendance : Form
    {
        string connectionString;
        public deleteAttendance()
        {
            InitializeComponent();
            connectionString = DatabaseConnections.GymDB;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int attendanceID = 0;
                int.TryParse(textBox1.Text, out attendanceID);
                if (attendanceID == 0)
                {
                    MessageBox.Show("Invalid ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult result = MessageBox.Show("Wanna delete record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
                DeleteWithdrawl(attendanceID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }


        private void DeleteWithdrawl(int attendanceID)
        {
            int rowsAffected = 0;
            string query = "delete from   Attendance where Id=@ID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", attendanceID);
                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            if (rowsAffected == 0)
            {
                MessageBox.Show("No record is deleted");
            }
            else
            {
                MessageBox.Show("Record deleted succesfully");
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

        private void deleteAttendance_Load(object sender, EventArgs e)
        {

        }
    }
}
