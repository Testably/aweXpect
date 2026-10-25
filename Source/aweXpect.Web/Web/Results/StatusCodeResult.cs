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
public class StatusCodeResult
{
	private readonly Func<HttpResponseMessage, HttpStatusCode> _mapper;
	private readonly IThat<HttpResponseMessage?> _source;

	internal StatusCodeResult(
		IThat<HttpResponseMessage?> source,
		Func<HttpResponseMessage, HttpStatusCode> mapper)
	{
		_source = source;
		_mapper = mapper;
	}

	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> EqualTo(
		HttpStatusCode expected)
		=> new(_source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint((expected, mapper: _mapper), static (state, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						state.expected,
						state.mapper,
						(a, e) => a == e,
						"status code ",
						true)),
			_source);

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> NotEqualTo(
		HttpStatusCode unexpected)
		=> new(_source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint((unexpected, mapper: _mapper), static (state, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						state.unexpected,
						state.mapper,
						(a, u) => a != u,
						"status code ",
						true,
						true)),
			_source);

	/// <summary>
	///     …is a success status code (2xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> Success()
		=> new(_source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(_mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 200 and < 300,
						"a success status code (2xx)")),
			_source);

	/// <summary>
	///     …is a redirection status code (3xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> Redirection()
		=> new(_source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(_mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 300 and < 400,
						"a redirection status code (3xx)")),
			_source);

	/// <summary>
	///     …is a client error status code (4xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> ClientError()
		=> new(_source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(_mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 400 and < 500,
						"a client error status code (4xx)")),
			_source);

	/// <summary>
	///     …is a server error status code (5xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> ServerError()
		=> new(_source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(_mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 500 and < 600,
						"a server error status code (5xx)")),
			_source);

	/// <summary>
	///     …is a client or server error status code (4xx or 5xx).
	/// </summary>
	public AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>> Error()
		=> new(_source.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint(_mapper, static (mapper, it, grammars) =>
					new PropertyConstraint(
						it, grammars,
						null,
						mapper,
						(a, _) => (int)a is >= 400 and < 600,
						"an error status code (4xx or 5xx)")),
			_source);

	internal sealed class PropertyConstraint(
		string it,
		ExpectationGrammars grammars,
		HttpStatusCode? expected,
		Func<HttpResponseMessage, HttpStatusCode> mapper,
		Func<HttpStatusCode, HttpStatusCode?, bool> condition,
		string expectation,
		bool appendExpected = false,
		bool isNegative = false)
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
			=> AppendExpectation(stringBuilder, isNegative);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" had status code ");
			Formatter.Format(stringBuilder, _statusCode);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> AppendExpectation(stringBuilder, !isNegative);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendNormalResult(stringBuilder, indentation);

		private void AppendExpectation(StringBuilder stringBuilder, bool doesNotHave)
		{
			stringBuilder.Append(doesNotHave
					? Grammars.Verb("does not have ", "do not have ")
					: Grammars.Verb("has ", "have "))
				.Append(expectation);
			if (appendExpected)
			{
				Formatter.Format(stringBuilder, expected);
			}
		}
	}
}
