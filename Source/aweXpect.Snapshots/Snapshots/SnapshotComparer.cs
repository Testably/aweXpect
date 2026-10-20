using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;

namespace aweXpect.Snapshots;

/// <summary>
///     Decides whether a value matches its snapshot, e.g. an image comparer that tolerates small differences of single
///     pixels.
/// </summary>
/// <remarks>
///     A comparer is set for one expectation with <c>.ComparedWith(…)</c>, or for all snapshots with
///     <c>Customize.aweXpect.Snapshots().Comparers</c>, e.g. <c>SnapshotComparer.ForExtension("png", comparer)</c>.
///     Without one, text is compared ignoring the newline style, and binary data byte by byte.<br />
///     It is only called when the snapshot exists, and gets the value after the rules of the expectation were applied.
///     It also compares a value that is used again for the same snapshot in the same test run with the first one, so
///     that two slightly different values of the same test are no conflict. It can be called concurrently, so it must
///     be thread-safe.<br />
///     An exception fails the expectation both ways, with the exception as its cause.<br />
///     An abstract class, not an interface, so that members can be added with a default.
/// </remarks>
public abstract class SnapshotComparer
{
	/// <summary>
	///     Compares the <see cref="SnapshotComparisonContext.Actual" /> value with the
	///     <see cref="SnapshotComparisonContext.Snapshot" />.
	/// </summary>
	/// <remarks>
	///     Return <see cref="SnapshotComparison.Equal" /> to keep the snapshot unchanged, or
	///     <see cref="SnapshotComparison.Different" /> with a description, which the failure shows.
	/// </remarks>
	public abstract ValueTask<SnapshotComparison> CompareAsync(SnapshotComparisonContext context,
		CancellationToken cancellationToken);

	/// <summary>
	///     Uses the <paramref name="comparer" /> for the snapshots whose comparison the <paramref name="predicate" />
	///     accepts, e.g. <c>context =&gt; context.Id.Extension == "png"</c>.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> sees the whole comparison, so it can also decide by the content, e.g. by its
	///     length.
	/// </remarks>
	public static SnapshotComparerRegistration When(Func<SnapshotComparisonContext, bool> predicate,
		SnapshotComparer comparer)
	{
		if (predicate is null)
		{
			throw Tracing.WriteException(
				new ArgumentNullException(nameof(predicate), "The 'predicate' cannot be null."));
		}

		if (comparer is null)
		{
			throw Tracing.WriteException(
				new ArgumentNullException(nameof(comparer), "The 'comparer' cannot be null."));
		}

		return new SnapshotComparerRegistration(predicate, comparer);
	}

	/// <summary>
	///     Uses the <paramref name="comparer" /> for the snapshots with the file <paramref name="extension" />, e.g.
	///     <c>png</c>.
	/// </summary>
	/// <remarks>
	///     The extension is compared ignoring its case, like file systems do on Windows and macOS, so that e.g. a
	///     <c>Cover.PNG</c> file is compared with the comparer for <c>png</c>. It cannot be empty, contain a <c>.</c>,
	///     whitespace, control characters or any of <c>&lt; &gt; : " / \ | ? * % @</c>, or be <c>pending</c>, as no
	///     snapshot could have it.
	/// </remarks>
	public static SnapshotComparerRegistration ForExtension(string extension, SnapshotComparer comparer)
	{
		SnapshotExtension.ThrowIfInvalid(extension);
		return When(context => string.Equals(context.Id.Extension, extension, StringComparison.OrdinalIgnoreCase),
			comparer);
	}
}
