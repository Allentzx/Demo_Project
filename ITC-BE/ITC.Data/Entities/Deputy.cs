using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Data.Entities
{
    public class Deputy
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public Guid PartnerId { get; set; }

        public virtual Partner? Partner { get; set; }
        public Account? Account { get; set; }

    }
}
