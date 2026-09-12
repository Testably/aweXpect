using System;
using aweXpect.Core;
using aweXpect.Core.Sources;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	/// <summary>
	///     Verifies that the delegate throws an exception.
	/// </summary>
	[GuaranteesNotNull]
	public ThatDelegateThrows<Exception> ThrowsException()
	{
		ThrowsOption throwOptions = new();
		return new ThatDelegateThrows<Exception>(ExpectationBuilder
				.AddConstraint((it, grammars)
					=> new DelegateIsNotNullWithinTimeoutConstraint(it, grammars, throwOptions))
				.ForWhich<DelegateValue, Exception?>(d => d.Exception)
				.AddConstraint((it, grammars) => new ThrowsConstraint(it, grammars, typeof(Exception), throwOptions))
				.And(" "),
			throwOptions);
	}
}
