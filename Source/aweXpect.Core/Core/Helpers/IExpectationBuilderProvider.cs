namespace aweXpect.Core.Helpers;

/// <summary>
///     A result that gives access to its <see cref="Core.ExpectationBuilder" />, so that
///     <see cref="Extending.EquivalencyExpectations.TryGetBuilder" /> finds the builder of any result of an
///     <c>It.Is…</c> chain.
/// </summary>
internal interface IExpectationBuilderProvider
{
	ExpectationBuilder ExpectationBuilder { get; }
}
