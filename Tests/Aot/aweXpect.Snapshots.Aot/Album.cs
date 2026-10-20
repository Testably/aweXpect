using System.Collections.Generic;

namespace aweXpect.Snapshots.Aot;

public sealed record Album(string Title, decimal Price, List<string> Tracks);
