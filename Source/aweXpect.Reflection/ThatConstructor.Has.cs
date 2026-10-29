using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Formatting;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatConstructor
{
	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HasAttributeWithoutInheritResult<ConstructorInfo, ConstructorInfo?> Has<TAttribute>(
		this IThat<ConstructorInfo?> subject)
		where TAttribute : Attribute
		=> subject.Has(typeof(TAttribute));

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute.</exception>
	[GuaranteesNotNull]
	public static HasAttributeWithoutInheritResult<ConstructorInfo, ConstructorInfo?> Has(
		this IThat<ConstructorInfo?> subject,
		Type attributeType)
	{
		AttributeFilter<ConstructorInfo?> attributeFilter = new(
			new AttributeFilterOptions<ConstructorInfo?>((a, type, p, i) => a.HasAttribute(type, p, i))
				.WithAttribute(attributeType));
		return new HasAttributeWithoutInheritResult<ConstructorInfo, ConstructorInfo?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter)),
			subject,
			attributeFilter);
	}

	/// <summary>
	///     Verifies that the <see cref="ConstructorInfo" /> has attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HasAttributeWithoutInheritResult<ConstructorInfo, ConstructorInfo?> Has<TAttribute>(
		this IThat<ConstructorInfo?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilter<ConstructorInfo?> attributeFilter = new(
			new AttributeFilterOptions<ConstructorInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute(predicate, doNotPopulateThisValue));
		return new HasAttributeWithoutInheritResult<ConstructorInfo, ConstructorInfo?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter)),
			subject,
			attributeFilter);
	}

	private sealed class HasAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilter<ConstructorInfo?> attributeFilter)
		: ConstraintResult.WithNotNullValue<ConstructorInfo?>(it, grammars),
			IValueConstraint<ConstructorInfo?>
	{
		public ConstraintResult IsMetBy(ConstructorInfo? actual)
		{
			Actual = actual;
			Outcome = attributeFilter.Matches(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> attributeFilter.AppendExpectation(stringBuilder, Grammars);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not in ");
			Formatter.Format(stringBuilder, FormattableMember.From(Actual));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> attributeFilter.AppendExpectation(stringBuilder, Grammars);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did in ");
			Formatter.Format(stringBuilder, FormattableMember.From(Actual));
		}
	}
}
