using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     A result of type <typeparamref name="TSelf" /> that compares <typeparamref name="TElement" /> values with
///     <see cref="ObjectMatchType{TElement}" />, e.g. via <c>Using(…)</c> or <c>Equivalent()</c>.
/// </summary>
/// <remarks>
///     The option methods are extension methods that infer <typeparamref name="TElement" /> from this interface, so a
///     result has to implement it only once, with itself as <typeparamref name="TSelf" />.
/// </remarks>
public interface IObjectEqualityResult<TSelf, TElement> : IOptionsProvider<ObjectMatchType<TElement>>
	where TSelf : IObjectEqualityResult<TSelf, TElement>;
