#if NET8_0_OR_GREATER
using System.Net.Http;
using aweXpect.Core;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the status code of the <see cref="HttpResponseMessage" /> subject…
	/// </summary>
	public static StatusCodeResult HasStatusCode(this IThat<HttpResponseMessage?> source)
		=> new(source, a => a.StatusCode);
}
#endif
