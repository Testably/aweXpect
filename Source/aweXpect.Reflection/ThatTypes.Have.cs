using System;
using System.Collections.Generic;
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

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<Type?>, Type?> Have<TAttribute>(
		this IThat<IEnumerable<Type?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Type?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.AddAttribute<TAttribute>();
		return new HaveAttributeResult<IEnumerable<Type?>, Type?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<Type?>, IEnumerable<Type?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IEnumerable<Type?>, Type?> Have<TAttribute>(
		this IThat<IEnumerable<Type?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Type?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.AddAttribute(predicate, doNotPopulateThisValue.TrimExpression());
		return new HaveAttributeResult<IEnumerable<Type?>, Type?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<Type?>, IEnumerable<Type?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Type?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.AddAttribute<TAttribute>();
		return new HaveAttributeResult<IAsyncEnumerable<Type?>, Type?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<Type?>, IAsyncEnumerable<Type?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     Attributes inherited from a base type or an overridden member are considered as well; restrict the check
	///     to the attributes declared directly on the member with <see cref="HaveAttributeResult{TType, TMember}.Directly" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<IAsyncEnumerable<Type?>, Type?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<Type?>?> subject,
		Func<TAttribute, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilterOptions<Type?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.AddAttribute(predicate, doNotPopulateThisValue.TrimExpression());
		return new HaveAttributeResult<IAsyncEnumerable<Type?>, Type?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<Type?>, IAsyncEnumerable<Type?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}
#endif

	private sealed class HaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilterOptions<Type?> attributeFilterOptions)
		: CollectionConstraintResult<Type?>(it, grammars),
			IContextConstraint<IEnumerable<Type?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<Type?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<Type?> actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, attributeFilterOptions.Matches, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<Type?> actual, IEvaluationContext context)
			=> SetValue(actual, context, attributeFilterOptions.Matches);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilterOptions.AppendExpectation(stringBuilder, Grammars);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching types");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilterOptions.AppendExpectation(stringBuilder, Grammars);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching types");
	}
}
