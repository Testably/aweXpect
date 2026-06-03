using System.Collections.Generic;
using System.Reflection;
using System.Text;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are virtual.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreVirtual(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreVirtualConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are virtual.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>> AreVirtual(
		this IThat<IAsyncEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreVirtualConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are not virtual.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreNotVirtual(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreNotVirtualConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> are not virtual.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>> AreNotVirtual(
		this IThat<IAsyncEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreNotVirtualConstraint(it, grammars)),
			subject);
#endif

	private sealed class AreVirtualConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<PropertyInfo?>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, property => property.IsReallyVirtual());
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
			=> SetValue(actual, property => property.IsReallyVirtual());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all virtual");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained non-virtual properties ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are not all virtual");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained virtual properties ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class AreNotVirtualConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<PropertyInfo?>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, property => !property.IsReallyVirtual());
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
			=> SetValue(actual, property => !property.IsReallyVirtual());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("are all not virtual");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained virtual properties ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a virtual property");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained non-virtual properties ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
