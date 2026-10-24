using aweXpect.Core.Extending;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class EquivalencyExpectationsTests
{
	public sealed class TryGetBuilder
	{
		[Test]
		public async Task WhenValueIsAChainedItIs_ShouldReturnTheBuilderOfTheChain()
		{
			object value = It.Is<string>().That.StartsWith("Let");

			bool result = EquivalencyExpectations.TryGetBuilder(value, out EquivalencyExpectationBuilder? builder);

			await That(result).IsTrue()
				.Because("the chain returns another result than It.Is, which shares its builder");
			await That(builder?.ToString()).IsEqualTo("is string");
		}

		[Test]
		public async Task WhenValueIsAnotherExpectation_ShouldReturnFalse()
		{
			object value = That(true).IsTrue();

			bool result = EquivalencyExpectations.TryGetBuilder(value, out EquivalencyExpectationBuilder? builder);

			await That(result).IsFalse()
				.Because("only the builder of an It.Is expectation is an equivalency expectation builder");
			await That(builder).IsNull();
		}

		[Test]
		public async Task WhenValueIsItIs_ShouldReturnItsBuilder()
		{
			object value = It.Is<int>();

			bool result = EquivalencyExpectations.TryGetBuilder(value, out EquivalencyExpectationBuilder? builder);

			await That(result).IsTrue();
			await That(builder?.ToString()).IsEqualTo("is int");
		}

		[Test]
		public async Task WhenValueIsNotAnExpectation_ShouldReturnFalse()
		{
			object value = "Let It Be";

			bool result = EquivalencyExpectations.TryGetBuilder(value, out EquivalencyExpectationBuilder? builder);

			await That(result).IsFalse();
			await That(builder).IsNull();
		}

		[Test]
		public async Task WhenValueIsNull_ShouldReturnFalse()
		{
			bool result = EquivalencyExpectations.TryGetBuilder(null, out EquivalencyExpectationBuilder? builder);

			await That(result).IsFalse();
			await That(builder).IsNull();
		}
	}
}
