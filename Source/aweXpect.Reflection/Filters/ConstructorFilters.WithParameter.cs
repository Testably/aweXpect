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

public static partial class ConstructorFilters
{
	/// <summary>
	///     Filter for constructors with a parameter of type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithParameter<T>(this Filtered.Constructors @this)
	{
		Type parameterType = typeof(T);
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		return new ConstructorsWithParameter<T>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of type <typeparamref name="T" /> with the <paramref name="expected" />
	///     name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithParameter<T>(this Filtered.Constructors @this, string expected)
	{
		Type parameterType = typeof(T);
		return new ConstructorsWithNamedParameter<T>(@this, WithParameterFilter, CollectionIndexOptions.Default,
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
	///     Filter for constructors with a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ConstructorsWithParameter<object?> WithParameter(this Filtered.Constructors @this, Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		return new ConstructorsWithParameter<object?>(@this, WithParameterFilter, collectionIndexOptions, parameterFilterOptions);
	}

	/// <summary>
	///     Filter for constructors with a parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithParameter(this Filtered.Constructors @this,
		Type parameterType, string expected)
	{
		return new ConstructorsWithNamedParameter<object?>(@this, WithParameterFilter, CollectionIndexOptions.Default,
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
	///     Filter for constructors with a parameter with the <paramref name="expected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithParameter(this Filtered.Constructors @this,
		string expected)
		=> new(@this, WithParameterFilter, CollectionIndexOptions.Default,
			stringEqualityOptions => new ParameterFilterOptions(
				p => stringEqualityOptions.Value.AreConsideredEqual(p.Name, expected, nameof(expected)),
				() => $"with name {stringEqualityOptions.Value.GetExpectation(expected, ExpectationGrammars.None)}"),
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default));


	/// <summary>
	///     Filter for constructors without a parameter of type <typeparamref name="T" />.
	/// </summary>
	public static ConstructorsWithParameter<T> WithoutParameter<T>(this Filtered.Constructors @this)
		=> WithoutParameterOfType<T>(@this, typeof(T));

	/// <summary>
	///     Filter for constructors without a parameter of type <typeparamref name="T" /> with the
	///     <paramref name="unexpected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<T> WithoutParameter<T>(this Filtered.Constructors @this,
		string unexpected)
		=> WithoutNamedParameter<T>(@this, typeof(T), unexpected);

	/// <summary>
	///     Filter for constructors without a parameter of type <paramref name="parameterType" />.
	/// </summary>
	public static ConstructorsWithParameter<object?> WithoutParameter(this Filtered.Constructors @this,
		Type parameterType)
		=> WithoutParameterOfType<object?>(@this, parameterType);

	/// <summary>
	///     Filter for constructors without a parameter of type <paramref name="parameterType" /> with the
	///     <paramref name="unexpected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithoutParameter(this Filtered.Constructors @this,
		Type parameterType, string unexpected)
		=> WithoutNamedParameter<object?>(@this, parameterType, unexpected);

	/// <summary>
	///     Filter for constructors without a parameter with the <paramref name="unexpected" /> name.
	/// </summary>
	public static ConstructorsWithNamedParameter<object?> WithoutParameter(this Filtered.Constructors @this,
		string unexpected)
		=> WithoutNamedParameter<object?>(@this, null, unexpected);

	private static ConstructorsWithParameter<T> WithoutParameterOfType<T>(Filtered.Constructors @this,
		Type parameterType)
	{
		CollectionIndexOptions collectionIndexOptions = CollectionIndexOptions.Default;
		ParameterFilterOptions parameterFilterOptions = new ParameterFilterOptions(p => p.GetUnderlyingType().IsOrInheritsFrom(parameterType),
			() => $"of type {Formatter.Format(parameterType)}");
		return new ConstructorsWithParameter<T>(@this, WithoutParameterFilter, collectionIndexOptions,
			parameterFilterOptions);
	}

	private static ConstructorsWithNamedParameter<T> WithoutNamedParameter<T>(Filtered.Constructors @this,
		Type? parameterType, string unexpected)
	{
		return new ConstructorsWithNamedParameter<T>(@this, WithoutParameterFilter, CollectionIndexOptions.Default,
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

	private static ConstructorsWithParameter<object?> WithParameterOfKind(Filtered.Constructors @this,
		Func<ParameterInfo, bool> predicate, string kind)
		=> new(@this,
			(collectionIndexOptions, parameterFilterOptions)
				=> WithParameterOfKindFilter(kind, collectionIndexOptions, parameterFilterOptions),
			CollectionIndexOptions.Default, new ParameterFilterOptions(predicate));

	private static IFilter<ConstructorInfo> WithParameterFilter(
		CollectionIndexOptions collectionIndexOptions, ParameterFilterOptions parameterFilterOptions)
		=> Filter.Suffix<ConstructorInfo>(
			constructorInfo =>
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				return parameters.AnyAsync(async (p, i) =>
				{
					return collectionIndexOptions.MatchesIndex(i, parameters.Length) == true &&
					       await parameterFilterOptions.Matches(p);
				});
			},
			()
				=> $"with parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");

	private static IFilter<ConstructorInfo> WithParameterOfKindFilter(string kind,
		CollectionIndexOptions collectionIndexOptions, ParameterFilterOptions parameterFilterOptions)
		=> Filter.Suffix<ConstructorInfo>(
			constructorInfo => constructorInfo.HasMatchingParameter(collectionIndexOptions, parameterFilterOptions),
			() =>
			{
				string description = parameterFilterOptions.GetDescription();
				return $"with {kind} parameter{(description.Length == 0 ? "" : " ")}{description}{collectionIndexOptions.Match.GetDescription()} ";
			});

	private static IFilter<ConstructorInfo> WithoutParameterFilter(
		CollectionIndexOptions collectionIndexOptions, ParameterFilterOptions parameterFilterOptions)
		=> Filter.Suffix<ConstructorInfo>(
			async constructorInfo
				=> !await constructorInfo.HasMatchingParameter(collectionIndexOptions, parameterFilterOptions),
			()
				=> $"without parameter {parameterFilterOptions.GetDescription()}{collectionIndexOptions.Match.GetDescription()} ");

	/// <summary>
	///     Additional filters on constructors with a parameter of a specific type.
	/// </summary>
	/// <remarks>
	///     Every method returns a new filter, so that a stored filter stays unchanged. Extensions add their own
	///     predicates with <see cref="Matching(Func{ParameterInfo, bool}, string)" />.
	/// </remarks>
	public class ConstructorsWithParameter<T> : Filtered.Constructors,
		IOptionsProvider<CollectionIndexOptions>,
		ICopyOnOption<CollectionIndexOptions, ConstructorsWithParameter<T>>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly Func<CollectionIndexOptions, ParameterFilterOptions, IFilter<ConstructorInfo>> _createFilter;
		private readonly ParameterFilterOptions _parameterFilterOptions;
		private readonly Filtered.Constructors _source;

		internal ConstructorsWithParameter(
			Filtered.Constructors source,
			Func<CollectionIndexOptions, ParameterFilterOptions, IFilter<ConstructorInfo>> createFilter,
			CollectionIndexOptions collectionIndexOptions,
			ParameterFilterOptions parameterFilterOptions)
			: base(source.Which(createFilter(collectionIndexOptions, parameterFilterOptions)))
		{
			_source = source;
			_createFilter = createFilter;
			_collectionIndexOptions = collectionIndexOptions;
			_parameterFilterOptions = parameterFilterOptions;
		}

		/// <summary>
		///     …without a default value.
		/// </summary>
		public ConstructorsWithParameter<T> WithoutDefaultValue()
			=> WithPredicate(p => !p.HasDefaultValue, () => "without a default value");

		/// <summary>
		///     …with a default value.
		/// </summary>
		public ConstructorsWithParameter<T> WithDefaultValue()
			=> WithPredicate(p => p.HasDefaultValue, () => "with a default value");

		/// <summary>
		///     …with the <paramref name="expected" /> default value.
		/// </summary>
		public ConstructorsWithParameter<T> WithDefaultValue<TValue>(TValue expected)
			where TValue : T
			=> WithPredicate(p => p.HasDefaultValue && Equals(p.DefaultValue, expected),
				() => $"with default value {Formatter.Format(expected)}");

		/// <summary>
		///     …with the <see langword="ref" /> modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithRefModifier()
			=> WithPredicate(p => p.IsRefParameter(), () => "with ref modifier");

		/// <summary>
		///     …with the <see langword="out" /> modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithOutModifier()
			=> WithPredicate(p => p.IsOutParameter(), () => "with out modifier");

		/// <summary>
		///     …with the <see langword="in" /> modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithInModifier()
			=> WithPredicate(p => p.IsInParameter(), () => "with in modifier");

		/// <summary>
		///     …with the optional modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithOptionalModifier()
			=> WithPredicate(p => p.IsOptionalParameter(), () => "with optional modifier");

		/// <summary>
		///     …with the <see langword="params" /> modifier.
		/// </summary>
		public ConstructorsWithParameter<T> WithParamsModifier()
			=> WithPredicate(p => p.IsParamsParameter(), () => "with params modifier");

		/// <summary>
		///     …matching the <paramref name="predicate" />.
		/// </summary>
		public ConstructorsWithParameter<T> Matching(Func<ParameterInfo, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
			=> new(_source, _createFilter, _collectionIndexOptions,
				_parameterFilterOptions.WithMatching(predicate, doNotPopulateThisValue));

		private ConstructorsWithParameter<T> WithPredicate(Func<ParameterInfo, bool> predicate,
			Func<string> description)
			=> new(_source, _createFilter, _collectionIndexOptions,
				_parameterFilterOptions.WithPredicate(predicate, description));

		/// <summary>
		///     Returns a copy with the modifier <paramref name="predicate" /> and <paramref name="description" />.
		/// </summary>
		internal ConstructorsWithParameter<T> WithModifier(Func<ParameterInfo, bool> predicate, string description)
			=> new(_source, _createFilter, _collectionIndexOptions,
				_parameterFilterOptions.WithModifier(predicate, () => description));

		/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
		OptionsHolder<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options
			=> new(_collectionIndexOptions);

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		ConstructorsWithParameter<T> ICopyOnOption<CollectionIndexOptions, ConstructorsWithParameter<T>>.WithOptions(
			CollectionIndexOptions options)
			=> new(_source, _createFilter, options, _parameterFilterOptions);
	}

	/// <summary>
	///     Additional filters on constructors with a named parameter of a specific type.
	/// </summary>
	public class ConstructorsWithNamedParameter<T>
		: ConstructorsWithParameter<T>,
			IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, ConstructorsWithNamedParameter<T>>,
			ICopyOnOption<CollectionIndexOptions, ConstructorsWithNamedParameter<T>>
	{
		private readonly CollectionIndexOptions _collectionIndexOptions;
		private readonly Func<CollectionIndexOptions, ParameterFilterOptions, IFilter<ConstructorInfo>> _createFilter;
		private readonly Func<IReadOnlyOptionsHolder<StringEqualityOptions>, ParameterFilterOptions> _createParameterFilterOptions;
		private readonly OptionsHolder<StringEqualityOptions> _options;
		private readonly Filtered.Constructors _source;

		internal ConstructorsWithNamedParameter(
			Filtered.Constructors source,
			Func<CollectionIndexOptions, ParameterFilterOptions, IFilter<ConstructorInfo>> createFilter,
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
		ConstructorsWithNamedParameter<T> ICopyOnOption<CollectionIndexOptions, ConstructorsWithNamedParameter<T>>.WithOptions(
			CollectionIndexOptions options)
			=> new(_source, _createFilter, options, _createParameterFilterOptions, new OptionsHolder<StringEqualityOptions>(_options.Value));

		/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
		ConstructorsWithNamedParameter<T> ICopyOnOption<StringEqualityOptions, ConstructorsWithNamedParameter<T>>.
			WithOptions(StringEqualityOptions options)
			=> new(_source, _createFilter, _collectionIndexOptions, _createParameterFilterOptions, new OptionsHolder<StringEqualityOptions>(options));

		/// <summary>
		///     Returns a copy with the modifier <paramref name="predicate" /> and <paramref name="description" />.
		/// </summary>
		/// <remarks>
		///     Hides the copy of the base filter, so that the modifier is kept when a string option
		///     rebuilds the parameter filter options.
		/// </remarks>
		internal new ConstructorsWithNamedParameter<T> WithModifier(Func<ParameterInfo, bool> predicate, string description)
			=> new(_source, _createFilter, _collectionIndexOptions,
				options => _createParameterFilterOptions(options).WithModifier(predicate, () => description),
				_options);
	}
}
