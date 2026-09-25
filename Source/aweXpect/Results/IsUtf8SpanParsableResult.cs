#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that the subject is parsable into the type <typeparamref name="TType" />.
/// </summary>
public class IsUtf8SpanParsableResult<TType>(
	ExpectationBuilder expectationBuilder,
	IThat<SpanWrapper<byte>> subject,
	IFormatProvider? formatProvider)
	: AndOrResult<SpanWrapper<byte>, IThat<SpanWrapper<byte>>>(expectationBuilder, subject)
	where TType : IUtf8SpanParsable<TType>
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     Gives access to the parsed value.
	/// </summary>
	public IThat<TType> Which
		=> new ThatSubject<TType>(_expectationBuilder
			.ForWhich<SpanWrapper<byte>, TType?>(d =>
			{
				if (TType.TryParse(d.AsSpan(), formatProvider, out TType? result))
				{
					return result;
				}

				return default;
			}, " that "));
}
#endif
