using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Model
{
    public class DeputyModel
    {
    }
    public class DeputyViewModel
    {
        public Guid? AccountId { get; set; }
        public Guid PartnerId { get; set; }
        public AccountModel? Account { get; set; }
    }

    public class ViewDeputyModel
    {
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public Guid PartnerId { get; set; }

        public virtual PartnerViewModel? Partner { get; set; }
        public ViewCurrentAccountModel? Account { get; set; }
    }

    public class ViewNameDeputitesModel
    {
        public Guid Id { get; set; }
        public Guid? AccountId { get; set; }
        public Guid PartnerId { get; set; }

        public ViewCurrentAccountModel? Account { get; set; }
    }
    public class CreateDeputyDTO
    {
        public Guid? AccountId { get; set; }
        public Guid PartnerId { get; set; }
    }

    public class UpdateDeputyDTO
    {
        public Guid? AccountId { get; set; }
        public Guid PartnerId { get; set; }
    }
}
