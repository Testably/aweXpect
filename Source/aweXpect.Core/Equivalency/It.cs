using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Equivalency;

/// <summary>
///     Static class to support equivalence checks on individual properties.
/// </summary>
public static class It
{
	/// <summary>
	///     Expects the property to be of type <typeparamref name="T" />.
	/// </summary>
	public static IsEquivalent<T> Is<T>() => new();

	/// <summary>
	///     Expectations on properties for equivalence.
	/// </summary>
	public class IsEquivalent<T> : ExpectationResult, IExpectThat<T>, IThatSubject<T>
	{
		/// <inheritdoc cref="IsEquivalent{T}" />
		public IsEquivalent() : this(new EquivalencyExpectationBuilder<T>())
		{
		}

		/// <inheritdoc cref="IsEquivalent{T}" />
		private IsEquivalent(EquivalencyExpectationBuilder<T> expectationBuilder) : base(expectationBuilder)
		{
		}

		/// <summary>
		///     …that…
		/// </summary>
		public IThatSubject<T> That => this;

		/// <inheritdoc cref="IExpectThat{T}.ExpectationBuilder" />
		ExpectationBuilder IExpectThat<T>.ExpectationBuilder => ExpectationBuilder;

		/// <inheritdoc cref="IThatSubject{T}.Is{TType}" />
		AndOrWhoseResult<TType, IThatSubject<T>> IThatSubject<T>.Is<TType>()
			=> new ThatSubject<T>(ExpectationBuilder).Is<TType>();

		/// <inheritdoc cref="IThatSubject{T}.IsNot{TType}" />
		AndOrResult<T, IThatSubject<T>> IThatSubject<T>.IsNot<TType>()
			=> new ThatSubject<T>(ExpectationBuilder).IsNot<TType>();

		/// <inheritdoc cref="IThatSubject{T}.IsExactly{TType}" />
		AndOrWhoseResult<TType, IThatSubject<T>> IThatSubject<T>.IsExactly<TType>()
			=> new ThatSubject<T>(ExpectationBuilder).IsExactly<TType>();

		/// <inheritdoc cref="IThatSubject{T}.IsNotExactly{TType}" />
		AndOrResult<T, IThatSubject<T>> IThatSubject<T>.IsNotExactly<TType>()
			=> new ThatSubject<T>(ExpectationBuilder).IsNotExactly<TType>();

		/// <inheritdoc cref="object.ToString()" />
		public override string? ToString() => ExpectationBuilder.ToString();
	}
}
