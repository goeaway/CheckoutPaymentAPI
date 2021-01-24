# CheckoutPaymentAPI
This project demonstrates how a payment API gateway might work. Merchants are able to access the API using a provided API key to process payments from their customers
and retrieve historical data from previous transactions.

The project consists of an ASPNET Core web api, running on .net core 3.1, and a .net core library merchants could use to access the API from a .net application.

## How to use

Clone the repo, then pick one of the below options

1. The API can be run in a docker container, follow instructions [here]() to do so. 
2. Open the solution in VS2019 and run from there.

When interacting with the API, add an `X-API-KEY` header to each request with either of the below keys:

`CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ`
`CheckoutPaymentAPI-DemoKey`

## Endpoints

The API utilises swagger to document the possible endpoints of the API, go to `/swagger` to see info on each endpoint.

#### `POST` `Payments/process`

This endpoint allows merchants to submit a new payment, provide the following JSON info in the body of the request:

```
{
  "cardNumber": "string",
  "expiry": "2021-01-23T22:59:32.156Z",
  "amount": "decimal",
  "currency": "string",
  "cvv": "string"
}
```

A successful request will get a response in the format below, where `success` represents if the payment was successfully completed and the `paymentId` represents
a numeric identifier to retrieve info on the payment in future.

```
{
  "success": bool,
  "paymentId": int
}
```

#### `GET` `PaymentDetails/{paymentId}`

This endpoint allows merchants to retrieve information on previously made payments to the API. Provide a paymentId number of a previously made payment.

A successful request will return a response in the below format, where the `cardNumber` and `cvv` are masked for security reasons.

```
{
  "cardNumber": "string",
  "expiry": "2021-01-23T23:05:17.066Z",
  "amount": "decimal",
  "currency": "string",
  "cvv": "string",
  "paymentResult": true
}
```

## Special Features

#### Payment hash caching

After discussion with Pawel, I understood it was important to try and mitigate the possibility of multiple similar requests being sent in quick succession. If a merchant tries to send the same request with the same request parameters, it could be a mistake. To mitigate this, I added an in memory cache to the application, which holds a hash string of the data provided in a process request with a short TTL. Upon new process requests coming in, their hash is checked against the those already in the cache, if they match, the API returns a 429 response and does not proceed with the payment. 

I chose to use an `IMemoryCache`, which is built in to ASPNET Core. The main reason for using this implementation over another was the fact that I knew it would have handling of stale data already included, meaning the API can assign a short TTL to items and they would be removed when time is up automatically. I didn't want to try and role my own implementation and have to manage the removal of data myself, in the interest of time.

I think using a cache would be a good solution in a production app, as it should be able to handle a lot of short lived data, the main downside of this implementation is that it is not distributed, if I wanted to have multiple instances of the API running, they couldn't share cached data between them. To take this further, I'd change to using a distributed cache, such as Redis. I didn't do so this time in the interest of time and to simplify the application.

