namespace aweXpect.Internal.Tests.ThatTests.Collections;

public sealed partial class QuantifiedCollectionResult
{
	public sealed class BeTests
	{
		[Fact]
		public async Task WhenCollectionContainsOtherValues_ShouldFail()
		{
			object[] subject =
			[
				new MyClass(1),
				new SubClass(1),
				new OtherClass(1),
			];

			async Task Act()
				=> await That(subject).All().Are<MyClass>();

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is of type QuantifiedCollectionResult.MyClass for all items,
				             but only 2 of 3 were
				             
				             Not matching items:
				             [
				               QuantifiedCollectionResult.OtherClass {
				                 Value = 1
				               }
				             ]
				             
				             Collection:
				             [
				               QuantifiedCollectionResult.MyClass {
				                 Value = 1
				               },
				               QuantifiedCollectionResult.SubClass {
				                 Value = 1
				               },
				               QuantifiedCollectionResult.OtherClass {
				                 Value = 1
				               }
				             ]
				             """);
		}

		[Fact]
		public async Task WhenCollectionOnlyContainsEqualValues_ShouldSucceed()
		{
			object[] subject =
			[
				new MyClass(1),
				new SubClass(1),
			];

			async Task Act()
				=> await That(subject).All().Are<MyClass>();

			await That(Act).DoesNotThrow();
		}
	}
}
