using Microsoft.AspNetCore.Mvc;

namespace OKNODOM.DTOs
{
    public class OrderConfigurationPostModel
    {
        public int OrderId { get; set; }
        public Dictionary<int, int> Products { get; set; } = new();
        public Dictionary<int, int> Services { get; set; } = new();
        public Dictionary<string, int> WindowToOpening { get; set; } = new();
        [FromForm]
        public Dictionary<int, decimal> CustomProductPrices { get; set; } = new();
        [FromForm]
        public Dictionary<int, decimal> CustomServicePrices { get; set; } = new();
    }
}
