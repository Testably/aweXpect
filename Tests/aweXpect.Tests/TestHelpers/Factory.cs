using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace aweXpect.Tests;

internal static class Factory
{
	/// <summary>
	///     The number of items after which an "infinite" sequence throws instead of yielding more.
	/// </summary>
	/// <remarks>
	///     The expectations stop enumerating after a handful of items, so this limit is never reached. It only takes
	///     effect when they fail to stop early - most notably under a mutant that drops the check which ends the
	///     enumeration. Without it such a mutant buffers items until the test host runs out of memory, which costs
	///     the mutation tests tens of seconds per mutant instead of failing immediately.
	/// </remarks>
	private const int SafetyLimit = 1000;

#if NET8_0_OR_GREATER
	/// <summary>
	///     Returns an "infinite" <see cref="IAsyncEnumerable{T}" /> of fibonacci numbers.
	/// </summary>
	public static async IAsyncEnumerable<int> GetAsyncFibonacciNumbers(
		int maxIterations = int.MaxValue,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		int a = 0, b = 1;

		int iterations = 0;
		do
		{
			await Task.Yield();
			yield return b;
			(a, b) = (b, a + b);
			ThrowIfEnumeratedTooFar(++iterations, maxIterations);
		} while (iterations < maxIterations && !cancellationToken.IsCancellationRequested);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Returns an "infinite" <see cref="IAsyncEnumerable{T}" /> of mapped fibonacci numbers.
	/// </summary>
	public static async IAsyncEnumerable<T> GetAsyncFibonacciNumbers<T>(
		Func<int, T> mapper,
		int maxIterations = int.MaxValue,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		int a = 0, b = 1;

		int iterations = 0;
		do
		{
			await Task.Yield();
			yield return mapper(b);
			(a, b) = (b, a + b);
			ThrowIfEnumeratedTooFar(++iterations, maxIterations);
		} while (iterations < maxIterations && !cancellationToken.IsCancellationRequested);
	}
#endif

#if NET8_0_OR_GREATER
	/// <summary>
	///     Returns an "infinite" <see cref="IAsyncEnumerable{T}" /> of <paramref name="value" />.
	/// </summary>
	public static async IAsyncEnumerable<T> GetConstantValueAsyncEnumerable<T>(T value,
		int maxIterations = int.MaxValue)
	{
		int iterations = 0;
		do
		{
			await Task.Yield();
			yield return value;
			ThrowIfEnumeratedTooFar(++iterations, maxIterations);
		} while (iterations < maxIterations);
	}
#endif

	/// <summary>
	///     Returns an "infinite" <see cref="IEnumerable{T}" /> of fibonacci numbers.
	/// </summary>
	public static IEnumerable<int> GetFibonacciNumbers(int maxIterations = int.MaxValue)
	{
		int a = 0, b = 1;

		int iterations = 0;
		do
		{
			yield return b;
			(a, b) = (b, a + b);
			ThrowIfEnumeratedTooFar(++iterations, maxIterations);
		} while (iterations < maxIterations);
	}

	/// <summary>
	///     Returns an "infinite" <see cref="IEnumerable{T}" /> of fibonacci numbers.
	/// </summary>
	public static IEnumerable<T> GetFibonacciNumbers<T>(Func<int, T> mapper, int maxIterations = int.MaxValue)
	{
		int a = 0, b = 1;

		int iterations = 0;
		do
		{
			yield return mapper(b);
			(a, b) = (b, a + b);
			ThrowIfEnumeratedTooFar(++iterations, maxIterations);
		} while (iterations < maxIterations);
	}

	/// <summary>
	///     Returns an "infinite" <see cref="IEnumerable{T}" /> of <paramref name="value" />.
	/// </summary>
	public static IEnumerable<T> GetConstantValueEnumerable<T>(T value, int maxIterations = int.MaxValue)
	{
		int iterations = 0;
		do
		{
			yield return value;
			ThrowIfEnumeratedTooFar(++iterations, maxIterations);
		} while (iterations < maxIterations);
	}

	/// <summary>
	///     Throws when the consumer is about to enumerate beyond the <see cref="SafetyLimit" /> of an otherwise
	///     unbounded sequence.
	/// </summary>
	private static void ThrowIfEnumeratedTooFar(int iterations, int maxIterations)
	{
		if (iterations >= SafetyLimit && iterations < maxIterations)
		{
			throw new InvalidOperationException(
				$"The expectation enumerated more than {SafetyLimit} items instead of stopping early.");
		}
	}
}
