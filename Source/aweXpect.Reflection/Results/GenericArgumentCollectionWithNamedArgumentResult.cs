using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a collection of generic arguments with a parameter with an expected name, which in
///     addition allows specifying how the name is compared via <see cref="StringEqualityOptionsExtensions" />.
/// </summary>
public class GenericArgumentCollectionWithNamedArgumentResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	GenericArgumentsFilterOptions genericArgumentsFilterOptions,
	CollectionIndexOptions collectionIndexOptions,
	StringEqualityOptions options)
	: GenericArgumentCollectionWithArgumentResult<TType, TThat, GenericArgumentCollectionWithNamedArgumentResult<TType, TThat>>(
			expectationBuilder, subject, genericArgumentsFilterOptions, collectionIndexOptions),
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions
{
	/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => options;
}
