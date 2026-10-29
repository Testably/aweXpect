using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Formatting;

namespace aweXpect.Reflection.Helpers;

/// <summary>
///     The expectation that the name of a single item equals the name that a selector returns for it.
/// </summary>
/// <remarks>
///     The selector is invoked once per item, and the listed items name the expected name next to their own.
/// </remarks>
internal sealed class HaveNameFromSelectorExpectation<TItem> : ItemExpectation<TItem>
	where TItem : class
{
	private readonly Func<TItem, string> _expectedNameSelector;
	private readonly string _expectedNameSelectorExpression;
	private readonly Func<TItem, string?> _getName;
	private readonly Dictionary<TItem, string> _mismatchedNames = new();
	private StringEqualityOptions? _evaluationOptions;

	public HaveNameFromSelectorExpectation(Func<TItem, string> expectedNameSelector,
		string expectedNameSelectorExpression, Func<TItem, string?> getName)
	{
		ThrowHelper.ThrowIfNull(expectedNameSelector, nameof(expectedNameSelector));
		_expectedNameSelector = expectedNameSelector;
		_expectedNameSelectorExpression = expectedNameSelectorExpression;
		_getName = getName;
	}

	public OptionsHolder<StringEqualityOptions> Options { get; } = new(StringEqualityOptions.Default);

	private StringEqualityOptions EvaluationOptions => _evaluationOptions ?? Options.Value;

	public override void OnEvaluating(IEvaluationContext context, CancellationToken cancellationToken)
	{
		_evaluationOptions = Options.Value.ForEvaluation(context, cancellationToken);
		_mismatchedNames.Clear();
	}

	public override void AppendContexts(ResultContextCollector contexts)
		=> EvaluationOptions.AppendContexts(contexts);

	public override async ValueTask<PredicateResult> IsMatch(TItem item, CancellationToken cancellationToken)
	{
		string expectedName = UserCode.Invoke(_expectedNameSelector, item, "the name selector");
		if (await EvaluationOptions.AreConsideredEqual(_getName(item), expectedName, "expectedNameSelector"))
		{
			return true;
		}

		_mismatchedNames[item] = expectedName;
		return false;
	}

	public override void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		=> stringBuilder.Append(grammars.IsPlural() ? "have name matching " : "has name matching ")
			.Append(_expectedNameSelectorExpression.TrimExpression());

	public override string FormatItems(IEnumerable<TItem?> items, FormattingOptions options)
		=> ItemListRenderer.Format(items.ToList(), options.TotalItemCount, (stringBuilder, item) =>
		{
			stringBuilder.Append(Formatter.Format(FormattableMember.From(item)));
			if (item is not null && _mismatchedNames.TryGetValue(item, out string? expectedName))
			{
				stringBuilder.Append(" with name ").Append(Formatter.Format(_getName(item)))
					.Append(" instead of ").Append(Formatter.Format(expectedName));
			}
		});
}
