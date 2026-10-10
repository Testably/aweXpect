namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsNotFile
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				Uri[] subjects = [new Uri("file:///C:/foo"),];

				async Task Act()
					=> await That(new { Uris = subjects, })
						.Whose(x => x.Uris, items => items.All()
							.ComplyWith(item => item.IsNotFile()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Uris are not file URIs for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsNotFile();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a file URI,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsAFileUri_ShouldFail()
			{
				Uri subject = new("file://server/filename.ext");

				async Task Act()
					=> await That(subject).IsNotFile();

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
					=> await That(subject).IsNotFile();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotFile());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a file URI,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsAFileUri_ShouldSucceed()
			{
				Uri subject = new("file://server/filename.ext");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotFile());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNotAFileUri_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotFile());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a file URI,
					             but it was https://www.awexpect.com/
					             """);
			}
		}
	}
}
