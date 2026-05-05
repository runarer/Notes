# Notes

A _Slightly_ over-engineered todo-api, because that's what one does.

This is a project for exploring Vertical Slice Architecture and to learn
more about important parts of an api like like logging and IAM.

VSA is based on the idea "what change together lives together" and is often
recommended as a way to make very extendable and scalable system. We can scale
different part of the system independent. If the api sees alot of read or alot
of write compared to the other, the part with lots of traffic can be scaled but
the other can stay as it is.

A new feature is added as its own slice, with a folder and files, and existing code
stays unaltered. If existing code need changes, everything is located in its feature
folder.

VSA is also often recommended as a way to work with AI/code agents since it limits
the context needed when creating new features.

## Start

The project is started with 'docker compose up'. '/swagger' is available for easy
api calls.
After signing up, a loggin is required to get the the Jwt.
Then lists can be added and items can be added to it.

## Stack:

**Postgres**; a database system so advanced it could replace the whole system.
And I use it just to hold three tables, nice. Sqlite would be more appropriate,
but I wanted a real database system so the docker compose became a bit "meatier"
and it offer more learing opportunities.

**OpenTelemetry**; every system need a log. And with this set to 'information'
level, everytime someone add or completes a task 99.5% of the data stored
is just the log.

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
