using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IAndOthers.Core.Data.Entity
{
    public abstract class IOEntityBase : IIOEntity
    {
        [Key]
        [HiddenInput]
        public virtual long Id { get; set; }
    }
}
