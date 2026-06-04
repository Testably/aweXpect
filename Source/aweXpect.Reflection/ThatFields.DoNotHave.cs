using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatFields
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="FieldInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>> DoNotHave<TAttribute>(
		this IThat<IEnumerable<FieldInfo?>> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<FieldInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(true);
		return new AndOrResult<IEnumerable<FieldInfo?>, IThat<IEnumerable<FieldInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<FieldInfo?>>((it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="FieldInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	public static AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<FieldInfo?>> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<FieldInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(true);
		return new AndOrResult<IAsyncEnumerable<FieldInfo?>, IThat<IAsyncEnumerable<FieldInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<FieldInfo?>>((it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject);
	}
#endif

	private sealed class DoNotHaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilterOptions<FieldInfo?> attributeFilterOptions)
		: CollectionConstraintResult<FieldInfo?>(grammars),
			IValueConstraint<IEnumerable<FieldInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<FieldInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<FieldInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, member => !attributeFilterOptions.Matches(member));
#endif

		public ConstraintResult IsMetBy(IEnumerable<FieldInfo?> actual)
			=> SetValue(actual, member => !attributeFilterOptions.Matches(member));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilterOptions.AppendDescription(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching fields ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilterOptions.AppendDescription(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching fields ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
