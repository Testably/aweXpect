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
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>> HaveName(
		this IThat<IEnumerable<Assembly?>?> subject, string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
					=> new HaveNameConstraint(it, grammars, expected, options)),
			subject,
			options);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     the <paramref name="expected" /> name.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>> HaveName(
		this IThat<IAsyncEnumerable<Assembly?>?> subject, string expected)
	{
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
					=> new HaveNameConstraint(it, grammars, expected, options)),
			subject,
			options);
	}
#endif

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
		StringEqualityOptions options = new(nameof(expectedNameSelector));
		return new StringEqualityTypeResult<IEnumerable<Assembly?>, IThat<IEnumerable<Assembly?>?>>(subject.Get()
				.ExpectationBuilder.AddConstraint<IEnumerable<Assembly?>>((it, grammars)
					=> new HaveNameFromSelectorConstraint(it, grammars, expectedNameSelector, doNotPopulateThisValue,
						options)),
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
		StringEqualityOptions options = new(nameof(expectedNameSelector));
		return new StringEqualityTypeResult<IAsyncEnumerable<Assembly?>, IThat<IAsyncEnumerable<Assembly?>?>>(subject
				.Get()
				.ExpectationBuilder.AddConstraint<IAsyncEnumerable<Assembly?>>((it, grammars)
					=> new HaveNameFromSelectorConstraint(it, grammars, expectedNameSelector, doNotPopulateThisValue,
						options)),
			subject,
			options);
	}
#endif

	private sealed class HaveNameConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		StringEqualityOptions options)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				assembly => options.AreConsideredEqual(assembly?.GetName().Name, expected), cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context,
				assembly => options.AreConsideredEqual(assembly?.GetName().Name, expected), cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching assemblies");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name ").Append(options.GetExpectation(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching assemblies");
	}

	private sealed class HaveNameFromSelectorConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<Assembly?, string> expectedNameSelector,
		string expectedNameSelectorExpression,
		StringEqualityOptions options)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				assembly => options.AreConsideredEqual(assembly?.GetName().Name,
					UserCode.Invoke(expectedNameSelector, assembly, "the name selector")), cancellationToken);
#endif

		public async ValueTask<ConstraintResult> IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetValue(actual, context,
				assembly => options.AreConsideredEqual(assembly?.GetName().Name,
					UserCode.Invoke(expectedNameSelector, assembly, "the name selector")), cancellationToken);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have name matching ")
				.Append(expectedNameSelectorExpression.TrimCommonWhiteSpace());

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching assemblies");

		protected override Func<string?> FormatItems(Assembly?[] items)
			=> IsNegated ? base.FormatItems(items) : () => FormatMismatches(items);

		private string FormatMismatches(Assembly?[] items)
		{
			StringBuilder stringBuilder = new();
			stringBuilder.Append('[');
			bool isFirst = true;
			foreach (Assembly? assembly in items)
			{
				if (!isFirst)
				{
					stringBuilder.Append(',');
				}

				isFirst = false;
				stringBuilder.AppendLine().Append("  ")
					.Append(Formatter.Format(assembly))
					.Append(" with name ").Append(Formatter.Format(assembly?.GetName().Name))
					.Append(" instead of ").Append(Formatter.Format(UserCode.Invoke(expectedNameSelector, assembly, "the name selector")));
			}

			return stringBuilder.AppendLine().Append(']').ToString();
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have name matching ")
				.Append(expectedNameSelectorExpression.TrimCommonWhiteSpace());

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching assemblies");
	}
}
