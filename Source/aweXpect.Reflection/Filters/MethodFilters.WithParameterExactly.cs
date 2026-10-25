using System;
using System.Linq;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods with a parameter of exact type <typeparamref name="T" />.
	/// </summary>
	public static MethodsWithParameter<T> WithParameterExactly<T>(this Filtered.Methods @this)
	{
		Type parameterType = typeof(T);
		OptionsSlot<CollectionIndexOptions> collectionIndexOptions = new(new CollectionIndexOptions());
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new MethodsWithParameter<T>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for methods with a parameter of exact type <typeparamref name="T" /> with the <paramref name="expected" />
	///     name.
	/// </summary>
	public static MethodsWithNamedParameter<T> WithParameterExactly<T>(this Filtered.Methods @this, string expected)
	{
		Type parameterType = typeof(T);
		return new MethodsWithNamedParameter<T>(@this, WithParameterFilter, new OptionsSlot<CollectionIndexOptions>(new CollectionIndexOptions()),
			stringEqualityOptions =>
			{
				ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
					() => $"of exact type {Formatter.Format(parameterType)}");
				parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
					() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
				return parameterFilterOptions;
			},
			new OptionsSlot<StringEqualityOptions>(new StringEqualityOptions(nameof(expected))));
	}

	/// <summary>
	///     Filter for methods with a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static MethodsWithParameter<object?> WithParameterExactly(this Filtered.Methods @this, Type parameterType)
	{
		OptionsSlot<CollectionIndexOptions> collectionIndexOptions = new(new CollectionIndexOptions());
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		return new MethodsWithParameter<object?>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for methods with a parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithParameterExactly(this Filtered.Methods @this, Type parameterType,
		string expected)
	{
		return new MethodsWithNamedParameter<object?>(@this, WithParameterFilter, new OptionsSlot<CollectionIndexOptions>(new CollectionIndexOptions()),
			stringEqualityOptions =>
			{
				ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
					() => $"of exact type {Formatter.Format(parameterType)}");
				parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
					() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
				return parameterFilterOptions;
			},
			new OptionsSlot<StringEqualityOptions>(new StringEqualityOptions(nameof(expected))));
	}
}
