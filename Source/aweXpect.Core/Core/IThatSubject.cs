using aweXpect.Results;

namespace aweXpect.Core;

/// <summary>
///     A subject that supports type-check expectations whose target type is fixed by the receiver.
///     <para />
///     Extends <see cref="IThat{T}" /> with <see cref="Is{TType}" />, <see cref="IsNot{TType}" />,
///     <see cref="IsExactly{TType}" /> and <see cref="IsNotExactly{TType}" /> as instance members so that
///     they can be invoked with a single explicit type argument inside generic contexts where the
///     receiver's type parameter is unconstrained.
/// </summary>
public interface IThatSubject<T> : IThat<T>
{
	/// <summary>
	///     Verifies that the subject is of type <typeparamref name="TType" />.
	/// </summary>
	[GuaranteesNotNull]
	AndOrWhoseResult<TType, IThatSubject<T>> Is<TType>();

	/// <summary>
	///     Verifies that the subject is not of type <typeparamref name="TType" />.
	/// </summary>
	AndOrResult<T, IThatSubject<T>> IsNot<TType>();

	/// <summary>
	///     Verifies that the subject is exactly of type <typeparamref name="TType" />.
	/// </summary>
	[GuaranteesNotNull]
	AndOrWhoseResult<TType, IThatSubject<T>> IsExactly<TType>();

	/// <summary>
	///     Verifies that the subject is not exactly of type <typeparamref name="TType" />.
	/// </summary>
	AndOrResult<T, IThatSubject<T>> IsNotExactly<TType>();
}
