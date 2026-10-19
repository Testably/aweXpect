using System.Collections.Generic;

namespace aweXpect.Snapshots.Aot;

internal enum ShipmentStatus
{
	Open,
	Sent,
}

internal sealed class Shipment
{
	public int Id { get; set; }
	public List<Parcel> Parcels { get; set; } = [];
	public ShipmentStatus Status { get; set; }
}

internal sealed class Parcel
{
	public decimal Weight { get; set; }
}
