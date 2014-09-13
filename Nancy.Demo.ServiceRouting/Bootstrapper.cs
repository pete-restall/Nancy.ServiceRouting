using System;
using Nancy;
using Nancy.Conventions;
using Restall.Nancy.ServiceRouting;
using Nancy.TinyIoc;

namespace Restall.Nancy.Demo.ServiceRouting
{
	public class Bootstrapper: DefaultNancyBootstrapper
	{
		protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
		{
			base.ConfigureRequestContainer(container, context);
			container.Register((ctx, args) => context);
			container.Register((ctx, args) => RouteRegistrarFactory.CreateDefaultInstance(ctx.Resolve));
		}

		protected override void ConfigureConventions(NancyConventions nancyConventions)
		{
			base.ConfigureConventions(nancyConventions);
			this.Conventions.ViewLocationConventions.Add((view, model, ctx) => ViewPathForType(model.GetType()) + "View");
			this.Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/assets"));
		}

		private static string ViewPathForType(Type type)
		{
			string namespaceRelativeToApiRoot = type.FullName.Replace("Restall.Nancy.Demo.ServiceRouting.Api", "");
			string path = "Views/" + namespaceRelativeToApiRoot.Replace('.', '/');
			return path.Replace("//", "/");
		}
	}
}
