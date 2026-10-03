using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Sources;

internal class AsyncValueSource<TValue>(Task<TValue> value) : IValueSource<TValue>
{
	/// <summary>
	///     Flag, indicating if the subject is a <see langword="null" /> task, which has no value to await.
	/// </summary>
	// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
	public bool IsNullTask => value is null;

	#region IValueSource<TValue> Members

	public ValueTask<TValue> GetValue(ITimeSystem timeSystem, CancellationToken cancellationToken)
		=> new(value.AbandonOnCancellation(cancellationToken));

	#endregion

	/// <inheritdoc cref="TaskHelpers.GetOtherExceptions(Task, Exception)" />
	public Exception[]? GetOtherExceptions(Exception exception)
		=> value.GetOtherExceptions(exception);
}
