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

public static partial class ThatEvents
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="EventInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<EventInfo?>, IEnumerable<EventInfo?>?> DoNotHave<TAttribute>(
		this IThat<IEnumerable<EventInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<EventInfo?> attributeFilter = new(
			new AttributeFilterOptions<EventInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new InheritanceScopeResult<IEnumerable<EventInfo?>, IEnumerable<EventInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<EventInfo?>, IEnumerable<EventInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter.Scope);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="EventInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<EventInfo?>, IAsyncEnumerable<EventInfo?>?> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<EventInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<EventInfo?> attributeFilter = new(
			new AttributeFilterOptions<EventInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new InheritanceScopeResult<IAsyncEnumerable<EventInfo?>, IAsyncEnumerable<EventInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<EventInfo?>, IAsyncEnumerable<EventInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter.Scope);
	}
#endif

	private sealed class DoNotHaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilter<EventInfo?> attributeFilter)
		: CollectionConstraintResult<EventInfo?>(it, grammars),
			IContextConstraint<IEnumerable<EventInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<EventInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<EventInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				member => !attributeFilter.Matches(member), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<EventInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, member => !attributeFilter.Matches(member));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching events");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching events");
	}
}
