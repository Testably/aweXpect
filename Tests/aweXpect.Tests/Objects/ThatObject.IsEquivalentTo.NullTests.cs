using aweXpect.Equivalency;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class NullTests
		{
			[Fact]
			public async Task WhenBothAreNull_ShouldSucceed()
			{
				OuterClass? subject = null;

				async Task Act()
					=> await That(subject).IsEquivalentTo(null);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotNull_ShouldFail()
			{
				OuterClass? subject = new()
				{
					Value = "Foo",
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to <null>,
					             but it was ThatObject.OuterClass { Inner = <null>, Value = "Foo" } instead of <null>

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Fact]
			public async Task WhenOptionsAreSpecified_ShouldApplyThem()
			{
				OuterClass? subject = new()
				{
					Value = "Foo",
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(null, o => o.IgnoringMember("Value"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to <null>,
					             but it was ThatObject.OuterClass { Inner = <null>, Value = "Foo" } instead of <null>

					             Equivalency options:
					              - include public fields and properties
					              - ignore members: ["Value"]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNotNull_ShouldStillUseTheTypedOverload()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
				};
				OuterClass expected = new()
				{
					Value = "Bar",
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o =>
					{
						// Only compiles when the generic overload is selected.
						EquivalencyOptions<OuterClass> typedOptions = o;
						return typedOptions.Ignoring(memberPath => memberPath == "Value");
					});

				await That(Act).DoesNotThrow();
			}
		}
	}
}
