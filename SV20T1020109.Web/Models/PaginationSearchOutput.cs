namespace SV20T1020109.Web.Models
{
    public class PaginationSearchOutput
    {
        /// <summary>
        /// Trang được hiển thị
        /// </summary>
        public int Page { get; set; } = 1;
        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; } = 0;
        /// <summary>
        /// Gía trị tìm kiếm
        /// </summary>
        public string SearchValue { get; set; } = "";
        /// <summary>
        /// Số dòng dữ liệu tìm được
        /// </summary>
        public int RowCount { get; set; }
        /// <summary>
        /// Số trang
        /// </summary>
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int p = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                {
                    p += 1;
                }
                return p;
            }
        }
    }
}
