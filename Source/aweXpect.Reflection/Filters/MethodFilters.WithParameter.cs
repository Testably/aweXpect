using System;
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
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
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
		return new MethodsWithNamedParameter<T>(@this, WithParameterFilter, CollectionIndexOptions.Default,
			stringEqualityOptions =>
			{
				ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
					() => $"of type {Formatter.Format(parameterType)}")
					.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)),
						() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
				return parameterFilterOptions;
			},
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));
	}

	/// <summary>
	///     Filter for methods with a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static MethodsWithParameter<object?> WithParameter(this Filtered.Methods @this, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
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
		return new MethodsWithNamedParameter<object?>(@this, WithParameterFilter, CollectionIndexOptions.Default,
			stringEqualityOptions =>
			{
				ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
					() => $"of type {Formatter.Format(parameterType)}")
					.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)),
						() => $"name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}");
				return parameterFilterOptions;
			},
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));
	}

	/// <summary>
	///     Filter for methods with a parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static MethodsWithNamedParameter<object?> WithParameter(this Filtered.Methods @this,
		string expected)
		=> new(@this, WithParameterFilter, CollectionIndexOptions.Default,
			stringEqualityOptions => new ParameterFilterOptions(
				p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)),
				() => $"with name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}"),
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));


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
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		return new MethodsWithParameter<T>(@this, WithoutParameterFilter, collectionIndexOptions,
			parameterFilterOptions);
	}

	private static MethodsWithNamedParameter<T> WithoutNamedParameter<T>(Filtered.Methods @this, Type? parameterType,
		string unexpected)
	{
		return new MethodsWithNamedParameter<T>(@this, WithoutParameterFilter, CollectionIndexOptions.Default,
			stringEqualityOptions =>
			{
				if (parameterType is null)
				{
					return new ParameterFilterOptions(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, unexpected, nameof(unexpected)),
						() => $"with name {stringEqualityOptions.Value.GetExpectation(unexpected, ExpectationGrammars.None)}");
				}

				ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
					() => $"of type {Formatter.Format(parameterType)}")
					.WithPredicate(p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, unexpected, nameof(unexpected)),
						() => $"name {stringEqualityOptions.Value.GetExpectation(unexpected, ExpectationGrammars.None)}");
				return parameterFilterOptions;
			},
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));
	}

	private static MethodsWithParameter<object?> WithParameterOfKind(Filtered.Methods @this,
		Func<ParameterInfo, bool> predicate, string kind)
		=> new(@this,
			(collectionIndexOptions, parameterFilterOptions)
				=> WithParameterOfKindFilter(kind, collectionIndexOptions, parameterFilterOptions),
			CollectionIndexOptions.Default, new ParameterFilterOptions(predicate));

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
	/// <remarks>
	///     Every method returns a new filter, so that a stored filter stays unchanged. Extensions add their own
	///     predicates with <see cref="Matching(Func{ParameterInfo, bool}, string)" />.
	/// </remarks>
	public class MethodsWithParameter<T> : Filtered.Methods,
		IOptionsProvider<CollectionIndexOptions>,
		ICopyOnOption<CollectionIndexOptions, MethodsWithParameter<T>>
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

		/// <summary>
		///     Returns a copy with the modifier <paramref name="predicate" /> and <paramref name="description" />.
		/// </summary>
		internal MethodsWithParameter<T> WithModifier(Func<ParameterInfo, bool> predicate, string description)
			=> new(_source, _createFilter, _collectionIndexOptions,
				_parameterFilterOptions.WithModifier(predicate, () => description));

		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		OptionsHolder<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options
			=> new(_collectionIndexOptions);

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		MethodsWithParameter<T> ICopyOnOption<CollectionIndexOptions, MethodsWithParameter<T>>.WithOptions(
			CollectionIndexOptions options)
			=> new(_source, _createFilter, options, _parameterFilterOptions);
	}

	/// <summary>
	///     Additional filters on methods with a named parameter of a specific type.
	/// </summary>
	public class MethodsWithNamedParameter<T>
		: MethodsWithParameter<T>,
			IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, MethodsWithNamedParameter<T>>,
			ICopyOnOption<CollectionIndexOptions, MethodsWithNamedParameter<T>>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly Func<CollectionIndexOptions, ParameterFilterOptions, IFilter<MethodInfo>> _createFilter;
		private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, ParameterFilterOptions> _createParameterFilterOptions;
		private readonly OptionsHolder<StringEqualityOptions> _options;
		private readonly Filtered.Methods _source;

		internal MethodsWithNamedParameter(
			Filtered.Methods source,
			Func<CollectionIndexOptions, ParameterFilterOptions, IFilter<MethodInfo>> createFilter,
			CollectionIndexOptions collectionIndexOptions,
			Func<IReadOnlyOptionsHolder<StringEqualityOptions>, ParameterFilterOptions> createParameterFilterOptions,
			OptionsHolder<StringEqualityOptions> options)
			: base(source, createFilter, collectionIndexOptions, createParameterFilterOptions(options))
		{
			_source = source;
			_createFilter = createFilter;
			_collectionIndexOptions = collectionIndexOptions;
			_createParameterFilterOptions = createParameterFilterOptions;
			_options = options;
		}

		/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
		OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => _options;

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		MethodsWithNamedParameter<T> ICopyOnOption<CollectionIndexOptions, MethodsWithNamedParameter<T>>.WithOptions(
			CollectionIndexOptions options)
			=> new(_source, _createFilter, options, _createParameterFilterOptions, new OptionsHolder<StringEqualityOptions>(_options.Value));

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		MethodsWithNamedParameter<T> ICopyOnOption<StringEqualityOptions, MethodsWithNamedParameter<T>>.WithOptions(
			StringEqualityOptions options)
			=> new(_source, _createFilter, _collectionIndexOptions, _createParameterFilterOptions, new OptionsHolder<StringEqualityOptions>(options));

		/// <summary>
		///     Returns a copy with the modifier <paramref name="predicate" /> and <paramref name="description" />.
		/// </summary>
		/// <remarks>
		///     Hides the copy of the base filter, so that the modifier is kept when a string option
		///     rebuilds the parameter filter options.
		/// </remarks>
		internal new MethodsWithNamedParameter<T> WithModifier(Func<ParameterInfo, bool> predicate, string description)
			=> new(_source, _createFilter, _collectionIndexOptions,
				options => _createParameterFilterOptions(options).WithModifier(predicate, () => description),
				_options);
	}
}
