using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class ThatField
{
	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is of exactly type <typeparamref name="TField" />.
	/// </summary>
	public static FieldOfTypeResult<FieldInfo?, IThat<FieldInfo?>> IsOfExactType<TField>(
		this IThat<FieldInfo?> subject)
		=> IsOfExactType(subject, typeof(TField));

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is of exactly type <paramref name="fieldType" />.
	/// </summary>
	public static FieldOfTypeResult<FieldInfo?, IThat<FieldInfo?>> IsOfExactType(
		this IThat<FieldInfo?> subject, Type fieldType)
	{
		TypeFilterOptions typeFilterOptions = new();
		typeFilterOptions.RegisterType(fieldType, true);
		return new FieldOfTypeResult<FieldInfo?, IThat<FieldInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	public sealed partial class FieldOfTypeResult<TValue, TResult>
	{
		/// <summary>
		///     Allow an alternative exact type <typeparamref name="TField" />.
		/// </summary>
		public FieldOfTypeResult<TValue, TResult> OrOfExactType<TField>()
			=> OrOfExactType(typeof(TField));

		/// <summary>
		///     Allow an alternative exact type <paramref name="fieldType" />.
		/// </summary>
		public FieldOfTypeResult<TValue, TResult> OrOfExactType(Type fieldType)
		{
			typeFilterOptions.RegisterType(fieldType, true);
			return this;
		}
	}
}
