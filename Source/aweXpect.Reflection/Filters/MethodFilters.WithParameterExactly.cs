using System;
using System.Linq;
using System.Reflection;
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
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		IAsyncChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo =>
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
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
		return new MethodsWithParameter<T>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for methods with a parameter of exact type <typeparamref name="T" /> with the <paramref name="expected" />
	///     name.
	/// </summary>
	public static MethodsWithNamedParameter<T> WithParameterExactly<T>(this Filtered.Methods @this, string expected)
	{
		Type parameterType = typeof(T);
		StringEqualityOptions stringEqualityOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		IAsyncChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo =>
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
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
		return new MethodsWithNamedParameter<T>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}

	/// <summary>
	///     Filter for methods with a parameter of exact type <paramref name="parameterType" />.
	/// </summary>
	public static MethodsWithParameter<object?> WithParameterExactly(this Filtered.Methods @this, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		IAsyncChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo =>
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
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
		return new MethodsWithParameter<object?>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for methods with a parameter of exact type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithParameterExactly(this Filtered.Methods @this, Type parameterType,
		string expected)
	{
		StringEqualityOptions stringEqualityOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType, true),
			() => $"of exact type {Formatter.Format(parameterType)}");
		parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
		CollectionIndexOptions collectionIndexOptions = new();
		IAsyncChangeableFilter<MethodInfo> filter = Filter.Suffix<MethodInfo>(
			methodInfo =>
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
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
		return new MethodsWithNamedParameter<object?>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}
}
