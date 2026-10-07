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
				[Test]
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

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to expected,
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

				[Test]
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

				[Test]
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

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to expected,
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
				[Test]
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

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to expected,
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

				[Test]
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

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to expected,
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

				[Test]
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

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to expected,
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
				[Test]
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

				[Test]
				public async Task WhenExpectedTypeIsANonNullableValueTypeAndItIsNull_ShouldFail()
				{
					DummyClass subject = new()
					{
						NullableIntValue = null,
					};
					var expected = new
					{
						NullableIntValue = It.Is<int>().That.IsEqualTo(0),
					};

					async Task Act()
						=> await That(subject).IsEquivalentTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to expected,
						             but it was not:
						               Property NullableIntValue differed:
						                   Actual: <null>
						                 Expected: is int

						             Equivalency options:
						              - include public fields and properties
						             """)
						.Because("null is no value of a non-nullable value type, so its expectations are not evaluated");
				}

				[Test]
				public async Task WhenMemberWithAsyncReasonIsMet_AndAnotherConditionFails_ShouldIncludeTheReason()
				{
					DummyClass subject = new()
					{
						StringValue = "foo",
					};
					var expected = new
					{
						StringValue = It.Is<string>().That
							.Whose(s => s.Length, l => l.IsEqualTo(3).Because(Task.FromResult<string?>("of a"))).And
							.IsEqualTo("bar"),
					};

					async Task Act()
						=> await That(subject).IsEquivalentTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to expected,
						             but it was not:
						               Property StringValue differed:
						                   Actual: "foo"
						                 Expected: is string that whose Length is equal to 3, because of a and is equal to "bar"

						             Equivalency options:
						              - include public fields and properties
						             """)
						.Because("a reason that must be awaited is shown like a string reason, although its member is met");
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
