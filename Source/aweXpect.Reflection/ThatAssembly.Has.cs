using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect.Reflection;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" />) specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static HasAttributeResult<Assembly?> Has<TAttribute>(this IThat<Assembly?> subject, bool inherit = true)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Assembly?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(inherit);
		return new HasAttributeResult<Assembly?>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" />) specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	public static HasAttributeResult<Assembly?> Has<TAttribute>(
		this IThat<Assembly?> subject,
		Func<TAttribute, bool> predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Assembly?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute(inherit, predicate, doNotPopulateThisValue);
		return new HasAttributeResult<Assembly?>(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

	private sealed class HasAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilterOptions<Assembly?> attributeFilterOptions)
		: ConstraintResult.WithNotNullValue<Assembly?>(it, grammars),
			IValueConstraint<Assembly?>
	{
		public ConstraintResult IsMetBy(Assembly? actual)
		{
			Actual = actual;
			Outcome = attributeFilterOptions.Matches(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> attributeFilterOptions.AppendDescription(stringBuilder, Grammars);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not in ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> attributeFilterOptions.AppendDescription(stringBuilder, Grammars);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did in ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
