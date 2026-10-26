using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<PropertyInfo?>, IEnumerable<PropertyInfo?>?> DoNotHave<TAttribute>(
		this IThat<IEnumerable<PropertyInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<PropertyInfo?> attributeFilter = new(
			new AttributeFilterOptions<PropertyInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new InheritanceScopeResult<IEnumerable<PropertyInfo?>, IEnumerable<PropertyInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<PropertyInfo?>, IEnumerable<PropertyInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter.Scope);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<PropertyInfo?>, IAsyncEnumerable<PropertyInfo?>?>
		DoNotHave<TAttribute>(
			this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<PropertyInfo?> attributeFilter = new(
			new AttributeFilterOptions<PropertyInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new InheritanceScopeResult<IAsyncEnumerable<PropertyInfo?>, IAsyncEnumerable<PropertyInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<PropertyInfo?>, IAsyncEnumerable<PropertyInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter.Scope);
	}
#endif

	private sealed class DoNotHaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilter<PropertyInfo?> attributeFilter)
		: CollectionConstraintResult<PropertyInfo?>(it, grammars),
			IContextConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				member => !attributeFilter.Matches(member), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, member => !attributeFilter.Matches(member));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching properties");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching properties");
	}
}
