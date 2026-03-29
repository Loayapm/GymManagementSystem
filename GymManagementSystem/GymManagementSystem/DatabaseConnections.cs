using System.Configuration;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public static class DatabaseConnections
    {
        public static string GymDB =>
            ConfigurationManager.ConnectionStrings["Database1"].ConnectionString;

        public static string UsersDB =>
            ConfigurationManager.ConnectionStrings["Database2"].ConnectionString; 

        public static SqlConnection GetGymConnection()
        {
            return new SqlConnection(GymDB);
        }

        public static SqlConnection GetUsersConnection()
        {
            return new SqlConnection(UsersDB);
        }
    }
}