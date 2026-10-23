using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatProperty
{
	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyOfTypeResult<PropertyInfo, IThat<PropertyInfo?>> IsOfExactType<TProperty>(
		this IThat<PropertyInfo?> subject)
		=> IsOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is of exactly type <paramref name="propertyType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static PropertyOfTypeResult<PropertyInfo, IThat<PropertyInfo?>> IsOfExactType(
		this IThat<PropertyInfo?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, true);
		return new PropertyOfTypeResult<PropertyInfo, IThat<PropertyInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is not of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<PropertyInfo, IThat<PropertyInfo?>> IsNotOfExactType<TProperty>(
		this IThat<PropertyInfo?> subject)
		=> IsNotOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is not of exactly type <paramref name="propertyType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<PropertyInfo, IThat<PropertyInfo?>> IsNotOfExactType(
		this IThat<PropertyInfo?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, true);
		return new AndOrResult<PropertyInfo, IThat<PropertyInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions).Invert()),
			subject);
	}

	public sealed partial class PropertyOfTypeResult<TValue, TResult>
	{
		/// <summary>
		///     Allow an alternative exact type <typeparamref name="TProperty" />.
		/// </summary>
		public PropertyOfTypeResult<TValue, TResult> OrOfExactType<TProperty>()
			=> OrOfExactType(typeof(TProperty));

		/// <summary>
		///     Allow an alternative exact type <paramref name="propertyType" />.
		/// </summary>
		public PropertyOfTypeResult<TValue, TResult> OrOfExactType(Type propertyType)
		{
			_typeFilterOptions.RegisterType(propertyType, true);
			return this;
		}
	}
}
