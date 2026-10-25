using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpRequestMessage
{
	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasRequestUri(this IThat<HttpRequestMessage?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		if (!Uri.TryCreate(expected, UriKind.Absolute, out Uri? expectedUri))
		{
			throw Tracing.WriteException(new ArgumentException(
				"The 'expected' must be a valid absolute URI.", nameof(expected)));
		}

		return subject.HasRequestUri(expectedUri);
	}

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> has the <paramref name="expected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasRequestUri(this IThat<HttpRequestMessage?> subject, Uri expected)
	{
		ThatUri.ThrowIfNotAbsolute(expected, nameof(expected));
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityResult<HttpRequestMessage, IThat<HttpRequestMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint((expected: expected.ToString(), options), static (state, it, grammars) =>
					new HasRequestUriConstraint(it, grammars, state.expected, "expected", state.options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> does not have the <paramref name="unexpected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		DoesNotHaveRequestUri(this IThat<HttpRequestMessage?> subject, string unexpected)
		=> subject.DoesNotHaveRequestUri(ThatUri.ToAbsoluteUri(unexpected, nameof(unexpected)));

	/// <summary>
	///     Verifies that the <see cref="HttpRequestMessage" /> does not have the <paramref name="unexpected" />
	///     <see cref="HttpRequestMessage.RequestUri" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		DoesNotHaveRequestUri(this IThat<HttpRequestMessage?> subject, Uri unexpected)
	{
		ThatUri.ThrowIfNotAbsolute(unexpected, nameof(unexpected));
		OptionsSlot<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityResult<HttpRequestMessage, IThat<HttpRequestMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint((unexpected: unexpected.ToString(), options), static (state, it, grammars) =>
					new HasRequestUriConstraint(it, grammars, state.unexpected, "unexpected", state.options).Invert()),
			subject,
			options);
	}

	private sealed class HasRequestUriConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		string parameterName,
		OptionsSlot<StringEqualityOptions> options)
		: ConstraintResult.WithNotNullValue<HttpRequestMessage>(it, grammars),
			IAsyncContextConstraint<HttpRequestMessage>
	{
		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;
		private string? _requestUri;

		public async ValueTask<ConstraintResult> IsMetBy(HttpRequestMessage? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_requestUri = actual.RequestUri?.ToString();
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			if (_requestUri is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_matchResult = await _evaluationOptions.MatchWithExplanation(_requestUri, expected, parameterName);
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options.Value).AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a request URI ", "have a request URI "))
				.Append(options.Value.GetExpectation(expected, ExpectationGrammars.None));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options.Value).GetExtendedMemberFailure(It, "request URI",
				Grammars, _requestUri, expected, _matchResult));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a request URI ", "do not have a request URI "))
				.Append(options.Value.GetExpectation(expected, Grammars.Negate()));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
