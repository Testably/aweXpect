#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that the subject is parsable into the type <typeparamref name="TType" />.
/// </summary>
public class IsParsableResult<TType>(
	ExpectationBuilder expectationBuilder,
	IThat<string?> subject,
	IFormatProvider? formatProvider)
	: AndOrResult<string?, IThat<string?>>(expectationBuilder, subject)
	where TType : IParsable<TType>
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     Gives access to the parsed value.
	/// </summary>
	public IThat<TType> Which
		=> new ThatSubject<TType>(_expectationBuilder
			.ForWhich<string, TType?>(d =>
			{
				if (TType.TryParse(d, formatProvider, out TType? result))
				{
					return result;
				}

				return default;
			}, " that "));
}
#endif
