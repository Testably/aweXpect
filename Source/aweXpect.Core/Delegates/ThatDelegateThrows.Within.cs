using System;
using aweXpect.Options;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <summary>
	///     Verifies that the delegate throws within the given <paramref name="duration" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="duration" /> is applied as timeout (a subsequent <c>WithTimeout(…)</c> overwrites it),
	///     so that a delegate accepting a <see cref="System.Threading.CancellationToken" /> is cancelled once it
	///     elapsed. A delegate without such a parameter cannot be interrupted and is awaited to completion,
	///     however long that takes.
	/// </remarks>
	public ThatDelegateThrows<TException> Within(TimeSpan duration)
	{
		if (duration < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException(nameof(duration), "The duration must not be negative.");
		}

		TimeSpanEqualityOptions options = new();
		options.Within(duration);
		ThrowOptions.ExecutionTimeOptions = options;
		ExpectationBuilder.WithTimeout(duration);
		return this;
	}
}
