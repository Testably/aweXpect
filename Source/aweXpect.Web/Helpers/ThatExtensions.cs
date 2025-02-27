using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;

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

	public static async Task<ConstraintResult> AddContext(this ConstraintResult result, HttpRequestMessage request,
		CancellationToken cancellationToken = default)
		=> result.WithContext(HttpRequestContext,
			await HttpResponseMessageFormatter.Format(request, "  ", cancellationToken));

	public static async Task<ConstraintResult> AddContext(this ConstraintResult result, HttpResponseMessage response,
		CancellationToken cancellationToken = default)
	{
		if (response.RequestMessage is null)
		{
			return result.WithContext(HttpResponseContext,
				await HttpResponseMessageFormatter.Format(response, "  ", cancellationToken));
		}

		return result.WithContexts(
			new ConstraintResult.Context(HttpRequestContext,
				await HttpResponseMessageFormatter.Format(response.RequestMessage, "  ", cancellationToken)),
			new ConstraintResult.Context(HttpResponseContext,
				await HttpResponseMessageFormatter.Format(response, "  ", cancellationToken)));
	}

	public static async Task<ConstraintResult.Context[]> GetContexts(this HttpResponseMessage? response,
		CancellationToken cancellationToken = default)
	{
		if (response is null)
		{
			return [];
		}

		if (response.RequestMessage is null)
		{
			return
			[
				new ConstraintResult.Context(HttpResponseContext,
					await HttpResponseMessageFormatter.Format(response, "  ", cancellationToken)),
			];
		}

		return
		[
			new ConstraintResult.Context(HttpRequestContext,
				await HttpResponseMessageFormatter.Format(response.RequestMessage, "  ", cancellationToken)),
			new ConstraintResult.Context(HttpResponseContext,
				await HttpResponseMessageFormatter.Format(response, "  ", cancellationToken)),
		];
	}

	public static async Task<ConstraintResult.Context[]> GetContexts(this HttpRequestMessage? request,
		CancellationToken cancellationToken = default)
	{
		if (request is null)
		{
			return [];
		}

		return
		[
			new ConstraintResult.Context(HttpRequestContext,
				await HttpResponseMessageFormatter.Format(request, "  ", cancellationToken)),
		];
	}
}
