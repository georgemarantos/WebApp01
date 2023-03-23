using System;
using System.Collections.Generic;

namespace CleanDDTest.Models
{
    public partial class Bankaccount
    {
        public int Id { get; set; }
        public string AccountType { get; set; }
        public string RoutingNumber { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string CityState { get; set; } = null!;
        public string Account { get; set; }
        public string EmpId { get; set; } = null!;
        public string AccountNum { get; set; } = null!;
        public double? DollarAmount { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? DateProccessed { get; set; }
    }
}
