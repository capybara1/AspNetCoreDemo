# ASP.NET Core

Demo Code with Examples for educational purpose

# Web Service Characteristics
  - Loose coupling
  - Dynamic binding
  - Utilization of (open) standards
  - Deployment
  - Shifts importance of Software Architects
  - Higher invest in comparison to the usage of libraries
  - Higher complexity in comparison to the usage of libraries
    - Availability
    - Security
    - Versioing
    - Performance

# REST

- Acronym for `Representational State Transfer`
- Dissertation of Roy Fielding (2000): [Architectural Styles and the Design of Network-based Software Architectures](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm)
- RESTfulness is decined by constraints
  - Client-Server
  - Stateless
  - Cache
  - Uniform Interface
    - Identification of resources
      
    ![Resource Based Design](Resource_Based_Design.svg)
    - Manipulation of resources through representations
    - Self-descriptive messages
    - Hypermedia as the engine of application state
  - Layered System
    
    ![Layered System](Layered_system.svg)
  - Code-On-Demand (optional)
- Richardson Maturity Model
  - See [article of Martin Fowler](https://martinfowler.com/articles/richardsonMaturityModel.html)
- Defferentiation to SOAP
  - Soap is considered RPC
  - Soap works without HTTP
    - Some HTTP Features are duplicated to remove dependencies
    - Tooling for HTTP cannot be used
      - Only used verb is POST
      - Informations in body
- HTTP Verbs
  - Overview
    - [HTTP 1.1 - Sec. 9](https://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html)
    - [Using HTTP Methods for RESTful Services](http://www.restapitutorial.com/lessons/httpmethods.html)
  - Importent properties
    - Safety
    - Idempotence
- Status Codes
  - Overview
    - [HTTP 1.1 - Sec. 10](https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html)
  - Most important
    - 200 OK
    - 201 Created
    - 202 Accepted
    - 204 No Content
    - 400 Bad Request
    - 401 Unauthorized
    - 403 Forbidden
    - 404 Not Found
    - 405 Method Not Allowed
    - 500 Internal Server Error
    - 503 Service Unavailable

# Versioning

https://github.com/Microsoft/aspnet-api-versioning