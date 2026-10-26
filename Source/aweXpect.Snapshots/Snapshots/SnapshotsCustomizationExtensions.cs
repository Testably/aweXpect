using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Snapshots;

/// <summary>
///     Extension methods on <see cref="AwexpectCustomization" /> for snapshots.
/// </summary>
public static class SnapshotsCustomizationExtensions
{
	/// <summary>
	///     Customize the snapshot settings.
	/// </summary>
	public static SnapshotsCustomization Snapshots(this AwexpectCustomization awexpectCustomization)
		=> new(awexpectCustomization);

	/// <summary>
	///     Customize the snapshot settings.
	/// </summary>
	public class SnapshotsCustomization
	{
		internal SnapshotsCustomization(IAwexpectCustomization awexpectCustomization)
		{
			Store = new CustomizationValue<SnapshotStore>(awexpectCustomization,
				"aweXpect.Snapshots.Store", DiskSnapshotStore.Instance,
				value =>
				{
					if (value is null)
					{
						throw Tracing.WriteException(
							new ArgumentNullException(nameof(value), "The 'value' cannot be null."));
					}
				});
			Acceptance = new CustomizationValue<SnapshotAcceptance?>(awexpectCustomization,
				"aweXpect.Snapshots.Acceptance", null);
			AttachPendingFiles = new CustomizationValue<bool>(awexpectCustomization,
				"aweXpect.Snapshots.AttachPendingFiles", false);
			NamedAfterTestArguments = new CustomizationValue<bool>(awexpectCustomization,
				"aweXpect.Snapshots.NamedAfterTestArguments", false);
			Comparers = new CustomizationListValue<SnapshotComparerRegistration>(awexpectCustomization,
				"aweXpect.Snapshots.Comparers", Array.Empty<SnapshotComparerRegistration>(), ThrowIfContainsNull);
			Rules = new CustomizationListValue<SnapshotRule>(awexpectCustomization,
				"aweXpect.Snapshots.Rules", Array.Empty<SnapshotRule>(), ThrowIfContainsNull);
			Reporters = new CustomizationListValue<SnapshotReporter>(awexpectCustomization,
				"aweXpect.Snapshots.Reporters", Array.Empty<SnapshotReporter>(), ThrowIfContainsNull);
		}

		/// <summary>
		///     Which snapshots are accepted, see <see cref="SnapshotAcceptance" />, unless the expectation sets
		///     <c>.Accepting(…)</c>.
		/// </summary>
		/// <remarks>
		///     Defaults to <see langword="null" />, which leaves it to the environment variable
		///     <c>AWEXPECT_ACCEPT_SNAPSHOTS</c> (<c>none</c>, <c>new</c> or <c>all</c>), which is ignored when a build server
		///     is detected, so that a forgotten variable cannot change the snapshots on a build server.
		/// </remarks>
		public ICustomizationValueSetter<SnapshotAcceptance?> Acceptance { get; }

		/// <summary>
		///     Whether the pending file of a recorded mismatch is attached to the result of the test, so that it can be
		///     reviewed after a run on a build server.
		/// </summary>
		/// <remarks>
		///     Defaults to <see langword="false" />. It only applies when the test framework supports attachments, see
		///     <see cref="aweXpect.Core.Adapters.TestFrameworkRegistry.TryAddAttachment" />, and to a store that records
		///     the mismatch in a file, like the default one.
		/// </remarks>
		public ICustomizationValueSetter<bool> AttachPendingFiles { get; }

		/// <summary>
		///     The comparers for the snapshots that their condition selects, e.g.
		///     <c>SnapshotComparer.ForExtension("png", imageComparer)</c>, unless the expectation sets
		///     <c>.ComparedWith(…)</c>.
		/// </summary>
		/// <remarks>
		///     Defaults to none, which compares text ignoring the newline style and binary data byte by byte. The first
		///     comparer in the list whose condition matches is used. Setting a value replaces the list, so add a comparer
		///     with <c>Set([.. Get(), comparer])</c>. The list is copied, so changing it afterwards does not change the
		///     setting.
		/// </remarks>
		public ICustomizationValueSetter<IReadOnlyList<SnapshotComparerRegistration>> Comparers { get; }

		/// <summary>
		///     Names each snapshot of a parameterized test after the arguments of the running test case, e.g.
		///     <c>&lt;Class&gt;.&lt;Method&gt;.Abbey Road_1969.awexpect-snapshot.txt</c>, so that its cases are kept apart
		///     without <c>.NamedAfter(…)</c>.
		/// </summary>
		/// <remarks>
		///     Defaults to <see langword="false" />, as it changes the snapshot of each parameterized test whose cases
		///     shared one so far.<br />
		///     It only applies when the test framework exposes the running test, see
		///     <see cref="aweXpect.Core.Adapters.TestFrameworkRegistry.TryGetCurrentTest" />, which MSTest 3 and xunit v2 do
		///     not, and MSTest 4 only with the MSBuild property <c>AweXpectExperimentalMSTest</c>, and to a test with
		///     arguments, so the snapshot of a test without parameters keeps its name. A name set by <c>.Named(…)</c> is
		///     appended to it, e.g. <c>Abbey Road_1969_cover</c>, and <c>.NamedAfter(…)</c> replaces it.<br />
		///     The arguments are written like those of <c>.NamedAfter(…)</c>: in the invariant culture and separated by
		///     <c>_</c>, <see langword="null" /> as <c>null</c>, an empty string as <c>""</c>, a <see cref="double" /> or
		///     <see cref="float" /> with the fewest digits that round-trip, and a collection, tuple, record or other object
		///     like in a snapshot, e.g. <c>[1, 2, 3]</c>; a <see cref="System.Threading.CancellationToken" /> is skipped. The
		///     arguments of a parameterized test class come first, but only values, so that an injected fixture is skipped,
		///     while every argument of the test method is written. Like every name, a name longer than 64 characters in the
		///     file name, where each escaped character takes three, is shortened and ends with <c>~</c> and a hash of the
		///     whole name, so that it stays unique.<br />
		///     An argument that is written differently in each run, e.g. a new <see cref="System.Guid" />, gives the snapshot
		///     another name in each run, so name such a test with <c>.Named(…)</c> or <c>.NamedAfter(…)</c> instead.
		/// </remarks>
		public ICustomizationValueSetter<bool> NamedAfterTestArguments { get; }

		/// <summary>
		///     The reporters that are told about a recorded mismatch and a match, in this order, e.g. to open a diff tool.
		/// </summary>
		/// <remarks>
		///     Defaults to none. Setting a value replaces the list, so add a reporter with <c>Set([.. Get(), reporter])</c>.
		///     The list is copied, so changing it afterwards does not change the setting. The reporters are never called on
		///     a build server.
		/// </remarks>
		public ICustomizationValueSetter<IReadOnlyList<SnapshotReporter>> Reporters { get; }

		/// <summary>
		///     The rules that are applied to every snapshot, after the rules of the expectation, e.g.
		///     <c>SnapshotRule.For.Guids().Mask()</c>.
		/// </summary>
		/// <remarks>
		///     Defaults to none. Setting a value replaces the list, so extend an outer setting with
		///     <c>Set([.. Get(), extra])</c>. The list is copied, so changing it afterwards does not change the setting.
		///     Member and value rules are ignored for a snapshot of a <see cref="string" />, which is compared as raw text,
		///     and line rules for a snapshot of an object. <c>SnapshotRule.For.Paths().Keep()</c> keeps the paths of the
		///     environment in every snapshot, which are masked by default.
		/// </remarks>
		public ICustomizationValueSetter<IReadOnlyList<SnapshotRule>> Rules { get; }

		/// <summary>
		///     The <see cref="SnapshotStore" /> that keeps the snapshots.
		/// </summary>
		/// <remarks>
		///     Defaults to a store that keeps every snapshot in a file next to its test file.
		/// </remarks>
		public ICustomizationValueSetter<SnapshotStore> Store { get; }

		private static void ThrowIfContainsNull<TItem>(IReadOnlyList<TItem> value)
		{
			if (value.Any(item => item is null))
			{
				throw Tracing.WriteException(
					new ArgumentException("The 'value' cannot contain null.", nameof(value)));
			}
		}
	}
}
