using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Models.DTOs
{
    /// <summary>
    /// Represents the body of the response the API returns when an error occurs
    /// </summary>
    public class ErrorResponseDTO
    {
        /// <summary>
        /// Gets or sets the message for the error
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets a collection of extra error information
        /// </summary>
        public List<string> Errors { get; set; }
            = new List<string>();
    }
}
