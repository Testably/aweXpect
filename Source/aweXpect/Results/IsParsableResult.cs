#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that the subject is parsable into the type <typeparamref name="TType" />.
/// </summary>
public class IsParsableResult<TType> : AndOrResult<string, IThat<string?>>
	where TType : IParsable<TType>
{
	private readonly IFormatProvider? _formatProvider;

	internal IsParsableResult(
		ExpectationBuilder expectationBuilder,
		IThat<string?> subject,
		IFormatProvider? formatProvider)
		: base(expectationBuilder, subject)
	{
		_formatProvider = formatProvider;
	}

	/// <summary>
	///     Gives access to the parsed value.
	/// </summary>
	public IThatSubject<TType> Which
		=> new ThatSubject<TType>(ExpectationBuilder
			.ForWhich<string, TType?>(d =>
			{
				if (TType.TryParse(d, _formatProvider, out TType? result))
				{
					return result;
				}

				return default;
			}, " that ", "it"));
}
#endif
