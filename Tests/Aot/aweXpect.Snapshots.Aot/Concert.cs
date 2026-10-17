using System;

namespace aweXpect.Snapshots.Aot;

internal sealed class Concert
{
	public DateTime Date { get; set; }
	public TimeSpan Duration { get; set; }
	public string Venue { get; set; } = "";
}
