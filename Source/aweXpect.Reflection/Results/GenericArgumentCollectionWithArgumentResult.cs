using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Helpers;
using aweXpect.Reflection.Options;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a collection of generic arguments with a specific type.
/// </summary>
public class GenericArgumentCollectionWithArgumentResult<TType, TThat>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	GenericArgumentsFilterOptions genericArgumentsFilterOptions,
	CollectionIndexOptions collectionIndexOptions)
	: GenericArgumentCollectionResult<TType, TThat>(
			expectationBuilder, subject, genericArgumentsFilterOptions),
		IOptionsProvider<CollectionIndexOptions>
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;
	private readonly GenericArgumentsFilterOptions _genericArgumentsFilterOptions = genericArgumentsFilterOptions;
	private readonly IThat<TThat> _subject = subject;

	/// <inheritdoc cref="IOptionsProvider{CollectionIndexOptions}.Options" />
	CollectionIndexOptions IOptionsProvider<CollectionIndexOptions>.Options => collectionIndexOptions;

	/// <summary>
	///     …at the given <paramref name="index" />.
	/// </summary>
	public GenericArgumentCollectionWithArgumentAtIndexResult<TType, TThat> AtIndex(int index)
	{
		collectionIndexOptions.SetMatch(new AtIndexMatch(index));
		return new GenericArgumentCollectionWithArgumentAtIndexResult<TType, TThat>(
			_expectationBuilder, _subject, _genericArgumentsFilterOptions, collectionIndexOptions);
	}
}
