---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2023 Group `24`
author:
- "Anton Vadsholt <avad@itu.dk>"
- "David August Ringenson <drin@itu.dk>"
- "Emil Fenger <emfe@itu.dk>"
- "Frederik Bjerre Bertelsen <frbb@itu.dk>"
- "Oskar Wernegreen <oskw@itu.dk>"
numbersections: true
---
![Chirp logo](/docs/images/ChirpLogo.png)

# Design and Architecture of _Chirp!_

## Domain model

Provide an illustration of your domain model.
Make sure that it is correct and complete.
In case you are using ASP.NET Identity, make sure to illustrate that accordingly.

## Architecture — In the small

Illustrate the organization of your code base.
That is, illustrate which layers exist in your (onion) architecture.
Make sure to illustrate which part of your code is residing in which layer.

## Architecture of deployed application

Illustrate the architecture of your deployed application.
Remember, you developed a client-server application.
Illustrate the server component and to where it is deployed, illustrate a client component, and show how these communicate with each other.

**OBS**: In case you ran out of credits for hosting an Azure SQL database and you switched back to deploying an application with in-process SQLite database, then do the following:

- Under this section, provide two diagrams, one that shows how _Chirp!_ was deployed with hosted database and one that shows how it is now again with SQLite.
- Under this section, provide a brief description of the reason for switching again to SQLite as database.
- In that description, provide a link to the commit hash in your GitHub repository that points to the latest version of your _Chirp!_ application with hosted database (we look at the entire history of your project, so we see that it was there at some point).

## User activities

Illustrate typical scenarios of a user journey through your _Chirp!_ application.
That is, start illustrating the first page that is presented to a non-authorized user, illustrate what a non-authorized user can do with your _Chirp!_ application, and finally illustrate what a user can do after authentication.

Make sure that the illustrations are in line with the actual behavior of your application.

## Sequence of functionality/calls trough _Chirp!_

With a UML sequence diagram, illustrate the flow of messages and data through your _Chirp!_ application.
Start with an HTTP request that is send by an unauthorized user to the root endpoint of your application and end with the completely rendered web-page that is returned to the user.

Make sure that your illustration is complete.
That is, likely for many of you there will be different kinds of "calls" and responses.
Some HTTP calls and responses, some calls and responses in C# and likely some more.
(Note the previous sentence is vague on purpose. I want that you create a complete illustration.)


## Injection
### SQL injection
### JS injection

# Process

## Build, test, release, and deployment

## Team work

## How to make _Chirp!_ work locally

## How to run test suite locally

# Ethics
## Data handling
### Forget me

## License

## LLMs, ChatGPT, CoPilot, and others

# figures we might want to make
## Database constraints
## Site-map.
## UML diagram of Chirp.
## Onion diagram
## Flow diagram (maybe over Azure susi B2C stuff)
## figure depicting the client server relationship

