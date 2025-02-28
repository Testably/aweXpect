using System;
using System.Net;
using System.Net.Http;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Web.Results;

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
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, _) =>
					new PropertyConstraint(
						expectationBuilder,
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
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, _) =>
					new PropertyConstraint(
						expectationBuilder,
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
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, _) =>
					new PropertyConstraint(
						expectationBuilder,
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
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, _) =>
					new PropertyConstraint(
						expectationBuilder,
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
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, _) =>
					new PropertyConstraint(
						expectationBuilder,
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
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, _) =>
					new PropertyConstraint(
						expectationBuilder,
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
		=> new(source.ThatIs().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, _) =>
					new PropertyConstraint(
						expectationBuilder,
						it,
						null,
						mapper,
						(a, _) => (int)a is >= 400 and < 600,
						"has an error status code (4xx or 5xx)")),
			source);

	internal readonly struct PropertyConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		HttpStatusCode? expected,
		Func<HttpResponseMessage, HttpStatusCode> mapper,
		Func<HttpStatusCode, HttpStatusCode?, bool> condition,
		string expectation) : IValueConstraint<HttpResponseMessage?>
	{
		public ConstraintResult IsMetBy(HttpResponseMessage? actual)
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

			expectationBuilder.AddContext(actual);
			return new ConstraintResult.Failure<HttpResponseMessage?>(actual, ToString(),
				$"{it} had status code {Formatter.Format(value)}");
		}

		public override string ToString()
			=> expectation;
	}
}
