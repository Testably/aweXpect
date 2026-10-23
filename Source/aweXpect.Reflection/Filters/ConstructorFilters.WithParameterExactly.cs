using System;
using System.Linq;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with a parameter of exact type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithParameterExactly<T>(this Filtered.Constructors @this)
	{
		Type parameterType = typeof(T);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new ConstructorsWithParameter<T>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of exact type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithParameterExactly<T>(this Filtered.Constructors @this, string expected)
	{
		Type parameterType = typeof(T);
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		return new ConstructorsWithNamedParameter<T>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ConstructorsWithParameter<object?> WithParameterExactly(this Filtered.Constructors @this,
		Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new ConstructorsWithParameter<object?>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithParameterExactly(this Filtered.Constructors @this,
		Type parameterType, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		return new ConstructorsWithNamedParameter<object?>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}
}
