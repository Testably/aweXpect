using System.Collections.Generic;
using aweXpect.Customization;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     An <see cref="ITraceWriter" /> that records the written exceptions.
/// </summary>
internal sealed class RecordingTraceWriter : ITraceWriter
{
	public List<Exception> Exceptions { get; } = new();

	public void WriteMessage(string message)
	{
		// Only the exceptions are checked.
	}

	public void WriteException(Exception exception) => Exceptions.Add(exception);
}
