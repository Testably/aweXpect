using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatConstructors
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<ConstructorInfo?, IEnumerable<ConstructorInfo?>?> Have<TAttribute>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<ConstructorInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(true);
		return new HaveAttributeWithoutInheritResult<ConstructorInfo?, IEnumerable<ConstructorInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<ConstructorInfo?, IEnumerable<ConstructorInfo?>?> Have<TAttribute>(
		this IThat<IEnumerable<ConstructorInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilterOptions<ConstructorInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute(true, predicate, doNotPopulateThisValue);
		return new HaveAttributeWithoutInheritResult<ConstructorInfo?, IEnumerable<ConstructorInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<ConstructorInfo?>>((it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<ConstructorInfo?, IAsyncEnumerable<ConstructorInfo?>?>
		Have<TAttribute>(
			this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<ConstructorInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(true);
		return new HaveAttributeWithoutInheritResult<ConstructorInfo?, IAsyncEnumerable<ConstructorInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="ConstructorInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	[GuaranteesNotNull]
	public static HaveAttributeWithoutInheritResult<ConstructorInfo?, IAsyncEnumerable<ConstructorInfo?>?>
		Have<TAttribute>(
			this IThat<IAsyncEnumerable<ConstructorInfo?>?> subject,
			Func<TAttribute, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilterOptions<ConstructorInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute(true, predicate, doNotPopulateThisValue);
		return new HaveAttributeWithoutInheritResult<ConstructorInfo?, IAsyncEnumerable<ConstructorInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<ConstructorInfo?>>((it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}
#endif

	private sealed class HaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilterOptions<ConstructorInfo?> attributeFilterOptions)
		: CollectionConstraintResult<ConstructorInfo?>(it, grammars),
			IContextConstraint<IEnumerable<ConstructorInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<ConstructorInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<ConstructorInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, attributeFilterOptions.Matches, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<ConstructorInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, attributeFilterOptions.Matches);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching constructors");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching constructors");
	}
}
