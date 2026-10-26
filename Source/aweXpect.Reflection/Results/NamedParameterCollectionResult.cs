using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a named parameter of a specific type, which in addition
///     allows specifying how the parameter name is compared via <see cref="StringEqualityOptionsExtensions" />.
/// </summary>
public sealed class NamedParameterCollectionResult<TType, TValue, TParameter>
	: ParameterCollectionResult<TType, TValue, TParameter, NamedParameterCollectionResult<TType, TValue, TParameter>>,
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions
{
	private readonly OptionsHolder<StringEqualityOptions> _options;

	internal NamedParameterCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		OptionsHolder<CollectionIndexOptions> collectionIndexOptions,
		ParameterFilterOptions parameterFilterOptions,
		OptionsHolder<StringEqualityOptions> options)
		: base(expectationBuilder, subject, collectionIndexOptions, parameterFilterOptions)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
	OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => _options;
}
