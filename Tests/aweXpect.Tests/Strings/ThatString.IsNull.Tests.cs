namespace aweXpect.Tests;

public sealed partial class ThatString
{
	public class IsNull
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenActualIsEmpty_ShouldFail()
			{
				string subject = "";

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is null,
					             but it was ""
					             """);
			}

			[Test]
			[AutoArguments]
			public async Task WhenActualIsNotNull_ShouldFail(string? subject)
			{
				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is null,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			public async Task WhenActualIsNull_AndNegatedWithOr_ShouldReportNullOnce()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNull().Or.StartsWith("a"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not null and does not start with "a",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenActualIsNull_ShouldSucceed()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).IsNull();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
