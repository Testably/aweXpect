using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     A result of type <typeparamref name="TSelf" /> that waits for signals with a <typeparamref name="TParameter" />,
///     which can be filtered via <c>With(…)</c>.
/// </summary>
/// <remarks>
///     The option methods are extension methods that infer <typeparamref name="TParameter" /> from this interface, so a
///     result has to implement it only once, with itself as <typeparamref name="TSelf" />.
/// </remarks>
public interface ISignalerResult<TSelf, TParameter> : IOptionsProvider<SignalerOptions<TParameter>>;
