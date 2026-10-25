namespace aweXpect.Results;

/// <summary>
///     A result of type <typeparamref name="TSelf" /> that compares <typeparamref name="TElement" /> values within a
///     <typeparamref name="TTolerance" />, specified via <c>Within(…)</c>.
/// </summary>
/// <remarks>
///     The option methods are extension methods that infer <typeparamref name="TTolerance" /> from this interface, so a
///     result has to implement it only once, with itself as <typeparamref name="TSelf" />. The match type of the
///     expectation starts as an <see cref="aweXpect.Options.ObjectToleranceMatchType{TSubject, TTolerance}" />.
/// </remarks>
public interface IObjectEqualityWithToleranceResult<TSelf, TElement, TTolerance>
	: IObjectEqualityResult<TSelf, TElement>
	where TSelf : IObjectEqualityWithToleranceResult<TSelf, TElement, TTolerance>;
