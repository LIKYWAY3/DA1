using System.Collections.Generic;

namespace ASPtestShop.Models.DTO.Order
{
    public class BatchConfirmOrderRequestDto
    {
        public List<int> OrderIds { get; set; } = new List<int>();
    }
}
