using aweXpect.Core;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	/// <summary>
	///     A delegate without value.
	/// </summary>
	public sealed partial class WithoutValue(ExpectationBuilder expectationBuilder)
		: ThatDelegate(expectationBuilder), IExpectThat<WithoutValue>
	{
		/// <inheritdoc cref="IExpectThat{T}.ExpectationBuilder" />
		ExpectationBuilder IExpectThat<WithoutValue>.ExpectationBuilder => _expectationBuilder;
	}
}
