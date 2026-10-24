using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Delegates;

/// <summary>
///     An <see cref="ExpectationResult" /> when an exception was thrown.
/// </summary>
public partial class ThatDelegateThrows<TException>
	: ExpectationResult<TException, ThatDelegateThrows<TException>>, IThatDelegateThrows<TException>
	where TException : Exception?
{
	internal ThatDelegateThrows(ExpectationBuilder expectationBuilder, ThatDelegate.ThrowsOption throwOptions)
		: base(expectationBuilder)
	{
		ThrowOptions = throwOptions;
	}

	/// <summary>
	///     The throw options.
	/// </summary>
	internal ThatDelegate.ThrowsOption ThrowOptions { get; }

	/// <inheritdoc cref="IExpectThat{T}.ExpectationBuilder" />
	ExpectationBuilder IExpectThat<TException>.ExpectationBuilder => ExpectationBuilder;
}
