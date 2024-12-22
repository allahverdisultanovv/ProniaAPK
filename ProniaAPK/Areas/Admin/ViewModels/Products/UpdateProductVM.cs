﻿using ProniaAPK.Models;
using System.ComponentModel.DataAnnotations;

namespace ProniaAPK.Areas.Admin.ViewModels
{
    public class UpdateProductVM
    {
        public List<ProductImage>? ProductImages { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public IFormFile? HoverPhoto { get; set; }
        public List<IFormFile>? AdditionalPhotos { get; set; }
        public string Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string SKU { get; set; }

        [Required(ErrorMessage = "Category daxil et")]
        public int? CategoryId { get; set; }
        public string Description { get; set; }
        public List<Category>? Categories { get; set; }
        public List<int>? TagIds { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<int>? ColorIds { get; set; }
        public List<Color>? Colors { get; set; }
        public List<int>? SizeIds { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<int>? ImageIds { get; set; }

    }
}
