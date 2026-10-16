using System.Runtime.CompilerServices;

namespace aweXpect.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed partial class ThatWeakReference
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference CreateCollectableReference()
		=> new(new object());

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference<object> CreateCollectableGenericReference()
		=> new(new object());

	/// <remarks>
	///     The target is only released after two rounds of finalizers, so a single forced collection does not suffice.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static WeakReference CreateReferenceReleasedByFinalizerChain(Holder targetHolder, Holder finalizerHolder)
	{
		targetHolder.Value = new object();
		finalizerHolder.Value = new ReleasingFinalizer(targetHolder);
		_ = new ReleasingFinalizer(finalizerHolder);
		return new WeakReference(targetHolder.Value);
	}

	private sealed class Holder
	{
		public object? Value { get; set; }
	}

	private sealed class ReleasingFinalizer(Holder holder)
	{
		~ReleasingFinalizer()
		{
			holder.Value = null;
		}
	}
}
