# CheckoutPaymentAPI
This project demonstrates how a payment API gateway might work. Merchants are able to access the API using a provided API key to process payments from their customers
and retrieve historical data from previous transactions.

The project consists of an ASPNET Core web api, running on .net core 3.1, and a .net core library merchants could use to access the API from a .net application.

### How to use

Clone the repo, then pick one of the below options

1. The API can be run in a docker container, follow instructions [here]() to do so. 
2. Open the solution in VS2019 and run from there.

When interacting with the API, add an `X-API-KEY` header to each request with either of the below keys:

`CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ`
`CheckoutPaymentAPI-DemoKey`

### Endpoints

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

