using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
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

public static partial class ThatAssemblies
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IThat<IEnumerable<Assembly?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<Assembly?> attributeFilter = new(
			new AttributeFilterOptions<Assembly?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<Assembly?>, IEnumerable<Assembly?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IThat<IEnumerable<Assembly?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilter<Assembly?> attributeFilter = new(
			new AttributeFilterOptions<Assembly?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));
		return new HaveAttributeWithoutInheritResult<IEnumerable<Assembly?>, Assembly?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<Assembly?>, IEnumerable<Assembly?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<Assembly?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<Assembly?> attributeFilter = new(
			new AttributeFilterOptions<Assembly?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<Assembly?>, IAsyncEnumerable<Assembly?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Assembly" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<Assembly?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilter<Assembly?> attributeFilter = new(
			new AttributeFilterOptions<Assembly?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute(predicate, doNotPopulateThisValue.TrimExpression()));
		return new HaveAttributeWithoutInheritResult<IAsyncEnumerable<Assembly?>, Assembly?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<Assembly?>, IAsyncEnumerable<Assembly?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter);
	}
#endif

	private sealed class HaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilter<Assembly?> attributeFilter)
		: CollectionConstraintResult<Assembly?>(it, grammars),
			IContextConstraint<IEnumerable<Assembly?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Assembly?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Assembly?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, attributeFilter.Matches, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Assembly?> actual, IEvaluationContext context)
			=> SetValue(actual, context, attributeFilter.Matches);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching assemblies");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching assemblies");
	}
}
