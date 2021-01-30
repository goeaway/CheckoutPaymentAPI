using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Models.DTOs
{
    /// <summary>
    /// Represents the body of the response the API returns when an error occurs
    /// </summary>
    public class ErrorResponseDTO
    {
        /// <summary>
        /// Gets or sets an Http status code for this error response
        /// </summary>
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
            = HttpStatusCode.BadRequest;
        /// <summary>
        /// Gets or sets the message for the error
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets a collection of extra error information
        /// </summary>
        public IEnumerable<string> Errors { get; set; }
            = new List<string>();
    }
}
