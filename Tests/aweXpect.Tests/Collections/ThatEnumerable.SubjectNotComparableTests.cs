using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	/// <summary>
	///     An item that a custom match type cannot compare fails the expectation and its negation alike, unless it is
	///     not compared at all.
	/// </summary>
	public sealed class SubjectNotComparableTests
	{
		[Test]
		public async Task AllAreEqualTo_AsNumber_WhenAnItemIsNoNumber_ShouldFail()
		{
			IEnumerable<string> subject = ["1", "foo",];

			async Task Act()
				=> await That(subject).All().AreEqualTo("1").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to "1" as number for all items,
				             but an item was "foo", which is no number
				             """);
		}

		[Test]
		public async Task AllComplyWith_IsEqualTo_AsNumber_WhenAnItemIsNoNumber_ShouldFail()
		{
			IEnumerable<string> subject = ["1", "foo",];

			async Task Act()
				=> await That(subject).All().ComplyWith(item => item.IsEqualTo("1").AsNumber());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is the number "1" for all items,
				             but for the item at index 1, it was "foo", which is no number

				             Collection:
				             [
				               "1",
				               "foo"
				             ]
				             """);
		}

		[Test]
		public async Task AllComplyWith_Whose_IsEqualTo_AsNumber_WhenAMemberIsNoNumber_ShouldNameTheItemAndTheMember()
		{
			IEnumerable<Container> subject = [new("1"), new("foo"),];

			async Task Act()
				=> await That(subject).All()
					.ComplyWith(item => item.Whose(x => x.Value, value => value.IsEqualTo("1").AsNumber()));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             whose Value is the number "1" for all items,
				             but for the item at index 1, Value was "foo", which is no number

				             Collection:
				             [
				               ThatEnumerable.SubjectNotComparableTests.Container {
				                 Value = "1"
				               },
				               ThatEnumerable.SubjectNotComparableTests.Container {
				                 Value = "foo"
				               }
				             ]
				             """);
		}

		[Test]
		public async Task Contains_AsNumber_WhenAnItemIsNoNumber_ShouldFail()
		{
			IEnumerable<string> subject = ["foo", "2",];

			async Task Act()
				=> await That(subject).Contains("1").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains "1" as number at least once,
				             but an item was "foo", which is no number
				             """);
		}

		[Test]
		public async Task DoesNotContain_AsNumber_WhenAnItemIsNoNumber_ShouldFail()
		{
			IEnumerable<string> subject = ["foo", "2",];

			async Task Act()
				=> await That(subject).DoesNotContain("1").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain "1" as number,
				             but an item was "foo", which is no number
				             """)
				.Because("an item that cannot be compared is not different from the unexpected value either");
		}

		[Test]
		public async Task HasItem_AtIndex_AsNumber_WhenAnotherItemIsNoNumber_ShouldSucceed()
		{
			IEnumerable<string> subject = ["1", "foo",];

			async Task Act()
				=> await That(subject).HasItem("1").AsNumber().AtIndex(0);

			await That(Act).DoesNotThrow()
				.Because("only the item at the index is compared");
		}

		[Test]
		public async Task IsEqualTo_AsNumber_WhenAnItemIsNoNumber_ShouldFail()
		{
			IEnumerable<string> subject = ["1", "foo",];
			string[] expected = ["1", "2",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected as number in order,
				             but an item was "foo", which is no number

				             Collection:
				             [
				               "1",
				               "foo"
				             ]

				             Expected:
				             [
				               "1",
				               "2"
				             ]
				             """);
		}

		[Test]
		public async Task IsEqualTo_InAnyOrder_AsNumber_WhenAnItemIsNoNumber_ShouldFail()
		{
			IEnumerable<string> subject = ["foo", "1",];
			string[] expected = ["1", "2",];

			async Task Act()
				=> await That(subject).IsEqualTo(expected).AsNumber().InAnyOrder();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection expected as number in any order,
				             but an item was "foo", which is no number

				             Collection:
				             [
				               "foo",
				               "1"
				             ]

				             Expected:
				             [
				               "1",
				               "2"
				             ]
				             """);
		}

		[Test]
		public async Task IsNotEqualTo_AsNumber_WhenAnItemIsNoNumber_ShouldFail()
		{
			IEnumerable<string> subject = ["1", "foo",];
			string[] unexpected = ["1", "2",];

			async Task Act()
				=> await That(subject).IsNotEqualTo(unexpected).AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not equal to collection unexpected as number in order,
				             but an item was "foo", which is no number

				             Collection:
				             [
				               "1",
				               "foo"
				             ]

				             Expected:
				             [
				               "1",
				               "2"
				             ]
				             """)
				.Because("a collection with an item that cannot be compared is not different from the unexpected one either");
		}

		[Test]
		public async Task Whose_Contains_AsNumber_WhenAnItemOfTheMemberIsNoNumber_ShouldNameTheMember()
		{
			Tagged subject = new(["foo", "2",]);

			async Task Act()
				=> await That(subject).Whose(x => x.Tags, tags => tags.Contains("1").AsNumber());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             whose Tags contain "1" as number at least once,
				             but an item of Tags was "foo", which is no number
				             """);
		}

		private sealed class Container(string value)
		{
			public string Value { get; } = value;
		}

		private sealed class Tagged(IEnumerable<string> tags)
		{
			public IEnumerable<string> Tags { get; } = tags;
		}
	}
}
