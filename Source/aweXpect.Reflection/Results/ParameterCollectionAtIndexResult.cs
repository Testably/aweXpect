using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Reflection.Results;

/// <summary>
///     Additional constraints on a parameter collection with a parameter at a specific index.
/// </summary>
public class ParameterCollectionAtIndexResult<TType, TThat, TParameter>(
	ExpectationBuilder expectationBuilder,
	IThat<TThat> subject,
	CollectionIndexOptions collectionIndexOptions)
	: AndOrResult<TType, IThat<TThat>>(expectationBuilder, subject)
{
	/// <summary>
	///     …from end.
	/// </summary>
	public ParameterCollectionAtIndexResult<TType, TThat, TParameter> FromEnd()
	{
		if (collectionIndexOptions.Match is CollectionIndexOptions.IMatchFromBeginning match)
		{
			collectionIndexOptions.SetMatch(match.FromEnd());
		}

		return this;
	}
}
