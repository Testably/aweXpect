using System;

namespace aweXpect.Snapshots.Aot;

internal sealed class RecordPlayerException(string message, int side, Exception innerException)
	: Exception(message, innerException)
{
	public int Side { get; } = side;
}
