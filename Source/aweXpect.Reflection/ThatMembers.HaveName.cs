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
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatMembers
{
	[GenerateCollectionExpectation("HaveName", "has name {expected}", "have name {expected}",
		CollectionOverload = true, NegatedName = "DoNotHaveName", FormattedItem = nameof(ToFormattable),
		NullItemFailsBothWays = true)]
	private static ValueTask<bool> HasName<TMember>(TMember member, [NegatedParameterName("unexpected")] string expected,
		[ExpectationOptions] StringEqualityOptions options)
		where TMember : MemberInfo
		=> options.AreConsideredEqual(member.Name, expected, nameof(expected));

	private static object? ToFormattable(MemberInfo? member) => FormattableMember.From(member);

	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> have the name
	///     returned by the <paramref name="expectedNameSelector" /> for the respective item.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>?>> HaveName<TMember>(
		this IThat<IEnumerable<TMember>?> subject,
		Func<TMember, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
		where TMember : MemberInfo?
	{
		ThrowHelper.ThrowIfNull(expectedNameSelector, nameof(expectedNameSelector));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(Func<TMember, string> ExpectedNameSelector, string DoNotPopulateThisValue, OptionsHolder<StringEqualityOptions> Options), IEnumerable<TMember>>((expectedNameSelector, doNotPopulateThisValue, options),
					static (s, it, grammars)
					=> new HaveNameFromSelectorConstraint<TMember>(it, grammars, s.ExpectedNameSelector,
						s.DoNotPopulateThisValue, s.Options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> have the name
	///     returned by the <paramref name="expectedNameSelector" /> for the respective item.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>?>>
		HaveName<TMember>(
			this IThat<IAsyncEnumerable<TMember>?> subject,
			Func<TMember, string> expectedNameSelector,
			[CallerArgumentExpression(nameof(expectedNameSelector))]
			string doNotPopulateThisValue = "")
		where TMember : MemberInfo?
	{
		ThrowHelper.ThrowIfNull(expectedNameSelector, nameof(expectedNameSelector));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<(Func<TMember, string> ExpectedNameSelector, string DoNotPopulateThisValue, OptionsHolder<StringEqualityOptions> Options), IAsyncEnumerable<TMember>>((expectedNameSelector, doNotPopulateThisValue, options),
					static (s, it, grammars)
					=> new HaveNameFromSelectorConstraint<TMember>(it, grammars, s.ExpectedNameSelector,
						s.DoNotPopulateThisValue, s.Options)),
			subject,
			options);
	}
#endif

	private sealed class HaveNameFromSelectorConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		Func<TMember, string> expectedNameSelector,
		string expectedNameSelectorExpression,
		OptionsHolder<StringEqualityOptions> options)
		: CollectionConstraintResult<TMember>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
		private StringEqualityOptions? _evaluationOptions;

		private StringEqualityOptions EvaluationOptions => _evaluationOptions ?? options.Value;

#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			return await SetAsyncValue(actual, context,
				memberInfo => EvaluationOptions.AreConsideredEqual(memberInfo?.Name,
					UserCode.Invoke(expectedNameSelector, memberInfo, "the name selector"), nameof(expectedNameSelector)), cancellationToken);
		}
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			return await SetValue(actual, context,
				memberInfo => EvaluationOptions.AreConsideredEqual(memberInfo?.Name,
					UserCode.Invoke(expectedNameSelector, memberInfo, "the name selector"), nameof(expectedNameSelector)), cancellationToken);
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

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name matching ")
				.Append(expectedNameSelectorExpression.TrimExpression());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendMatchingCount(stringBuilder);

		protected override Func<string?> FormatItems(TMember[] items)
			=> IsNegated ? base.FormatItems(items) : () => FormatMismatches(items);

		private string FormatMismatches(TMember[] items)
			=> ItemListRenderer.Format(items, (stringBuilder, memberInfo) => stringBuilder
				.Append(Formatter.Format(FormattableMember.From(memberInfo)))
				.Append(" with name ").Append(Formatter.Format(memberInfo?.Name))
				.Append(" instead of ").Append(Formatter.Format(UserCode.Invoke(expectedNameSelector, memberInfo, "the name selector"))));
	}
}
