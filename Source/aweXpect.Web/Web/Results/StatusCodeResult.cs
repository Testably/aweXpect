using System;
using System.Net;
using System.Net.Http;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
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
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint((expected, mapper), static (state, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						state.expected,
						state.mapper,
						(a, e) => a.Equals(e),
						$"status code {Formatter.Format(state.expected)}")),
			source);

	/// <summary>
	///     …is different to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> DifferentTo(
		HttpStatusCode? unexpected)
		=> new(source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint((unexpected, mapper), static (state, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						state.unexpected,
						state.mapper,
						(a, u) => !a.Equals(u),
						$"status code different to {Formatter.Format(state.unexpected)}",
						$"status code {Formatter.Format(state.unexpected)}")),
			source);

	/// <summary>
	///     …is a success status code (2xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> Success()
		=> new(source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 200 and < 300,
						"a success status code (2xx)")),
			source);

	/// <summary>
	///     …is a redirection status code (3xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> Redirection()
		=> new(source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 300 and < 400,
						"a redirection status code (3xx)")),
			source);

	/// <summary>
	///     …is a client error status code (4xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> ClientError()
		=> new(source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 400 and < 500,
						"a client error status code (4xx)")),
			source);

	/// <summary>
	///     …is a server error status code (5xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> ServerError()
		=> new(source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 500 and < 600,
						"a server error status code (5xx)")),
			source);

	/// <summary>
	///     …is a client or server error status code (4xx or 5xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage?, IThat<HttpResponseMessage?>> Error()
		=> new(source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 400 and < 600,
						"an error status code (4xx or 5xx)")),
			source);

	internal sealed class PropertyConstraint(
		string it,
		ExpectationGrammars grammars,
		HttpStatusCode? expected,
		Func<HttpResponseMessage, HttpStatusCode> mapper,
		Func<HttpStatusCode, HttpStatusCode?, bool> condition,
		string expectation,
		string? hasExpectationWhenNegated = null)
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

			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has ", "have ")).Append(expectation);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had status code ");
			Formatter.Format(stringBuilder, _statusCode);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(hasExpectationWhenNegated is null
				? Grammars.Verb("does not have ", "do not have ") + expectation
				: Grammars.Verb("has ", "have ") + hasExpectationWhenNegated);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);
	}
}
