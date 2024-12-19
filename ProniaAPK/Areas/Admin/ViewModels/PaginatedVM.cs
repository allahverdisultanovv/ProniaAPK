﻿namespace ProniaAPK.Areas.Admin.ViewModels
{
    public class PaginatedVM<T>
    {
        public double TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public List<T> ItemVMs { get; set; }
    }
}
