using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _001TN0172.Models
{
    public class Pager
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public Pager()
        {
        }

        public Pager(int totalItems, int page, int pageSize = 4)
        {
            //int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            //int currentPage = page;

            //int startPage = currentPage - 5;
            //int endPage = currentPage + 4;

            TotalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            CurrentPage = page;

            StartPage = CurrentPage - 5;
            EndPage = CurrentPage + 4;

            if (StartPage <= 0)
            {
                EndPage = EndPage - (StartPage - 1);
                StartPage = 1;
            }

            if (EndPage > TotalPages)
            {
                EndPage = TotalPages;
                if (EndPage > 4)
                {
                    StartPage = EndPage - 9;
                }
            }

            TotalItems = totalItems;
            PageSize = pageSize;
        }

    }
}
