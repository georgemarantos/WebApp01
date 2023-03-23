using System;
using System.Collections.Generic;

namespace CleanDDTest.Models
{
    public partial class Voidedcheck
    {
        public int BankAcctId { get; set; }
        public string AccountNum { get; set; } = null!;
        public byte[] FileName { get; set; } = null!;
        public string EmpId { get; set; } = null!;
    }
}
