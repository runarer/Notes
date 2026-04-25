# Notes
A _Slightly_ over-engineered todo-api, because that's what one does.

## Stack:
**Postgresql**; a database system so advanced it could replace the rest of
the stack. And I use it just to hold two tables, nice.

**OpenTelemetry**; every system need a log. And with this set to 'information'
level, everytime someone add or completes a task 99.5% of the data stored
is just the log.

## Tools:
**Seq**; we need a way to store and read the very important log.

**PgAdmin**; useful when "undocumented features" or forgotten functionality
make themselfs known.

## Testing
Testing is done with TestContainer and FastEndpoints.Testing package.
