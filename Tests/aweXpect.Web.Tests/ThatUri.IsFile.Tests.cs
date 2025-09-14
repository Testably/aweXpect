namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsFile
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsAFileUri_ShouldSucceed()
			{
				Uri subject = new("file://server/filename.ext");

				async Task Act()
					=> await That(subject).IsFile();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotAFileUri_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).IsFile();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a file URI,
					             but it was https://www.awexpect.com/
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsAFileUri_ShouldFail()
			{
				Uri subject = new("file://server/filename.ext");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsFile());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a file URI,
					             but it was file://server/filename.ext
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNotAFileUri_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsFile());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
