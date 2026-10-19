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

namespace aweXpect.Reflection;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<PropertyInfo?, IEnumerable<PropertyInfo?>?> Have<TAttribute>(
		this IThat<IEnumerable<PropertyInfo?>?> subject, bool inherit = true)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<PropertyInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(inherit);
		return new HaveAttributeResult<PropertyInfo?, IEnumerable<PropertyInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<PropertyInfo?>, IEnumerable<PropertyInfo?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<PropertyInfo?, IEnumerable<PropertyInfo?>?> Have<TAttribute>(
		this IThat<IEnumerable<PropertyInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilterOptions<PropertyInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute(inherit, predicate, doNotPopulateThisValue.TrimExpression());
		return new HaveAttributeResult<PropertyInfo?, IEnumerable<PropertyInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<PropertyInfo?>, IEnumerable<PropertyInfo?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<PropertyInfo?, IAsyncEnumerable<PropertyInfo?>?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject, bool inherit = true)
		where TAttribute : Attribute
	{
		AttributeFilterOptions<PropertyInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute<TAttribute>(inherit);
		return new HaveAttributeResult<PropertyInfo?, IAsyncEnumerable<PropertyInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<PropertyInfo?>, IAsyncEnumerable<PropertyInfo?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="PropertyInfo" /> have
	///     attribute of type <typeparamref name="TAttribute" />.
	/// </summary>
	/// <remarks>
	///     The optional parameter <paramref name="inherit" /> (default value <see langword="true" /> specifies, if
	///     the attribute can be inherited from a base type.
	/// </remarks>
	[GuaranteesNotNull]
	public static HaveAttributeResult<PropertyInfo?, IAsyncEnumerable<PropertyInfo?>?> Have<TAttribute>(
		this IThat<IAsyncEnumerable<PropertyInfo?>?> subject,
		Func<TAttribute, bool> predicate,
		bool inherit = true,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TAttribute : Attribute
	{
		AttributeFilterOptions<PropertyInfo?> attributeFilterOptions =
			new((a, attributeType, p, i) => a.HasAttribute(attributeType, p, i));
		attributeFilterOptions.RegisterAttribute(inherit, predicate, doNotPopulateThisValue.TrimExpression());
		return new HaveAttributeResult<PropertyInfo?, IAsyncEnumerable<PropertyInfo?>?>(
			subject.Get().ExpectationBuilder.AddConstraint<AttributeFilterOptions<PropertyInfo?>, IAsyncEnumerable<PropertyInfo?>>(attributeFilterOptions,
				static (attributeFilterOptions, it, grammars)
				=> new HaveAttributeConstraint(it, grammars | ExpectationGrammars.Plural, attributeFilterOptions)),
			subject,
			attributeFilterOptions);
	}
#endif

	private sealed class HaveAttributeConstraint(
		string it,
		ExpectationGrammars grammars,
		AttributeFilterOptions<PropertyInfo?> attributeFilterOptions)
		: CollectionConstraintResult<PropertyInfo?>(it, grammars),
			IContextConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncContextConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async ValueTask<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			IEvaluationContext context, CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, context, attributeFilterOptions.Matches, cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, attributeFilterOptions.Matches);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all ");
			attributeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching properties");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all ");
			attributeFilterOptions.AppendDescription(stringBuilder, Grammars);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching properties");
	}
}
