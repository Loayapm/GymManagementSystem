using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Bunifu.Framework.UI;
using System.Data.SqlClient;
using System.Collections;
using OfficeOpenXml;
using System.IO;
using System.Globalization;
using Microsoft.Win32; // Required for OpenFileDialog
using System.Numerics;
using System.Data.SQLite;
using Microsoft.VisualBasic.ApplicationServices;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form


    {
        /*
         * 
         * This program contains 11500+ line of code.
         * 
         */

        //connection string to the client's SSMS
        // const string connectionString = "data source=DESKTOP-T1STQLB;initial catalog=Database1;trusted_connection=true";



        //sqlite connection string
        //  const string connectionString = @"Data Source=.db";


        //connection string to the new database in the project files
        // const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Admin\\source\\repos\\WindowsFormsApp1\\WindowsFormsApp1\\NewDatabase.mdf;Integrated Security=True";

        //const string connectionString = "data source=DESKTOP-4J8VJT3;initial catalog=Database;trusted_connection=true";

        //connection string after installing
         string connectionString;// "Server=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Program Files (x86)\\sawa\\Database1.mdf;Database=Database1;Trusted_Connection=True;";
        //connection string to the project file database
       // const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Admin\\source\\repos\\WindowsFormsApp1\\WindowsFormsApp1\\Database1.mdf;Integrated Security=True";
        public static List<string> ClientNames { get; set; }




        //moving the form
        int move;
        int moveX;
        int moveY;

        //Showing the date and time in the main form
        DateTime dateOfToday = DateTime.Now;

        // Format the DateTime to display only the date
         string formattedDate =  DateTime.Now.ToString("yyyy-MM-dd");

        string preSelected=null;
        string selected=null;


      

        public Form1()
        {
            InitializeComponent();
            label7.Text = formattedDate;
            connectionString = DatabaseConnections.GymDB;


            this.WindowState = FormWindowState.Maximized;
            

        }


        //fill the list with the names
        private void LoadClientNamesInTheList()
        {
            // fill the List with the names
            List<string> clientNames = new List<string>();
          

            string query = "SELECT ClientName FROM Clients";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);


                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string ClientName = reader["ClientName"].ToString();
                    clientNames.Add(ClientName);
                }

                reader.Close();
            }


            //the list in the constructor has the same elements for the list in this function
            ClientNames = clientNames;

            comboBox4.Items.AddRange(ClientNames.ToArray());
            comboBox3.Items.AddRange(ClientNames.ToArray());
            comboBox1.Items.AddRange(ClientNames.ToArray());
            comboBox8.Items.AddRange(ClientNames.ToArray());
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

      


        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
            ModifyButtonProperties("bunifuFlatButton1");


        }
        private void switchTheProperties(string buttonName)
        {
    

            BunifuFlatButton button = this.Controls[buttonName] as BunifuFlatButton;
            button.Text = "selected";

        }

        private void ModifyButtonProperties(string buttonName)
        {
            if (selected != null)
            {
                preSelected = selected;
                Control[] foundControls1 = this.Controls.Find(preSelected, true);
                if (foundControls1.Length > 0 && foundControls1[0] is BunifuFlatButton)
                {
                    BunifuFlatButton foundButton = (BunifuFlatButton)foundControls1[0];
                    foundButton.Normalcolor = Color.Black;
                    foundButton.Textcolor = Color.White;
                }
                else
                {
                    MessageBox.Show("Button not found or is not a valid BunifuFlatButton.");
                }


            }
            selected = buttonName;
            Control[] foundControls = this.Controls.Find(buttonName, true);

            if (foundControls.Length > 0 && foundControls[0] is BunifuFlatButton)
            {
                BunifuFlatButton foundButton = (BunifuFlatButton)foundControls[0];
                foundButton.Normalcolor = Color.FromArgb(255, 192, 128);
                foundButton.OnHovercolor = Color.FromArgb(255, 192, 128);
                foundButton.Textcolor = Color.Black;
            }
            else
            {
                MessageBox.Show("Button not found or is not a valid BunifuFlatButton.");
            }
            
        }
        private void tabPage2_Click(object sender, EventArgs e)
        {
        
            

        }

        private void bunifuFlatButton7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
            ModifyButtonProperties("bunifuFlatButton7");
            LoadAttendance();
            

        }



        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            move = 1;
            moveX = e.X;
            moveY = e.Y;

        }

        private void panel1_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (move == 1)
            {
                this.SetDesktopLocation(MousePosition.X - moveX, MousePosition.Y - moveY);
            }
            

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            move = 0;


        }

      

        private void bunifuFlatButton4_Click_2(object sender, EventArgs e)
        {
            
            addForm addForm = new addForm(ClientNames);
            addForm.Show();
            
        }

        private void bunifuFlatButton5_Click(object sender, EventArgs e)
        {
            removeSubscription removeSubscription = new removeSubscription(ClientNames);
            removeSubscription.Show();
        }

        private void bunifuFlatButton6_Click_1(object sender, EventArgs e)
        {
            editeSubscription editesubscription = new editeSubscription(ClientNames);
            editesubscription.Show();
        }

        private void bunifuFlatButton3_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton3");

            tabControl1.SelectedTab = tabPage3;
            //LoadPayments();


        }

        private void LoadPayments()
        {
            DataTable dataTable;
            string query = @"
                 SELECT 
              PaymentsTable.Id as 'رقم السجل',
              Clients.ClientName AS 'اسم المشترك', 
              Sections.SectionName AS 'قسم التسجيل', 
              TrainingTypes.TrainingName as 'نوع التدريب',
              SubscriptionType.SubscriptionName AS 'نوع الاشتراك',
              Coaches.CoachName AS 'اسم المدرب', 
              PaymentsTable.PaymentAmount AS 'قيمة الدفعة', 
              FORMAT(PaymentsTable.PaymentDate, 'dd/MM/yyyy  hh:mm tt') AS 'تاريخ الدفعة', 
              PaymentsTable.RemainingPaymentAmount AS 'المبلغ المتبقي'
              FROM PaymentsTable
              JOIN Clients ON PaymentsTable.ClientID = Clients.ClientId
              left Join TrainingTypes on PaymentsTable.TrainingTypeID=TrainingTypes.Id
              left JOIN Sections ON PaymentsTable.SectionID = Sections.SectionID
              left JOIN SubscriptionType ON PaymentsTable.SubscriptionTypeID = SubscriptionType.SubscriptionID
              left JOIN Coaches ON PaymentsTable.CoachID = Coaches.CoachID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);
                }
            }

            dataGridView8.DataSource = dataTable;
        }

       


       

        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
            addPayment addpayment = new addPayment(ClientNames);
            addpayment.Show();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuFlatButton9_Click_1(object sender, EventArgs e)
        {
            attendanceForm attendanceform=new attendanceForm(ClientNames);
            attendanceform.Show();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            
            
        }

        private void bunifuFlatButton14_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton14");

            tabControl1.SelectedTab = tabPage4;
        }

        private void bunifuFlatButton13_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton13");

            tabControl1.SelectedTab = tabPage5;
            if(comboBox5.Items.Count==0)
            PopulateCoachComboBox();


        }
        private void PopulateCoachComboBox()
        {
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
                            comboBox5.Items.Add(coach);

                        }
                    }
                }
            }
        }


        private void bunifuFlatButton10_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton10");

            tabControl1.SelectedTab = tabPage6;
        }

        private void bunifuFlatButton11_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton11");

            tabControl1.SelectedTab = tabPage7;
            loadSections();
        }

        private void loadSections()
        {
            string query = "select SectionID as 'رقم القسم', SectionName as 'اسم القسم', SectionMonthlyPrice as 'سعر الاشتراك الشهري' from Sections where IsDeleted = 0";
            DataTable dataTable;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                     dataTable = new DataTable();
                    adapter.Fill(dataTable);
                }
            }

            dataGridView5.DataSource = dataTable;
        }

        private void bunifuFlatButton21_Click(object sender, EventArgs e)
        {
            deleteSpending deletespending = new deleteSpending();
            deletespending.Show();
        }

        private void bunifuFlatButton22_Click(object sender, EventArgs e)
        {
            addSection addsection = new addSection();
            addsection.Show();
        }


        private void bunifuFlatButton15_Click(object sender, EventArgs e)
        {
            addCoach addcoach = new addCoach();
            addcoach.Show();
        }

        private void bunifuFlatButton16_Click(object sender, EventArgs e)
        {
            deleteCoach deletecoach = new deleteCoach();
            deletecoach.Show();
            
        }

        private void bunifuFlatButton26_Click(object sender, EventArgs e)
        {
            searchForClient searchforclient = new searchForClient(ClientNames);
            searchforclient.Show();
        }

        private void bunifuFlatButton12_Click(object sender, EventArgs e)
        {
            pauseSubscription pausesubscription=new pauseSubscription(ClientNames);
            pausesubscription.Show();
        }

 

        private void bunifuFlatButton18_Click(object sender, EventArgs e)
        {
            addWithdrow addwithdrow = new addWithdrow();
           addwithdrow.Show();
        }

        private void bunifuFlatButton19_Click(object sender, EventArgs e)
        {
            removeWithdrow removeWithdrow= new removeWithdrow();
            removeWithdrow.Show();
        }

        private void bunifuFlatButton20_Click(object sender, EventArgs e)
        {
            addSpendings addspendings = new addSpendings();
            addspendings.Show();
        }

        private void bunifuFlatButton22_Click_1(object sender, EventArgs e)
        {
            deleteSection deletesection = new deleteSection();
            deletesection.Show();
        }

        private void bunifuFlatButton24_Click(object sender, EventArgs e)
        {
            editeSectionPrice editesectionprice = new editeSectionPrice();
            editesectionprice.Show();
        }

        private void bunifuFlatButton29_Click(object sender, EventArgs e)
        {
            searchForPayments searchforpayments = new searchForPayments(ClientNames);
            searchforpayments.Show();
        }

    

        private void bunifuFlatButton28_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton28");

            tabControl1.SelectedTab = tabPage9;
           
        }

      
        //Load the data into the tables
        private void Form1_Activated(object sender, EventArgs e)
        {
            try
            {
                if (tabControl1.SelectedTab == tabPage1)
                {
                    LoadSubscriptions();
                    //  LoadSubscriptions2(); subsTable 

                }

                if (tabControl1.SelectedTab == tabPage4)
                {
                    coachesTableRefresh();

                }
                if (tabControl1.SelectedTab == tabPage6)
                {
                    spendingsTableRefresh();

                }
                if (tabControl1.SelectedTab == tabPage10)
                {
                    ClientsLoad();


                }

                if (tabControl1.SelectedTab == tabPage1)
                {
                    LoadClientNamesInTheList();


                }
                if (tabControl1.SelectedTab == tabPage5)
                {
                    LoadWithdrawalData();
                }

                if (tabControl1.SelectedTab == tabPage3 || tabControl1.SelectedTab == tabPage1)
                {
                    LoadPayments();

                }
                if (tabControl1.SelectedTab == tabPage7)
                {
                    loadSections();
                }
                if (tabControl1.SelectedTab == tabPage8)
                {
                    LoadMedicalRecords();
                }
                if (tabControl1.SelectedTab == tabPage2)
                {
                    LoadAttendance();
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Error message:" + ex.Message);
            }
           

        }

      

        //works fine
        private void LoadAttendance()
        {
          //  MessageBox.Show("I am here");

            // Use the connectionString defined at the class level
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Define the SQL command to select data from the Clients table
                string SQLCommand = "SELECT Id as 'رقم السجل', ClientName as 'اسم المشترك', format (AttendanceDate,'dd/MM/yyyy hh:mm tt') as 'تاريخ ووقت الحضور'  FROM Attendance join Clients on Attendance.ClientID=Clients.ClientId";

                // Create a SqlDataAdapter to execute the SQL command and fill the DataTable
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(SQLCommand, connection))
                {
                    // Create a new DataTable to hold the data
                    DataTable datatable = new DataTable();

                    // Open the database connection
                    connection.Open();

                    // Fill the DataTable with the data retrieved from the database
                    dataAdapter.Fill(datatable);

                    // Create a new BindingSource and set its DataSource to the DataTable
                    BindingSource bindingSource1 = new BindingSource();
                    bindingSource1.DataSource = datatable;

                    // Set the DataSource of dataGridView1 to the BindingSource
                    dataGridView6.DataSource = bindingSource1;

                    // Close the database connection
                    connection.Close();
                }
            }
        }


        //fixed and working                 FORMAT(PaymentsTable.PaymentDate, 'dd/MM/yyyy') AS 'تاريخ الدفعة', 
        private void LoadSubscriptions()
        {
            SqlConnection connection = new SqlConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable datatable = new DataTable();

            connection.ConnectionString = connectionString;
            var SQLCommand = @"
    SELECT 
        s.SubscriptionID as 'رقم السجل', 
        c.ClientName as 'اسم المشترك',  
        se.SectionName as 'القسم',  
        co.CoachName as 'اسم المدرب', 
        st.SubscriptionName as 'نوع الاشتراك',
        t.TrainingName as 'نوع التدريب' ,
        s.TotalSubscriptionAmount as 'قيمة الاشتراك', 
        FORMAT(s.SubscriptionDate, 'dd/MM/yyyy') as 'تاريخ الاشتراك', 
        FORMAT(s.SubscriptionEndDate, 'dd/MM/yyyy') as 'تاربخ انتهاء الاشتراك',
        ActiveState as 'حالة الاشتراك', 
        FORMAT(PauseDate, 'dd/MM/yyyy') as 'تاريخ ايقاف الاشتراك'
    FROM Subscriptions s 
    LEFT JOIN Clients c ON s.ClientID = c.ClientID 
    LEFT JOIN Coaches co ON s.CoachID = co.CoachID 
    LEFT JOIN Sections se ON s.SectionID = se.SectionID 
    LEFT JOIN SubscriptionType st ON s.SubscriptionTypeID = st.SubscriptionID
    LEFT JOIN TrainingTypes t ON s.TrainingTypeID = t.Id 
    order by s.SubscriptionID desc
    ;";
            dataAdapter.SelectCommand = new SqlCommand(SQLCommand, connection);
            dataAdapter.Fill(datatable);
            dataGridView1.DataSource = datatable;
        }

        private void LoadSubs()
        {
            SqlConnection connection = new SqlConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable datatable = new DataTable();
           
        }

        //fixed and working
        private void LoadWithdrawalData()
        {
            string query = @"SELECT w.Id as 'رقم السحب', c.CoachName as 'اسم المدرب', w.WithdrawalAmount as 'قيمة السحب', w.WithdrawalDate as 'تاريخ السحب' 
                     FROM Withdrawals w
                     JOIN Coaches c ON w.coachID = c.coachID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);
                    dataGridView3.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading withdrawal data: " + ex.Message);
                }
            }
        }

        //fixed and working
        private void spendingsTableRefresh()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Create the SQL query to retrieve the spendings table data
                string query = "SELECT id as 'رقم السجل', spendingName as 'اسم المنتج', spendingAmount as 'السعر', spendingDate as 'تاريخ الشراء'  FROM Spendings";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Create a DataTable to hold the spendings data
                    DataTable spendingsTable = new DataTable();

                    // Create a SqlDataAdapter to fill the DataTable
                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    // Fill the DataTable with the spendings data  Load
                    adapter.Fill(spendingsTable);

                    // Bind the DataTable to the DataGridView
                    dataGridView4.DataSource = spendingsTable;
                }
            }
        }

        //fixed and working
        private void coachesTableRefresh()
        {
            if (true) // Check if data is not already loaded
            {
                // Clear any existing data in the dataGridView
                dataGridView2.DataSource = null;

                // Construct the SQL query to select coach information
                string query = "SELECT CoachName as 'اسم المدرب', CoachPhoneNumber as 'رقم الهاتف', Salary as 'الراتب' FROM Coaches";

                // Create a DataTable to store the retrieved data
                DataTable dataTable = new DataTable();

                // Create a SqlDataAdapter to execute the query and fill the DataTable
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection))
                    {
                        // Open the database connection
                        connection.Open();

                        // Fill the DataTable with the retrieved data
                        dataAdapter.Fill(dataTable);

                        // Close the database connection
                        connection.Close();
                    }
                }

                // Set the DataTable as the dataSource for the dataGridView
                dataGridView2.DataSource = dataTable;

              //  isDataLoaded = true; // Set the flag to indicate that data is loaded
            }
        }

        private void ClientsLoad()
        {
            // Use the connectionString defined at the class level
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Define the SQL command to select data from the Clients table
                string SQLCommand = "SELECT ClientId as 'رقم المشترك', ClientName as 'اسم المشترك', PhoneNumber as 'رقم الهاتف' FROM Clients";

                // Create a SqlDataAdapter to execute the SQL command and fill the DataTable
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(SQLCommand, connection))
                {
                    // Create a new DataTable to hold the data
                    DataTable datatable = new DataTable();

                    // Open the database connection
                    connection.Open();

                    // Fill the DataTable with the data retrieved from the database
                    dataAdapter.Fill(datatable);

                    // Create a new BindingSource and set its DataSource to the DataTable
                    BindingSource bindingSource1 = new BindingSource();
                    bindingSource1.DataSource = datatable;

                    // Set the DataSource of dataGridView1 to the BindingSource
                    dataGridView7.DataSource = bindingSource1;

                    // Close the database connection
                    connection.Close();
                }
            }
        }
     

      
        private void bunifuFlatButton30_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton30");
            SqlConnection connection = new SqlConnection();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable datatable = new DataTable();
            tabControl1.SelectedTab= tabPage10;
            connection.ConnectionString = connectionString;
            
            var SQLCommend = "SELECT ClientId as ' رقم المشترك ' , ClientName as 'اسم المشترك',PhoneNumber as 'رقم الهاتف' FROM Clients";
            dataAdapter  = new SqlDataAdapter(SQLCommend, connection);
            dataAdapter.Fill(datatable);
            dataGridView7.DataSource=datatable;
            

           
        }




        //this function is to change the format of the date in a dataGridView from mm/dd/yyyy to dd/mm/yyyy 
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Check if the current cell is in the subscription date column and the value is not null
            if ((e.ColumnIndex == 5 ||e.ColumnIndex==6) && e.Value != null)
            {
                // Convert the value to a DateTime object
                if (DateTime.TryParse(e.Value.ToString(), out DateTime dateValue))
                {
                    // Format the date as dd/MM/yyyy
                    e.Value = dateValue.ToString("dd/MM/yyyy");
                    e.FormattingApplied = true; // Set this to true to indicate that the formatting has been applied
                }
            }
        }




        private void Form1_Load(object sender, EventArgs e)
        {
           

      


            try
            {
                LoadSubscriptions();
                coachesTableRefresh();
                spendingsTableRefresh();
                ClientsLoad();
                LoadWithdrawalData();
                LoadPayments();
                LoadAttendance();
                LoadClientNamesInTheList();
                LoadRedSubscription();
            }
            catch (SqlException ex)
            {
                // Log the error or show a message to the user
                MessageBox.Show("An error occurred while loading data: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                MessageBox.Show("An unexpected error occurred: " + ex.Message);
            }




        }



        private void bunifuFlatButton31_Click(object sender, EventArgs e)
        {
            addClientForm addclientform = new addClientForm();
            addclientform.Show();
        }

        private void bunifuImageButton5_Click(object sender, EventArgs e)
        {

            string selectedClient = comboBox8.SelectedIndex >= 0 ? comboBox8.SelectedItem.ToString() : string.Empty;
            string query = "SELECT * FROM Clients WHERE 1=1"; // Replace with your actual table name and column names
            string phoneNumber = textBox8.Text;

            if (comboBox8.SelectedIndex >= 0)
            {
                query += " AND ClientName = @ClientName";
            }
            else if(textBox8.Text!=string.Empty)
            {
                query += " AND PhoneNumber=@PhoneNumber";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                if (comboBox8.SelectedIndex >= 0)
                {
                    command.Parameters.AddWithValue("@ClientName", selectedClient);
                }
                else if(textBox8.Text!=string.Empty)
                {
                    
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                }

                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

                dataAdapter.Fill(dataTable);
              //  dataTable.Columns.Remove("SubscriptionDate"); // Remove the unwanted column

                dataGridView7.DataSource = dataTable;
            

            // Change column names in the DataGridView
                dataGridView7.Columns["ClientName"].HeaderText = "اسم المشترك";
                dataGridView7.Columns["PhoneNumber"].HeaderText = "رقم الهاتف";
                dataGridView7.Columns["ClientId"].HeaderText = "رقم المشترك";




            }
        }

        private void bunifuFlatButton32_Click(object sender, EventArgs e)
        {
            
            deleteClient deleteclient = new deleteClient(ClientNames);
            deleteclient.Show();
        }

      

        private void bunifuFlatButton33_Click(object sender, EventArgs e)
        {
            EditClientForm editClientform = new EditClientForm(ClientNames);
            editClientform.Show();
        }

      

        private void bunifuImageButton6_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

     


        private void bunifuImageButton7_Click(object sender, EventArgs e)
        {
            string month=textBox4.Text;
            string year = textBox1.Text;
           
            string day = textBox2.Text;
            textBox3.Text = CalculateTotalSpending( year,  month,  day).ToString();
            FilterRecordsByDate(year, month, day);
        }

        public void FilterRecordsByDate(string year, string month, string day)
        {
            try
            {
                // Initialize the query string
                string query = "SELECT id as 'رقم السجل', spendingName as 'اسم المنتج', spendingAmount as 'السعر', spendingDate as 'تاريخ الشراء'  FROM Spendings where 1=1";

                // Append conditions based on non-empty inputs
                if (!string.IsNullOrEmpty(year))
                {
                    query += $" AND YEAR(spendingDate) = {year}";
                }
                if (!string.IsNullOrEmpty(month))
                {
                    query += $" AND MONTH(spendingDate) = {month}";
                }
                if (!string.IsNullOrEmpty(day))
                {
                    query += $" AND DAY(spendingDate) = {day}";
                }

                // Execute the query
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Assuming dataGridView4 is bound to a DataTable named dataTable
                    DataTable dataTable = (DataTable)dataGridView4.DataSource;
                    dataTable.Clear(); // Clear existing data

                    // Fill the dataTable with the query results
                    dataTable.Load(reader);

                    // Update the DataGridView
                    dataGridView4.DataSource = dataTable;

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public double CalculateTotalSpending(string year, string month, string day)
        {
            double totalSpending = 0;
            string query = "SELECT SUM(spendingAmount) FROM Spendings WHERE 1=1";

            // Construct the WHERE clause based on the input
            if (!string.IsNullOrEmpty(year))
            {
                query += $" AND YEAR(spendingDate) = {year}";
            }
            if (!string.IsNullOrEmpty(month))
            {
                query += $" AND MONTH(spendingDate) = {month}";
            }
            if (!string.IsNullOrEmpty(day))
            {
                query += $" AND DAY(spendingDate) = {day}";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            totalSpending = Convert.ToDouble(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message,"خطأ",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }

            return totalSpending;
        }

  

      

      

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == -1)
            {
                LoadPayments();
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Get the selected client ID from the ComboBox
                int selectedClientId = GetClientId(comboBox3.SelectedItem.ToString());

                // Create a SQL query to select the records based on the selected client ID
                string query = @"
                 SELECT 
              PaymentsTable.Id as 'رقم السجل',
              Clients.ClientName AS 'اسم المشترك', 
              Sections.SectionName AS 'قسم التسجيل', 
              TrainingTypes.TrainingName as 'نوع التدريب',
              SubscriptionType.SubscriptionName AS 'نوع الاشتراك',
              Coaches.CoachName AS 'اسم المدرب', 
              PaymentsTable.PaymentAmount AS 'قيمة الدفعة', 
              FORMAT(PaymentsTable.PaymentDate, 'dd/MM/yyyy  hh:mm tt') AS 'تاريخ الدفعة', 
              PaymentsTable.RemainingPaymentAmount AS 'المبلغ المتبقي'
              FROM PaymentsTable
              JOIN Clients ON PaymentsTable.ClientID = Clients.ClientId
              left Join TrainingTypes on PaymentsTable.TrainingTypeID=TrainingTypes.Id
              left JOIN Sections ON PaymentsTable.SectionID = Sections.SectionID
              left JOIN SubscriptionType ON PaymentsTable.SubscriptionTypeID = SubscriptionType.SubscriptionID
              left JOIN Coaches ON PaymentsTable.CoachID = Coaches.CoachID
              where PaymentsTable.ClientID=@ClientId";

                // Create a SqlCommand with the query and connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add the client ID as a parameter to the query
                    command.Parameters.AddWithValue("@ClientId", selectedClientId);

                    // Create a SqlDataAdapter to retrieve the data from the SQL query
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        // Create a DataTable to store the retrieved data
                        DataTable dataTable = new DataTable();

                        // Fill the DataTable with the data from the adapter
                        adapter.Fill(dataTable);

                        // Set the DataTable as the DataSource for the DataGridView
                        dataGridView8.DataSource = dataTable;
                    }
                }
            }
        }

    

        private void bunifuImageButton4_Click(object sender, EventArgs e)
        {
            int year =  string.IsNullOrEmpty(textBox5.Text) ? 0 : int.Parse(textBox5.Text);
            int month =  string.IsNullOrEmpty(textBox6.Text) ? 0 : int.Parse(textBox6.Text);
            string name = comboBox5.SelectedIndex>=0 ? comboBox5.SelectedItem.ToString() : string.Empty;
            
            int coachID = comboBox5.SelectedIndex>=0 ? GetCoachID(name) : 0;
            GetTotalWithdrawls(year, month, coachID);
            FilterRecordsWithSQL( year, month, coachID);
        }

        public void FilterRecordsWithSQL(int year, int month, int coachID)
        {
            string query = @"SELECT w.Id as 'رقم السحب', c.CoachName as 'اسم المدرب', w.WithdrawalAmount as 'قيمة السحب', w.WithdrawalDate as 'تاريخ السحب' 
                 FROM Withdrawals w
                 JOIN Coaches c ON w.CoachID = c.CoachID
                 WHERE 1=1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if (coachID != 0)
            {
                query += " AND w.CoachID = @CoachID";
                parameters.Add(new SqlParameter("@CoachID", coachID));
            }

            if (year != 0)
            {
                query += " AND YEAR(w.WithdrawalDate) = @WithdrawalYear";
                parameters.Add(new SqlParameter("@WithdrawalYear", year));
            }

            if (month != 0)
            {
                query += " AND MONTH(w.WithdrawalDate) = @WithdrawalMonth";
                parameters.Add(new SqlParameter("@WithdrawalMonth", month));
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    command.Parameters.AddRange(parameters.ToArray());
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        dataGridView3.DataSource = dataTable; 
                    }
                }
            }
        }


        private void GetTotalWithdrawls(int year,int month, int coachID)
        {
            string query = @"select sum (WithdrawalAmount) from Withdrawals where 1=1";

            List<SqlParameter> parameters = new List<SqlParameter>();

            if(coachID !=0 )
            {
                query += " and CoachID=@CoachID";
                parameters.Add(new SqlParameter("@CoachID", coachID));

            }

            if(year != 0)
            {
                query += " AND YEAR(WithdrawalDate) = @WithdrawalYear";
                parameters.Add(new SqlParameter("@WithdrawalYear", year));

            }

            if (month != 0)
            {
                query += " AND MONTH(WithdrawalDate) = @WithdrawalMonth";
                parameters.Add(new SqlParameter("@WithdrawalMonth", month));

            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    object result = 0;
                    connection.Open();
                    command.Parameters.AddRange(parameters.ToArray());
                     result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        // Display the total payment amount in the TextBox
                        textBox7.Text = result.ToString();
                    }
                    else
                    {
                        // No payments found, clear the TextBox or set a default message
                        textBox7.Text = "لم يتم العثور على دفعات";
                    }
                }
            }

        }

        private int GetCoachID(string coachName)
        {
            string query = "SELECT CoachID FROM Coaches WHERE CoachName = @CoachName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CoachName", coachName);

                connection.Open();
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private void bunifuFlatButton8_Click_1(object sender, EventArgs e)
        {
            deletePayment deletepayment = new deletePayment();
            deletepayment.Show();
            
        }

        private void bunifuFlatButton27_Click_1(object sender, EventArgs e)
        {
            ExportSubscriptionsToExcel(connectionString);
        }

        public void ExportSubscriptionsToExcel(string connectionString)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial; // Adjust based on your usage


            // SQL query to execute load
            string SQLCommand = @"
     SELECT s.SubscriptionID as 'رقم السجل', c.ClientName as 'اسم المشترك', c.PhoneNumber as 'رقم الهاتف', se.SectionName as 'القسم', co.CoachName as 'اسم المدرب', 
    st.SubscriptionName as 'نوع الاشتراك', s.TotalSubscriptionAmount as 'قيمة الاشتراك', 
    FORMAT(s.SubscriptionDate, 'dd/MM/yyyy') as 'تاريخ الاشتراك', 
    FORMAT(s.SubscriptionEndDate, 'dd/MM/yyyy') as 'تاربخ انتهاء الاشتراك', 
    ActiveState as 'حالة الاشتراك', FORMAT(PauseDate, 'dd/MM/yyyy') as 'تاريخ ايقاف الاشتراك' 
    FROM Subscriptions s 
    LEFT JOIN Clients c ON s.ClientID = c.ClientID 
    LEFT JOIN Coaches co ON s.CoachID = co.CoachID 
    LEFT JOIN Sections se ON s.SectionID = se.SectionID 
    LEFT JOIN SubscriptionType st ON s.SubscriptionTypeID = st.SubscriptionID;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(SQLCommand, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        // Use SaveFileDialog to let the user select the file path
                        SaveFileDialog saveFileDialog = new SaveFileDialog
                        {
                            Filter = "Excel Files|*.xlsx",
                            Title = "Save an Excel File"
                        };

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // Export to Excel
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Subscriptions");
                                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                                package.SaveAs(new FileInfo(saveFileDialog.FileName));
                            }

                           MessageBox.Show("Export successful!");
                        }
                    }
                }
            }
        }

        private void bunifuFlatButton37_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton37");

            tabControl1.SelectedTab = tabPage8;

            LoadMedicalRecords();
        }

        private void LoadMedicalRecords()
        {
            // Use the connectionString defined at the class level
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Define the SQL command to select data from the Clients table
                string SQLCommand = "Select ID as 'رقم السجل', ClientName as 'اسم المشترك', Address as 'العنوان', format ( BirthDate , 'dd/MM/yyyy' ) as 'تاريخ الميلاد' , Work as 'العمل', Diseases as 'الأمراض العامة او المزمنة', BloodType as 'الزمرة الدموية', Purpos as 'الهدف من التدريب'  from MedicalRecords join Clients on MedicalRecords.ClientID=Clients.ClientID";

                // Create a SqlDataAdapter to execute the SQL command and fill the DataTable
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(SQLCommand, connection))
                {
                    // Create a new DataTable to hold the data
                    DataTable datatable = new DataTable();

                    // Open the database connection
                    connection.Open();

                    // Fill the DataTable with the data retrieved from the database
                    dataAdapter.Fill(datatable);

                    // Create a new BindingSource and set its DataSource to the DataTable
                    BindingSource bindingSource1 = new BindingSource();
                    bindingSource1.DataSource = datatable;

                    // Set the DataSource of dataGridView1 to the BindingSource
                    dataGridView9.DataSource = bindingSource1;

                    // Close the database connection
                    connection.Close();
                }
            }

            // Check if the column "العمر" already exists before adding it
            if (!dataGridView9.Columns.Contains("العمر"))
            {
                // Add the "العمر" column to the DataGridView
                dataGridView9.Columns.Add("العمر", "العمر");
            }

            foreach (DataGridViewRow row in dataGridView9.Rows)
            {
                object birthdateValue = row.Cells["تاريخ الميلاد"].Value;

                if (birthdateValue != null && !string.IsNullOrWhiteSpace(birthdateValue.ToString()))
                {
                    string birthdateString = birthdateValue.ToString();
                    DateTime birthdate;

                    // Specify the input date format
                    string inputFormat = "dd/MM/yyyy";

                    // Try parsing the birthdate string with the specified format
                    if (DateTime.TryParseExact(birthdateString, inputFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out birthdate))
                    {
                        int age = DateTime.Today.Year - birthdate.Year;

                        if (birthdate > DateTime.Today.AddYears(-age))
                            age--;

                        row.Cells["العمر"].Value = age;
                    }
                    else
                    {
                       
                    }
                }
            }

        }

        private void bunifuFlatButton25_Click_1(object sender, EventArgs e)
        {
            addMedicalRecord addmed = new addMedicalRecord(ClientNames);
            addmed.Show();
        }

        private void bunifuFlatButton35_Click(object sender, EventArgs e)
        {
            deleteMedicalRecord deletemedicalrecord = new deleteMedicalRecord(ClientNames);
            deletemedicalrecord.Show();
        }

        private void bunifuFlatButton36_Click(object sender, EventArgs e)
        {
            editeMedicalRecord editeMedical = new editeMedicalRecord(ClientNames);
            editeMedical.Show();
        }

        private void dataGridView1_CellFormatting_1(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void dataGridView9_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
          
        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void bunifuFlatButton38_Click(object sender, EventArgs e)
        {
            deleteAttendance deleteattendance = new deleteAttendance();
            deleteattendance.Show();
        }

        private void bunifuFlatButton34_Click_1(object sender, EventArgs e)
        {
            string backupFolderPath = @"E:\Backups\"; // Folder where backups will be saved
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            


            try
            {
                string backupFileName = $"Database1-{currentDate}-backup"; 
                BackupDatabase(connectionString, $"{backupFolderPath}{backupFileName}.bak");
               MessageBox.Show($": تم نسخ جميع البيانات احتياطياً بنجاح الى المسار التالي  " +
                   $"\n {backupFolderPath}{backupFileName}.bak");
            }
            catch (Exception ex)
            {
               MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private static void BackupDatabase(string connectionString, string backupFilePath)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("BACKUP DATABASE [Database1] TO DISK = @backupPath", connection))
                {
                    command.Parameters.AddWithValue("@backupPath", backupFilePath);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void bunifuFlatButton41_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = @"E:\Backups",
                Filter = "Backup Files (*.bak)|*.bak",
                Title = "Select a Backup File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string backupFilePath = openFileDialog.FileName;
                ConfirmRestore(backupFilePath);
            }
        }

        private void ConfirmRestore(string backupFilePath)
        {
            DialogResult result = MessageBox.Show($" هل تريد تأكيد استرجاع البيانات من المسار '{backupFilePath}'?", "تأكيد الاستعادة", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                RestoreDatabase(backupFilePath);
            }
        }

        //      const  string connectionString = "Server=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Program Files (x86)\\sawa\\Database1.mdf;Database=Database1;Trusted_Connection=True;";

        private void RestoreDatabase(string backupFilePath)
        {
            string databaseName = "Database1";
            string dataFilePath = @"C:\Program Files (x86)\sawa\Database1.mdf";
            string logFilePath = @"C:\Program Files (x86)\sawa\Database1_log.ldf";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Switch to the 'master' database
                using (SqlCommand switchCommand = new SqlCommand("USE master", connection))
                {
                    switchCommand.ExecuteNonQuery();
                }

                // Terminate any existing connections to the target database
                using (SqlCommand killCommand = new SqlCommand($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", connection))
                {
                    killCommand.ExecuteNonQuery();
                }

                // Restore the database with MOVE command to specify new file locations
                string restoreQuery = $@"
            RESTORE DATABASE [{databaseName}]
            FROM DISK = @backupPath
            WITH REPLACE,
            MOVE '{databaseName}' TO '{dataFilePath}',
            MOVE '{databaseName}_log' TO '{logFilePath}'";

                using (SqlCommand command = new SqlCommand(restoreQuery, connection))
                {
                    command.Parameters.AddWithValue("@backupPath", backupFilePath);

                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("تم استرجاع البيانات بنجاح", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred during restoration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void bunifuFlatButton39_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton39");

            tabControl1.SelectedTab = tabPage11;
            LoadEndedSubscription();

        }


        private void LoadEndedSubscription()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                DataTable datatable = new DataTable();

                var SQLCommand = @"
            SELECT 
                s.SubscriptionID as 'رقم السجل', 
                c.ClientName as 'اسم المشترك',  
                se.SectionName as 'القسم',  
                co.CoachName as 'اسم المدرب', 
                st.SubscriptionName as 'نوع الاشتراك',
                t.TrainingName as 'نوع التدريب',
                s.TotalSubscriptionAmount as 'قيمة الاشتراك', 
                FORMAT(s.SubscriptionDate, 'dd/MM/yyyy') as 'تاريخ الاشتراك', 
                FORMAT(s.SubscriptionEndDate, 'dd/MM/yyyy') as 'تاربخ انتهاء الاشتراك',
                s.ActiveState as 'حالة الاشتراك', 
                FORMAT(s.PauseDate, 'dd/MM/yyyy') as 'تاريخ ايقاف الاشتراك'
            FROM Subscriptions s
            LEFT JOIN Clients c ON s.ClientID = c.ClientID 
            LEFT JOIN Coaches co ON s.CoachID = co.CoachID 
            LEFT JOIN Sections se ON s.SectionID = se.SectionID 
            LEFT JOIN SubscriptionType st ON s.SubscriptionTypeID = st.SubscriptionID
            LEFT JOIN TrainingTypes t ON s.TrainingTypeID = t.Id 
            WHERE s.SubscriptionEndDate < @todayDate or s.SubscriptionEndDate is null
            ;";

                using (SqlCommand command = new SqlCommand(SQLCommand, connection))
                {
                    command.Parameters.AddWithValue("@todayDate", DateTime.Today);

                    try
                    {
                        connection.Open();
                        dataAdapter.SelectCommand = command;
                        dataAdapter.Fill(datatable);
                        dataGridView10.DataSource = datatable;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
        }

        private void LoadRedSubscription()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                DataTable datatable = new DataTable();

                var SQLCommand = @"
            SELECT 
                s.SubscriptionID as 'رقم السجل', 
                c.ClientName as 'اسم المشترك',  
                se.SectionName as 'القسم',  
                co.CoachName as 'اسم المدرب', 
                st.SubscriptionName as 'نوع الاشتراك',
                t.TrainingName as 'نوع التدريب',
                s.TotalSubscriptionAmount as 'قيمة الاشتراك', 
                FORMAT(s.SubscriptionDate, 'dd/MM/yyyy') as 'تاريخ الاشتراك', 
                FORMAT(s.SubscriptionEndDate, 'dd/MM/yyyy') as 'تاربخ انتهاء الاشتراك',
                s.ActiveState as 'حالة الاشتراك', 
                FORMAT(s.PauseDate, 'dd/MM/yyyy') as 'تاريخ ايقاف الاشتراك'
            FROM Subscriptions s
            LEFT JOIN Clients c ON s.ClientID = c.ClientID 
            LEFT JOIN Coaches co ON s.CoachID = co.CoachID 
            LEFT JOIN Sections se ON s.SectionID = se.SectionID 
            LEFT JOIN SubscriptionType st ON s.SubscriptionTypeID = st.SubscriptionID
            LEFT JOIN TrainingTypes t ON s.TrainingTypeID = t.Id 
            WHERE s.isRed=1
            ;";

                using (SqlCommand command = new SqlCommand(SQLCommand, connection))
                {
                    command.Parameters.AddWithValue("@todayDate", DateTime.Today);

                    try
                    {
                        connection.Open();
                        dataAdapter.SelectCommand = command;
                        dataAdapter.Fill(datatable);
                        dataGridView11.DataSource = datatable;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }
        }

        private void bunifuFlatButton40_Click(object sender, EventArgs e)
        {
            ModifyButtonProperties("bunifuFlatButton40");

            tabControl1.SelectedTab = tabPage12;
            LoadRedSubscription();

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        //finished
        public void ResetIdentity(string connectionStr)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                connection.Open();

                string tableName = "Subscriptions";
                string resetIdentityQuery = $"DBCC CHECKIDENT ('{tableName}', RESEED, 0)";

                using (SqlCommand command = new SqlCommand(resetIdentityQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Identity reset successfully.");
            }
        }
        //finished
        public void InsertRecordsFromFile(string filePath, string connectionStr)
        {
            string[] lines = File.ReadAllLines(filePath);

            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                connection.Open();

                foreach (string line in lines)
                {
                    string[] data = line.Split('\t');

                    try
                    {
                        int clientId = int.Parse(data[0].Trim());
                        int sectionId = int.Parse(data[1].Trim());

                        string dateStr = data[2].Trim();
                        DateTime subscriptionDate;
                        string[] formats = { "d/M/yyyy", "M/d/yyyy" };

                        if (!DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out subscriptionDate))
                        {
                            MessageBox.Show("Invalid date format for line: " + line);
                            continue; // Skip this line and move to the next iteration
                        }

                        int coachId = int.Parse(data[3].Trim());
                        int subscriptionTypeId = int.Parse(data[4].Trim());
                        int trainingTypeId = int.Parse(data[5].Trim());
                        int activeState = int.Parse(data[6].Trim());

                        int isRed;
                        string isRedStr = data[7].Trim();
                        if (!int.TryParse(isRedStr, out isRed))
                        {
                            MessageBox.Show("Invalid isRed value for line: " + line);
                            continue; // Skip this line and move to the next iteration
                        }

                        string query = "INSERT INTO [dbo].[Subscriptions] ([ClientID], [SectionID], [SubscriptionDate], [CoachID], [SubscriptionTypeID], [TrainingTypeID], [ActiveState], [IsRed]) " +
                                        "VALUES (@ClientId, @SectionId, @SubscriptionDate, @CoachId, @SubscriptionTypeId, @TrainingTypeId, @ActiveState, @IsRed)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@ClientId", clientId);
                            command.Parameters.AddWithValue("@SectionId", sectionId);
                            command.Parameters.AddWithValue("@SubscriptionDate", subscriptionDate);
                            command.Parameters.AddWithValue("@CoachId", coachId);
                            command.Parameters.AddWithValue("@SubscriptionTypeId", subscriptionTypeId);
                            command.Parameters.AddWithValue("@TrainingTypeId", trainingTypeId);
                            command.Parameters.AddWithValue("@ActiveState", activeState);
                            command.Parameters.AddWithValue("@IsRed", isRed);
                            command.ExecuteNonQuery();
                        }

                     
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Error inserting record: " + ex.Message +" in line :" +line);
                    }
                }
                MessageBox.Show("Records inserted successfully.");
            }
        }

        private void bunifuFlatButton42_Click(object sender, EventArgs e)
        {
            //does not need to send client names to this form
            editSubscriptions editsubscriptions = new editSubscriptions();
            editsubscriptions.Show();
        }

        private void bunifuFlatButton17_Click_1(object sender, EventArgs e)
        {
            editCoach editCoach = new editCoach();
            editCoach.Show();
        }

        private void bunifuFlatButton43_Click(object sender, EventArgs e)
        {
          //  string connectionString = "Data Source=YourServer;Initial Catalog=master;Integrated Security=True";
          //  string mdfPath = @"C:\Program Files (x86)\sawa\Database1.mdf";
            string query = "ALTER DATABASE [Database1] SET READ_WRITE WITH NO_WAIT";
           
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        // Attempt to attach the MDF file
                      

                        MessageBox.Show("Query executed");
                    }

                   
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

        private void bunifuImageButton3_Click(object sender, EventArgs e)
        {
            if(comboBox4.SelectedIndex==-1)
            {
                LoadAttendance();
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Get the selected client ID from the ComboBox
                int selectedClientId = GetClientId(comboBox4.SelectedItem.ToString());

                // Create a SQL query to select the records based on the selected client ID
                string query = "SELECT Id as 'رقم السجل', ClientName as 'اسم المشترك', format (AttendanceDate,'dd/MM/yyyy hh:mm tt') as 'تاريخ ووقت الحضور'  FROM Attendance join Clients on Attendance.ClientID=Clients.ClientId where Attendance.ClientID=@ClientId";

                // Create a SqlCommand with the query and connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add the client ID as a parameter to the query
                    command.Parameters.AddWithValue("@ClientId", selectedClientId);

                    // Create a SqlDataAdapter to retrieve the data from the SQL query
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        // Create a DataTable to store the retrieved data
                        DataTable dataTable = new DataTable();

                        // Fill the DataTable with the data from the adapter
                        adapter.Fill(dataTable);

                        // Set the DataTable as the DataSource for the DataGridView
                        dataGridView6.DataSource = dataTable;
                    }
                }
            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuImageButton9_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                LoadMedicalRecords();
                return;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Get the selected client ID from the ComboBox
                    int selectedClientId = GetClientId(comboBox1.SelectedItem.ToString());

                    // Create a SQL query to select the records based on the selected client ID
                    string query = "SELECT ID AS 'رقم السجل', ClientName AS 'اسم المشترك', Address AS 'العنوان', FORMAT(BirthDate, 'dd/MM/yyyy') AS 'تاريخ الميلاد', Work AS 'العمل', Diseases AS 'الأمراض العامة أو المزمنة', BloodType AS 'الزمرة الدموية', Purpos AS 'الهدف من التدريب' FROM MedicalRecords JOIN Clients ON MedicalRecords.ClientID = Clients.ClientID WHERE MedicalRecords.ClientID = @ClientId";

                    // Create a SqlCommand with the query and connection
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add the client ID as a parameter to the query
                        command.Parameters.AddWithValue("@ClientId", selectedClientId);

                        // Create a SqlDataAdapter to retrieve the data from the SQL query
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            // Create a DataTable to store the retrieved data
                            DataTable dataTable = new DataTable();

                            // Fill the DataTable with the data from the adapter
                            adapter.Fill(dataTable);

                            // Set the DataTable as the DataSource for the DataGridView
                            dataGridView9.DataSource = dataTable;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during database operations
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }

        }

        private void bunifuFlatButton43_Click_1(object sender, EventArgs e)
        {
            passwordForm passform = new passwordForm();
            passform.Show();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
          //  string path = "J:\\newSubs2.txt";
           // InsertSubscriptionsFromTextFile(path, connectionString2);
        }
        public void InsertSubscriptionsFromTextFile(string filePath, string connectionString)
        {
            string insertQuery = "INSERT INTO Subscriptions (ClientID, SectionID, SubscriptionDate, CoachID, TrainingTypeID, SubscriptionTypeID) " +
                                 "VALUES (@ClientID, @SectionID, @SubscriptionDate, @CoachID, @TrainingTypeID, @SubscriptionTypeID)";

            using (StreamReader reader = new StreamReader(filePath))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.Add("@ClientID", SqlDbType.Int);
                        command.Parameters.Add("@SectionID", SqlDbType.Int);
                        command.Parameters.Add("@SubscriptionDate", SqlDbType.Date);
                        command.Parameters.Add("@CoachID", SqlDbType.Int);
                        command.Parameters.Add("@TrainingTypeID", SqlDbType.Int);
                        command.Parameters.Add("@SubscriptionTypeID", SqlDbType.Int);

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] values = line.Split('\t');

                            command.Parameters["@ClientID"].Value = int.Parse(values[0]);
                            command.Parameters["@SectionID"].Value = int.Parse(values[1]);
                            command.Parameters["@SubscriptionDate"].Value = DateTime.Parse(values[2]);
                            command.Parameters["@CoachID"].Value = int.Parse(values[3]);
                            command.Parameters["@TrainingTypeID"].Value = int.Parse(values[4]);
                            command.Parameters["@SubscriptionTypeID"].Value = int.Parse(values[5]);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void trimRecords()
        {
            string query = "UPDATE Clients  SET PhoneNumber = CONCAT('0', PhoneNumber) WHERE PhoneNumber NOT LIKE '0%'";

            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command= new SqlCommand(query,connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertRecordsFromTextFile(string filePath, string connectionString, string insertQuery)
        {
           // string insertQuery = "INSERT INTO Clients (ClientName, PhoneNumber) VALUES (@Name, @PhoneNumber)";

            using (StreamReader reader = new StreamReader(filePath))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.Add("@ClientName", SqlDbType.NVarChar);
                        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar);

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] values = line.Split('\t');

                            command.Parameters["@ClientName"].Value = values[0];
                            command.Parameters["@PhoneNumber"].Value = values[1];

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void button1_Click_2(object sender, EventArgs e)  
        {
       // string query = "select ClientID, SectionID, CoachID, SubscriptionTypeID ,SubscriptionDate, ActiveState, TrainingTypeID, IsRed from Subscriptions";


           // string filePath = "J:\\Txt files\\Subscriptions.txt";
            // ExportDataToTextFile(connectionString, query, filePath);
            // InsertClientsFromFile(filePath);
          //  InsertSubscriptionsFromFile(filePath);
           //  MoveRecordsToTheNewDatabase();
          

        }

        private void InsertSubscriptionsFromFile(string filePath)
        {
            string query = @"
        INSERT INTO dbo.Subscriptions (
            ClientID, SectionID, CoachID, SubscriptionTypeID, SubscriptionDate, ActiveState, TrainingTypeID, IsRed
        ) VALUES (
            @ClientID, @SectionID, @CoachID, @SubscriptionTypeID, @SubscriptionDate, @ActiveState, @TrainingTypeID, @IsRed
        );";

            using (StreamReader reader = new StreamReader(filePath))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ClientID", SqlDbType.Int);
                        command.Parameters.Add("@SectionID", SqlDbType.Int);
                        command.Parameters.Add("@CoachID", SqlDbType.Int);
                        command.Parameters.Add("@SubscriptionTypeID", SqlDbType.Int);
                        command.Parameters.Add("@SubscriptionDate", SqlDbType.DateTime);
                        command.Parameters.Add("@ActiveState", SqlDbType.Bit);
                        command.Parameters.Add("@TrainingTypeID", SqlDbType.Int);
                        command.Parameters.Add("@IsRed", SqlDbType.Bit);

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] values = line.Split('\t');
                            DateTime subscriptionDate;

                            // Parse the date string to DateTime
                            if (!DateTime.TryParseExact(values[4], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out subscriptionDate))
                            {
                                Console.WriteLine("Invalid date format.");
                                continue; // Skip this line if date format is invalid
                            }

                            command.Parameters["@ClientID"].Value = int.Parse(values[0]);
                            command.Parameters["@SectionID"].Value = int.Parse(values[1]);
                            command.Parameters["@CoachID"].Value = int.Parse(values[2]);
                            command.Parameters["@SubscriptionTypeID"].Value = int.Parse(values[3]);
                            command.Parameters["@SubscriptionDate"].Value = subscriptionDate;
                            command.Parameters["@ActiveState"].Value = bool.Parse(values[5]);
                            command.Parameters["@TrainingTypeID"].Value = int.Parse(values[6]);
                            command.Parameters["@IsRed"].Value = bool.Parse(values[7]);

                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void InsertClientsFromFile(string filePath)
        {
            int rowsAdded = 0;
            string query = "INSERT INTO Clients (ClientName, PhoneNumber) VALUES (@ClientName, @PhoneNumber);";

            using (StreamReader reader = new StreamReader(filePath))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@ClientName", SqlDbType.NVarChar, 255);
                        command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 50);

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] values = line.Split('\t');
                            if (values.Length == 2) // Ensure there are exactly two values
                            {
                                command.Parameters["@ClientName"].Value = values[0];
                                command.Parameters["@PhoneNumber"].Value = values[1];
                                command.ExecuteNonQuery();
                               
                            }
                            else
                            {
                                MessageBox.Show("Skipping line due to incorrect format.");
                            }
                        }
                        MessageBox.Show(rowsAdded + "records added");
                    } 
                }
            }
        }

        private List<int> CountMissingIDs()
        {
            List<int> allClientIds = new List<int>();
            List<int> missingIds = new List<int>();

            string selectQuery = "SELECT ClientID FROM Clients";
           // string insertQuery = "INSERT INTO Clients (ClientID) VALUES (@ClientID)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve all existing ClientIDs
                    using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                    {
                        SqlDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            allClientIds.Add(reader.GetInt32(0));
                        }
                        reader.Close();
                    }

                    // Check for missing IDs
                    for (int i = 1; i <= 2492; i++)
                    {
                        if (!allClientIds.Contains(i))
                        {
                            missingIds.Add(i);
                        }
                    }

                   
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MessageBox.Show("An error occurred: " + ex.Message);
            }

            return missingIds;
        }


        private void MoveRecordsToTheNewDatabase()
        {
            string connectionStringDatabase1= "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Admin\\source\\repos\\WindowsFormsApp1\\WindowsFormsApp1\\Database1.mdf;Integrated Security=True";
            string connectionStringDatabase2= "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Admin\\source\\repos\\WindowsFormsApp1\\WindowsFormsApp1\\NewDatabase.mdf;Integrated Security=True";
            try
            {
                using (SqlConnection targetConnection = new SqlConnection(connectionStringDatabase2))
                {
                    targetConnection.Open();

                    // Enable identity insert for the Clients table
                    using (SqlCommand enableIdentityInsert = new SqlCommand("SET IDENTITY_INSERT Clients ON", targetConnection))
                    {
                        enableIdentityInsert.ExecuteNonQuery();
                    }

                    using (SqlCommand selectCommand = new SqlCommand("SELECT ClientID, ClientName, PhoneNumber FROM Clients", new SqlConnection(connectionStringDatabase1)))
                    {
                        selectCommand.Connection.Open();
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Clients (ClientID, ClientName, PhoneNumber) VALUES (@ClientID, @ClientName, @PhoneNumber)", targetConnection))
                            {
                                insertCommand.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                                insertCommand.Parameters.Add(new SqlParameter("@ClientName", SqlDbType.NVarChar));
                                insertCommand.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar));

                                while (reader.Read())
                                {
                                    insertCommand.Parameters["@ClientID"].Value = reader["ClientID"];
                                    insertCommand.Parameters["@ClientName"].Value = reader["ClientName"];
                                    insertCommand.Parameters["@PhoneNumber"].Value = reader["PhoneNumber"];

                                    insertCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }

                    // Disable identity insert for the Clients table
                    using (SqlCommand disableIdentityInsert = new SqlCommand("SET IDENTITY_INSERT Clients OFF", targetConnection))
                    {
                        disableIdentityInsert.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
          
        }

        public static void ExportDataToTextFile(string connectionString, string query, string filePath)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            // Write column headers
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                writer.Write(reader.GetName(i));
                                if (i < reader.FieldCount - 1)
                                {
                                    writer.Write("\t"); // Separate with tab
                                }
                            }
                            writer.WriteLine();

                            // Write data rows
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    writer.Write(reader[i]);
                                    if (i < reader.FieldCount - 1)
                                    {
                                        writer.Write("\t"); // Separate with tab
                                    }
                                }
                                writer.WriteLine();
                            }
                        }
                    }
                }
            }

            MessageBox.Show("Data exported to file: " + filePath);

        }

        private void bunifuFlatButton43_Click_2(object sender, EventArgs e)
        {
            editPayment editpayment = new editPayment(ClientNames);
            editpayment.Show();
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            MoveDataFromMDFDatabase();
        }

        private void lightTheSideButton(string buttonName)
        {

        }

        private void MoveDataFromMDFDatabase()
        {
            const string mdfConnectionString = "Server=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Program Files (x86)\\sawa\\Database1.mdf;Database=Database1;Trusted_Connection=True;";
            string sqliteConnectionString = "Data Source=C:\\Users\\Admin\\Desktop\\database\\Database1;";

            try
            {
                using (SqlConnection mdfConnection = new SqlConnection(mdfConnectionString))
                {
                    mdfConnection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT ClientId, ClientName, PhoneNumber FROM Clients", mdfConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            using (SQLiteConnection sqliteConnection = new SQLiteConnection(sqliteConnectionString))
                            {
                                sqliteConnection.Open();
                                using (SQLiteTransaction transaction = sqliteConnection.BeginTransaction())
                                {
                                    try
                                    {
                                        using (SQLiteCommand sqliteCommand = new SQLiteCommand("INSERT INTO Clients (ClientId, ClientName, PhoneNumber) VALUES (@id, @name, @phone)", sqliteConnection, transaction))
                                        {
                                            while (reader.Read())
                                            {
                                                sqliteCommand.Parameters.AddWithValue("@id", reader["ClientId"]);
                                                sqliteCommand.Parameters.AddWithValue("@name", reader["ClientName"]);
                                                sqliteCommand.Parameters.AddWithValue("@phone", reader["PhoneNumber"]);
                                                sqliteCommand.ExecuteNonQuery();
                                                sqliteCommand.Parameters.Clear();
                                            }
                                        }
                                        transaction.Commit(); // Commit the transaction
                                    }
                                    catch (Exception ex)
                                    {
                                       // transaction.Rollback(); // Rollback in case of error
                                        MessageBox.Show($"An error occurred: {ex.Message}");
                                        throw; // Re-throw the exception
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to transfer data: {ex.Message}");
            }

        }

    }

}




