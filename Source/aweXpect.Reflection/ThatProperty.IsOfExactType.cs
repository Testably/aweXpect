using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class ThatProperty
{
	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is of exactly type <typeparamref name="TProperty" />.
	/// </summary>
	public static PropertyOfTypeResult<PropertyInfo?, IThat<PropertyInfo?>> IsOfExactType<TProperty>(
		this IThat<PropertyInfo?> subject)
		=> IsOfExactType(subject, typeof(TProperty));

	/// <summary>
	///     Verifies that the <see cref="PropertyInfo" /> is of exactly type <paramref name="propertyType" />.
	/// </summary>
	public static PropertyOfTypeResult<PropertyInfo?, IThat<PropertyInfo?>> IsOfExactType(
		this IThat<PropertyInfo?> subject, Type propertyType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(propertyType, true);
		return new PropertyOfTypeResult<PropertyInfo?, IThat<PropertyInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
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
			typeFilterOptions.RegisterType(propertyType, true);
			return this;
		}
	}
}
