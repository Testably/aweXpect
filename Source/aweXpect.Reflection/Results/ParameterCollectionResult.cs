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
public class ParameterCollectionResult<TType, TThat, TParameter>
	: ParameterCollectionResult<TType, TThat, TParameter, ParameterCollectionResult<TType, TThat, TParameter>>
{
	internal ParameterCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		CollectionIndexOptions collectionIndexOptions,
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
public class ParameterCollectionResult<TType, TThat, TParameter, TSelf>
	: AndOrResult<TType, IThat<TThat>, TSelf>,
		IOptionsProvider<CollectionIndexOptions>,
		IOptionsProvider<ParameterFilterOptions>
	where TSelf : ParameterCollectionResult<TType, TThat, TParameter, TSelf>
{
	private readonly CollectionIndexOptions _collectionIndexOptions;
	private readonly ParameterFilterOptions _parameterFilterOptions;

	internal ParameterCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		CollectionIndexOptions collectionIndexOptions,
		ParameterFilterOptions parameterFilterOptions)
		: base(expectationBuilder, subject)
	{
		_collectionIndexOptions = collectionIndexOptions;
		_parameterFilterOptions = parameterFilterOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;

	/// <inheritdoc cref="IOptionsProvider{ParameterFilterOptions}.Options" />
	ParameterFilterOptions IOptionsProvider<ParameterFilterOptions>.Options => _parameterFilterOptions;

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
	public TSelf WithDefaultValue<TValue>(TValue expected)
		where TValue : TParameter
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
