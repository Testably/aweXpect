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
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>?>> DoNotHave<TAttribute>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<ConstructorInfo?> attributeFilter = new(
			new AttributeFilterOptions<ConstructorInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new AndOrResult<IEnumerable<ConstructorInfo?>, IThat<IEnumerable<ConstructorInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<ConstructorInfo?>, IEnumerable<ConstructorInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>?>>
		DoNotHave<TAttribute>(
			this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilter<ConstructorInfo?> attributeFilter = new(
			new AttributeFilterOptions<ConstructorInfo?>((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i))
				.WithAttribute<TAttribute>());
		return new AndOrResult<IAsyncEnumerable<ConstructorInfo?>, IThat<IAsyncEnumerable<ConstructorInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilter<ConstructorInfo?>, IAsyncEnumerable<ConstructorInfo?>>(attributeFilter,
				static (attributeFilter, it, grammars)
				=> new DoNotHaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilter)),
			subject);
	}
#endif

	private sealed class DoNotHaveAttributeConstraint(
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
			=> await SetAsyncValue(actual, context,
				member => !attributeFilter.Matches(member), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<ConstructorInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, member => !attributeFilter.Matches(member));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching constructors");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilter.AppendExpectation(stringBuilder, Grammars.Negate());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching constructors");
	}
}
