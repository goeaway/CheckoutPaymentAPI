using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Models.DTOs
{
    public class ErrorResponseDTO
    {
        public string Message { get; set; }
        public List<string> Errors { get; set; }
            = new List<string>();
    }
}
