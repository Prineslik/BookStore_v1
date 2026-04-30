using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Books
{
    public class BookQueryParameters
    {
        public string? SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        //public int? CategoryId { get; set; }
        public bool? IsInStock { get; set; }
        public string? SortBy { get; set; } = "Id";
        public bool SortDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int MaxPageSize { get; set; } = 100;

        public int GetValidPageSize() => PageSize > MaxPageSize ? MaxPageSize : PageSize;
    }
}
