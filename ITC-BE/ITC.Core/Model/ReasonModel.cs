using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Model
{
    public class ReasonModel
    {
    }
    public class ReasonViewModel
    {
        public Guid? Id { get; set; }
        public string? ReasonContent { get; set; }
        public DateTime DateCreated { get; set; }


        //FK
        public Guid? DeputyId { get; set; }
        public int? SlotId { get; set; }

        public ViewNameDeputitesModel? Deputies { get; set; }
    }
    public class CreateReasonModel
    {
        public string? ReasonContent { get; set; }
        public int? SlotId { get; set; }
    }
    
}
