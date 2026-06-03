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
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> override a base class property.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> Override(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new OverrideConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> override a base class property.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>> Override(
		this IThat<IAsyncEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new OverrideConstraint(it, grammars)),
			subject);
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> do not override a base class
	///     property.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> DoNotOverride(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new DoNotOverrideConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> do not override a base class
	///     property.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>> DoNotOverride(
		this IThat<IAsyncEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new DoNotOverrideConstraint(it, grammars)),
			subject);
#endif

	private sealed class OverrideConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<PropertyInfo?>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, property => property.IsOverride());
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
			=> SetValue(actual, property => property.IsOverride());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all override a base property");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained properties which do not override a base property ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("do not all override a base property");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained properties which override a base property ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}

	private sealed class DoNotOverrideConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<PropertyInfo?>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, property => !property.IsOverride());
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
			=> SetValue(actual, property => !property.IsOverride());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all do not override a base property");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained properties which override a base property ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("also contain a property which overrides a base property");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained properties which do not override a base property ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
