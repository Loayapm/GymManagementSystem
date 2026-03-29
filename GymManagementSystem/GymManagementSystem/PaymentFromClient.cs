using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class PaymentFromClient
    {
        public int PaymentID { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public int TotalPaymentAmount { get; set; }
        public int Payment { get; set; }
        public int RemainingPaymentAmount { get; set; }

    }
}
