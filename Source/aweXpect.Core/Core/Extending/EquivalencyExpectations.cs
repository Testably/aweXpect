using System.Diagnostics.CodeAnalysis;
using System.Threading;
using aweXpect.Core.EvaluationContext;
using aweXpect.Equivalency;

namespace aweXpect.Core.Extending;

/// <summary>
///     Gives extensions access to the expectations created with <see cref="It.Is{T}" /> as values in an expected object,
///     which an equivalency comparison evaluates instead of comparing the value.
/// </summary>
public static class EquivalencyExpectations
{
	/// <summary>
	///     Gets the builder of the <c>It.Is…</c> expectation, when the <paramref name="value" /> is one.
	/// </summary>
	/// <remarks>
	///     The <paramref name="value" /> is the result of the whole chain, e.g.
	///     <c>It.Is&lt;string&gt;().That.StartsWith("Let")</c>, so it is found by its builder rather than by its type.
	///     Evaluate the expectation with
	///     <see cref="EquivalencyExpectationBuilder.IsMetBy{TValue}(TValue, IEvaluationContext, CancellationToken)" />
	///     and the context and the token of your constraint.
	/// </remarks>
	public static bool TryGetBuilder(object? value,
		[NotNullWhen(true)] out EquivalencyExpectationBuilder? expectationBuilder)
	{
		if (value is Expectation and IOptionsProvider<ExpectationBuilder>
		    {
			    Options: EquivalencyExpectationBuilder builder,
		    })
		{
			expectationBuilder = builder;
			return true;
		}

		expectationBuilder = null;
		return false;
	}
}
