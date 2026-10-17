using System;
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
		///     The <see cref="SnapshotStore" /> that keeps the snapshots.
		/// </summary>
		/// <remarks>
		///     Defaults to a store that keeps every snapshot in a file next to its test file.
		/// </remarks>
		public ICustomizationValueSetter<SnapshotStore> Store { get; }
	}
}
