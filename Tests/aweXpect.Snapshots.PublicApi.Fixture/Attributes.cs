using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace aweXpect.Snapshots.PublicApi.Fixture;

[AttributeUsage(AttributeTargets.All)]
internal sealed class InternalMarkerAttribute : Attribute;

public static class Prioritized
{
	[InternalMarker]
	[OverloadResolutionPriority(1)]
	public static void Pick(object value) => throw new NotSupportedException();
}

[Serializable]
public class Payload;

public static class Native
{
	[DllImport("kernel32")]
	public static extern uint GetTickCount();

	[DllImport("user32", EntryPoint = "MessageBoxW", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern int ShowMessage(IntPtr owner, string text, string caption, uint type);

	public static void Retry([Optional] int attempts) => throw new NotSupportedException();
}
