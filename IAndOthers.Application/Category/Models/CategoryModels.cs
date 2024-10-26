using IAndOthers.Core.Mvc.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IAndOthers.Application.Category.Models
{
    public class CategoryCreateModel
    {
        [IODropDown("CategoryList")]
        [UIHint("DropDown")]
        [Display(Name = "Parent Category")]
        public long? ParentCategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        
    }

    public class CategoryEditModel : CategoryCreateModel
    {
        [HiddenInput]
        public long Id { get; set; }
    }

    public class CategoryViewModel : CategoryEditModel
    {
        public string ParentCategoryName { get; set; }
        public bool Deleted { get; set; }
    }
}
