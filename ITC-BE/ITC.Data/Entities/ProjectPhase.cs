using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Data.Entities
{
    public class ProjectPhase
    {
        [Key]
		public Guid? Id { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool Status { get; set; }

        public Guid? ProjectId { get; set; }
        public int? PhaseId { get; set; }
       
        public virtual Phase? Phase { get; set; }
        public virtual Project? Project { get; set; }
        public virtual ICollection<Tasks>? Tasks { get; set; }
    }
}
