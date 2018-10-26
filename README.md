# ASP.NET Core

Demo Code with Examples for educational purpose

# Overview

![ASP.NET Core](AspNetCore.svg)

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

# Comparison to Alternative Technologies

## SOAP
  - Soap is considered RPC
  - Soap works without HTTP
    - Some HTTP Features are duplicated to remove dependencies
    - Tooling for HTTP cannot be used
      - Only used verb is POST
      - Informations in body
# HTTP

## Verbs
  - Overview
    - [HTTP 1.1 - Sec. 9](https://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html)
    - [Using HTTP Methods for RESTful Services](http://www.restapitutorial.com/lessons/httpmethods.html)
  - Importent properties
    - Safety
    - Idempotence

## Status Codes
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

# REST

## Architectural Style

- Acronym for `Representational State Transfer`
- Dissertation of Roy Fielding (2000): [Architectural Styles and the Design of Network-based Software Architectures](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm)
- RESTfulness of an architecture is defined by constraints
  - [Client-Server](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm#sec_5_1_2)
  - [Stateless](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm#sec_5_1_3)
  - [Cache](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm#sec_5_1_4)
  - [Uniform Interface](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm#sec_5_1_5)
    - [Identification of resources](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm#sec_5_2_1_1)
    ![Resource Based Design](Resource_Based_Design.svg)
    - [Manipulation of resources through representations](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm#sec_5_2_1_2)
    - [Self-descriptive messages](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm#sec_5_3_1)
      - Interaction is stateless between requests
      - Standard methods used
      - Media types are used to indicate semantics and exchange information
      - Responses explicitly indicate cacheability
    - Hypermedia as the engine of application state
  - [Layered System](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm#sec_5_1_6)
    ![Layered System](Layered_system.svg)
  - [Code-On-Demand](https://www.ics.uci.edu/~fielding/pubs/dissertation/rest_arch_style.htm#sec_5_1_7) (optional)
- See also [REST APIs must be hypertext-driven](https://roy.gbiv.com/untangled/2008/rest-apis-must-be-hypertext-driven)

## Richardson Maturity Model

See [article of Martin Fowler](https://martinfowler.com/articles/richardsonMaturityModel.html)

## Guidelines

In alphabetical order:
- [apistylebook.com](http://apistylebook.com/design/guidelines/zalando-restful-api-guidelines)
- [API Design Guide](https://cloud.google.com/apis/design/)
  - Maintained by Google
  - Current version released in February 2017
- [hackernoon.com](https://hackernoon.com/restful-api-designing-guidelines-the-best-practices-60e1d954e7c9)
- [Microsoft REST API Guidelines](https://github.com/Microsoft/api-guidelines)
  - Maintained by Microsoft
  - Current Version released in October 2018, continuously improved
- [Zalando RESTful API and Event Scheme Guidelines](https://opensource.zalando.com/restful-api-guidelines/)
  - Maintained by Zalando
  - Current version released on October 2016

# Standardization

In alphabetical order:
- [Collection+JSON](http://amundsen.com/media-types/collection/)
  - Current version released on February 2013
- [HAL](http://stateless.co/hal_specification.html) (**H**ypertext **A**pplication **L**anguage)
  - Current version release on September 2013
- [Hydra](http://www.markus-lanthaler.com/hydra/) (**Hy**permedia-**Dr**iven Web **A**PIs)
  - Builds upon JSON-LD
  - Current Version released in 2013
- [JSON API](https://jsonapi.org/format/)
  - Version 1.0 released on May 2015
- [JSON-LD](https://json-ld.org/spec/latest/json-ld-api/) (**JSON** for **L**inked **D**ocuments)
  - Standardized by W3C
  - Version 1.1 released on September 2018
- [OData](http://docs.oasis-open.org/odata/odata/v4.0/os/part1-protocol/odata-v4.0-os-part1-protocol.html)
  - Standardized by OASIS
  - Version 4.0 released on February 2014
- [Open API](https://github.com/OAI/OpenAPI-Specification)
  - Also known as *Swagger*
  - Version 3.0.2 released in 2018
  - Favours usage of a descriptional document over discovery by hypermedia
- [RAML](https://github.com/raml-org/raml-spec/blob/master/versions/raml-10/raml-10.md/)
  - Standardized by RAML Workgroup
  - Version 1.0 released on August 2016
  - Favours usage of a descriptional document over discovery by hypermedia
- [SIREN](https://github.com/kevinswiber/siren)
  - Version 0.6.2released on April 2017
- [URI Template](https://tools.ietf.org/html/rfc6570)
  - Standardized by IETF
  - Current Version released in 2012

See also [On choosing a hypermedia type for your API](https://sookocheff.com/post/api/on-choosing-a-hypermedia-format/)

# Articles

## Versioning

In alphabetical order:
- [REST & API Versioning](https://www.predic8.de/rest-api-versioning.htm)
  - Published on November 2017
- [Your API versioning is wrong, which is why I decided to do it 3 different wrong ways](https://www.troyhunt.com/your-api-versioning-is-wrong-which-is/)
  - Published on February 2014