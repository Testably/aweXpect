using System.Collections.Generic;
using System.Threading;

namespace aweXpect.Snapshots.Tests.TestHelpers;

/// <summary>
///     A <see cref="SnapshotComparer" /> that returns the result of <paramref name="compare" /> and records every
///     comparison.
/// </summary>
internal sealed class FakeSnapshotComparer(Func<SnapshotComparisonContext, SnapshotComparison> compare)
	: SnapshotComparer
{
	private readonly List<SnapshotComparisonContext> _contexts = new();

	public IReadOnlyList<SnapshotComparisonContext> Contexts
	{
		get
		{
			lock (_contexts)
			{
				return _contexts.ToArray();
			}
		}
	}

	public static FakeSnapshotComparer Equal() => new(_ => SnapshotComparison.Equal);

	public static FakeSnapshotComparer Different(string description)
		=> new(_ => SnapshotComparison.Different(description));

	public override ValueTask<SnapshotComparison> CompareAsync(SnapshotComparisonContext context,
		CancellationToken cancellationToken)
	{
		lock (_contexts)
		{
			_contexts.Add(context);
		}

		return new ValueTask<SnapshotComparison>(compare(context));
	}
}
