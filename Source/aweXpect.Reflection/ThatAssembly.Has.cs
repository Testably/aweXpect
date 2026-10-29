using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatAssembly
{
	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HasAttributeWithoutInheritResult<Assembly, Assembly?> Has<TAttribute>(this IThat<Assembly?> subject)
		where TAttribute : Attribute
		=> subject.Has(typeof(TAttribute));

	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has attribute of type <paramref name="attributeType" />.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown if <paramref name="attributeType" /> is no attribute or an open generic type.</exception>
	[GuaranteesNotNull]
	public static HasAttributeWithoutInheritResult<Assembly, Assembly?> Has(
		this IThat<Assembly?> subject,
		Type attributeType)
	{
		AttributeFilter<Assembly?> attributeFilter = new(
			new AttributeFilterOptions<Assembly?>((a, type, p, i) => a.HasAttribute(type, p, i))
				.WithAttribute(attributeType));
		return new HasAttributeWithoutInheritResult<Assembly, Assembly?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter)),
			subject,
			attributeFilter);
	}

	/// <summary>
	///     Verifies that the <see cref="Assembly" /> has attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HasAttributeWithoutInheritResult<Assembly, Assembly?> Has<TAttribute>(
		this IThat<Assembly?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilter<Assembly?> attributeFilter = new(
			new AttributeFilterOptions<Assembly?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));
		return new HasAttributeWithoutInheritResult<Assembly, Assembly?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilter)),
			subject,
			attributeFilter);
	}

	private sealed class HasAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilter<Assembly?> attributeFilter)
		: ConstraintResult.WithNotNullValue<Assembly?>(it, grammars),
			IValueConstraint<Assembly?>
	{
		public ConstraintResult IsMetBy(Assembly? actual)
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
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> attributeFilter.AppendExpectation(stringBuilder, Grammars);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did in ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
