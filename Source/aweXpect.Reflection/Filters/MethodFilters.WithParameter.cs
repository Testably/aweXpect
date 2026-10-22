using System;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

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
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
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
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
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
	///     Filter for methods with a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static MethodsWithParameter<object?> WithParameter(this Filtered.Methods @this, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
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
		return new MethodsWithParameter<object?>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for methods with a parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithParameter(this Filtered.Methods @this, Type parameterType,
		string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
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
		return new MethodsWithNamedParameter<object?>(@this.Which(filter), collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}

	/// <summary>
	///     Filter for methods with a parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithParameter(this Filtered.Methods @this,
		string expected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(expected));
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
	public class MethodsWithParameter<T>
		: Filtered.Methods,
			IOptionsProvider<CollectionIndexOptions>,
			IOptionsProvider<ParameterFilterOptions>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly ParameterFilterOptions _parameterFilterOptions;

		internal MethodsWithParameter(
			Filtered.Methods inner,
			CollectionIndexOptions collectionIndexOptions,
			ParameterFilterOptions parameterFiltersOptions)
			: base(inner)
		{
			_collectionIndexOptions = collectionIndexOptions;
			_parameterFilterOptions = parameterFiltersOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;

		/// <inheritdoc cref="IOptionsProvider{ParameterFilterOptions}.Options" />
		ParameterFilterOptions IOptionsProvider<ParameterFilterOptions>.Options => _parameterFilterOptions;

		/// <summary>
		///     …at the given <paramref name="index" />.
		/// </summary>
		public MethodsWithParameterAtIndex<T> AtIndex(int index)
		{
			_collectionIndexOptions.SetMatch(new AtIndexMatch(index));
			return new MethodsWithParameterAtIndex<T>(this, _collectionIndexOptions);
		}

		/// <summary>
		///     …without a default value.
		/// </summary>
		public MethodsWithParameter<T> WithoutDefaultValue()
		{
			_parameterFilterOptions.AddPredicate(p => !p.HasDefaultValue, () => "without a default value");
			return this;
		}

		/// <summary>
		///     …with a default value.
		/// </summary>
		public MethodsWithParameter<T> WithDefaultValue()
		{
			_parameterFilterOptions.AddPredicate(p => p.HasDefaultValue, () => "with a default value");
			return this;
		}

		/// <summary>
		///     …with the <paramref name="expected" /> default value.
		/// </summary>
		public MethodsWithParameter<T> WithDefaultValue<TValue>(TValue expected)
			where TValue : T
		{
			_parameterFilterOptions.AddPredicate(p => p.HasDefaultValue && Equals(p.DefaultValue, expected),
				() => $"with default value {Formatter.Format(expected)}");
			return this;
		}

		/// <summary>
		///     …with the <see langword="ref" /> modifier.
		/// </summary>
		public MethodsWithParameter<T> WithRefModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsRefParameter(), () => "with ref modifier");
			return this;
		}

		/// <summary>
		///     …with the <see langword="out" /> modifier.
		/// </summary>
		public MethodsWithParameter<T> WithOutModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsOutParameter(), () => "with out modifier");
			return this;
		}

		/// <summary>
		///     …with the <see langword="in" /> modifier.
		/// </summary>
		public MethodsWithParameter<T> WithInModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsInParameter(), () => "with in modifier");
			return this;
		}

		/// <summary>
		///     …with the optional modifier.
		/// </summary>
		public MethodsWithParameter<T> WithOptionalModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsOptionalParameter(), () => "with optional modifier");
			return this;
		}

		/// <summary>
		///     …with the <see langword="params" /> modifier.
		/// </summary>
		public MethodsWithParameter<T> WithParamsModifier()
		{
			_parameterFilterOptions.AddPredicate(p => p.IsParamsParameter(), () => "with params modifier");
			return this;
		}
	}

	/// <summary>
	///     Additional filters on methods with a parameter at a specific index.
	/// </summary>
	public class MethodsWithParameterAtIndex<T>
		: Filtered.Methods,
			IOptionsProvider<CollectionIndexOptions>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;

		internal MethodsWithParameterAtIndex(
			Filtered.Methods inner,
			CollectionIndexOptions collectionIndexOptions)
			: base(inner)
		{
			_collectionIndexOptions = collectionIndexOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;

		/// <summary>
		///     …from end.
		/// </summary>
		public MethodsWithParameterAtIndex<T> FromEnd()
		{
			if (_collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
			{
				_collectionIndexOptions.SetMatch(match.FromEnd());
			}

			return this;
		}
	}

	/// <summary>
	///     Additional filters on methods with a named parameter of a specific type.
	/// </summary>
	public class MethodsWithNamedParameter<T>
		: MethodsWithParameter<T>,
			IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions
	{
		private readonly StringEqualityOptions _options;

		internal MethodsWithNamedParameter(
			Filtered.Methods inner,
			CollectionIndexOptions collectionIndexOptions,
			ParameterFilterOptions parameterFiltersOptions,
			StringEqualityOptions options)
			: base(inner, collectionIndexOptions, parameterFiltersOptions)
		{
			_options = options;
		}

		/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
		StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
	}
}
