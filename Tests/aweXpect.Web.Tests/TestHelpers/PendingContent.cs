#if NET8_0_OR_GREATER
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace aweXpect.Web.Tests.TestHelpers;

/// <summary>
///     A content that cancels the <paramref name="evaluation" /> when it is read and then waits until reading it is
///     canceled.
/// </summary>
internal sealed class PendingContent(CancellationTokenSource evaluation) : HttpContent
{
	/// <summary>
	///     The cancellation token that reading the content received.
	/// </summary>
	public CancellationToken ReadToken { get; private set; }

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
		=> SerializeToStreamAsync(stream, context, CancellationToken.None);

	protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context,
		CancellationToken cancellationToken)
	{
		ReadToken = cancellationToken;
		evaluation.Cancel();
		return Task.Delay(Timeout.Infinite, cancellationToken);
	}

	protected override bool TryComputeLength(out long length)
	{
		length = 0;
		return false;
	}
}
#endif
