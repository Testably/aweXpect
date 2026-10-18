using System;
using System.Runtime.CompilerServices;

namespace aweXpect.Snapshots.PublicApi.Fixture;

[AttributeUsage(AttributeTargets.All)]
internal sealed class InternalMarkerAttribute : Attribute;

public static class Prioritized
{
	[InternalMarker]
	[OverloadResolutionPriority(1)]
	public static void Pick(object value) => throw new NotSupportedException();
}
