namespace SV20T1020109.DomainModels
{
    public class ProductAttribute
    {
        public long AttributeId { get; set; }
        public int ProductId { get; set; }
        public string AttributeName { get; set; } = "";
        public string AttributeValue { get; set; } = "";
        public int DisplayOrder { get; set; }
    }
}
