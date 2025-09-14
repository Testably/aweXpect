namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsNotUnc
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsNotUnc_ShouldFail()
			{
				Uri subject = new("file://server/filename.ext");

				async Task Act()
					=> await That(subject).IsNotUnc();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an UNC path,
					             but it was file://server/filename.ext
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNotUnc_ShouldSucceed()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).IsNotUnc();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsNotUnc_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotUnc());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is an UNC path,
					             but it was https://www.awexpect.com/
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNotUnc_ShouldSucceed()
			{
				Uri subject = new("file://server/filename.ext");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotUnc());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
