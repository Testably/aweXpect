using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAssemblies
{
	[GenerateCollectionExpectation("HaveName", "has name {expected}", "have name {expected}",
		CollectionOverload = true, NegatedName = "DoNotHaveName", NullItemFailsBothWays = true)]
	private static ValueTask<bool> HasName(Assembly assembly, [NegatedParameterName("unexpected")] string expected,
		[ExpectationOptions] StringEqualityOptions options)
		=> options.AreConsideredEqual(assembly.GetName().Name, expected, nameof(expected));

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have the name
	///     returned by the <paramref name="expectedNameSelector" /> for the respective item.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> HaveName(
		this IThat<IEnumerable<Assembly?>?> subject,
		Func<Assembly?, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(expectedNameSelector, nameof(expectedNameSelector));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(Func<Assembly?, string> ExpectedNameSelector, string DoNotPopulateThisValue, OptionsHolder<StringEqualityOptions> Options), IEnumerable<Assembly?>>((expectedNameSelector, doNotPopulateThisValue, options),
					static (s, it, grammars)
					=> new HaveNameFromSelectorConstraint(it, grammars, s.ExpectedNameSelector, s.DoNotPopulateThisValue,
						s.Options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have the name
	///     returned by the <paramref name="expectedNameSelector" /> for the respective item.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> HaveName(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		Func<Assembly?, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
	{
		ThrowHelper.ThrowIfNull(expectedNameSelector, nameof(expectedNameSelector));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint<(Func<Assembly?, string> ExpectedNameSelector, string DoNotPopulateThisValue, OptionsHolder<StringEqualityOptions> Options), IAsyncEnumerable<Assembly?>>((expectedNameSelector, doNotPopulateThisValue, options),
					static (s, it, grammars)
					=> new HaveNameFromSelectorConstraint(it, grammars, s.ExpectedNameSelector, s.DoNotPopulateThisValue,
						s.Options)),
			subject,
			options);
	}
#endif

	private sealed class HaveNameFromSelectorConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<Assembly?, string> expectedNameSelector,
		string expectedNameSelectorExpression,
		OptionsHolder<StringEqualityOptions> options)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
		private StringEqualityOptions? _evaluationOptions;

		private StringEqualityOptions EvaluationOptions => _evaluationOptions ?? options.Value;

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			return await SetAsyncValue(actual, context,
				assembly => EvaluationOptions.AreConsideredEqual(assembly?.GetName().Name,
					UserCode.Invoke(expectedNameSelector, assembly, "the name selector"), nameof(expectedNameSelector)), cancellationToken);
		}
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			return await SetValue(actual, context,
				assembly => EvaluationOptions.AreConsideredEqual(assembly?.GetName().Name,
					UserCode.Invoke(expectedNameSelector, assembly, "the name selector"), nameof(expectedNameSelector)), cancellationToken);
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			base.AppendContexts(contexts);
			EvaluationOptions.AppendContexts(contexts);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name matching ")
				.Append(expectedNameSelectorExpression.TrimExpression());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendMatchingCount(stringBuilder);

		protected override Func<string?> FormatItems(Assembly?[] items)
			=> IsNegated ? base.FormatItems(items) : () => FormatMismatches(items);

		private string FormatMismatches(Assembly?[] items)
			=> ItemListRenderer.Format(items, (stringBuilder, assembly) => stringBuilder
				.Append(Formatter.Format(assembly))
				.Append(" with name ").Append(Formatter.Format(assembly?.GetName().Name))
				.Append(" instead of ").Append(Formatter.Format(UserCode.Invoke(expectedNameSelector, assembly, "the name selector"))));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name matching ")
				.Append(expectedNameSelectorExpression.TrimExpression());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendMatchingCount(stringBuilder);
	}
}
