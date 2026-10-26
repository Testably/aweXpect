using System;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect;

public static partial class ThatType
{
	/// <summary>
	///     Verifies that the <see cref="Type" /> has attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HasAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HasAttributeResult<Type, Type?> Has<TAttribute>(this IThat<Type?> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Type?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.AddAttribute<TAttribute>();
		return new HasAttributeResult<Type, Type?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="Type" /> has attribute of type <typeparamref name="TAttribute" /> that
	///     matches the <paramref name="predicate" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HasAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HasAttributeResult<Type, Type?> Has<TAttribute>(
		this IThat<Type?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Type?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.AddAttribute(predicate, doNotPopulateThisValue.TrimExpression());
		return new HasAttributeResult<Type, Type?>(subject.Get().ExpectationBuilder.AddConstraint(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HasAttributeConstraint(it, grammars, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

	private sealed class HasAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilterOptions<Type?> attributeFilterOptions)
		: ConstraintResult.WithNotNullValue<Type?>(it, grammars),
			IValueConstraint<Type?>
	{
		public ConstraintResult IsMetBy(Type? actual)
		{
			Actual = actual;
			Outcome = attributeFilterOptions.Matches(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> attributeFilterOptions.AppendExpectation(stringBuilder, Grammars);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did not in ");
			Formatter.Format(stringBuilder, Actual);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> attributeFilterOptions.AppendExpectation(stringBuilder, Grammars);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" did in ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
