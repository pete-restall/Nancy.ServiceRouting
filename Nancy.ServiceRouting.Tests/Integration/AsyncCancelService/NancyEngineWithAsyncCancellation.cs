using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.ErrorHandling;
using Nancy.Routing;

namespace Restall.Nancy.ServiceRouting.Tests.Integration.AsyncCancelService
{
	public class NancyEngineWithAsyncCancellation: INancyEngine
	{
		private readonly INancyEngine engine;

		public NancyEngineWithAsyncCancellation(
			IRequestDispatcher requestDispatcher,
			INancyContextFactory nancyContextFactory,
			IEnumerable<IStatusCodeHandler> statusCodeHandlers,
			IRequestTracing requestTracing,
			DiagnosticsConfiguration diagnosticsConfiguration,
			IStaticContentProvider staticContentProvider)
		{
			this.engine = new NancyEngine(
				requestDispatcher,
				nancyContextFactory,
				statusCodeHandlers,
				requestTracing,
				diagnosticsConfiguration,
				staticContentProvider);
		}

		public Task<NancyContext> HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest, CancellationToken cancellationToken)
		{
			return this.engine.HandleRequest(request, preRequest, this.CancellationToken);
		}

		public CancellationToken CancellationToken { get; set; }

		public Func<NancyContext, IPipelines> RequestPipelinesFactory
		{
			get { return this.engine.RequestPipelinesFactory; }
			set { this.engine.RequestPipelinesFactory = value; }
		}
	}
}
