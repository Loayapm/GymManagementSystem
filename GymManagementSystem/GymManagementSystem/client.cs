using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1;

namespace WindowsFormsApp1
{
    public class Client
    {
        public int id { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public DateTime date { get; set; }
        public string section { get; set; }
        public int monthsOfSubscription { get; set; }
        public DateTime LastPaymentDate { get; set; } //in payment table
        public DateTime SubscriptionDate { get; set; }
        public long TotalSubscriptionAmount { get; set; }
        public long RemainingAmount { get; set; } //in payment table




    }

}

