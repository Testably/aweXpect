#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that the subject is parsable into the type <typeparamref name="TType" />.
/// </summary>
public class IsUtf8SpanParsableResult<TType> : AndOrResult<SpanWrapper<byte>, IThat<SpanWrapper<byte>>>
	where TType : IUtf8SpanParsable<TType>
{
	private readonly IFormatProvider? _formatProvider;

	internal IsUtf8SpanParsableResult(
		ExpectationBuilder expectationBuilder,
		IThat<SpanWrapper<byte>> subject,
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
			.ForWhich<SpanWrapper<byte>, TType?>(d =>
			{
				if (TType.TryParse(d.AsSpan(), _formatProvider, out TType? result))
				{
					return result;
				}

				return default;
			}, " that ", "it"));
}
#endif
