namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class CombinedTests
	{
		[Test]
		public async Task WhenAbsoluteCheckPrecedesACheckOnARelativeSubject_ShouldReportBothResults()
		{
			Uri subject = new("tracks/1", UriKind.Relative);

			async Task Act()
				=> await That(subject).IsAbsolute().And.IsFile();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is an absolute URI and is a file URI,
				             but it was tracks/1 and was relative
				             """)
				.Because("the relative URI fails both expectations, each with a result of its own");
		}

		[Test]
		public async Task WhenNegatedCombinationOnRelativeSubject_ShouldFail()
		{
			Uri subject = new("tracks/1", UriKind.Relative);

			async Task Act()
				=> await That(subject).DoesNotComplyWith(it => it.IsFile().Or.IsAbsolute());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is not a file URI and is not an absolute URI,
				             but it was relative
				             """)
				.Because("the relative URI cannot answer the first expectation, which decides the negation as well");
		}

		[Test]
		public async Task WhenRelativeSubjectFailsTheFirstAlternative_ShouldNotEvaluateTheOtherOne()
		{
			Uri subject = new("tracks/1", UriKind.Relative);

			async Task Act()
				=> await That(subject).IsNotLoopback().Or.IsFile();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not reference the local host or is a file URI,
				             but it was relative
				             """);
		}

		[Test]
		public async Task WhenRelativeSubjectFailsTheFirstOfCombinedExpectations_ShouldReportBothResults()
		{
			Uri subject = new("tracks/1", UriKind.Relative);

			async Task Act()
				=> await That(subject).IsFile().And.IsAbsolute();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is a file URI and is an absolute URI,
				             but it was relative and was tracks/1
				             """)
				.Because("both expectations fail for the relative URI, the first one because it cannot be answered");
		}
	}
}
