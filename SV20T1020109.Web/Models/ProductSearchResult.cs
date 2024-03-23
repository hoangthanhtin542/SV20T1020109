using SV20T1020109.DomainModels;
using SV20T1020109.Web.Models;

namespace SV20T1020109.Web.Models
{
    public class ProductSearchResutl : BasePaginationResult
    {
        public List<Product> Data { get; set; }

    }
}
