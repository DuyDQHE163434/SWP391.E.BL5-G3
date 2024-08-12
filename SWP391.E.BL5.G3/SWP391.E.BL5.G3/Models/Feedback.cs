using System;
using System.Collections.Generic;

namespace SWP391.E.BL5.G3.Models
{
    public partial class Feedback
    {
        public Feedback()
        {
            InverseParent = new HashSet<Feedback>();
        }

        public int FeedbackId { get; set; }
        public int? ParentId { get; set; }
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Feedback Parent { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Feedback> InverseParent { get; set; }
    }
}
