using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     A result of type <typeparamref name="TSelf" /> that compares <typeparamref name="TElement" /> values within a
///     <typeparamref name="TTolerance" />, specified via <c>Within(…)</c>.
/// </summary>
/// <remarks>
///     The option methods are extension methods that infer <typeparamref name="TTolerance" /> from this interface, so a
///     result has to implement it only once, with itself as <typeparamref name="TSelf" />.
/// </remarks>
public interface IObjectEqualityWithToleranceResult<TSelf, TElement, TTolerance>
	: IObjectEqualityResult<TSelf, TElement>,
		IOptionsProvider<ObjectEqualityWithToleranceOptions<TElement, TTolerance>>
	where TSelf : IObjectEqualityWithToleranceResult<TSelf, TElement, TTolerance>;
