using CheckoutPaymentAPI.Client.Responses;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Client
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;

        public ApiClient(HttpClient client, string apiKey)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }

        public ApiClient(string apiKey) : this (new HttpClient { BaseAddress = new Uri("http://localhost:8080/") }, apiKey)
        {
        }

        public async Task<ApiResponse<GetPaymentDetailsResponse>> GetPaymentDetails(int paymentId)
        {
            // make request using http client
            var response = await _client.GetAsync($"/payments/{paymentId}");

            var clientResponse = new ApiResponse<GetPaymentDetailsResponse>
            {
                StatusCode = response.StatusCode
            };

            var responseContent = await response.Content.ReadAsStringAsync();

            // if response is success, populate data with response.data, otherwise populate error with it
            if (response.IsSuccessStatusCode)
            {
                clientResponse.Data = JsonConvert.DeserializeObject<GetPaymentDetailsResponse>(responseContent);
            }
            else
            {
                clientResponse.Error = JsonConvert.DeserializeObject<ApiError>(responseContent);
            }

            return clientResponse;

        }

        public async Task<ApiResponse<ProcessPaymentResponse>> ProcessPayment(
            string cardNumber, 
            string cvv,
            MonthYear expiry, 
            decimal amount, 
            string currency)
        {
            if(cardNumber == null)
            {
                throw new ArgumentNullException(nameof(cardNumber));
            }

            if(currency == null)
            {
                throw new ArgumentNullException(nameof(currency));
            }

            if(cvv == null)
            {
                throw new ArgumentNullException(nameof(cvv));
            }

            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                CardNumber = cardNumber,
                CVV = cvv,
                Expiry = expiry,
                Amount = amount,
                Currency = currency
            }), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/payments", content);
            
            var clientResponse = new ApiResponse<ProcessPaymentResponse>
            {
                StatusCode = response.StatusCode
            };

            var responseContent = await response.Content.ReadAsStringAsync();
            
            // if response is success, populate data with response.data, otherwise populate error with it
            if (response.IsSuccessStatusCode)
            {
                clientResponse.Data = JsonConvert.DeserializeObject<ProcessPaymentResponse>(responseContent);
            }
            else
            {
                clientResponse.Error = JsonConvert.DeserializeObject<ApiError>(responseContent);
            }

            return clientResponse;
        }
    }
}
