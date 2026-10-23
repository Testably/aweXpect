using System;
using System.Collections.Generic;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatProperties
{
	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	[GuaranteesNotNull]
	public static PropertiesOfTypeResult<IEnumerable<PropertyInfo?>?, IThat<IEnumerable<PropertyInfo?>?>>
		AreOfExactType<TProperty>(
			this IThat<IEnumerable<PropertyInfo?>?> subject)
		=> AreOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <paramref name="propertyType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static PropertiesOfTypeResult<IEnumerable<PropertyInfo?>?, IThat<IEnumerable<PropertyInfo?>?>> AreOfExactType(
		this IThat<IEnumerable<PropertyInfo?>?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, true);
		return new PropertiesOfTypeResult<IEnumerable<PropertyInfo?>?, IThat<IEnumerable<PropertyInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<PropertyInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	[GuaranteesNotNull]
	public static PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>?, IThat<IAsyncEnumerable<PropertyInfo?>?>>
		AreOfExactType<TProperty>(
			this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		=> AreOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that all properties in the filtered collection are of exactly type <paramref name="propertyType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>?, IThat<IAsyncEnumerable<PropertyInfo?>?>>
		AreOfExactType(
			this IThat<IAsyncEnumerable<PropertyInfo?>?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, true);
		return new PropertiesOfTypeResult<IAsyncEnumerable<PropertyInfo?>?, IThat<IAsyncEnumerable<PropertyInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<PropertyInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}
#endif

	/// <summary>
	///     Verifies that none of the properties in the filtered collection are of exactly type
	///     <typeparamref name="TProperty" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>>
		AreNotOfExactType<TProperty>(
			this IThat<IEnumerable<PropertyInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that none of the properties in the filtered collection are of exactly type
	///     <paramref name="propertyType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>> AreNotOfExactType(
		this IThat<IEnumerable<PropertyInfo?>?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, true);
		return new AndOrResult<IEnumerable<PropertyInfo?>, IThat<IEnumerable<PropertyInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IEnumerable<PropertyInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
	}

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that none of the properties in the filtered collection are of exactly type
	///     <typeparamref name="TProperty" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>>
		AreNotOfExactType<TProperty>(
			this IThat<IAsyncEnumerable<PropertyInfo?>?> subject)
		=> AreNotOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that none of the properties in the filtered collection are of exactly type
	///     <paramref name="propertyType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>>
		AreNotOfExactType(
			this IThat<IAsyncEnumerable<PropertyInfo?>?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, true);
		return new AndOrResult<IAsyncEnumerable<PropertyInfo?>, IThat<IAsyncEnumerable<PropertyInfo?>?>>(
			subject.Get().ExpectationBuilder.AddConstraint<TypeFilterOptions, IAsyncEnumerable<PropertyInfo?>>(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new AreNotOfTypeConstraint(it, grammars | ExpectationGrammars.Plural, typeFilterOptions)),
			subject);
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
			_typeFilterOptions.RegisterType(propertyType, true);
			return this;
		}
	}
}
