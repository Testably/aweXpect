using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a parameter at a specific index.
/// </summary>
public class ParameterCollectionAtIndexResult<TType, TThat, TParameter>
	: AndOrResult<TType, IThat<TThat>, ParameterCollectionAtIndexResult<TType, TThat, TParameter>>
{
	private readonly CollectionIndexOptions _collectionIndexOptions;

	internal ParameterCollectionAtIndexResult(
		ExpectationBuilder expectationBuilder,
		IThat<TThat> subject,
		CollectionIndexOptions collectionIndexOptions)
		: base(expectationBuilder, subject)
	{
		_collectionIndexOptions = collectionIndexOptions;
	}

	/// <summary>
	///     …from end.
	/// </summary>
	public ParameterCollectionAtIndexResult<TType, TThat, TParameter> FromEnd()
	{
		if (_collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
		{
			_collectionIndexOptions.SetMatch(match.FromEnd());
		}

		return this;
	}
}
