using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     The item expectations of a quantified <c>ComplyWith</c> and their expectation text.
/// </summary>
/// <remarks>
///     In a nested expectation, the quantifier is the subject of the item expectations, so their verb agrees with its
///     number (<c>none start with …</c>, <c>at least one starts with …</c>). The negation can change this number, so
///     the negated text then comes from a second set of item expectations, which is only used for the text.
/// </remarks>
internal sealed class ComplyWithItemExpectations<TItem>
{
	private readonly ExpectationGrammars _grammars;
	private readonly ManualExpectationBuilder<TItem> _negatedBuilder;
	private readonly EnumerableQuantifier _quantifier;

	public ComplyWithItemExpectations(EnumerableQuantifier quantifier, ExpectationGrammars grammars,
		Action<IThatSubject<TItem>> expectations)
	{
		_quantifier = quantifier;
		_grammars = grammars;
		Builder = Create(false, expectations);
		_negatedBuilder = grammars.IsNested() && quantifier.IsRenderedSingle(true) != quantifier.IsRenderedSingle(false)
			? Create(true, expectations)
			: Builder;
	}

	/// <summary>
	///     The item expectations that are evaluated.
	/// </summary>
	public ManualExpectationBuilder<TItem> Builder { get; }

	public async Task PrepareExpectation(IEvaluationContext context, CancellationToken cancellationToken)
	{
		await Builder.PrepareExpectation(context, cancellationToken);
		if (!ReferenceEquals(_negatedBuilder, Builder))
		{
			await _negatedBuilder.PrepareExpectation(context, cancellationToken);
		}
	}

	public void AppendExpectation(StringBuilder stringBuilder, bool isNegated, string? indentation)
	{
		if (_grammars.IsNested())
		{
			ManualExpectationBuilder<TItem> builder = isNegated ? _negatedBuilder : Builder;
			stringBuilder.AppendNestedQuantifier(_quantifier, isNegated, _grammars, _ =>
			{
				StringBuilder itemExpectation = new();
				builder.AppendExpectation(itemExpectation, indentation);
				return itemExpectation.ToString();
			});
			return;
		}

		Builder.AppendExpectation(stringBuilder, indentation);
		if (isNegated)
		{
			_quantifier.AppendNegated(stringBuilder);
		}
		else
		{
			stringBuilder.Append(" for ").Append(_quantifier).Append(' ').Append(_quantifier.GetItemString());
		}
	}

	private ManualExpectationBuilder<TItem> Create(bool isNegated, Action<IThatSubject<TItem>> expectations)
	{
		// Without a nested quantifier, the item expectations keep the number of the subject that a connector such as
		// "whose values" introduced.
		ExpectationGrammars itemGrammars = _grammars;
		if (_grammars.IsNested())
		{
			// The quantifier is the subject of the item expectations, so they are no longer nested in a member.
			itemGrammars &= ~(ExpectationGrammars.Nested | ExpectationGrammars.Negated);
			itemGrammars = _quantifier.IsRenderedSingle(isNegated)
				? itemGrammars & ~ExpectationGrammars.Plural
				: itemGrammars | ExpectationGrammars.Plural;
		}

		ManualExpectationBuilder<TItem> builder = new(null, itemGrammars);
		expectations.Invoke(new ThatSubject<TItem>(builder));
		return builder;
	}
}
