# Notes

Den orginale innleveringen er i branchen OldMain.

Prosjektet er en API, men skiller seg noe fra 'Case 8: RestAPI Applikasjon' ved bruk av
Vertical slice architecture og [Fast-Endpoints](https://fast-endpoints.com/).

## Vertical Slice Architecture

A _Slightly_ over-engineered todo-api, because that's what one does.

This is a project for exploring Vertical Slice Architecture and to learn
more about important parts of an api like like logging and IAM.

The library [Fast-Endpoints](https://fast-endpoints.com/) is used instead of
Minimal-API or controllers, since it's tailored to VSA.

VSA is based on the idea "what change together lives together" and is often
recommended as a way to make very extendable and scalable system. We can scale
different part of the system independent. If the api sees alot of read or alot
of write compared to the other, the part with lots of traffic can be scaled but
the other can stay as it is.

A new feature is added as its own slice, with a folder and files, and existing code
stays unaltered. If existing code need changes, everything is located in its feature
folder.

VSA is also often recommended as a way to work with AI/code agents since it limits
the context needed. I have not tested this claim, but it seems like a good idea to
use an architecture that both human and ai can understand and work with.

## Start

The project is started with 'docker compose up'. '/swagger' is available for easy
api calls.
After signing up, a loggin is required to get the the Jwt.
Then lists can be added and items can be added to it.

## Stack:

**Postgres;** a database system so advanced it could replace the whole system.
I wanted a real database system so the docker compose became a bit "meatier"
and it offer more learing opportunities.

**OpenTelemetry;**
For this system, writing the log to a file would be enough, however OpenTelemetry
is being used more and more in the industry and thought it would be usefull to
experiment with it.

## Tools:

**Seq**; we need a way to store and read the very important log.

**PgAdmin**; useful when "undocumented features" or forgotten functionality
make themselfs known.

## Testing

Testing is done with TestContainer and FastEndpoints.Testing package.
TestContainer needs docker running.

## Rework Todo

- [x] Remake the model
- [x] Add a check if valid user for each request
- [x] Remove every file thats not used
- [x] Remove code thats not used, and comment code that is.
- [x] Add some comments for redactor and such
- [x] Add filtering for Due date
- [x] Remove "Commons/PreProcessors"
- [x] Add endpoint for combined list and items
- [x] Add tests for new endpoint
