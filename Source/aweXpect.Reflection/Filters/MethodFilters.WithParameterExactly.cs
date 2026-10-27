using System;
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
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
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
		return new MethodsWithNamedParameter<T>(@this, WithParameterFilter, CollectionIndexOptions.Default,
			stringEqualityOptions =>
			{
				ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
					() => $"of exact type {Formatter.Format(parameterType)}")
					.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)),
						() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
				return parameterFilterOptions;
			},
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));
	}

	/// <summary>
	///     Filter for methods with a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static MethodsWithParameter<object?> WithParameterExactly(this Filtered.Methods @this, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
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
		return new MethodsWithNamedParameter<object?>(@this, WithParameterFilter, CollectionIndexOptions.Default,
			stringEqualityOptions =>
			{
				ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
					() => $"of exact type {Formatter.Format(parameterType)}")
					.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)),
						() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
				return parameterFilterOptions;
			},
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
