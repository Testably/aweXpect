namespace aweXpect.Core;

/// <summary>
///     An expectation on a delegate that is expected to throw an exception of type <typeparamref name="T" />.
/// </summary>
// ReSharper disable once UnusedTypeParameter
public interface IThatDelegateThrows<out T> : IExpectThat<T>
{
}
