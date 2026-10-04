using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Delegates;

/// <summary>
///     An <see cref="ExpectationResult" /> when an exception was thrown.
/// </summary>
public partial class ThatDelegateThrows<TException>(
	ExpectationBuilder expectationBuilder,
	ThatDelegate.ThrowsOption throwOptions)
	: ExpectationResult<TException, ThatDelegateThrows<TException>>(expectationBuilder), IThatDelegateThrows<TException>
	where TException : Exception?
{
	/// <summary>
	///     The throw options.
	/// </summary>
	internal ThatDelegate.ThrowsOption ThrowOptions { get; } = throwOptions;

	/// <inheritdoc cref="IExpectThat{T}.ExpectationBuilder" />
	ExpectationBuilder IExpectThat<TException>.ExpectationBuilder => ExpectationBuilder;
}
