# Engineering recruitment test

Welcome to the engineering test. Here at we really value
quality code, and this test has been designed to allow you to show us how 
you think quality code should be written. 

To allow you to focus on the design and implementation of the code we have 
added all the use cases we expect you to implement to the bottom of the 
instructions. In return we ask that you make sure your implementation 
follows all the best practices you are aware of, and that at the end of it, 
the code you submit, is code you are proud of. 

We have not set a time limit, we prefer that you spend some extra time to get
it right and write the highest quality code you can. Please feel free to make
any changes you want to the solution, add classes, remove projects etc.

We are interested in seeing how you approach the task so please commit more
regularly than you normally would so we can see each step and **include
the .git folder in your submission**.

When complete please upload your solution and answers in a .zip and send it to us.

## Programming Exercise - Post Office

You have been tasked with creating a service that calculates the estimated 
despatch dates of customers' orders. 

An order consists of an order date and a collection of products that a 
customer has added to their shopping basket. 

Each of these products is supplied to on demand through a number of 
3rd party suppliers.

As soon as an order is received by a supplier, the supplier will start 
processing the order. The supplier has an agreed lead time in which to 
process the order before delivering it to the Post Office.

Once the Post Office has received all products in an order it is 
despatched to the customer.

**Assumptions**:

1. Suppliers start processing an order on the same day that the order is 
	received. For example, a supplier with a lead time of one day, receiving
	an order today will send it to tomorrow.

2. For the purposes of this exercise we are ignoring time i.e. if a 
	supplier has a lead time of 1 day then an order received any time on 
	Tuesday would arrive at on the Wednesday.

3. Once all products for an order have arrived at from the suppliers,
	they will be despatched to the customer on the same day.

### Part 1 

When the /api/DespatchDate endpoint is hit return the despatch date of that
order.

### Part 2

Post Office staff are getting complaints from customers expecting
packages to be delivered on the weekend. You find out that the post
office is shut over the weekend. Packages received from a supplier on a
weekend will be despatched the following Monday.

Modify the existing code to ensure that any orders received from a supplier
on the weekend are despatched on the following Monday.

### Part 3

The post office is still getting complaints... It turns out suppliers
don't work during the weekend as well, i.e. if an order is received on the
Friday with a lead time of 2 days, would receive and despatch on the
Tuesday.

Modify the existing code to ensure that any orders that would have been 
processed during the weekend resume processing on Monday.

---

Parts 1 & 2 have already been completed albeit lacking in quality. Please
review the code, document the problems you find (see question 1), and refactor
into what you would consider quality code.

Once you have completed the refactoring, extend your solution to capture the 
requirements listed in part 3.

Please note, the provided DbContext is a stubbed class which provides test 
data. Please feel free to use this in your implementation and tests but do 
keep in mind that it would be switched for something like an EntityFramework 
DBContext backed by a real database in production.

While completing the exercise please answer the questions listed below. 
We are not looking for essay length answers. You can add the answers in this 
document.

## Questions

Q1. What 'code smells' / anti-patterns did you find in the existing 
	implementation of part 1 & 2?
	
- Business logic and data access present in an API controller
- No dependency injection; `DbContext` has an interface, but that's pointless when used like `DbContext` currently is (original `line:21`)
- `_mlt` is in the wrong scope (and it certainly doesn't need to be public) (original `line:13`)
- Instantiating a new instance of `DbContext` for every product (original `line:21`)
  - This can cause things like connection pool exhaustion and/or memory leaks (when `DbContext` is replaced with EF)
- Calling the DB (twice) separately for every product on the order, rather than using any kind of batch query or caching (original `line:22-23`)
- Poorly named variables (`_mlt, s, lt`)
- hardcoded ("magic") numbers with no context (original `line:29,31`)
  - Not *that bad* in this case, as the `AddDays()` function does arguably provide context, but it's still a bit of a smell
- No error handling
- No validation on the input parameters
- Inconsistent syntax around the "Part 2" solution `if/else` clause (original `line:27-32`)
- Hard to test in its current state
  - Has to be tested all as one "black box"
  - Existing tests will break easily as soon as we start to make improvements to the method
- No documentation/summary comments
- Everything is synchronous/single-threaded
- No security (authentication or authorisation)
- `DateTime.Now` used instead of `DateTime.UtcNow`
- Using a moving variable like `DateTime.Now/UtcNow` in tests, causing some tests to pass or fail depending on what day of the week it is that they're being run on

Q2. What best practices have you used while implementing your solution?

- Clean architecture and a separation of concerns (to conform to the Single Responsibility Principle)
- Use of interfaces for the business logic and data access layers, providing such advantages as (or foundations for):
  - applying the Open/Closed Principle, although there wasn't a particularly good use case in this project to demonstrate the principle fully
  - the Liskov Substitution Principle, for if we wanted to swap in a different kind of database implementation, for example
  - conforming to the Interface Segregation Principle, but again, the size of the project mean this arguably isn't being demonstrated fully here
  - dependency injection rather than concrete implementations, as per the Dependency Inversion Principle
  - components in the solution are loosely coupled
  - the ability to use mocking for tests
- API input parameters are now being validated
- Basic authentication (a bit meaningless in its current implementation, but worth demonstrating, I think)
- Rate limiting, helping to protect the service from being overwhelmed
- Request/Response models
- Error handling
- Asynchronous programming
- Expanded range of tests

Q3. What further steps would you take to improve the solution given more time?

- Logging and monitoring:
  - Dedicated controller and `HealthCheck` endpoint that would be reporting to a dashboard of some kind
  - Telemetry logging to monitor the broader performance of the service
  - Exception/debug/warning/info log storage for more detailed investigative work
- Global exception handler, allowing for custom exception types and further cleaning up the Get() endpoint (as the try/catch block would no longer need to be there)
- Extend to additional environments (e.g. Testing, Staging)
- Caching (depending on the amount of Supplier/Product data in the system and the amount of memory available to the service)
- Extend calculation logic to include "edge case" things:
  - bank holidays
  - leap years
  - daylight savings time transitions (assuming we would include time in a full version of this service)
- Further testing improvements
  - Integration tests could be extended to work with an actual database (with the help of data seeding mechanisms)
    - On a related point, using hardcoded product IDs in the integration tests like I've done isn't ideal, as what if the lead time of a supplier changes? or the supplier of a product changes? Then the tests break. But solving that problem would be tied in to the data seeding mentioned above
  - Could arguably take "magic numbers" out of tests but in my view moving things like the valid/invalid product IDs out of the tests and into something like a static TestConstants class make the tests less clear in terms of what data is going in to each test case. I think it's one of those cases where there are pros and cons to either approach
  - We could also look to implement a TestStartup class to enable the creation of integration tests for middleware like auth and validation stuff
  - Performance-oriented tests such as:
    - Response times
    - Concurrent requests
    - Memory usage
- Further refinement of business logic:
  - Could the input parameters of `GET /DespatchDate` be replaced with just an order ID? Depending on the overall data flow of the system (i.e. what data do the clients calling this API have?), it would potentially make more sense to require only the Order ID
- Improve end-user API experience:
  - Returning an informative error when 0 product IDs are found
  - When some product IDs are found but some aren't, expanding our response model to attach these "mystery" IDs as unfound, as that may prompt them to investigate whether there's an issue with their data or a bug in the client itself
- Further validation for the input parameters based on business requirements:
  - Limiting the number of product IDs passed (e.g. no more than 200 product IDs per request)
  - Setting a lower limit on the order date (e.g. date must be more recent than 2010-01-01)
  - Validating the format of the date to deal with US style date formats

Q4. What's a technology that you're excited about and where do you see this 
    being applicable? (Your answer does not have to be related to this problem)

~~~~
Maybe a bit cliché at the moment, but for me it's AI coding assistants. They really compliment the way my brain works in that very rarely do I code raw from 
memory, rather I'm regularly researching better techniques or patterns for achieving my current goal. So whereas I used to be going to places like 
StackOverflow or looking through MSDN docs, now I use an AI coding assistant. Which has dramatically increased my productivity, as the former of those is
considerably more time consuming.

Of course there are drawbacks too. Just like an actual person, AI is fallible. But unlike a person (or most people at least) when an AI doesn't 
know the answer, most models have a tendency to confidently make up nonsense. So, for that reason, I never let the AI change the code directly and I don't 
copy-paste any code it gives me.
~~~~

## Request and Response Examples

Please see examples for how to make requests and the expected response below.

### Request

The service is setup as a Web API and takes a request in the following format

~~~~ 
GET /api/DespatchDate?ProductIds={product_id}&orderDate={order_date} 
~~~~

e.g.

~~~~ 
GET /api/DespatchDate?ProductIds=1&orderDate=2018-01-29T00:00:00
GET /api/DespatchDate?ProductIds=2&ProductIds=3&orderDate=2018-01-29T00:00:00 
~~~~

### Response

The response will be a JSON object with a date property set to the resulting 
Despatch Date

~~~~ 
{
    "date" : "2018-01-30T00:00:00"
}
~~~~ 

## Acceptance Criteria

### Lead time added to despatch date  

**Given** an order contains a product from a supplier with a lead time of 1 day  
**And** the order is place on a Monday - 01/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Tuesday - 02/01/2018  

**Given** an order contains a product from a supplier with a lead time of 2 days  
**And** the order is place on a Monday - 01/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Wednesday - 03/01/2018  

### Supplier with longest lead time is used for calculation

**Given** an order contains a product from a supplier with a lead time of 1 day  
**And** the order also contains a product from a different supplier with a lead time of 2 days  
**And** the order is place on a Monday - 01/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Wednesday - 03/01/2018  

### Lead time is not counted over a weekend

**Given** an order contains a product from a supplier with a lead time of 1 day  
**And** the order is place on a Friday - 05/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Monday - 08/01/2018  

**Given** an order contains a product from a supplier with a lead time of 1 day  
**And** the order is place on a Saturday - 06/01/18  
**When** the despatch date is calculated  
**Then** the despatch date is Tuesday - 09/01/2018  

**Given** an order contains a product from a supplier with a lead time of 1 days  
**And** the order is place on a Sunday - 07/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Tuesday - 09/01/2018  

### Lead time over multiple weeks

**Given** an order contains a product from a supplier with a lead time of 6 days  
**And** the order is place on a Friday - 05/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Monday - 15/01/2018  

**Given** an order contains a product from a supplier with a lead time of 11 days  
**And** the order is place on a Friday - 05/01/2018  
**When** the despatch date is calculated  
**Then** the despatch date is Monday - 22/01/2018

# Swagger Instructions

The API uses JWT Bearer token authentication. To use the protected endpoints:

## Using the Swagger UI:
  - Run the solution locally
  - Navigate to http://localhost:5000/swagger
  - Get an authentication token from the GetToken endpoint
  - Click the "Authorize" button (lock icon) at the top of the page
  - In the authorization popup, enter your token in the format: `Bearer your-token-here`
  - Click "Authorize" to save
  - All subsequent API requests will include your authentication token

## Auth for Using API directly:

1. Get an authentication token:
   ```http
   POST http://localhost:5000/api/Auth/token
   ```
   - This endpoint will return a JWT token.

2. Add the `Authorization` header to your requests
   - Format: `Bearer your-token-here`
