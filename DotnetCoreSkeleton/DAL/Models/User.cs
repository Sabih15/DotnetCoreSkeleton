using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }
        public Guid UserPublicId { get; set; }
    }
}
