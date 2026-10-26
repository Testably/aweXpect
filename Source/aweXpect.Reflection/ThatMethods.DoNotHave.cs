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

public static partial class ThatMethods
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?> DoNotHave<TAttribute>(
		this IThat<IEnumerable<MethodInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<MethodInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.AddAttribute<TAttribute>();
		return new InheritanceScopeResult<IEnumerable<MethodInfo?>, IEnumerable<MethodInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<MethodInfo?>, IEnumerable<MethodInfo?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions.Scope);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="MethodInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="InheritanceScopeResult{TType, TValue}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static InheritanceScopeResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?> DoNotHave<TAttribute>(
		this IThat<IAsyncEnumerable<MethodInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<MethodInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.AddAttribute<TAttribute>();
		return new InheritanceScopeResult<IAsyncEnumerable<MethodInfo?>, IAsyncEnumerable<MethodInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<MethodInfo?>, IAsyncEnumerable<MethodInfo?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions.Scope);
	}
#endif

	private sealed class DoNotHaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilterOptions<MethodInfo?> attributeFilterOptions)
		: CollectionConstraintResult<MethodInfo?>(it, grammars),
			IContextConstraint<IEnumerable<MethodInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<MethodInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<MethodInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context,
				member => !attributeFilterOptions.Matches(member), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<MethodInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, member => !attributeFilterOptions.Matches(member));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilterOptions.AppendExpectation(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching methods");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilterOptions.AppendExpectation(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching methods");
	}
}
