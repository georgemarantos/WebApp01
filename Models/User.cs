using System;
using System.Collections.Generic;

namespace CleanDDTest.Models
{
    public partial class User
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleInit { get; set; }
        public string EmpId { get; set; } = null!;
        public string? NetId { get; set; }
        public string? PayPeriod { get; set; }
    }
}
