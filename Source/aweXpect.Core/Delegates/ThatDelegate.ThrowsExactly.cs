using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	/// <summary>
	///     Verifies that the delegate throws exactly an exception of type <typeparamref name="TException" />.
	/// </summary>
	[GuaranteesNotNull]
	public ThatDelegateThrows<TException> ThrowsExactly<TException>()
		where TException : Exception
		=> ThrowsOfType<TException>(typeof(TException), true);

	/// <summary>
	///     Verifies that the delegate throws exactly an exception of type <paramref name="type" />.
	/// </summary>
	[GuaranteesNotNull]
	public ThatDelegateThrows<Exception> ThrowsExactly(Type type)
	{
		type.ThrowIfNotAnExceptionType();
		return ThrowsOfType<Exception>(type, true);
	}
}
