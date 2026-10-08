using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class ThatExtensions
{
	private const string HttpRequestContext = "HTTP-Request";
	private const string HttpResponseContext = "HTTP-Response";

	public static readonly Action<HttpRequestMessage, ResultContextCollector> RequestContexts
		= (request, contexts) => contexts.AddContext(request);

	public static readonly Action<HttpResponseMessage, ResultContextCollector> ResponseContexts
		= (response, contexts) => contexts.AddContext(response);

	[ExcludeFromCodeCoverage]
	public static IExpectThat<T> Get<T>(this IThat<T> subject)
	{
		if (subject is IExpectThat<T> expectThat)
		{
			return expectThat;
		}

		throw new NotSupportedException("IThat<T> must also implement IExpectThat<T>");
	}

	public static void AddContext(this ResultContextCollector contexts, HttpRequestMessage request)
		=> contexts.Add(new ResultContext.AsyncCallback(HttpRequestContext,
			async cancellationToken
				=> await HttpFormatter.Format(request, "  ", cancellationToken)));

	public static void AddContext(this ResultContextCollector contexts, HttpResponseMessage response)
	{
		if (response.RequestMessage is not null)
		{
			contexts.AddContext(response.RequestMessage);
		}

		contexts.Add(new ResultContext.AsyncCallback(HttpResponseContext,
			async cancellationToken
				=> await HttpFormatter.Format(response, "  ", cancellationToken)));
	}
}
