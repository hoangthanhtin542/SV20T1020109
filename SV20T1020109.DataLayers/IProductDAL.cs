
using SV20T1020109.DomainModels;

namespace SV20T1020109.DataLayers
{
    public interface IProductDAL
    {
        IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
        Product? Get(int productID);
        int Add(Product data);
        bool Update(Product data);
        bool Delete(int productID);
        bool IsUsed(int productID);

        IList<ProductPhoto> ListPhotos(int productId);
        ProductPhoto? GetPhoto(long photoId);
        long AddPhoto(ProductPhoto data);
        bool UpdatePhoto(ProductPhoto data);
        bool DeletePhoto(long photoId);

        IList<ProductAttribute> ListAttributes(int productId);
        ProductAttribute? GetAttribute(long attributeId);
        long AddAttribute(ProductAttribute data);
        bool UpdateAttribute(ProductAttribute data);
        bool DeleteAttribute(long attributeId);
    }
}
