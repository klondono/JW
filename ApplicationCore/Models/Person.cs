using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models
{
    public class Person : Base
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleInitial { get; set; }
        public string FormattedName { get; set; }
        public DateTime? BaptismDate { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
        public int? CongregationId { get; set; }
        public int? AddressId { get; set; }

    }
}
