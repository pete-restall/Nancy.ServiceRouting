# What is this ?
[Nancy.ServiceRouting](https://github.com/pete-restall/Nancy.ServiceRouting)
is a library for the automatic discovery and wiring of Attribute-Based Routes
to Services with [NancyFX](https://github.com/NancyFx/Nancy).

The Routes, Pipeline, Modules, Model Binders, etc. are all from Nancy - this
library is Convention-based pure syntactic sugary goodness wrapped around
the regular Nancy constructs in order to provide a more OCP-friendly approach
to structuring your web application.

See [Nancy.Demo.ServiceRouting](https://github.com/pete-restall/Nancy.ServiceRouting/tree/master/Nancy.Demo.ServiceRouting)
for basic-but-functioning examples.  The demo shows how to use Service
Methods that are dispatched both Synchronously and Asynchronously, as well
as Named Routes that are handy for building URIs automatically without
scattering magic strings throughout your code and Views.

See the [Wiki](https://github.com/pete-restall/Nancy.ServiceRouting/wiki)
for further documentation.

# Why use this ?
Nancy is a superb web stack and it has a lot to offer, but Nancy's Modules
have some smells if you're hooked on
[SOLID](http://en.wikipedia.org/wiki/SOLID_%28object-oriented_design%29).
Modules have at least two responsibilities - they declare Routes and they
contain the implementations behind those Routes.

As Routes are added or the Request processing behind those Routes evolves
and becomes larger or more complex then you will start to get Modules that
resemble spaghetti - adding code in a centralised way like this smells
like an OCP violation.  And, of course, the more code that is added to the
Module the more its Coupling increases.

Wouldn't it be nice if you could just add new functionality by Convention
rather than having to remember to tie it back to some sort of central
location like a Module ?
[Nancy.ServiceRouting](https://github.com/pete-restall/Nancy.ServiceRouting/)
is my attempt at doing just this.

If you like the way
[ServiceStack](https://github.com/ServiceStack/ServiceStack/wiki/Routing)
separates its Routing concerns from its Service implementations then you
should have no problem getting up and running with
[Nancy.ServiceRouting](https://github.com/pete-restall/Nancy.ServiceRouting/).
ServiceStack's Wiki also has some
[good justifications for Message-Based Services](https://github.com/ServiceStack/ServiceStack/wiki/What-is-a-message-based-web-service%3F)
like
[Nancy.ServiceRouting](https://github.com/pete-restall/Nancy.ServiceRouting/)
offers.

# Features
* Allows a more SOLID approach to web services with Nancy - [NancyModules](https://github.com/NancyFx/Nancy/wiki/Exploring-the-nancy-module) are effectively reduced to defining Conventions that discover groups of Services
* Attribute-based Routing for declaratively associating DTOs with HTTP Paths and Verbs
* Convention-based discovery of Routes and Services
* Supports a variety of method signatures for Synchronous and Asynchronous dispatch of Service calls, including the <em>async</em> keyword
* Named Routes allow easy construction of URIs to Services in a maintainable way, without scattering magic path segments throughout code and Views
* Dependency Injection for Services using Nancy's IoC container by default - but easily customisable to be an arbitrary Factory
* Customisable discovery, registration and dispatch semantics
* Compatible with the .NET 4.0 Client Profile
* Uses Nancy's existing [Model Binding](https://github.com/NancyFx/Nancy/wiki/Model-binding) functionality by default, but this can be easily customised
* Utilises Nancy's existing Request Pipeline - Pre- and Post-Request Event Hooks are still available to use
* No configuration required - all the magic can be wired in via trivial code in the [Bootstrapper](https://github.com/NancyFx/Nancy/wiki/Bootstrapper) and [NancyModule](https://github.com/NancyFx/Nancy/wiki/Exploring-the-nancy-module)

# In a Nutshell
Set up your IoC container in your
[Bootstrapper](https://github.com/NancyFx/Nancy/wiki/Bootstrapper) to be able
to resolve the
[RouteRegistrar](https://github.com/pete-restall/Nancy.ServiceRouting/blob/master/Nancy.ServiceRouting/RouteRegistrar.cs)
class, which is the Facade for the library.  There's even a
[RouteRegistrarFactory](https://github.com/pete-restall/Nancy.ServiceRouting/blob/master/Nancy.ServiceRouting/RouteRegistrarFactory.cs)
to help keep with Nancy's super-duper-happy-path by providing sensible defaults:

```C#
    public class Bootstrapper: DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(
            TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register((ctx, args) =>
                RouteRegistrarFactory.CreateDefaultInstance(ctx.Resolve));
        }

        ...
    }
```

Create your DTOs and decorate them with your routes:

```C#
    [Route("/some/{awesome}/route", "GET", "PUT", ...)]
    public class RequestDto
    {
        public int Awesome { get; set; }
        ...
    }
```

Create your Services that use the decorated DTOs:

```C#
    public class SooperDooperService
    {
        public object DoSomethingCool(RequestDto request)
        {
            return ...;
        }
    }
```

Create your NancyModule that discovers / registers the Services and Routes:

```C#
    public class ApiModule: NancyModule
    {
        public ApiModule(RouteRegistrar routes)
        {
            routes.RegisterServicesInto(
                this, typeof(SooperDooperService), typeof(...));
        }

        ...
    }
```

For a quickstart, that's all there is - Nancy handles all the View Resolution,
Content Negotiation and other Pipeline stuff in her usual way.

# Builds
[![Main CI](https://ci.appveyor.com/api/projects/status/ad199gnwd4lyc6wm)](https://ci.appveyor.com/project/pete-restall/nancy-servicerouting)
