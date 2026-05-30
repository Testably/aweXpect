using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	public static PropertiesOfTypeResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>>
		AreOfExactType<TProperty>(
			this IThat<IEnumerable<PropertyInfo?>> subject)
		=> AreOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <paramref name="propertyType" />.
	/// </summary>
	public static PropertiesOfTypeResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>> AreOfExactType(
		this IThat<IEnumerable<PropertyInfo?>> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, true);
		return new PropertiesOfTypeResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	public static PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>>
		AreOfExactType<TProperty>(
			this IThat<IAsyncEnumerable<PropertyInfo?>> subject)
		=> AreOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <paramref name="propertyType" />.
	/// </summary>
	public static PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>>
		AreOfExactType(
			this IThat<IAsyncEnumerable<PropertyInfo?>> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, true);
		return new PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>>>(
			subject.Get().ExpectationBuilder.AddConstraint<IAsyncEnumerable<PropertyInfo?>>((it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	public sealed partial class PropertiesOfTypeResult<TValue, TResult>
	{
		/// <summary>
		///     Allow an alternative exact type <typeparamref name="TProperty" />.
		/// </summary>
		public PropertiesOfTypeResult<TValue, TResult> OrOfExactType<TProperty>()
			=> OrOfExactType(typeof(TProperty));

		/// <summary>
		///     Allow an alternative exact type <paramref name="propertyType" />.
		/// </summary>
		public PropertiesOfTypeResult<TValue, TResult> OrOfExactType(Type propertyType)
		{
			typeFilterOptions.RegisterType(propertyType, true);
			return this;
		}
	}
}
