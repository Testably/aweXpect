using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
public class ParameterCollectionResult<TType, TThat, TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	CollectionIndexOptions collectionIndexOptions,
	ParameterFilterOptions parameterFilterOptions)
	: ParameterCollectionResult<TType, TThat, TParameter, ParameterCollectionResult<TType, TThat, TParameter>>(
		expectationBuilder, subject, collectionIndexOptions, parameterFilterOptions);

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
public class ParameterCollectionResult<TType, TThat, TParameter, TSelf>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	CollectionIndexOptions collectionIndexOptions,
	ParameterFilterOptions parameterFilterOptions)
	: AndOrResult<TType, IThat<TThat>, TSelf>(expectationBuilder, subject),
		IOptionsProvider<CollectionIndexOptions>,
		IOptionsProvider<ParameterFilterOptions>
	where TSelf : ParameterCollectionResult<TType, TThat, TParameter, TSelf>
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;
	private readonly IThat<TThat> _subject = subject;

	/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

	/// <inheritdoc cref="IOptionsProvider{ParameterFilterOptions}.Options" />
	ParameterFilterOptions IOptionsProvider<ParameterFilterOptions>.Options => parameterFilterOptions;

	/// <summary>
	///     …at the given <paramref name="index" />.
	/// </summary>
	public ParameterCollectionAtIndexResult<TType, TThat, TParameter> AtIndex(int index)
	{
		collectionIndexOptions.SetMatch(new AtIndexMatch(index));
		return new ParameterCollectionAtIndexResult<TType, TThat, TParameter>(_expectationBuilder, _subject,
			collectionIndexOptions);
	}

	/// <summary>
	///     …without a default value.
	/// </summary>
	public TSelf WithoutDefaultValue()
	{
		parameterFilterOptions.AddPredicate(p => !p.HasDefaultValue, () => "without a default value");
		return (TSelf)this;
	}

	/// <summary>
	///     …with a default value.
	/// </summary>
	public TSelf WithDefaultValue()
	{
		parameterFilterOptions.AddPredicate(p => p.HasDefaultValue, () => "with a default value");
		return (TSelf)this;
	}

	/// <summary>
	///     …with the <paramref name="expected" /> default value.
	/// </summary>
	public TSelf WithDefaultValue<TValue>(TValue expected)
		where TValue : TParameter
	{
		parameterFilterOptions.AddPredicate(p => p.HasDefaultValue && Equals(p.DefaultValue, expected),
			() => $"with default value {Formatter.Format(expected)}");
		return (TSelf)this;
	}
}
