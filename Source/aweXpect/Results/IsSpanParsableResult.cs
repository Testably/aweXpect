#if NET8_0_OR_GREATER
using System;
using aweXpect.Core;

namespace aweXpect.Results;

/// <summary>
///     The result for verifying that the subject is parsable into the type <typeparamref name="TType" />.
/// </summary>
public class IsSpanParsableResult<TType> : AndOrResult<SpanWrapper<char>, IThat<SpanWrapper<char>>>
	where TType : ISpanParsable<TType>
{
	private readonly IFormatProvider? _formatProvider;

	internal IsSpanParsableResult(
		ExpectationBuilder expectationBuilder,
		IThat<SpanWrapper<char>> subject,
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
			.ForWhich<SpanWrapper<char>, TType?>(d =>
			{
				if (TType.TryParse(d.AsSpan(), _formatProvider, out TType? result))
				{
					return result;
				}

				return default;
			}, " that ", "it"));
}
#endif
