using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
public sealed class ParameterCollectionResult<TType, TValue, TParameter>
	: ParameterCollectionResult<TType, TValue, TParameter, ParameterCollectionResult<TType, TValue, TParameter>>
{
	internal ParameterCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsSlot<CollectionIndexOptions> collectionIndexOptions,
		ParameterFilterOptions parameterFilterOptions)
		: base(expectationBuilder, subject, collectionIndexOptions, parameterFilterOptions)
	{
	}
}

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
/// <remarks>
///     The index of the parameter is specified via <see cref="CollectionIndexOptionsExtensions" />.
/// </remarks>
public class ParameterCollectionResult<TType, TValue, TParameter, TSelf>
	: AndOrResult<TType, IThat<TValue>, TSelf>,
		IOptionsProvider<CollectionIndexOptions>,
		IOptionsProvider<ParameterFilterOptions>
	where TSelf : ParameterCollectionResult<TType, TValue, TParameter, TSelf>
{
	private readonly OptionsSlot<CollectionIndexOptions> _collectionIndexOptions;
	private readonly ParameterFilterOptions _parameterFilterOptions;

	internal ParameterCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsSlot<CollectionIndexOptions> collectionIndexOptions,
		ParameterFilterOptions parameterFilterOptions)
		: base(expectationBuilder, subject)
	{
		_collectionIndexOptions = collectionIndexOptions;
		_parameterFilterOptions = parameterFilterOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
	OptionsSlot<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;

	/// <inheritdoc cref="IOptionsProvider{ParameterFilterOptions}.Options" />
	/// <remarks>
	///     A new slot around the options that the filter keeps itself.
	/// </remarks>
	OptionsSlot<ParameterFilterOptions> IOptionsProvider<ParameterFilterOptions>.Options
		=> new(_parameterFilterOptions);

	/// <summary>
	///     …without a default value.
	/// </summary>
	public TSelf WithoutDefaultValue()
	{
		_parameterFilterOptions.AddPredicate(p => !p.HasDefaultValue, () => "without a default value");
		return (TSelf)this;
	}

	/// <summary>
	///     …with a default value.
	/// </summary>
	public TSelf WithDefaultValue()
	{
		_parameterFilterOptions.AddPredicate(p => p.HasDefaultValue, () => "with a default value");
		return (TSelf)this;
	}

	/// <summary>
	///     …with the <paramref name="expected" /> default value.
	/// </summary>
	public TSelf WithDefaultValue<TDefault>(TDefault expected)
		where TDefault : TParameter
	{
		_parameterFilterOptions.AddPredicate(p => p.HasDefaultValue && Equals(p.DefaultValue, expected),
			() => $"with default value {Formatter.Format(expected)}");
		return (TSelf)this;
	}

	/// <summary>
	///     …matching the <paramref name="predicate" />.
	/// </summary>
	public TSelf Matching(Func<ParameterInfo, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
	{
		_parameterFilterOptions.AddMatching(predicate, doNotPopulateThisValue);
		return (TSelf)this;
	}
}
