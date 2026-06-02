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
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have a
	///     (regular, non-init) setter.
	/// </summary>
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> HaveASetter(
		this IThat<IEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new HaveASetterConstraint(it, grammars)),
			subject);

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have a
	///     (regular, non-init) setter.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>> HaveASetter(
		this IThat<IAsyncEnumerable<PropertyInfo?>> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new HaveASetterConstraint(it, grammars)),
			subject);
#endif

	private sealed class HaveASetterConstraint(string it, ExpectationGrammars grammars)
		: CollectionConstraintResult<PropertyInfo?>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, property => property.HasSetter());
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
			=> SetValue(actual, property => property.HasSetter());

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("all have a setter");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained properties without a setter ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("not all have a setter");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained properties with a setter ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
