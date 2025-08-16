using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static HaveAttributeResult<PropertyInfo?> Have<TAttribute>(
		this IThat<IEnumerable<PropertyInfo?>> subject, bool inherit = true)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<PropertyInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(inherit);
		return new HaveAttributeResult<PropertyInfo?>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static HaveAttributeResult<PropertyInfo?> Have<TAttribute>(
		this IThat<IEnumerable<PropertyInfo?>> subject,
		Func<TAttribute, bool> predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilterOptions<PropertyInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute(inherit, predicate, doNotPopulateThisValue);
		return new HaveAttributeResult<PropertyInfo?>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

	private sealed class HaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilterOptions<PropertyInfo?> attributeFilterOptions)
		: ConstraintResult.WithNotNullValue<IEnumerable<PropertyInfo?>>(it, grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
	{
		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
		{
			Actual = actual;
			Outcome = actual.All(attributeFilterOptions.Matches) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" contained not matching properties ");
			Formatter.Format(stringBuilder,
				Actual?.Where(propertyInfo => !attributeFilterOptions.Matches(propertyInfo)),
				FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilterOptions.AppendDescription(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" only contained matching properties ");
			Formatter.Format(stringBuilder, Actual?.Where(attributeFilterOptions.Matches),
				FormattingOptions.Indented(indentation));
		}
	}
}
