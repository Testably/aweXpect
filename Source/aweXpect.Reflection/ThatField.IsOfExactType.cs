using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatField
{
	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is of exactly type <typeparamref name="TField" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<FieldInfo> IsOfExactType<TField>(
		this IThat<FieldInfo?> subject)
		=> IsOfExactType(subject, typeof(TField));

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is of exactly type <paramref name="fieldType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static OfTypeResult<FieldInfo> IsOfExactType(
		this IThat<FieldInfo?> subject, Type fieldType)
	{
		OptionsHolder<TypeFilterOptions> typeFilterOptions = new(TypeFilterOptions.Default.WithType(fieldType, true));
		return new OfTypeResult<FieldInfo>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions)),
			subject,
			typeFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is not of exactly type <typeparamref name="TField" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<FieldInfo, IThat<FieldInfo?>> IsNotOfExactType<TField>(
		this IThat<FieldInfo?> subject)
		=> IsNotOfExactType(subject, typeof(TField));

	/// <summary>
	///     Verifies that the <see cref="FieldInfo" /> is not of exactly type <paramref name="fieldType" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<FieldInfo, IThat<FieldInfo?>> IsNotOfExactType(
		this IThat<FieldInfo?> subject, Type fieldType)
	{
		OptionsHolder<TypeFilterOptions> typeFilterOptions = new(TypeFilterOptions.Default.WithType(fieldType, true));
		return new AndOrResult<FieldInfo, IThat<FieldInfo?>>(
			subject.Get().ExpectationBuilder.AddConstraint(typeFilterOptions,
				static (typeFilterOptions, it, grammars)
				=> new IsOfTypeConstraint(it, grammars, typeFilterOptions).Invert()),
			subject);
	}
}
