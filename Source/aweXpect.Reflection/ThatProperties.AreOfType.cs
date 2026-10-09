using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all properties in the filtered collection are of type <typeparamref name="TProperty" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static PropertiesOfTypeResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>>
		AreOfType<TProperty>(
			this IThat<IEnumerable<PropertyInfo?>> subject)
		=> AreOfType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static PropertiesOfTypeResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreOfType(
		this IThat<IEnumerable<PropertyInfo?>> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, false);
		return new PropertiesOfTypeResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all properties in the filtered collection are of type <typeparamref name="TProperty" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>>
		AreOfType<TProperty>(
			this IThat<IAsyncEnumerable<PropertyInfo?>> subject)
		=> AreOfType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
	[GuaranteesNotNull]
	public static PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>>
		AreOfType(
			this IThat<IAsyncEnumerable<PropertyInfo?>> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, false);
		return new PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	/// <summary>
	///     Result that allows chaining additional types for property collections.
	/// </summary>
	public sealed partial class PropertiesOfTypeResult<TValue, TResult>(
		ExpectationBuilder expectationBuilder,
		TResult subject,
		TypeFilterOptions typeFilterOptions)
		: AndOrResult<TValue, TResult>(expectationBuilder, subject),
			IOptionsProvider<TypeFilterOptions>
		where TResult : IThat<TValue>
	{
		/// <inheritdoc cref="IOptionsProvider{TypeFilterOptions}.Options" />
		TypeFilterOptions IOptionsProvider<TypeFilterOptions>.Options => typeFilterOptions;

		/// <summary>
		///     Allow an alternative type <typeparamref name="TProperty" /> (or a subtype).
		/// </summary>
		public PropertiesOfTypeResult<TValue, TResult> OrOfType<TProperty>()
			=> OrOfType(typeof(TProperty));

		/// <summary>
		///     Allow an alternative type <paramref name="propertyType" /> (or a subtype).
		/// </summary>
		public PropertiesOfTypeResult<TValue, TResult> OrOfType(Type propertyType)
		{
			typeFilterOptions.RegisterType(propertyType, false);
			return this;
		}
	}

	private sealed class AreOfTypeConstraint(
		string it,
		ExpectationGrammars grammars,
		TypeFilterOptions typeFilterOptions)
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
				property => typeFilterOptions.Matches(property?.PropertyType), cancellationToken);
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual, IEvaluationContext context)
			=> SetValue(actual, context, property => typeFilterOptions.Matches(property?.PropertyType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all are ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" contained not matching properties");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all are ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" only contained matching properties");
	}
}
