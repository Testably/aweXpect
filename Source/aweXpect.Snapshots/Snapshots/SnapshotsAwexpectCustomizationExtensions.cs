using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Customization;

namespace aweXpect.Snapshots;

/// <summary>
///     Extension methods on <see cref="AwexpectCustomization" /> for snapshots.
/// </summary>
public static class SnapshotsAwexpectCustomizationExtensions
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
			Rules = new CopyingSetter<SnapshotRule>(new CustomizationValue<IReadOnlyList<SnapshotRule>>(
				awexpectCustomization, "aweXpect.Snapshots.Rules", Array.Empty<SnapshotRule>(),
				ThrowIfNullOrContainsNull));
			Reporters = new CopyingSetter<SnapshotReporter>(new CustomizationValue<IReadOnlyList<SnapshotReporter>>(
				awexpectCustomization, "aweXpect.Snapshots.Reporters", Array.Empty<SnapshotReporter>(),
				ThrowIfNullOrContainsNull));
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

		private static void ThrowIfNullOrContainsNull<TItem>(IReadOnlyList<TItem> value)
		{
			if (value is null)
			{
				throw Tracing.WriteException(
					new ArgumentNullException(nameof(value), "The 'value' cannot be null."));
			}

			if (value.Any(item => item is null))
			{
				throw Tracing.WriteException(
					new ArgumentException("The 'value' cannot contain null.", nameof(value)));
			}
		}

		/// <summary>
		///     Stores a copy of the list, as a setting would otherwise change with a mutable list it was set from.
		/// </summary>
		/// <remarks>
		///     A <see langword="null" /> list is passed on unchanged, so that the validation of the
		///     <paramref name="inner" /> value rejects it.
		/// </remarks>
		private sealed class CopyingSetter<TItem>(ICustomizationValueSetter<IReadOnlyList<TItem>> inner)
			: ICustomizationValueSetter<IReadOnlyList<TItem>>
		{
			public IReadOnlyList<TItem> Get() => inner.Get();

			public CustomizationLifetime Set(IReadOnlyList<TItem> value)
				=> inner.Set(value is null ? value! : value.ToArray());
		}
	}
}
