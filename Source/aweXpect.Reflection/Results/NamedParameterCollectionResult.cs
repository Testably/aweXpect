using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a named parameter of a specific type, which in addition
///     allows specifying how the parameter name is compared via <see cref="StringEqualityOptionsExtensions" />.
/// </summary>
public class NamedParameterCollectionResult<TType, TThat, TParameter>
	: ParameterCollectionResult<TType, TThat, TParameter, NamedParameterCollectionResult<TType, TThat, TParameter>>,
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions
{
	private readonly StringEqualityOptions _options;

	internal NamedParameterCollectionResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		CollectionIndexOptions collectionIndexOptions,
		ParameterFilterOptions parameterFilterOptions,
		StringEqualityOptions options)
		: base(expectationBuilder, subject, collectionIndexOptions, parameterFilterOptions)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
}
