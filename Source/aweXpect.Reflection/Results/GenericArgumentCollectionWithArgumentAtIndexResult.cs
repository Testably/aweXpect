using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a collection of generic arguments at a given index.
/// </summary>
public class GenericArgumentCollectionWithArgumentAtIndexResult<TType, TThat>
	: GenericArgumentCollectionResult<TType, TThat, GenericArgumentCollectionWithArgumentAtIndexResult<TType, TThat>>,
		IOptionsProvider<CollectionIndexOptions>
{
	private readonly CollectionIndexOptions _collectionIndexOptions;

	internal GenericArgumentCollectionWithArgumentAtIndexResult(
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

	/// <summary>
	///     …from end.
	/// </summary>
	public GenericArgumentCollectionWithArgumentAtIndexResult<TType, TThat> FromEnd()
	{
		if (_collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
		{
			_collectionIndexOptions.SetMatch(match.FromEnd());
		}

		return this;
	}
}
