namespace SV20T1020109.DomainModels
{
    public class ProductPhoto
    {
        public long PhotoId { get; set; }
        public int ProductId { get; set; }
        public string Photo { get; set; } = "";
        public string Description { get; set; } = "";
        public int DisplayOrder { get; set; }
        public bool IsHidden { get; set; }
    }
}
