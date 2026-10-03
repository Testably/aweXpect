using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Sources;

internal interface IValueSource<TValue>
{
	/// <summary>
	///     Flag, indicating if the subject is a <see langword="null" /> task, which has no value to await.
	/// </summary>
	bool IsNullTaskSubject { get; }

	ValueTask<TValue> GetValue(ITimeSystem timeSystem, CancellationToken cancellationToken);

	/// <summary>
	///     The exceptions of the faulted subject besides the <paramref name="exception" />, which
	///     <see cref="GetValue" /> threw, or <see langword="null" /> when there are none.
	/// </summary>
	Exception[]? GetOtherExceptions(Exception exception);
}
