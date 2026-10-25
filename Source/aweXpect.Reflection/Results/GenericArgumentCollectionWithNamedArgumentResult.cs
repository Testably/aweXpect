using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a collection of generic arguments with a parameter with an expected name, which in
///     addition allows specifying how the name is compared via <see cref="StringEqualityOptionsExtensions" />.
/// </summary>
public sealed class GenericArgumentCollectionWithNamedArgumentResult<TType, TValue>
	: GenericArgumentCollectionWithArgumentResult<TType, TValue, GenericArgumentCollectionWithNamedArgumentResult<TType, TValue>>,
		IOptionsProvider<StringEqualityOptions>,
		IStringMatchTypeOptions
{
	private readonly OptionsSlot<StringEqualityOptions> _options;

	internal GenericArgumentCollectionWithNamedArgumentResult(
		ExpectationBuilder expectationBuilder,
		IThat<TValue> subject,
		GenericArgumentsFilterOptions genericArgumentsFilterOptions,
		OptionsSlot<CollectionIndexOptions> collectionIndexOptions,
		OptionsSlot<StringEqualityOptions> options)
		: base(expectationBuilder, subject, genericArgumentsFilterOptions, collectionIndexOptions)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{StringEqualityOptions}.Options" />
	OptionsSlot<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => _options;
}
