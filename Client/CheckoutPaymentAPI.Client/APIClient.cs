using CheckoutPaymentAPI.Client.Responses;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutPaymentAPI.Client
{
    public class APIClient : IAPIClient
    {
        private readonly HttpClient _client;

        public APIClient(HttpClient client, string apiKey)
        {
            _client = client;
            _client.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }

        public APIClient(string apiKey) : this (new HttpClient { BaseAddress = new Uri("https://localhost:44346/") }, apiKey)
        {
        }

        public async Task<ClientResponse<GetPaymentDetailsResponse>> GetPaymentDetails(int paymentId)
        {
            var response = await _client.GetAsync($"/paymentdetails/{paymentId}");

            var clientResponse = new ClientResponse<GetPaymentDetailsResponse>
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
                clientResponse.Error = JsonConvert.DeserializeObject<ClientError>(responseContent);
            }

            return clientResponse;

        }

        public async Task<ClientResponse<ProcessPaymentResponse>> ProcessPayment(
            string cardNumber, 
            DateTime expiry, 
            decimal amount, 
            string currency, 
            string cvv)
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

            var response = await _client.PostAsync("/payments/process", content);
            
            var clientResponse = new ClientResponse<ProcessPaymentResponse>
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
                clientResponse.Error = JsonConvert.DeserializeObject<ClientError>(responseContent);
            }

            return clientResponse;
        }
    }
}
