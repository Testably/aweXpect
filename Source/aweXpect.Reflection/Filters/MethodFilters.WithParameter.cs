using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
		return new MethodsWithParameter<T>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions);
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
		return new MethodsWithNamedParameter<T>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}

	/// <summary>
	///     Filter for methods with a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static MethodsWithParameter<object?> WithParameter(this Filtered.Methods @this, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		return new MethodsWithParameter<object?>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions);
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
		return new MethodsWithNamedParameter<object?>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
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
		return new MethodsWithNamedParameter<object?>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions, stringEqualityOptions);
	}


	/// <summary>
	///     Filter for methods without a parameter of type <typeparamref name="T" />.
	/// </summary>
	public static MethodsWithParameter<T> WithoutParameter<T>(this Filtered.Methods @this)
		=> WithoutParameterOfType<T>(@this, typeof(T));

	/// <summary>
	///     Filter for methods without a parameter of type <typeparamref name="T" /> with the <paramref name="unexpected" />
	///     name.
	/// </summary>
	public static MethodsWithNamedParameter<T> WithoutParameter<T>(this Filtered.Methods @this, string unexpected)
		=> WithoutNamedParameter<T>(@this, typeof(T), unexpected);

	/// <summary>
	///     Filter for methods without a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static MethodsWithParameter<object?> WithoutParameter(this Filtered.Methods @this, Type parameterType)
		=> WithoutParameterOfType<object?>(@this, parameterType);

	/// <summary>
	///     Filter for methods without a parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="unexpected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithoutParameter(this Filtered.Methods @this, Type parameterType,
		string unexpected)
		=> WithoutNamedParameter<object?>(@this, parameterType, unexpected);

	/// <summary>
	///     Filter for methods without a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithoutParameter(this Filtered.Methods @this,
		string unexpected)
		=> WithoutNamedParameter<object?>(@this, null, unexpected);

	private static MethodsWithParameter<T> WithoutParameterOfType<T>(Filtered.Methods @this, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		return new MethodsWithParameter<T>(@this, WithoutParameterFilter, collectionIndexOptions,
			parameterFilterOptions);
	}

	private static MethodsWithNamedParameter<T> WithoutNamedParameter<T>(Filtered.Methods @this, Type? parameterType,
		string unexpected)
	{
		StringEqualityOptions stringEqualityOptions = new(nameof(unexpected));
		CollectionIndexOptions collectionIndexOptions = new();
		ParameterFilterOptions parameterFilterOptions;
		if (parameterType is null)
		{
			parameterFilterOptions = new(p => stringEqualityOptions.AreConsideredEqual(p.Name, unexpected),
				() => $"with name {stringEqualityOptions.GetExpectation(unexpected, ExpectationGrammars.None)}");
		}
		else
		{
			parameterFilterOptions = new(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
				() => $"of type {Formatter.Format(parameterType)}");
			parameterFilterOptions.AddPredicate(p => stringEqualityOptions.AreConsideredEqual(p.Name, unexpected),
				() => $"name {stringEqualityOptions.GetExpectation(unexpected, ExpectationGrammars.None)}");
		}

		return new MethodsWithNamedParameter<T>(@this, WithoutParameterFilter, collectionIndexOptions,
			parameterFilterOptions, stringEqualityOptions);
	}

	private static MethodsWithParameter<object?> WithParameterOfKind(Filtered.Methods @this,
		Func<ParameterInfo, bool> predicate, string kind)
		=> new(@this,
			(collectionIndexOptions, parameterFilterOptions)
				=> WithParameterOfKindFilter(kind, collectionIndexOptions, parameterFilterOptions),
			new CollectionIndexOptions(), new ParameterFilterOptions(predicate));

	private static IFilter<MethodInfo> WithParameterFilter(
		CollectionIndexOptions collectionIndexOptions, ParameterFilterOptions parameterFilterOptions)
		=> Filter.Suffix<MethodInfo>(
			methodInfo =>
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.MatchesIndex(i, parameters.Length) == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");

	private static IFilter<MethodInfo> WithParameterOfKindFilter(string kind,
		CollectionIndexOptions collectionIndexOptions, ParameterFilterOptions parameterFilterOptions)
		=> Filter.Suffix<MethodInfo>(
			methodInfo => methodInfo.HasMatchingParameter(collectionIndexOptions, parameterFilterOptions),
			() =>
			{
				string description = parameterFilterOptions.GetDescription();
				return $"with {kind} parameter{(description.Length == 0 ? "" : " ")}{description}{collectionIndexOptions.Match.GetDescription()} ";
			});

	private static IFilter<MethodInfo> WithoutParameterFilter(
		CollectionIndexOptions collectionIndexOptions, ParameterFilterOptions parameterFilterOptions)
		=> Filter.Suffix<MethodInfo>(
			async methodInfo => !await methodInfo.HasMatchingParameter(collectionIndexOptions, parameterFilterOptions),
			()
				=> $"without parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");

	/// <summary>
	///     Additional filters on methods with a parameter of a specific type.
	/// </summary>
	public class MethodsWithParameter<T>
		: Filtered.Methods,
			IOptionsProvider<ParameterFilterOptions>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly Func<CollectionIndexOptions, ParameterFilterOptions, IFilter<MethodInfo>> _createFilter;
		private readonly ParameterFilterOptions _parameterFilterOptions;
		private readonly Filtered.Methods _source;

		internal MethodsWithParameter(
			Filtered.Methods source,
			Func<CollectionIndexOptions, ParameterFilterOptions, IFilter<MethodInfo>> createFilter,
			CollectionIndexOptions collectionIndexOptions,
			ParameterFilterOptions parameterFiltersOptions)
			: base(source.Which(createFilter(collectionIndexOptions, parameterFiltersOptions)))
		{
			_source = source;
			_createFilter = createFilter;
			_collectionIndexOptions = collectionIndexOptions;
			_parameterFilterOptions = parameterFiltersOptions;
		}

		/// <inheritdoc cref="IOptionsProvider{ParameterFilterOptions}.Options" />
		ParameterFilterOptions IOptionsProvider<ParameterFilterOptions>.Options => _parameterFilterOptions;

		/// <summary>
		///     …at the given zero-based <paramref name="index" />.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is negative.</exception>
		/// <exception cref="InvalidOperationException">An index is already specified.</exception>
		public MethodsWithParameter<T> AtIndex(int index)
			=> WithIndex(o => o.AtIndex(index));

		/// <summary>
		///     …at the given zero-based <paramref name="index" />, counted from the end of the parameters.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The <paramref name="index" /> is negative.</exception>
		/// <exception cref="InvalidOperationException">An index is already specified.</exception>
		public MethodsWithParameter<T> AtIndexFromEnd(int index)
			=> WithIndex(o => o.AtIndexFromEnd(index));

		/// <summary>
		///     …without a default value.
		/// </summary>
		public MethodsWithParameter<T> WithoutDefaultValue()
			=> WithPredicate(p => !p.HasDefaultValue, () => "without a default value");

		/// <summary>
		///     …with a default value.
		/// </summary>
		public MethodsWithParameter<T> WithDefaultValue()
			=> WithPredicate(p => p.HasDefaultValue, () => "with a default value");

		/// <summary>
		///     …with the <paramref name="expected" /> default value.
		/// </summary>
		public MethodsWithParameter<T> WithDefaultValue<TValue>(TValue expected)
			where TValue : T
			=> WithPredicate(p => p.HasDefaultValue && Equals(p.DefaultValue, expected),
				() => $"with default value {Formatter.Format(expected)}");

		/// <summary>
		///     …with the <see langword="ref" /> modifier.
		/// </summary>
		public MethodsWithParameter<T> WithRefModifier()
			=> WithPredicate(p => p.IsRefParameter(), () => "with ref modifier");

		/// <summary>
		///     …with the <see langword="out" /> modifier.
		/// </summary>
		public MethodsWithParameter<T> WithOutModifier()
			=> WithPredicate(p => p.IsOutParameter(), () => "with out modifier");

		/// <summary>
		///     …with the <see langword="in" /> modifier.
		/// </summary>
		public MethodsWithParameter<T> WithInModifier()
			=> WithPredicate(p => p.IsInParameter(), () => "with in modifier");

		/// <summary>
		///     …with the optional modifier.
		/// </summary>
		public MethodsWithParameter<T> WithOptionalModifier()
			=> WithPredicate(p => p.IsOptionalParameter(), () => "with optional modifier");

		/// <summary>
		///     …with the <see langword="params" /> modifier.
		/// </summary>
		public MethodsWithParameter<T> WithParamsModifier()
			=> WithPredicate(p => p.IsParamsParameter(), () => "with params modifier");

		/// <summary>
		///     …matching the <paramref name="predicate" />.
		/// </summary>
		public MethodsWithParameter<T> Matching(Func<ParameterInfo, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			=> new(_source, _createFilter, _collectionIndexOptions,
				_parameterFilterOptions.WithMatching(predicate, doNotPopulateThisValue));

		private MethodsWithParameter<T> WithPredicate(Func<ParameterInfo, bool> predicate, Func<string> description)
			=> new(_source, _createFilter, _collectionIndexOptions,
				_parameterFilterOptions.WithPredicate(predicate, description));

		private MethodsWithParameter<T> WithIndex(Action<CollectionIndexOptions> setIndex)
			=> new(_source, _createFilter, FilterIndexOptions.With(_collectionIndexOptions, setIndex),
				_parameterFilterOptions);
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
			Filtered.Methods source,
			Func<CollectionIndexOptions, ParameterFilterOptions, IFilter<MethodInfo>> createFilter,
			CollectionIndexOptions collectionIndexOptions,
			ParameterFilterOptions parameterFiltersOptions,
			StringEqualityOptions options)
			: base(source, createFilter, collectionIndexOptions, parameterFiltersOptions)
		{
			_options = options;
		}

		/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
		StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
	}
}
