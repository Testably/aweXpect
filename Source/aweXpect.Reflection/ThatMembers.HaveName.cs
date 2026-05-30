using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
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
	public static StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> HaveName<TMember>(
		this IThat<IEnumerable<TMember>> subject, string expected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>>(subject.Get()
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
	public static StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>>
		HaveName<TMember>(
			this IThat<IAsyncEnumerable<TMember>> subject, string expected)
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>>(subject.Get()
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
	public static StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>> HaveName<TMember>(
		this IThat<IEnumerable<TMember>> subject,
		Func<TMember, string> expectedNameSelector,
		[CallerArgumentExpression(nameof(expectedNameSelector))]
		string doNotPopulateThisValue = "")
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IEnumerable<TMember>, IThat<IEnumerable<TMember>>>(subject.Get()
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
	public static StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>>
		HaveName<TMember>(
			this IThat<IAsyncEnumerable<TMember>> subject,
			Func<TMember, string> expectedNameSelector,
			[CallerArgumentExpression(nameof(expectedNameSelector))]
			string doNotPopulateThisValue = "")
		where TMember : MemberInfo?
	{
		StringEqualityOptions options = new();
		return new StringEqualityTypeResult<IAsyncEnumerable<TMember>, IThat<IAsyncEnumerable<TMember>>>(subject.Get()
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
		: CollectionConstraintResult<TMember>(grammars),
			IAsyncConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, memberInfo => options.AreConsideredEqual(memberInfo?.Name, expected));
#endif

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TMember> actual, CancellationToken cancellationToken)
			=> await SetValue(actual, memberInfo => options.AreConsideredEqual(memberInfo?.Name, expected));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching items ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching items ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class HaveNameFromSelectorConstraint<TMember>(
		string it,
		ExpectationGrammars grammars,
		Func<TMember, string> expectedNameSelector,
		string expectedNameSelectorExpression,
		StringEqualityOptions options)
		: CollectionConstraintResult<TMember>(grammars),
			IAsyncConstraint<IEnumerable<TMember>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<TMember>>
#endif
		where TMember : MemberInfo?
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TMember> actual, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual,
				memberInfo => options.AreConsideredEqual(memberInfo?.Name, expectedNameSelector(memberInfo)));
#endif

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TMember> actual, CancellationToken cancellationToken)
			=> await SetValue(actual,
				memberInfo => options.AreConsideredEqual(memberInfo?.Name, expectedNameSelector(memberInfo)));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name matching ")
				.Append(expectedNameSelectorExpression.TrimCommonWhiteSpace());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendMismatches(stringBuilder, indentation);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name matching ")
				.Append(expectedNameSelectorExpression.TrimCommonWhiteSpace());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching items ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}

		private void AppendMismatches(StringBuilder stringBuilder, string? indentation)
		{
			stringBuilder.Append(it).Append(" contained not matching items [");
			bool isFirst = true;
			foreach (TMember memberInfo in NotMatching)
			{
				if (!isFirst)
				{
					stringBuilder.Append(',');
				}

				isFirst = false;
				stringBuilder.AppendLine().Append(indentation).Append("  ")
					.Append(Formatter.Format(memberInfo))
					.Append(" with name ").Append(Formatter.Format(memberInfo?.Name))
					.Append(" instead of ").Append(Formatter.Format(expectedNameSelector(memberInfo)));
			}

			stringBuilder.AppendLine().Append(indentation).Append(']');
		}
	}
}
