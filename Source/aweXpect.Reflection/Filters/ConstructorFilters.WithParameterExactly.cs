using System;
using System.Linq;
using System.Reflection;
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
		IAsyncChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					bool? isIndexInRange = collectionIndexOptions.Match switch
					{
						CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
						CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, parameters.Length),
						_ => false,
					};
					return isIndexInRange == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
		return new ConstructorsWithParameter<T>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of exact type <typeparamref name="T" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithParameterExactly<T>(this Filtered.Constructors @this, string expected)
	{
		Type parameterType = typeof(T);
		StringEqualityOptions stringEqualityOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		IAsyncChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					bool? isIndexInRange = collectionIndexOptions.Match switch
					{
						CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
						CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, parameters.Length),
						_ => false,
					};
					return isIndexInRange == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
		return new ConstructorsWithNamedParameter<T>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
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
		IAsyncChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					bool? isIndexInRange = collectionIndexOptions.Match switch
					{
						CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
						CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, parameters.Length),
						_ => false,
					};
					return isIndexInRange == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
		return new ConstructorsWithParameter<object?>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithParameterExactly(this Filtered.Constructors @this,
		Type parameterType, string expected)
	{
		StringEqualityOptions stringEqualityOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		IAsyncChangeableFilter<ConstructorInfo> filter = Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					bool? isIndexInRange = collectionIndexOptions.Match switch
					{
						CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(i),
						CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(i, parameters.Length),
						_ => false,
					};
					return isIndexInRange == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");
		return new ConstructorsWithNamedParameter<object?>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}
}
