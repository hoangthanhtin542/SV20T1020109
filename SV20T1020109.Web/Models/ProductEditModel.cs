using SV20T1020109.DomainModels;

namespace SV20T1020109.Web.Models
{
    public class ProductEditModel
    {
        public Product data { get; set; }

        public List<ProductAttribute> ListProductAttribute { get; set; }

        public List<ProductPhoto> ListProductPhoto { get; set; }
    }



}
