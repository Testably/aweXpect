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
	{
		ExpectationBuilder builder = ExpectationBuilder;
		return new(builder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint<T, TType>(builder, it, grammars)),
			this);
	}

	/// <inheritdoc cref="IThatSubject{T}.IsNot{TType}" />
	[GuaranteesNotNull]
	public AndOrResult<T, IThatSubject<T>> IsNot<TType>()
	{
		ExpectationBuilder builder = ExpectationBuilder;
		return new(builder.AddConstraint((it, grammars)
				=> new IsOfTypeConstraint<T, TType>(builder, it, grammars).Invert()),
			this);
	}

	/// <inheritdoc cref="IThatSubject{T}.IsExactly{TType}" />
	[GuaranteesNotNull]
	public AndOrWhoseResult<TType, IThatSubject<T>> IsExactly<TType>()
	{
		ExpectationBuilder builder = ExpectationBuilder;
		return new(builder.AddConstraint((it, grammars)
				=> new IsExactlyOfTypeConstraint<T, TType>(builder, it, grammars)),
			this);
	}

	/// <inheritdoc cref="IThatSubject{T}.IsNotExactly{TType}" />
	[GuaranteesNotNull]
	public AndOrResult<T, IThatSubject<T>> IsNotExactly<TType>()
	{
		ExpectationBuilder builder = ExpectationBuilder;
		return new(builder.AddConstraint((it, grammars)
				=> new IsExactlyOfTypeConstraint<T, TType>(builder, it, grammars).Invert()),
			this);
	}
}
