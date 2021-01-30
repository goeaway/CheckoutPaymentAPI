# CheckoutPaymentAPI
This project demonstrates how a payment API gateway might work. Merchants are able to access the API using a provided API key to process payments from their customers
and retrieve historical data from previous transactions.

The project consists of an ASPNET Core web api, running on .net core 3.1, and a .net core library merchants could use to access the API from a .net application.

## Feedback changes

Some feedback points and what I did below:
* Card number validation should not be delegated - For this I implemented my own Luhn algorithm in the validator to ensure card numbers are correct
* CVV should only allow digits - This was an oversight before and has been rectified by adding more validation rules for the CVV
* Amount should be positive - I had originally assumed that a negative amount would be okay, but I've changed the API to only allow positive values
* Expiry should be month/year only - I originally used a DateTime object but after feedback I've changed this to be an object with month and year integer properties
* Validation errors should not be implemented as exceptions - This change was a bit harder to make as the API didn't really make it easy to allow for two different types of return object. I found [this article](https://medium.com/the-cloud-builders-guild/manual-validation-in-the-business-layer-without-exceptions-846712497cc2) online about how to implement a flow to choose between the normal response and an error response. This worked well and so I've removed the exceptions and handling from the Startup.cs and am using this instead.
* Swagger UI try it out - I re-enabled the try it out feature and updated the Swagger config to add the `X-API-KEY` header to try it out requests
* Payment result should be a string message - I updated the acquiring bank service to return a status value, I then updated the processed payment entity to store a string version of this status instead.
* Endpoints could be more restful - I removed the `PaymentDetailsController` and updated the endpoints.

## How to use

Clone the repo, then pick one of the below options

1. The API can be run in a docker container, follow instructions [here](https://github.com/goeaway/CheckoutPaymentAPI/blob/master/Deploy/README.md) to do so. 
2. Open the solution in VS2019 and run the `CheckoutPaymentAPI` project from there.

It's best to use an application like Postman to interact with the API. When interacting with the API, add an `X-API-KEY` header to each request with either of the below keys:

`CheckoutPaymentAPI-Q2hlY2tvdXRQYXltZW50QVBJ`
`CheckoutPaymentAPI-DemoKey`

## Endpoints

The API utilises swagger to document the possible endpoints of the API, go to `/swagger` while the API is running to see info on each endpoint.

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

## About the API

The application is built with ASPNET Core 3.1 and implements a clean architecture style. The API is split into 5 projects:

* `CheckoutPaymentAPI` - this is the ASPNET Core web api project which receives requests and provides responses. The project holds Startup code and Controllers to define the endpoints.
* `CheckoutPaymentAPI.Application` - this is a .NET Core library and contains the handling code for each request.
* `CheckoutPaymentAPI.Models` - this is another .NET Core library that holds all the models used in the application, such as DTOs, Entities or models used in handlers.
* `CheckoutPaymentAPI.Core` - this library project holds "low level" classes that can be used throughout the application, such as exceptions and an `INowProvider`, which I've utilised to make mocking of `DateTime.Now` easier.
* `CheckoutPaymentAPI.Persistence` - this library holds the EntityFramework Core Context used by the application

The `CheckoutPaymentAPI.Application` project uses a library called Mediatr to implement a CQRS style architecture, where commands and queries are split up. This architecture makes it really easy to add new request flows, as each one is separate from the others and won't be affected by new ones being added. In my API, each request has its own folder, with a request object to hold data about the request, a handler which will receive the request object and return a response, and a validator that ensures the request object is in a particular state. Setting up the API this way also makes it really easy to decouple the implementations from the ASPNET Core controllers, which makes testing a lot easier and means we could change to a different way of setting up the API in future.

I used EntityFrameworkCore to manage storage of data, although at this time the application only uses an InMemoryDatabase. EntityFrameworkCore makes swapping out different database providers quite easy, so replacing the in memory database usage with a SQL Server one, for example, would be easy to do. The main reason I used InMemoryDatabase was to simplify the application.

To validate requests, I used the FluentValidation nuget package, which goes nicely with Mediatr and can be slotted into the Mediatr pipeline by using behaviours. This package makes managing validations on models really easy and can be easily tested in isolation. I added validation to ensure each part of the process payment request is present. I added further validation to ensure:
* The card number is a valid card number (validation provided by FluentValidation)
* The CVV is 3 digits long, from information I found online, all CVVs these days are 3 digits long
* The expiry is not in the past. I chose to represent the expiry as a full date instead of just Month/Year, because I was unsure if the expiry is always represented like this. At present, the validation could fail for a valid date if the current date is newer than the expiry, even if they're in the same month. Going forward, it may be better to represent the expiry as Month/Year. I know for my card at least, it doesn't expire until the last day of the month.
* The amount is not 0. Negative and positive values are allowed, but I assumed it shouldn't be possible to process a 0 amount request. It seemed like an unnecessary waste of resources to go through the whole process in this case.
* The currency should be an ISO 4217 recognised currency code. For this application I simply added the most common codes to a list for validation, but a real application should have all recognised codes.

New payments that come in are sent off to the Acquiring Bank via a class service in the process payment handler. The result of this is then stored, along with partially masked data of the request, in the application's DB. I chose to mask the card number and cvv here as I felt the DB shouldn't be holding sensitive information like this, like you would not store plain text passwords.

## Special Features

### Payment hash caching

After discussion with Pawel, I understood it was important to try and mitigate the possibility of multiple similar requests being sent in quick succession. If a merchant tries to send the same request with the same request parameters, it could be a mistake. To mitigate this, I added an in memory cache to the application, which holds a hash string of the data provided in a process request with a short TTL. Upon new process requests coming in, their hash is checked against the those already in the cache, if they match, the API returns a 429 response and does not proceed with the payment. When a payment is added to the DB, we add it to the cache for a short period (at the moment it's only a minute, but the time can be changed via app config). I chose to cache both successful payments and failed, but I may be okay to not cache failed payments.

I chose to use an `IMemoryCache`, which is built in to ASPNET Core. The main reason for using this implementation over another was the fact that I knew it would have handling of stale data already included, meaning the API can assign a short TTL to items and they would be removed when time is up automatically. I didn't want to try and roll my own implementation and have to manage the removal of data myself, in the interest of time. 

An issue occurred with this where I needed to use a slightly more complex version of the API than expected, because the normal `Set` method is actually an extension method. This caused a problem when testing using the Moq nuget package, because it cannot mock extension methods. I opted to use the slightly more complex API of the `IMemoryCache`, but looking back I should have created an adapter abstraction which the handlers could depend on, and then within that adapter make the call. This would help in two ways, one; I could mock the adapter when testing the handler and two; when the time comes to use a real cache, I wouldn't have to make changes to the handler, just create a new adapter implementation.

I think using a cache would be a good solution in a production app, as it should be able to handle a lot of short lived data, the main downside of this implementation is that it is not distributed, if I wanted to have multiple instances of the API running, they couldn't share cached data between them. To take this further, I'd change to using a distributed cache, such as Redis. I didn't do so this time in the interest of time and to simplify the application.

### Authentication via API key

A real application may want to use authentication methods like OAuth/OpenId connect, but in the interest of time and complexity, I opted for a really simple API key approach. Where merchants are provided an API key ahead of time, and then they add this in each request made to the API, otherwise they receive a 401 status code. To implement this, I used the AspNetCore.Authentication.ApiKey nuget package, which provides a straightforward flow for you to authenticate users with. In this demo application, I simply hardcoded a couple of API keys in an IApiKeyRepository, but a better solution might be to load keys into the secret manager instead.

### Application logging

The application makes use of Serilog to log events to a rolling file. All application errors are logged but some important actions, such as saving a new payment, are logged too. Find the log in the application's executing directory, or with the command shown in the [deploy README](https://github.com/goeaway/CheckoutPaymentAPI/blob/master/Deploy/README.md) if you're running in docker.

### API Client

I created a separate .net core library to house an API client for the API. This could be published as a nuget package and then used by merchants in their own .net solutions. The package includes an `APIClient` class, which implements the `IAPIClient` interface. This class provides two methods for merchants to use `GetPaymentDetails` and `ProcessPayment`. These methods use `HttpClient` under the hood and return typed versions of the possible responses from the API. I originally intended to use RestSharp instead of HttpClient, but opted for the latter for it's simpler API.

### Docker support

The application can be built into a docker image and then easily installed on different systems. I am able to run an instance of the API in a docker container on my windows PC using Docker for Windows. Using docker enables the app to be installed anywhere docker is supported. It also allows us to scale the application better, we could spin up multiple instances of the API and then share user traffic between them by using a load balancer.

In its current state, a couple of changes should be made first however. 
* We should update the app to not use an EntityFrameworkCore InMemoryDatabase and instead use a real DB provider, such as SQL Server. This could be housed in a separate docker container, and scaled separately.
* We should also update the app to use a separate cache, instead of the in memory cache we're currently using. We could house a Redis cache in another docker container, and again scale that separately.

## Testing

The testing of the API is split into three main projects 
1. `CheckoutPaymentAPI.Tests.API.Unit` - which holds unit tests for the API project. This ensures validation and the handlers are doing what we want them to.
2. `CheckoutPaymentAPI.Tests.API.Integration` - which holds integration tests for the API project. This ensures the API is returning the correct response for different requests, such as 401 when not authenticated, or 429 when the same process payment request is sent multiple times.
3. `CheckoutPaymentAPI.Tests.Client` - which holds unit tests for the API client project.

The solution also contains `CheckoutPaymentAPI.Tests.Core`, which contains a `Setup` class used by the test projects to create the context and test server.

Within the `CheckoutPaymentAPI.Tests.API.Unit` project, I split the tests up between the two different features (processing payments and getting details) and subdivided into tests that ensure the handler is working correctly, and tests that ensure validation is working.

When writing tests, I like to follow the Arrange, Act, Assert principle to ensure my tests follow a known pattern. I also try to stick to having a single Assert statement per test, although I'm willing to add a few extras if it makes sense (such as when testing all the properties of a returned object are correct).

I also aim to avoid writing logic into tests, such as loops and if statements. Ideally, each test will just call a single method, and then assert the result of that call. I liked to have all input values stored in const values with easily recognisable names by naming them with uppercase names. I think this makes it easier to read all of the inputs at a glance, and using the constants ensures I'm not accidentally misspelling string values later on in the test.

Most of the tests make use of the `Setup` class from the `CheckoutPaymentAPI.Tests.Core` project. I like to call methods like this to setup dependencies in tests instead of creating test-class-wide setup and teardown methods. It makes each test independent of each other, which reduces the chance of a test failing when it shouldn't, and allows newcomers to see everything involved in the test just by looking at it. 

I made use of the Moq nuget package throughout the testing suite, as it allows you to mock certain dependencies to ensure they provide a certain result. I made good use of this when testing the payment process handler when I wanted the IAcquiringBank to return a particular result. I find mocking greatly improves test quality. It can be hard to do it if the system under test doesn't use Dependency Injection though, so it's extremely important that all parts of the API support that (which I think they do).

I also used the Microsoft.AspNetCore.TestHost nuget package, which allows me to create a test server within the testing environment. This means I can test the API in a very similar way to how it will actually be used in production. The package enables me to send requests to the API with an HttpClient and get real responses back. This enables me to have integration tests, to ensure the API is hooked up correctly with validation, handlers, cache and database.

## Future Changes

As mentioned earlier. The application uses an in memory cache and an in memory database. The first step I would take in order to improve it would be to change these into implementations that can be separated from the application, to make it easier to scale the API application separately if it needs to. As the application can be deployed in a docker container, I'd add containers for the DB and cache as well, and then I could manage them all much easier. 

I'd also change the `IMemoryCache` usage to have an adapter abstraction in front of it, for slightly improved testing experience and to make it easier to change cache providers in the future.

As well as that an actual implementation for the acquiring bank would be needed. In this application, this is represented as an interface whose implementation can be easily swapped in and out. So a new derived class that actually interacts with a real bank would be needed. This would only require adding the new implementation, then updating the Startup class in the `CheckoutPaymentAPI` project to use that instead of the current class. I may also add a separate project to the solution to house the Acquiring bank service and any similar services that may be needed in future, such as an email sending service. In clean architecture, this might be known as `CheckoutPaymentAPI.Infrastructure`.

When running in production, we may want to log events to somewhere other than a file on disk. Serilog has many options where the events can be logged to, such as AWSCloudWatch. We can easily choose which "sink" (or sinks) we log to in the startup class.

I'd also update the authentication system to not have hardcoded api keys in the code. I'd probably store the keys in the database, but would load them into a secret manager on startup to access them easier while the API is running.

Deployment of this application is fairly straightforward at the moment, but when moving to a production environment, with separate cache, DB or other services it will become a lot more complicated. I would set up a Continuous Integration system that allows me to automatically build, test, and publish the application when I want to release a new version of it. In the past I've used Azure Dev Ops to do this to good effect. I've been able to release projects to Azure, AWS, a raspberry pi and a few other places. I really like CI because it allows you to build a lot of trust with the release process. Trying to manually update lots of different parts can get very tricky very quickly, especially if there are multiple environments involved.

Finally, I considered trying to implement an EventSourcing architecture to store changes to data in an event log instead of just the latest version of the data. I would be more inclined to do this if the API had a requirement to allow existing data to be updated, but I felt for the current requirements this wasn't needed, as no data is overwritten at all. If new requirements were to come in for this API to allow for something like that I'd definitely revisit the possibilty of adding EventSourcing.
