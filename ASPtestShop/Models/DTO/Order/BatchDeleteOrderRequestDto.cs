using System.Collections.Generic;

namespace ASPtestShop.Models.DTO.Order
{
    public class BatchDeleteOrderRequestDto
    {
        public List<int> OrderIds { get; set; } = new List<int>();
    }
}
