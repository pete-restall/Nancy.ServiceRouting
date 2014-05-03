# What is this ?
[Nancy.ServiceRouting](https://github.com/pete-restall/Nancy.ServiceRouting)
is a library for the automatic discovery / wiring of Attribute-Based Routes
with [NancyFX](https://github.com/NancyFx/Nancy).

The Routes, Pipeline, Modules, Model Binders, etc. are all from Nancy - this
library is convention-based pure syntactic sugary goodness whose intention is
to allow a more OCP-friendly way to get Routes into Nancy.

See [Nancy.Demo.ServiceRouting](https://github.com/pete-restall/Nancy.ServiceRouting/tree/master/Nancy.Demo.ServiceRouting)
for a basic-but-functioning example.

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
