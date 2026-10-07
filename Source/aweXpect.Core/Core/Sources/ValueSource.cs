using aweXpect.Core.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Core.Sources;

internal class ValueSource<TValue>(TValue value) : IValueSource<TValue>
{
	/// <summary>
	///     The value of the subject.
	/// </summary>
	public TValue Value => value;

	#region IValueSource<TValue> Members

	public bool IsNullTaskSubject => false;

	public ValueTask<TValue> GetValue(ITimeSystem timeSystem, CancellationToken cancellationToken)
		=> new(value);

	public Exception[]? GetOtherExceptions(Exception exception) => null;

	#endregion
}
