using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;
using aweXpect.Core.Internal;

namespace aweXpect.Core.Sources;

internal class AsyncValueSource<TValue>(Task<TValue> value) : IValueSource<TValue>
{
	#region IValueSource<TValue> Members

	// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
	public bool IsNullTaskSubject => value is null;

	public ValueTask<TValue> GetValue(ITimeSystem timeSystem, CancellationToken cancellationToken)
		=> new(value.AbandonOnCancellation(cancellationToken));

	/// <inheritdoc cref="TaskHelpers.GetOtherExceptions(Task, Exception)" />
	public Exception[]? GetOtherExceptions(Exception exception)
		=> value.GetOtherExceptions(exception);

	#endregion
}
