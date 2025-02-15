#if NET8_0_OR_GREATER
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Result for an <see langword="HttpStatusCode" /> property.
/// </summary>
public class StatusCodeResult(
	IThat<HttpResponseMessage?> source,
	Func<HttpResponseMessage, HttpStatusCode> mapper)
{
	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> EqualTo(
		HttpStatusCode? expected)
		=> new(source.ThatIs().ExpectationBuilder.AddConstraint((it, _) =>
				new PropertyConstraint(
					it,
					expected,
					mapper,
					(a, e) => a.Equals(e),
					$"has status code {Formatter.Format(expected)}")),
			source);

	/// <summary>
	///     …is different to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> DifferentTo(
		HttpStatusCode? unexpected)
		=> new(source.ThatIs().ExpectationBuilder.AddConstraint((it, _) =>
				new PropertyConstraint(
					it,
					unexpected,
					mapper,
					(a, u) => !a.Equals(u),
					$"has status code different to {Formatter.Format(unexpected)}")),
			source);

	/// <summary>
	///     …is a success status code (2xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> Success()
		=> new(source.ThatIs().ExpectationBuilder.AddConstraint((it, _) =>
				new PropertyConstraint(
					it,
					null,
					mapper,
					(a, _) => (int)a is >= 200 and < 300,
					"has a success status code (2xx)")),
			source);

	/// <summary>
	///     …is a redirection status code (3xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> Redirection()
		=> new(source.ThatIs().ExpectationBuilder.AddConstraint((it, _) =>
				new PropertyConstraint(
					it,
					null,
					mapper,
					(a, _) => (int)a is >= 300 and < 400,
					"has a redirection status code (3xx)")),
			source);

	/// <summary>
	///     …is a client error status code (4xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> ClientError()
		=> new(source.ThatIs().ExpectationBuilder.AddConstraint((it, _) =>
				new PropertyConstraint(
					it,
					null,
					mapper,
					(a, _) => (int)a is >= 400 and < 500,
					"has a client error status code (4xx)")),
			source);

	/// <summary>
	///     …is a server error status code (5xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> ServerError()
		=> new(source.ThatIs().ExpectationBuilder.AddConstraint((it, _) =>
				new PropertyConstraint(
					it,
					null,
					mapper,
					(a, _) => (int)a is >= 500 and < 600,
					"has a server error status code (5xx)")),
			source);

	/// <summary>
	///     …is a client or server error status code (4xx or 5xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> Error()
		=> new(source.ThatIs().ExpectationBuilder.AddConstraint((it, _) =>
				new PropertyConstraint(
					it,
					null,
					mapper,
					(a, _) => (int)a is >= 400 and < 600,
					"has an error status code (4xx or 5xx)")),
			source);

	private readonly struct PropertyConstraint(
		string it,
		HttpStatusCode? expected,
		Func<HttpResponseMessage, HttpStatusCode> mapper,
		Func<HttpStatusCode, HttpStatusCode?, bool> condition,
		string expectation) : IAsyncConstraint<HttpResponseMessage?>
	{
		public async Task<ConstraintResult> IsMetBy(HttpResponseMessage? actual, CancellationToken cancellationToken)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
					$"{it} was <null>");
			}

			HttpStatusCode value = mapper(actual);
			if (condition(value, expected))
			{
				return new ConstraintResult.Success<HttpResponseMessage>(actual, ToString());
			}

			string formattedResponse =
				await HttpResponseMessageFormatter.Format(actual, "  ", cancellationToken);
			return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
				$"{it} had status code {Formatter.Format(value)}").WithContext("HTTP-Request", formattedResponse);
		}

		public override string ToString()
			=> expectation;
	}
}
#endif
