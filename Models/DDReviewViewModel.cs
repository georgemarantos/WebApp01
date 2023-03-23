namespace CleanDDTest.Models
{
    public class DDReviewViewModel
    {
        // user info
        public string firstName { get; set; } = null!;
        public string lastName { get; set; } = null!;
        public string? middleInitial { get; set; }
        public string employeeID { get; set; } = null!;
        public string? netID { get; set; }
        public string? payPeriod { get; set; }

        // bank info
        public int bankAccountID { get; set; }
        public string accountType { get; set; }
        public string routingNumber { get; set; } = null!;
        public string bankName { get; set; } = null!;
        public string cityState { get; set; } = null!;
        public string checkingOrSavings { get; set; }
        public string accountNum { get; set; } = null!;
        public double? dollarAmount { get; set; }
        public DateTime? dateReceived { get; set; }
        public DateTime? dateProcessed { get; set; }

        //voided checks
        public byte[] checkImage { get; set; } = null!;



    }
}
