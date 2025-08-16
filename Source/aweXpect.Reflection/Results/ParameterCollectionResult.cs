using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a parameter of a specific type.
/// </summary>
public class ParameterCollectionResult<TThat, TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	CollectionIndexOptions collectionIndexOptions,
	ParameterFilterOptions parameterFilterOptions)
	: AndOrResult<TThat, IThat<TThat>>(expectationBuilder, subject),
		IOptionsProvider<CollectionIndexOptions>,
		IOptionsProvider<ParameterFilterOptions>
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
	public ParameterCollectionAtIndexResult<TThat, TParameter> AtIndex(int index)
	{
		collectionIndexOptions.SetMatch(new HasParameterAtIndexMatch(index));
		return new ParameterCollectionAtIndexResult<TThat, TParameter>(_expectationBuilder, _subject,
			collectionIndexOptions);
	}

	/// <summary>
	///     …without a default value.
	/// </summary>
	public ParameterCollectionResult<TThat, TParameter> WithoutDefaultValue()
	{
		parameterFilterOptions.AddPredicate(p => !p.HasDefaultValue, () => "without a default value");
		return this;
	}

	/// <summary>
	///     …with a default value.
	/// </summary>
	public ParameterCollectionResult<TThat, TParameter> WithDefaultValue()
	{
		parameterFilterOptions.AddPredicate(p => p.HasDefaultValue, () => "with a default value");
		return this;
	}

	/// <summary>
	///     …with the <paramref name="expected" /> default value.
	/// </summary>
	public ParameterCollectionResult<TThat, TParameter> WithDefaultValue<TValue>(TValue expected)
		where TValue : TParameter
	{
		parameterFilterOptions.AddPredicate(p => p.HasDefaultValue && Equals(p.DefaultValue, expected),
			() => $"with default value {Formatter.Format(expected)}");
		return this;
	}
}
