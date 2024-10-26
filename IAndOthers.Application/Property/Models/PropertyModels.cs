using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IAndOthers.Application.Property.Models
{
    public class PropertyCreateModel
    {
        [Display(Name = "Key")]
        public string Key { get; set; }
        [Display(Name = "Value")]
        public string Value { get; set; }

    }

    public class PropertyEditModel : PropertyCreateModel
    {
        [HiddenInput]
        public long Id { get; set; }
    }

    public class PropertyViewModel : PropertyEditModel
    {
        public bool Deleted { get; set; }
    }
}
