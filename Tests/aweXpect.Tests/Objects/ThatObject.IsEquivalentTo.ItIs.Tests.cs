using aweXpect.Equivalency;

// ReSharper disable UnusedMember.Local

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class ItIs
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenAllConditionsFail_ShouldFail()
				{
					DummyClass subject = new()
					{
						StringValue = "foo",
						IntValue = 42,
					};
					var expected = new
					{
						StringValue = It.Is<string>().That.IsEqualTo("folly"),
						IntValue = It.Is<int>().That.IsLessThan(2),
						BoolValue = true,
					};

					async Task Act()
						=> await That(subject).IsEquivalentTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to {
						                 BoolValue = True,
						                 IntValue = is int that is less than 2,
						                 StringValue = is string that is equal to "folly"
						               },
						             but it was not:
						               Property StringValue differed:
						                   Actual: "foo"
						                 Expected: is string that is equal to "folly"
						             and
						               Property IntValue differed:
						                   Actual: 42
						                 Expected: is int that is less than 2
						             and
						               Property BoolValue differed:
						                   Actual: False
						                 Expected: True

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task WhenAllConditionsMeet_ShouldSucceed()
				{
					DummyClass subject = new()
					{
						StringValue = "foo",
						IntValue = 42,
					};
					var expected = new
					{
						StringValue = It.Is<string>().That.IsNotEmpty(),
						IntValue = It.Is<int>().That.IsGreaterThan(2),
					};

					async Task Act()
						=> await That(subject).IsEquivalentTo(expected);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAnyConditionFails_ShouldFail()
				{
					DummyClass subject = new()
					{
						StringValue = "foo",
						IntValue = 42,
					};
					var expected = new
					{
						StringValue = It.Is<string>().That.IsNotEmpty(),
						IntValue = It.Is<int>().That.IsLessThan(2),
					};

					async Task Act()
						=> await That(subject).IsEquivalentTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to {
						                 IntValue = is int that is less than 2,
						                 StringValue = is string that is not empty
						               },
						             but it was not:
						               Property IntValue differed:
						                   Actual: 42
						                 Expected: is int that is less than 2

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				/// <summary>
				///     It is not possible to determine the type of <see langword="null" />!
				/// </summary>
				[Fact]
				public async Task WhenAnyNotNullCheckAndItIsNull_ShouldFail()
				{
					DummyClass subject = new()
					{
						StringValue = null,
						NullableIntValue = null,
						IntValue = 42,
					};
					var expected = new
					{
						StringValue = It.Is<string>().That.IsEmpty(),
						NullableIntValue = It.Is<int?>().That.IsEqualTo(0),
						IntValue = It.Is<int>().That.IsGreaterThan(2),
					};

					async Task Act()
						=> await That(subject).IsEquivalentTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to {
						                 IntValue = is int that is greater than 2,
						                 NullableIntValue = is int? that is equal to 0,
						                 StringValue = is string that is empty
						               },
						             but it was not:
						               Property StringValue differed:
						                   Actual: <null>
						                 Expected: is string that is empty
						             and
						               Property NullableIntValue differed:
						                   Actual: <null>
						                 Expected: is int? that is equal to 0

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task WhenAnyTypeDoesNotMatch_ShouldFail()
				{
					DummyClass subject = new()
					{
						StringValue = "foo",
						IntValue = 1,
					};
					var expected = new
					{
						StringValue = It.Is<DateTime>(),
						IntValue = It.Is<int>().That.IsGreaterThan(2),
					};

					async Task Act()
						=> await That(subject).IsEquivalentTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to {
						                 IntValue = is int that is greater than 2,
						                 StringValue = is DateTime
						               },
						             but it was not:
						               Property StringValue differed:
						                   Actual: "foo" (string)
						                 Expected: is DateTime
						             and
						               Property IntValue differed:
						                   Actual: 1
						                 Expected: is int that is greater than 2

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task WhenCheckForNullAndItIsNotNull_ShouldFail()
				{
					DummyClass subject = new()
					{
						StringValue = "",
						IntValue = 42,
					};
					var expected = new
					{
						StringValue = It.Is<string>().That.IsNull(),
						IntValue = It.Is<int>().That.IsGreaterThan(2),
					};

					async Task Act()
						=> await That(subject).IsEquivalentTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to {
						                 IntValue = is int that is greater than 2,
						                 StringValue = is string that is null
						               },
						             but it was not:
						               Property StringValue differed:
						                   Actual: ""
						                 Expected: is string that is null

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				/// <summary>
				///     It is not possible to determine the type of <see langword="null" />!
				/// </summary>
				[Fact]
				public async Task WhenCheckForNullAndItIsNull_ShouldSucceed()
				{
					DummyClass subject = new()
					{
						StringValue = null,
						IntValue = 42,
					};
					var expected = new
					{
						StringValue = It.Is<string>().That.IsNull(),
						IntValue = It.Is<int>().That.IsGreaterThan(2),
					};

					async Task Act()
						=> await That(subject).IsEquivalentTo(expected);

					await That(Act).DoesNotThrow();
				}

				// ReSharper disable UnusedAutoPropertyAccessor.Local
				private sealed class DummyClass
				{
					public string? StringValue { get; set; }
					public int? NullableIntValue { get; set; }
					public int IntValue { get; set; }
					public bool BoolValue { get; set; }
				}
				// ReSharper restore UnusedAutoPropertyAccessor.Local
			}
		}
	}
}
