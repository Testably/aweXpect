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
	public class IsEquivalent<T> : ExpectationResult, IExpectThat<T>
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
		public IThat<T> That => this;

		/// <inheritdoc cref="IExpectThat{T}.ExpectationBuilder" />
		ExpectationBuilder IExpectThat<T>.ExpectationBuilder => ExpectationBuilder;

		/// <inheritdoc cref="object.ToString()" />
		public override string? ToString() => ExpectationBuilder.ToString();
	}
}
