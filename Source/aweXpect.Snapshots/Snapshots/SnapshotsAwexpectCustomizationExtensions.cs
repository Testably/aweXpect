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
			AcceptChanges = new CustomizationValue<bool?>(awexpectCustomization,
				"aweXpect.Snapshots.AcceptChanges", null);
			Maskings = new CopyingSetter<SnapshotMasking>(new CustomizationValue<IReadOnlyList<SnapshotMasking>>(
				awexpectCustomization, "aweXpect.Snapshots.Maskings", Array.Empty<SnapshotMasking>(),
				value =>
				{
					if (value is null)
					{
						throw Tracing.WriteException(
							new ArgumentNullException(nameof(value), "The 'value' cannot be null."));
					}

					if (value.Any(masking => masking is null))
					{
						throw Tracing.WriteException(
							new ArgumentException("The 'value' cannot contain null.", nameof(value)));
					}
				}));
		}

		/// <summary>
		///     Whether a missing or different snapshot is accepted, unless the expectation sets
		///     <c>.AcceptingChanges(…)</c>.
		/// </summary>
		/// <remarks>
		///     Defaults to <see langword="null" />, which accepts when the environment variable
		///     <c>AWEXPECT_ACCEPT_SNAPSHOTS</c> is set (to anything but <c>0</c> or <c>false</c>) and no build server is
		///     detected, so that a forgotten variable cannot change the snapshots on a build server.
		/// </remarks>
		public ICustomizationValueSetter<bool?> AcceptChanges { get; }

		/// <summary>
		///     The maskings that are applied to every snapshot, after the maskings of the expectation.
		/// </summary>
		/// <remarks>
		///     Defaults to none. Setting a value replaces the list, so extend an outer setting with
		///     <c>Set([.. Get(), extra])</c>. The list is copied, so changing it afterwards does not change the setting.
		/// </remarks>
		public ICustomizationValueSetter<IReadOnlyList<SnapshotMasking>> Maskings { get; }

		/// <summary>
		///     The <see cref="SnapshotStore" /> that keeps the snapshots.
		/// </summary>
		/// <remarks>
		///     Defaults to a store that keeps every snapshot in a file next to its test file.
		/// </remarks>
		public ICustomizationValueSetter<SnapshotStore> Store { get; }

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
