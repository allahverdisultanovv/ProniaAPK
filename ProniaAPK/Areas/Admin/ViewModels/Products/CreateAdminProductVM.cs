﻿using ProniaAPK.Models;
using System.ComponentModel.DataAnnotations;

namespace ProniaAPK.Areas.Admin.ViewModels
{
    public class CreateAdminProductVM
    {
        public string Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string SKU { get; set; }

        [Required(ErrorMessage = "Category daxil et")]
        public int? CategoryId { get; set; }
        public List<int>? TagIds { get; set; }
        public string Description { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }


    }
}
