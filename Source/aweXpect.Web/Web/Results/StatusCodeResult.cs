using System;
using System.Net;
using System.Net.Http;
using System.Text;
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
		=> new(source.Get().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammars) =>
					new PropertyConstraint(
						expectationBuilder,
						it, grammars,
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
		=> new(source.Get().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammars) =>
					new PropertyConstraint(
						expectationBuilder,
						it, grammars,
						unexpected,
						mapper,
						(a, u) => !a.Equals(u),
						$"has status code different to {Formatter.Format(unexpected)}")),
			source);

	/// <summary>
	///     …is a success status code (2xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> Success()
		=> new(source.Get().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammars) =>
					new PropertyConstraint(
						expectationBuilder,
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 200 and < 300,
						"has a success status code (2xx)")),
			source);

	/// <summary>
	///     …is a redirection status code (3xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> Redirection()
		=> new(source.Get().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammars) =>
					new PropertyConstraint(
						expectationBuilder,
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 300 and < 400,
						"has a redirection status code (3xx)")),
			source);

	/// <summary>
	///     …is a client error status code (4xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> ClientError()
		=> new(source.Get().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammars) =>
					new PropertyConstraint(
						expectationBuilder,
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 400 and < 500,
						"has a client error status code (4xx)")),
			source);

	/// <summary>
	///     …is a server error status code (5xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> ServerError()
		=> new(source.Get().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammars) =>
					new PropertyConstraint(
						expectationBuilder,
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 500 and < 600,
						"has a server error status code (5xx)")),
			source);

	/// <summary>
	///     …is a client or server error status code (4xx or 5xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> Error()
		=> new(source.Get().ExpectationBuilder
				.UpdateContexts(c => c.Close())
				.AddConstraint((expectationBuilder, it, grammars) =>
					new PropertyConstraint(
						expectationBuilder,
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 400 and < 600,
						"has an error status code (4xx or 5xx)")),
			source);

	internal sealed class PropertyConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		HttpStatusCode? expected,
		Func<HttpResponseMessage, HttpStatusCode> mapper,
		Func<HttpStatusCode, HttpStatusCode?, bool> condition,
		string expectation)
		: ConstraintResult.WithNotNullValue<HttpResponseMessage?>(it, grammars),
			IValueConstraint<HttpResponseMessage?>
	{
		private HttpStatusCode _statusCode;

		public ConstraintResult IsMetBy(HttpResponseMessage? actual)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_statusCode = mapper(actual);
			if (condition(_statusCode, expected))
			{
				Outcome = Outcome.Success;
				return this;
			}

			expectationBuilder.AddContext(actual);
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectation);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had status code ");
			Formatter.Format(stringBuilder, _statusCode);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> throw new NotSupportedException();

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> throw new NotSupportedException();
	}
}
