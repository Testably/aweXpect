using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	/// <summary>
	///     A value that a custom match type cannot compare fails the expectation and its negation alike.
	/// </summary>
	public sealed class SubjectNotComparableTests
	{
		[Test]
		public async Task Contains_AsNumber_WhenTheValueIsNoNumber_ShouldFail()
		{
			IDictionary<int, string> subject = new Dictionary<int, string>
			{
				[1] = "foo",
			};

			async Task Act()
				=> await That(subject).Contains(1, "1").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains [1] = "1" as number,
				             but a value was "foo", which is no number

				             Dictionary:
				             {
				               [1] = "foo"
				             }
				             """);
		}

		[Test]
		public async Task ContainsValue_AsNumber_WhenAValueIsNoNumber_ShouldFail()
		{
			IDictionary<int, string> subject = new Dictionary<int, string>
			{
				[1] = "foo",
			};

			async Task Act()
				=> await That(subject).ContainsValue("1").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains value "1" as number,
				             but a value was "foo", which is no number

				             Dictionary:
				             {
				               [1] = "foo"
				             }
				             """);
		}

		[Test]
		public async Task DoesNotContainValue_AsNumber_WhenAValueIsNoNumber_ShouldFail()
		{
			IDictionary<int, string> subject = new Dictionary<int, string>
			{
				[1] = "foo",
			};

			async Task Act()
				=> await That(subject).DoesNotContainValue("1").AsNumber();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain value "1" as number,
				             but a value was "foo", which is no number

				             Dictionary:
				             {
				               [1] = "foo"
				             }
				             """)
				.Because("a value that cannot be compared is not different from the unexpected one either");
		}
	}
}