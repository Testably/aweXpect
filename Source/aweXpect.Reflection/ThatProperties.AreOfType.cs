using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Reflection;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all properties in the filtered collection are of type <typeparamref name="TProperty" /> (or a subtype).
	/// </summary>
	public static PropertiesOfTypeResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>>
		AreOfType<TProperty>(
			this IThat<IEnumerable<PropertyInfo?>> subject)
		=> AreOfType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
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
	public static PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>>
		AreOfType<TProperty>(
			this IThat<IAsyncEnumerable<PropertyInfo?>> subject)
		=> AreOfType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of type <paramref name="propertyType" /> (or a subtype).
	/// </summary>
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
		: CollectionConstraintResult<PropertyInfo?>(grammars),
			IValueConstraint<IEnumerable<PropertyInfo?>>
#if NET8_0_OR_GREATER
			, IAsyncConstraint<IAsyncEnumerable<PropertyInfo?>>
#endif
	{
#if NET8_0_OR_GREATER
		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<PropertyInfo?> actual,
			CancellationToken cancellationToken)
			=> await SetAsyncValue(actual, property => typeFilterOptions.Matches(property?.PropertyType));
#endif

		public ConstraintResult IsMetBy(IEnumerable<PropertyInfo?> actual)
			=> SetValue(actual, property => typeFilterOptions.Matches(property?.PropertyType));

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("all are ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" contained not matching properties ");
			Formatter.Format(stringBuilder, NotMatching, FormattingOptions.Indented(indentation));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("not all are ");
			typeFilterOptions.AppendOfTypeDescription(stringBuilder);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" only contained matching properties ");
			Formatter.Format(stringBuilder, Matching, FormattingOptions.Indented(indentation));
		}
	}
}
