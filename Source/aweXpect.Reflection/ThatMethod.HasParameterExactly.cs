using System;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Reflection.Results;

namespace aweXpect.Reflection;

public static partial class ThatMethod
{
	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of exact type <typeparamref name="TParameter" />.
	/// </summary>
	public static ParameterCollectionResult<MethodInfo?, TParameter> HasParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject)
	{
		Type parameterType = typeof(TParameter);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true));
		return new ParameterCollectionResult<MethodInfo?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new HasParameterConstraint(it, grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of exact type <typeparamref name="TParameter" /> with
	///     the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, TParameter> HasParameterExactly<TParameter>(
		this IThat<MethodInfo?> subject, string expected)
	{
		Type parameterType = typeof(TParameter);
		StringEqualityOptions stringEqualityOptions = new();
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected));
		return new NamedParameterCollectionResult<MethodInfo?, TParameter>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new HasParameterConstraint(it, grammars, parameterType, expected,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ParameterCollectionResult<MethodInfo?, object?> HasParameterExactly(
		this IThat<MethodInfo?> subject, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true));
		return new ParameterCollectionResult<MethodInfo?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new HasParameterConstraint(it, grammars, parameterType, null,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions);
	}

	/// <summary>
	///     Verifies that the <see cref="MethodInfo" /> has a parameter of exact type <paramref name="parameterType" /> with
	///     the <paramref name="expected" /> name.
	/// </summary>
	public static NamedParameterCollectionResult<MethodInfo?, object?> HasParameterExactly(
		this IThat<MethodInfo?> subject, Type parameterType, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new();
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions =
			new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true));
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected));
		return new NamedParameterCollectionResult<MethodInfo?, object?>(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new HasParameterConstraint(it, grammars, parameterType, expected,
						collectionIndexOptions,
						parameterFilterOptions,
						true)),
			subject,
			collectionIndexOptions,
			parameterFilterOptions,
			stringEqualityOptions);
	}
}
