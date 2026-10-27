using System;
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
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
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
		return new ConstructorsWithNamedParameter<T>(@this, WithParameterFilter, CollectionIndexOptions.Default,
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
	///     Filter for constructors with a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static ConstructorsWithParameter<object?> WithParameterExactly(this Filtered.Constructors @this,
		Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
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
		return new ConstructorsWithNamedParameter<object?>(@this, WithParameterFilter, CollectionIndexOptions.Default,
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
