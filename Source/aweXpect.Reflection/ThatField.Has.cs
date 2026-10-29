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

public static partial class ThatField
{
	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> has attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HasAttributeWithoutInheritResult<FieldInfo, FieldInfo?> Has<TAttribute>(this IThat<FieldInfo?> subject)
		where TAttribute : Attribute
		=> subject.Has(typeof(TAttribute));

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> has attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static HasAttributeWithoutInheritResult<FieldInfo, FieldInfo?> Has(
		this IThat<FieldInfo?> subject,
		Type attributeType)
	{
		AttributeFilter<FieldInfo?> attributeFilter = new(
			new AttributeFilterOptions<FieldInfo?>((a, type, p, i) => a.HasAttribute(type, p, i))
				.WithAttribute(attributeType));
		return new HasAttributeWithoutInheritResult<FieldInfo, FieldInfo?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter)),
			subject,
			attributeFilter);
	}

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> has attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HasAttributeWithoutInheritResult<FieldInfo, FieldInfo?> Has<TAttribute>(
		this IThat<FieldInfo?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilter<FieldInfo?> attributeFilter = new(
			new AttributeFilterOptions<FieldInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));
		return new HasAttributeWithoutInheritResult<FieldInfo, FieldInfo?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter)),
			subject,
			attributeFilter);
	}

	private sealed class HasAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilter<FieldInfo?> attributeFilter)
		: ConstraintResult.WithNotNullValue<FieldInfo?>(it, grammars),
			IValueConstraint<FieldInfo?>
	{
		public ConstraintResult IsMetBy(FieldInfo? actual)
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
