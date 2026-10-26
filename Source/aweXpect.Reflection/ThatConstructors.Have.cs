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

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<ConstructorInfo?> attributeFilter = new(
			new AttributeFilterOptions<ConstructorInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<ConstructorInfo?>, IEnumerable<ConstructorInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?> Have<TAttribute>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilter<ConstructorInfo?> attributeFilter = new(
			new AttributeFilterOptions<ConstructorInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute(predicate, doNotPopulateThisValue));
		return new HaveAttributeWithoutInheritResult<IEnumerable<ConstructorInfo?>, ConstructorInfo?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<ConstructorInfo?>, IEnumerable<ConstructorInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?>
		Have<TAttribute>(
			this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<ConstructorInfo?> attributeFilter = new(
			new AttributeFilterOptions<ConstructorInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?>
		Have<TAttribute>(
			this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject,
			Func<TAttribute, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilter<ConstructorInfo?> attributeFilter = new(
			new AttributeFilterOptions<ConstructorInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute(predicate, doNotPopulateThisValue));
		return new HaveAttributeWithoutInheritResult<IAsyncEnumerable<ConstructorInfo?>, ConstructorInfo?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject,
			attributeFilter);
	}
#endif

	private sealed class HaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilter<ConstructorInfo?> attributeFilter)
		: CollectionConstraintResult<ConstructorInfo?>(it, grammars),
			IContextConstraint<IEnumerable<ConstructorInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<ConstructorInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<ConstructorInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, attributeFilter.Matches, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<ConstructorInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, attributeFilter.Matches);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching constructors");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching constructors");
	}
}
