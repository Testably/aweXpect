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
	///     Verifies that the string content is equal to <paramref name="expected" />
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasContent(this IThat<HttpRequestMessage?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		return new StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint(expected, static (expected, it, grammars) =>
						new HasContentConstraint(it, grammars, expected, "expected"),
					out HasContentConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the string content satisfies the <paramref name="expectations" />
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		HasContent(this IThat<HttpRequestMessage?> subject, Action<IThatSubject<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return new AndOrResult<HttpRequestMessage, IThat<HttpRequestMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.ForAsyncMember(MemberAccessor<HttpRequestMessage, Task<string?>>.FromFuncWithName(
						async (m, _, cancellationToken) => await m.Content.ReadAsStringOrEmptyAsync(cancellationToken),
						" the string content"),
					(_, stringBuilder) => stringBuilder.Append("has a string content which "))
				.AddExpectations(expectations, MemberGrammars.AfterWhich),
			subject);
	}

	/// <summary>
	///     Verifies that the string content is not equal to <paramref name="unexpected" />
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>
		DoesNotHaveContent(this IThat<HttpRequestMessage?> subject, string unexpected)
	{
		ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));
		return new StringEqualityTypeResult<HttpRequestMessage, IThat<HttpRequestMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.RequestContexts)
				.AddConstraint(unexpected, static (unexpected, it, grammars) =>
						new HasContentConstraint(it, grammars, unexpected, "unexpected").Invert(),
					out HasContentConstraint constraint),
			subject,
			constraint);
	}

	private sealed class HasContentConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		string parameterName)
		: ConstraintResult.WithNotNullValue<HttpRequestMessage>(it, grammars),
			IAsyncContextConstraint<HttpRequestMessage>,
			IOptionsHolderSource<StringEqualityOptions>
	{
		private OptionsHolder<StringEqualityOptions>? _optionsHolder;

		private StringEqualityOptions CurrentOptions
			=> _optionsHolder is null ? StringEqualityOptions.Default : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<StringEqualityOptions> IOptionsHolderSource<StringEqualityOptions>.Holder
			=> _optionsHolder ??= new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default);

		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;
		private string? _message;
		private Exception? _readException;

		/// <inheritdoc />
		public override Exception? FailureCause => _readException;

		public async ValueTask<ConstraintResult> IsMetBy(
			HttpRequestMessage? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_readException = null;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			try
			{
				_message = await actual.Content.ReadAsStringOrEmptyAsync(cancellationToken);
			}
			catch (Exception exception) when (!exception.IsCancellationOf(cancellationToken))
			{
				_readException = exception;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_evaluationOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(_message, expected, parameterName);
			if (_matchResult.IsMatch)
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? CurrentOptions).AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a string content ", "have a string content "))
				.Append(CurrentOptions.GetExpectation(expected, ExpectationGrammars.None));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_readException is not null)
			{
				stringBuilder.AppendDidThrow("the string content", _readException, indentation);
				return;
			}

			stringBuilder.Append((_evaluationOptions ?? CurrentOptions).GetExtendedMemberFailure(It, "string content",
				Grammars, _message, expected, _matchResult));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a string content ", "do not have a string content "))
				.Append(CurrentOptions.GetExpectation(expected, ExpectationGrammars.None));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_readException is not null)
			{
				stringBuilder.AppendDidThrow("the string content", _readException, indentation);
				return;
			}

			stringBuilder.Append(It).Append(" had");
		}
	}
}
