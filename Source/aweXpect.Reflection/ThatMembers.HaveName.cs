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
using aweXpect.Options;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatMembers
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> have
	///     the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>?>> HaveName<TMember>(
		this IThat<IEnumerable<TMember>?> subject, string expected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<TMember>>((it, grammars)
					=> new HaveNameConstraint<TMember>(it, grammars, expected, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <typeparamref name="TMember" /> have
	///     the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>?>>
		HaveName<TMember>(
			this IThat<IAsyncEnumerable<TMember>?> subject, string expected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<TMember>>((it, grammars)
					=> new HaveNameConstraint<TMember>(it, grammars, expected, options)),
			subject,
			options);
	}
#endif

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
		StringEqualityOptions options = new(nameof(expectedNameSelector));
		return new StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<TMember>>((it, grammars)
					=> new HaveNameFromSelectorConstraint<TMember>(it, grammars, expectedNameSelector,
						doNotPopulateThisValue, options)),
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
		StringEqualityOptions options = new(nameof(expectedNameSelector));
		return new StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<TMember>>((it, grammars)
					=> new HaveNameFromSelectorConstraint<TMember>(it, grammars, expectedNameSelector,
						doNotPopulateThisValue, options)),
			subject,
			options);
	}
#endif

	private sealed class HaveNameConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: CollectionConstraintResult<TMember>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				memberInfo => options.AreConsideredEqual(memberInfo?.Name, expected), cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context,
				memberInfo => options.AreConsideredEqual(memberInfo?.Name, expected), cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching items");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching items");
	}

	private sealed class HaveNameFromSelectorConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		Func<TMember, string> expectedNameSelector,
		string expectedNameSelectorExpression,
		StringEqualityOptions options)
		: CollectionConstraintResult<TMember>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				memberInfo => options.AreConsideredEqual(memberInfo?.Name,
					UserCode.Invoke(expectedNameSelector, memberInfo, "the name selector")), cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<TMember> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context,
				memberInfo => options.AreConsideredEqual(memberInfo?.Name,
					UserCode.Invoke(expectedNameSelector, memberInfo, "the name selector")), cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name matching ")
				.Append(expectedNameSelectorExpression.TrimCommonWhiteSpace());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching items");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name matching ")
				.Append(expectedNameSelectorExpression.TrimCommonWhiteSpace());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching items");

		protected override Func<string?> FormatItems(TMember[] items)
			=> IsNegated ? base.FormatItems(items) : () => FormatMismatches(items);

		private string FormatMismatches(TMember[] items)
		{
			StringBuilder stringBuilder = new();
			stringBuilder.Append('[');
			bool isFirst = true;
			foreach (TMember memberInfo in items)
			{
				if (!isFirst)
				{
					stringBuilder.Append(',');
				}

				isFirst = false;
				stringBuilder.AppendLine().Append("  ")
					.Append(Formatter.Format(FormattableMember.From(memberInfo)))
					.Append(" with name ").Append(Formatter.Format(memberInfo?.Name))
					.Append(" instead of ").Append(Formatter.Format(UserCode.Invoke(expectedNameSelector, memberInfo, "the name selector")));
			}

			return stringBuilder.AppendLine().Append(']').ToString();
		}
	}
}
