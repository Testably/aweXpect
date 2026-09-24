#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that the subject is parsable into the type <typeparamref name="TType" />.
/// </summary>
public class IsSpanParsableResult<TType>(
	ExpectationBuilder expectationBuilder,
	IThat<SpanWrapper<char>> subject,
	IFormatProvider? formatProvider)
	: AndOrResult<SpanWrapper<char>, IThat<SpanWrapper<char>>>(expectationBuilder, subject)
	where TType : ISpanParsable<TType>
{
	private readonly ExpectationBuilder _expectationBuilder = expectationBuilder;

	/// <summary>
	///     Gives access to the parsed value.
	/// </summary>
	public IThat<TType> Which
		=> new ThatSubject<TType>(_expectationBuilder
			.ForWhich<SpanWrapper<char>, TType?>(d =>
			{
				if (TType.TryParse(d.AsSpan(), formatProvider, out TType? result))
				{
					return result;
				}

				return default;
			}, " which "));
}
#endif
