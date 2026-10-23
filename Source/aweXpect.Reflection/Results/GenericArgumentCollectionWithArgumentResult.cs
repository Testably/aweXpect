using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a collection of generic arguments with a specific type.
/// </summary>
public class GenericArgumentCollectionWithArgumentResult<TType, TThat>
	: GenericArgumentCollectionWithArgumentResult<TType, TThat, GenericArgumentCollectionWithArgumentResult<TType, TThat>>
{
	internal GenericArgumentCollectionWithArgumentResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		GenericArgumentsFilterOptions genericArgumentsFilterOptions,
		CollectionIndexOptions collectionIndexOptions)
		: base(expectationBuilder, subject, genericArgumentsFilterOptions, collectionIndexOptions)
	{
	}
}

/// <summary>
///     Additional constraints on a collection of generic arguments with a specific type.
/// </summary>
/// <remarks>
///     The index of the generic argument is specified via <see cref="CollectionIndexOptionsExtensions" />.
/// </remarks>
public class GenericArgumentCollectionWithArgumentResult<TType, TThat, TSelf>
	: GenericArgumentCollectionResult<TType, TThat, TSelf>,
		IOptionsProvider<CollectionIndexOptions>
	where TSelf : GenericArgumentCollectionWithArgumentResult<TType, TThat, TSelf>
{
	private readonly CollectionIndexOptions _collectionIndexOptions;

	internal GenericArgumentCollectionWithArgumentResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		GenericArgumentsFilterOptions genericArgumentsFilterOptions,
		CollectionIndexOptions collectionIndexOptions)
		: base(expectationBuilder, subject, genericArgumentsFilterOptions)
	{
		_collectionIndexOptions = collectionIndexOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;
}
