using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection;

public static partial class MethodFilters
{
	/// <summary>
	///     Filter for methods with a parameter of type <typeparamref name="T" />.
	/// </summary>
	public static MethodsWithParameter<T> WithParameter<T>(this Filtered.Methods @this)
	{
		Type parameterType = typeof(T);
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.ParameterType == parameterType,
			() => $"of type {Formatter.Format(parameterType)}");
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
	///     Filter for methods with a parameter of type <typeparamref name="T" /> with the <paramref name="expected" />
	///     name.
	/// </summary>
	public static MethodsWithNamedParameter<T> WithParameter<T>(this Filtered.Methods @this, string expected)
	{
		Type parameterType = typeof(T);
		StringEqualityOptions stringEqualityOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.ParameterType == parameterType,
			() => $"of type {Formatter.Format(parameterType)}");
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
	///     Filter for methods with a parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithParameter(this Filtered.Methods @this,
		string expected)
	{
		StringEqualityOptions stringEqualityOptions = new();
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => stringEqualityOptions.AreConsideredEqual(p.Name, expected),
			() => $"with name {stringEqualityOptions.GetExpectation(expected, ExpectationGrammars.None)}");
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


	/// <summary>
	///     Additional filters on methods with a parameter of a specific type.
	/// </summary>
	public class MethodsWithParameter<T>(
		Filtered.Methods inner,
		CollectionIndexOptions collectionIndexOptions,
		ParameterFilterOptions parameterFiltersOptions)
		: Filtered.Methods(inner),
			IOptionsProvider<CollectionIndexOptions>,
			IOptionsProvider<ParameterFilterOptions>
	{
		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

		/// <inheritdoc cref="IOptionsProvider{ParameterFilterOptions}.Options" />
		ParameterFilterOptions IOptionsProvider<ParameterFilterOptions>.Options => parameterFiltersOptions;

		/// <summary>
		///     …at the given <paramref name="index" />.
		/// </summary>
		public MethodsWithParameterAtIndex<T> AtIndex(int index)
		{
			collectionIndexOptions.SetMatch(new AtIndexMatch(index));
			return new MethodsWithParameterAtIndex<T>(this, collectionIndexOptions);
		}

		/// <summary>
		///     …without a default value.
		/// </summary>
		public MethodsWithParameter<T> WithoutDefaultValue()
		{
			parameterFiltersOptions.AddPredicate(p => !p.HasDefaultValue, () => "without a default value");
			return this;
		}

		/// <summary>
		///     …with a default value.
		/// </summary>
		public MethodsWithParameter<T> WithDefaultValue()
		{
			parameterFiltersOptions.AddPredicate(p => p.HasDefaultValue, () => "with a default value");
			return this;
		}

		/// <summary>
		///     …with the <paramref name="expected"/> default value.
		/// </summary>
		public MethodsWithParameter<T> WithDefaultValue<TValue>(TValue expected)
			where TValue : T
		{
			parameterFiltersOptions.AddPredicate(p => p.HasDefaultValue && Equals(p.DefaultValue, expected),
				() => $"with default value {Formatter.Format(expected)}");
			return this;
		}
	}

	/// <summary>
	///     Additional filters on methods with a parameter at a specific index.
	/// </summary>
	public class MethodsWithParameterAtIndex<T>(
		Filtered.Methods inner,
		CollectionIndexOptions collectionIndexOptions)
		: Filtered.Methods(inner),
			IOptionsProvider<CollectionIndexOptions>
	{
		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

		/// <summary>
		///     …from end.
		/// </summary>
		public MethodsWithParameterAtIndex<T> FromEnd()
		{
			if (collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
			{
				collectionIndexOptions.SetMatch(match.FromEnd());
			}

			return this;
		}
	}

	/// <summary>
	///     Additional filters on methods with a named parameter of a specific type.
	/// </summary>
	public class MethodsWithNamedParameter<T>(
		Filtered.Methods inner,
		CollectionIndexOptions collectionIndexOptions,
		ParameterFilterOptions parameterFiltersOptions,
		StringEqualityOptions options)
		: MethodsWithParameter<T>(inner, collectionIndexOptions, parameterFiltersOptions),
			IOptionsProvider<StringEqualityOptions>
	{
		/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
		StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;

		/// <summary>
		///     Ignores casing when comparing the parameter name,
		///     according to the <paramref name="ignoreCase" /> parameter.
		/// </summary>
		public MethodsWithNamedParameter<T> IgnoringCase(bool ignoreCase = true)
		{
			options.IgnoringCase(ignoreCase);
			return this;
		}

		/// <summary>
		///     Uses the provided <paramref name="comparer" /> for comparing parameter names.
		/// </summary>
		public MethodsWithNamedParameter<T> Using(IEqualityComparer<string> comparer)
		{
			options.UsingComparer(comparer);
			return this;
		}

		/// <summary>
		///     Interprets the expected parameter name as a prefix, so that the actual value starts with it.
		/// </summary>
		public MethodsWithNamedParameter<T> AsPrefix()
		{
			options.AsPrefix();
			return this;
		}

		/// <summary>
		///     Interprets the expected parameter name as a <see cref="Regex" /> pattern.
		/// </summary>
		public MethodsWithNamedParameter<T> AsRegex()
		{
			options.AsRegex();
			return this;
		}

		/// <summary>
		///     Interprets the expected parameter name as a suffix, so that the actual value ends with it.
		/// </summary>
		public MethodsWithNamedParameter<T> AsSuffix()
		{
			options.AsSuffix();
			return this;
		}

		/// <summary>
		///     Interprets the expected parameter name as wildcard pattern.<br />
		///     Supports * to match zero or more characters and ? to match exactly one character.
		/// </summary>
		public MethodsWithNamedParameter<T> AsWildcard()
		{
			options.AsWildcard();
			return this;
		}
	}
}
