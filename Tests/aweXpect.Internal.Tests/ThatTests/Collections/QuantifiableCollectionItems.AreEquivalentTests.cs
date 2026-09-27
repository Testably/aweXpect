namespace aweXpect.Internal.Tests.ThatTests.Collections;

public sealed partial class QuantifiableCollectionItems
{
	public sealed class AreEquivalentTests
	{
		[Fact]
		public async Task WhenCollectionContainsOtherValues_ShouldFail()
		{
			MyClass[] subject =
			[
				new()
				{
					Value = "Foo",
				},
				new()
				{
					Value = "Foo",
				},
				new()
				{
					Value = "Foo",
				},
				new()
				{
					Value = "Bar",
				},
			];

			MyClass expected = new()
			{
				Value = "Foo",
			};

			async Task Act()
				=> await That(subject).All().ComplyWith(item => item.IsEquivalentTo(expected));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is equivalent to expected for all items,
				             but only 3 of 4 were

				             Not matching items:
				             [
				               QuantifiableCollectionItems.MyClass {
				                 Inner = <null>,
				                 Value = "Bar"
				               }
				             ]

				             Collection:
				             [
				               QuantifiableCollectionItems.MyClass {
				                 Inner = <null>,
				                 Value = "Foo"
				               },
				               QuantifiableCollectionItems.MyClass {
				                 Inner = <null>,
				                 Value = "Foo"
				               },
				               QuantifiableCollectionItems.MyClass {
				                 Inner = <null>,
				                 Value = "Foo"
				               },
				               QuantifiableCollectionItems.MyClass {
				                 Inner = <null>,
				                 Value = "Bar"
				               }
				             ]
				             """);
		}

		[Fact]
		public async Task WhenCollectionOnlyContainsEqualValues_ShouldSucceed()
		{
			MyClass[] subject =
			[
				new()
				{
					Value = "Foo",
				},
				new()
				{
					Value = "Foo",
				},
				new()
				{
					Value = "Foo",
				},
			];

			MyClass expected = new()
			{
				Value = "Foo",
			};

			async Task Act()
				=> await That(subject).All().ComplyWith(item => item.IsEquivalentTo(expected));

			await That(Act).DoesNotThrow();
		}
	}
}
