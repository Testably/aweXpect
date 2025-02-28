using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class ThatExtensions
{
	private const string HttpRequestContext = "HTTP-Request";
	private const string HttpResponseContext = "HTTP-Response";

	[ExcludeFromCodeCoverage]
	public static IThatIs<T> ThatIs<T>(this IThat<T> subject)
	{
		if (subject is IThatIs<T> thatIs)
		{
			return thatIs;
		}

		if (subject is IThatVerb<T> thatVerb)
		{
			return new ThatSubject<T>(thatVerb.ExpectationBuilder);
		}

		throw new NotSupportedException("IThat<T> must also implement IThatIs<T>");
	}

	public static ExpectationBuilder AddContext(this ExpectationBuilder expectationBuilder, HttpRequestMessage request)
		=> expectationBuilder.UpdateContexts(contexts => contexts
			.Open()
			.Clear()
			.Add(new ResultContext(HttpRequestContext,
				async cancellationToken
					=> await HttpFormatter.Format(request, "  ", cancellationToken)))
			.Close());

	public static ExpectationBuilder AddContext(this ExpectationBuilder expectationBuilder,
		HttpResponseMessage response)
	{
		if (response.RequestMessage is null)
		{
			return expectationBuilder.UpdateContexts(contexts => contexts
				.Open()
				.Clear()
				.Add(new ResultContext(HttpResponseContext,
					async cancellationToken
						=> await HttpFormatter.Format(response, "  ", cancellationToken)))
				.Close());
		}

		return expectationBuilder.UpdateContexts(contexts => contexts
			.Open()
			.Clear()
			.Add(new ResultContext(HttpRequestContext,
				async cancellationToken
					=> await HttpFormatter.Format(response.RequestMessage, "  ", cancellationToken)))
			.Add(new ResultContext(HttpResponseContext,
				async cancellationToken
					=> await HttpFormatter.Format(response, "  ", cancellationToken)))
			.Close());
	}
}
