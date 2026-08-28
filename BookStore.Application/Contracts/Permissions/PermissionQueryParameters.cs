using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Application.Contracts.Permissions
{
    public class PermissionQueryParameters
    {
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "Id";
        public bool SortDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int MaxPageSize { get; set; } = 100;

        public int GetValidPageSize() => PageSize > MaxPageSize ? MaxPageSize : PageSize;
    }
}
