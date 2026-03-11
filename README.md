# Introduction 

A solution for ??? TODO ???.

# Getting Started

You can launch the application via Visual Studio (being sure to set Web as your startup project
and Development as Web's environment on the project's Debug tab).


# Build and Test

To build, open the solution in Visual Studio and hit F6. All functional, integration and unit tests can then be run via 
Visual Studio's test explorer. The solution uses xUnit as its underlying testing framework and Moq as its mocking framework.

# Design Goals

The primary goal of the solution design is to provide a foundation on which to build a well-factored, **highly-testable**, 
SOLID application using .NET Core and Clean Architecture. Learn more about these topics here:

- [SOLID Principles for C# Developers](https://www.pluralsight.com/courses/csharp-solid-principles)
- [SOLID Principles of Object Oriented Design](https://www.pluralsight.com/courses/principles-oo-design) (the original, longer course)
- [Clean Architecture: Patterns, Practices, and Principles](https://www.pluralsight.com/courses/clean-architecture-patterns-practices-principles)

If you're used to building applications as a single-project or as a set of projects that follow the traditional 
UI -> Business Layer -> Data Access Layer "N-Tier" architecture, check out these two courses (ideally before Clean Architecture):

- [Creating N-Tier Applications in C#, Part 1](https://www.pluralsight.com/courses/n-tier-apps-part1)
- [Creating N-Tier Applications in C#, Part 2](https://www.pluralsight.com/courses/n-tier-csharp-part2)

# The Core Project

The Core project is the center of the Clean Architecture design, and all other project dependencies should point toward it. 
As such, it has very few external dependencies. The Core project should include things like:

- Queries
- Commands
- Events
- Custom Exceptions
- Factories
- Guard Clauses
- Interfaces
- DTOs
- Models
- Enums
- Validators

Under the hood, Core depends on `Mediatr` to send commands, queries and events to their respective handlers. 

# The Infrastructure Project

Most of the application's dependencies on external resources (including databases) should be implemented 
in classes defined in the Infrastructure project. These classes should implement interfaces defined in Core. 

# The Web Project

The entry point of the application is an ASP.NET Core Web project. This is simply a console application. It  
hosts the Blazor Server app. It is configured using the default `appsettings.json` file 
plus application secrets (when running in development mode locally).

Web relies on the following open source component libraries:

- [MudBlazor](https://mudblazor.com/)
- [Heron MudCalendar](https://danheron.github.io/Heron.MudCalendar/#features)

# The Test Projects

Test projects are organized based on the kind of test (unit, integration, functional). 

In terms of dependencies, there are three worth noting:

- [xunit](https://www.nuget.org/packages/xunit) We're using xunit because that's what ASP.NET Core uses internally to test the product. It works great and as new versions of ASP.NET Core ship, I'm confident it will continue to work well with it.
- [Moq](https://www.nuget.org/packages/Moq/) We're using Moq as a mocking framework for white box behavior-based tests. If I have a method that, under certain circumstances, should perform an action that isn't evident from the object's observable state, mocks provide a way to test that. I could also use my own Fake implementation, but that requires a lot more typing and files. Moq is great once you get the hang of it.
- [Microsoft.AspNetCore.Mvc.Testing](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Testing) We're using Testing to test the web project using its full stack, not just unit testing action methods. Using a custom WebApplicationFactory, you make actual HttpClient requests without going over the wire (so no firewall or port configuration issues). Tests run in memory and are very fast, and requests exercise the full ASP.NET Core stack, including routing, model binding, model validation, filters, etc.

The following diagram provides a high-level overview of the solution structure.

![Solution Structure Diagram](./SolutionStructure.png)

# Key Patterns Used

This solution supports a several patterns, especially Domain-Driven Design patterns. Here is a brief overview of a key frew of them work.

## Domain Events

Domain events are a great pattern for decoupling a trigger for an operation from its implementation. This is especially useful from 
within domain entities since the handlers of the events can have dependencies while the entities themselves typically do not. 
The following sequence diagram demonstrates how the event and its handler are used when an item is marked complete through a web API endpoint.

![Domain Event Sequence Diagram](./DomainEvents.png)

## Guard Clauses

A guard clause is a software pattern that simplifies complex functions by "failing fast", checking for invalid inputs up front and 
immediately failing if any are found. If a method (or object instance) requires certain values in order to function properly, 
and there's no way the system should ever try to call the code with invalid inputs, then an exception-throwing guard clause makes sense.

To implement this pattern, we currently rely on the following package:

- [Throw](https://github.com/amantinband/throw)

Note that this package is extensible, making it very easy to add custom guard clauses that suit our particular needs.
