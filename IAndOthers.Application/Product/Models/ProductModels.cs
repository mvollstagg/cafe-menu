using IAndOthers.Core.Mvc.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IAndOthers.Application.Product.Models
{
    public class ProductCreateModel
    {
        [IODropDown("CategoryList")]
        [UIHint("DropDown")]
        [Display(Name = "Category")]
        public long CategoryId { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Media")]
        public IFormFile Media { get; set; }

        [UIHint("CheckBoxList")]
        [IOMultiCheckBox("ProductPropertiesList")]
        [Display(Name = "Properties")]
        public List<long> SelectedProductPropertyIds { get; set; }
    }

    public class ProductEditModel
    {
        [UIHint("Hidden")]
        public long Id { get; set; }

        [UIHint("Hidden")]
        public string? ImagePath { get; set; }

        [IODropDown("CategoryList")]
        [UIHint("DropDown")]
        [Display(Name = "Category")]
        public long CategoryId { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Media")]
        public IFormFile? Media { get; set; }

        [UIHint("CheckBoxList")]
        [IOMultiCheckBox("ProductPropertiesList")]
        [Display(Name = "Properties")]
        public List<long>? SelectedProductPropertyIds { get; set; }
    }

    public class ProductViewModel : ProductEditModel
    {
        public string ImagePath { get; set; }
        public string CategoryName { get; set; }
        public List<string> PropertyNames { get; set; }
    }
}
