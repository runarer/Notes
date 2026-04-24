# Notes
A _Slightly_ over-engineered todo-api, because that's what one does.

## Stack:
**Postgresql**; a database system so advanced it could replace the rest of
the stack. And I use it just to hold two tables, nice.

**Keycloak**; after spending a good few days trying to implement a
user and login solution, I took the good old advice of "do not make your
own system" to heath and desided to use keycloak. It's a lot safer and 
more versetile, but like most people I used it because it's just so much
easier.

**OpenTelemetry**; every system need a log. And with this set to 'information'
level, everytime someone add or completes a task 99.5% of the data stored
is just the log.

## Tools:
**Seq**; we need a way to store and read the very important log.

**PgAdmin**; useful when "undocumented features" or forgotten functionality
make themselfs known.

## Testing
Testing is done with TestContainer and FastEndpoints.Testing package.
