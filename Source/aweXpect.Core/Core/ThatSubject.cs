using System.Diagnostics;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect.Core;

/// <summary>
///     Wraps the <see cref="ExpectationBuilder" />.
/// </summary>
[DebuggerDisplay("ThatSubject<{typeof(T)}>: {ExpectationBuilder}")]
public readonly struct ThatSubject<T>(ExpectationBuilder expectationBuilder)
	: IExpectThat<T>, IThatSubject<T>
{
	/// <inheritdoc cref="IExpectThat{T}.ExpectationBuilder" />
	public ExpectationBuilder ExpectationBuilder { get; } = expectationBuilder;

	/// <inheritdoc cref="IThatSubject{T}.Is{TType}" />
	[GuaranteesNotNull]
	public AndOrWhoseResult<TType, IThatSubject<T>> Is<TType>()
		=> new(ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint<T, TType>(it, grammars)),
			this);

	/// <inheritdoc cref="IThatSubject{T}.IsNot{TType}" />
	public AndOrResult<T, IThatSubject<T>> IsNot<TType>()
		=> new(ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint<T, TType>(it, grammars).Invert()),
			this);

	/// <inheritdoc cref="IThatSubject{T}.IsExactly{TType}" />
	[GuaranteesNotNull]
	public AndOrWhoseResult<TType, IThatSubject<T>> IsExactly<TType>()
		=> new(ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsExactlyOfTypeConstraint<T, TType>(it, grammars)),
			this);

	/// <inheritdoc cref="IThatSubject{T}.IsNotExactly{TType}" />
	public AndOrResult<T, IThatSubject<T>> IsNotExactly<TType>()
		=> new(ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsExactlyOfTypeConstraint<T, TType>(it, grammars).Invert()),
			this);
}
