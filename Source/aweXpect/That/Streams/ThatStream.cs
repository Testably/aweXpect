using System;
using System.IO;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="Stream" /> values.
/// </summary>
public static partial class ThatStream
{
	/// <summary>
	///     A disposed, non-seekable or broken <see cref="Stream" /> cannot answer what its length or position is, which
	///     is a failed expectation about the stream and not a defect in the expectation itself.
	/// </summary>
	private static bool IsUnreadableStreamProperty(Exception exception)
		=> exception is IOException or NotSupportedException or ObjectDisposedException;
}
