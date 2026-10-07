using System;
using aweXpect.Core.Helpers;

namespace aweXpect.Delegates;

public partial class ThatDelegateThrows<TException>
{
	/// <summary>
	///     Verifies that the exception was thrown only if the <paramref name="condition" /> is <see langword="true" />,
	///     otherwise it verifies that no exception was thrown.
	/// </summary>
	/// <exception cref="InvalidOperationException">A condition is already set.</exception>
	public ThatDelegateThrows<TException?> OnlyIf(bool condition)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(ThrowOptions.IsOnlyIfSpecified, nameof(OnlyIf));
		ThrowOptions.IsOnlyIfSpecified = true;
		ThrowOptions.DoCheckThrow = condition;
		return new ThatDelegateThrows<TException?>(ExpectationBuilder, ThrowOptions);
	}
}
