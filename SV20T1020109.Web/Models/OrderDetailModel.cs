using SV20T1020109.DomainModels;

namespace SV20T1020109.Web.Models
{
    public class OrderDetailModel
    {
        public Orders Order { get; set; }
        public List<OrderDetails> Details { get; set; }

    }
}
