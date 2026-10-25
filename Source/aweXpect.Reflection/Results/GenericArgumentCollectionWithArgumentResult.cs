using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a collection of generic arguments with a specific type.
/// </summary>
public sealed class GenericArgumentCollectionWithArgumentResult<TType, TValue>
	: GenericArgumentCollectionWithArgumentResult<TType, TValue, GenericArgumentCollectionWithArgumentResult<TType, TValue>>
{
	internal GenericArgumentCollectionWithArgumentResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		GenericArgumentsFilterOptions genericArgumentsFilterOptions,
		OptionsSlot<CollectionIndexOptions> collectionIndexOptions)
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
public class GenericArgumentCollectionWithArgumentResult<TType, TValue, TSelf>
	: GenericArgumentCollectionResult<TType, TValue, TSelf>,
		IOptionsProvider<CollectionIndexOptions>
	where TSelf : GenericArgumentCollectionWithArgumentResult<TType, TValue, TSelf>
{
	private readonly OptionsSlot<CollectionIndexOptions> _collectionIndexOptions;

	internal GenericArgumentCollectionWithArgumentResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		GenericArgumentsFilterOptions genericArgumentsFilterOptions,
		OptionsSlot<CollectionIndexOptions> collectionIndexOptions)
		: base(expectationBuilder, subject, genericArgumentsFilterOptions)
	{
		_collectionIndexOptions = collectionIndexOptions;
	}

	/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
	OptionsSlot<CollectionIndexOptions> IOptionsProvider<CollectionIndexOptions>.Options => _collectionIndexOptions;
}
