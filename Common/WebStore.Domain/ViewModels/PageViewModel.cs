﻿using System;

namespace WebStore.Domain.ViewModels
{
    public class PageViewModel
    {
        public int Page { get; set; }
        
        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        /// <summary>
        /// Округление вверх (TotalItems / PageSize)
        /// </summary>
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}
