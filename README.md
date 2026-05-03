# Notes

## The

A _Slightly_ over-engineered todo-api, because that's what one does.

This is a project for exploring Vertical Slice Architecture and to learn
more about important parts of an api like like logging and IAM.

### Stack:

**Postgresql**; a database system so advanced it could replace the rest of
the stack. And I use it just to hold two tables, nice.

**OpenTelemetry**; every system need a log. And with this set to 'information'
level, everytime someone add or completes a task 99.5% of the data stored
is just the log.

### Tools:

**Seq**; we need a way to store and read the very important log.

**PgAdmin**; useful when "undocumented features" or forgotten functionality
make themselfs known.

### Testing

Testing is done with TestContainer and FastEndpoints.Testing package.

## Toughts

The reasoning behind this project was to explore Vertical Slice Architecture as
it's often recommended as a way to make very extendable and scalable system.
VSA is based on the idea "what change together lives together", it's also

When we want to add a feature we just add files and folders to the project. We do
not need to change the code that's already written. I can see how we could add features
such as notes or ... just by adding and not changing code.

VSA is also often recommended as a way to work with AI/code agents since it limits
the context needed when creating new features. And it can work in a more isolated manners.
