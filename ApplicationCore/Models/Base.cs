using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models
{
    public class Base
    {
        public bool? IsActive { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public int? UserAdded { get; set; }
        public int? UserModified { get; set; }
    }
}
