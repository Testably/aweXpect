using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a named parameter of a specific type, which in addition
///     allows specifying how the parameter name is compared via <see cref="StringEqualityOptionsExtensions" />.
/// </summary>
public class NamedParameterCollectionResult<TType, TThat, TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	CollectionIndexOptions collectionIndexOptions,
	ParameterFilterOptions parameterFilterOptions,
	StringEqualityOptions options)
	: ParameterCollectionResult<TType, TThat, TParameter, NamedParameterCollectionResult<TType, TThat, TParameter>>(
			expectationBuilder, subject, collectionIndexOptions, parameterFilterOptions),
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions
{
	/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;
}
