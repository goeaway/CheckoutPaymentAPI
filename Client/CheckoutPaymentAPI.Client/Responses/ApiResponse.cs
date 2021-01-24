using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CheckoutPaymentAPI.Client
{
    public class ApiResponse<TData>
    {
        public HttpStatusCode StatusCode { get; set; }
        public TData Data { get; set; }
        public ApiError Error { get; set; }
    }
}
