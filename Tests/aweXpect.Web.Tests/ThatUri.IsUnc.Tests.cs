namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsUnc
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsNotUnc_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).IsUnc();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an UNC path,
					             but it was https://www.awexpect.com/
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsUnc_ShouldSucceed()
			{
				Uri subject = new("file://server/filename.ext");

				async Task Act()
					=> await That(subject).IsUnc();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsNotUnc_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsUnc());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsUnc_ShouldFail()
			{
				Uri subject = new("file://server/filename.ext");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsUnc());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an UNC path,
					             but it was file://server/filename.ext
					             """);
			}
		}
	}
}
