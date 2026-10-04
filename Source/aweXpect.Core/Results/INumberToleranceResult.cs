#if NET8_0_OR_GREATER
using System.Numerics;
#else
using System;
#endif
using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     A result of type <typeparamref name="TSelf" /> that compares <typeparamref name="TNumber" /> values within a
///     tolerance, specified via <c>Within(…)</c>.
/// </summary>
/// <remarks>
///     The option methods are extension methods that infer <typeparamref name="TNumber" /> from this interface, so that
///     e.g. <c>Within(1)</c> on a <see langword="double" /> converts the tolerance to a <see langword="double" />.
/// </remarks>
public interface INumberToleranceResult<TSelf, TNumber> : IOptionsProvider<NumberTolerance<TNumber>>
	where TSelf : INumberToleranceResult<TSelf, TNumber>
#if NET8_0_OR_GREATER
	where TNumber : struct, INumber<TNumber>;
#else
	where TNumber : struct, IComparable<TNumber>;
#endif
