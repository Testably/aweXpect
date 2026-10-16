using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     A result of type <typeparamref name="TResult" /> directly after <c>Within(…)</c>, whose
///     <typeparamref name="TTolerance" /> can still be made relative to the expected value, e.g. via
///     <c>Percent()</c>.
/// </summary>
/// <remarks>
///     The implementing type must derive from <typeparamref name="TResult" />.
/// </remarks>
public interface IWithinToleranceResult<TResult, TTolerance>
	: IOptionsProvider<IRelativeToleranceOptions<TTolerance>>;
