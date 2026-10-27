using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;

namespace aweXpect.Web;

/// <summary>
///     Verifies that the media type in the <c>Content-Type</c> header of the <see cref="HttpContent" /> of a
///     <typeparamref name="TMessage" /> matches the <paramref name="expected" /> value.
/// </summary>
/// <remarks>
///     A message without content or without a <c>Content-Type</c> header fails, so that its negation succeeds.
/// </remarks>
internal sealed class HasContentTypeConstraint<TMessage>(
	string it,
	ExpectationGrammars grammars,
	string expected,
	string parameterName,
	StringEqualityOptions options,
	Func<TMessage, HttpContent?> getContent)
	: ConstraintResult.WithNotNullValue<TMessage>(it, grammars),
		IAsyncContextConstraint<TMessage>,
		IOptionsHolderSource<StringEqualityOptions>
	where TMessage : class
{
	private OptionsHolder<StringEqualityOptions>? _optionsHolder;

	private StringEqualityOptions CurrentOptions
		=> _optionsHolder is null ? options : _optionsHolder.Value;

	/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
	OptionsHolder<StringEqualityOptions> IOptionsHolderSource<StringEqualityOptions>.Holder
		=> _optionsHolder ??= new OptionsHolder<StringEqualityOptions>(options);

	private string? _contentType;
	private StringEqualityOptions? _evaluationOptions;
	private MatchResult _matchResult;

	public async ValueTask<ConstraintResult> IsMetBy(TMessage? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		if (actual == null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		if (!getContent(actual).TryGetMediaType(out _contentType))
		{
			Outcome = Outcome.Failure;
			return this;
		}

		_evaluationOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
		_matchResult = await _evaluationOptions.MatchWithExplanation(_contentType, expected, parameterName);
		if (!_matchResult.IsMatch)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		Outcome = Outcome.Success;
		return this;
	}

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
		=> (_evaluationOptions ?? CurrentOptions).AppendContexts(contexts);

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("has a `Content-Type` header ", "have a `Content-Type` header "))
			.Append(CurrentOptions.GetExpectation(expected, Grammars));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_contentType is null)
		{
			stringBuilder.Append(It).Append(" had no `Content-Type` header");
		}
		else
		{
			stringBuilder.Append((_evaluationOptions ?? CurrentOptions).GetExtendedMemberFailure(It, "content type",
				Grammars, _contentType, expected, _matchResult));
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("does not have a `Content-Type` header ", "do not have a `Content-Type` header "))
			.Append(CurrentOptions.GetExpectation(expected, Grammars.Negate()));

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(It).Append(" had");
}
