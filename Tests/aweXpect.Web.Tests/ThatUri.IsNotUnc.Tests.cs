namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class IsNotUnc
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				Uri[] subjects = [new Uri(@"\\server\share"),];

				async Task Act()
					=> await That(new { Uris = subjects, })
						.Whose(x => x.Uris, items => items.All()
							.ComplyWith(item => item.IsNotUnc()));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Uris are not UNC paths for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).IsNotUnc();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a UNC path,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).IsNotUnc();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a UNC path,
					             but it was relative
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNotUnc_ShouldFail()
			{
				Uri subject = new("file://server/filename.ext");

				async Task Act()
					=> await That(subject).IsNotUnc();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a UNC path,
					             but it was file://server/filename.ext
					             """);
			}

			[Test]
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
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotUnc());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a UNC path,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks/1", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotUnc());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a UNC path,
					             but it was relative
					             """)
					.Because("a relative URI cannot answer the question, so its negation fails as well");
			}

			[Test]
			public async Task WhenSubjectIsNotUnc_ShouldFail()
			{
				Uri subject = new("https://www.awexpect.com");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotUnc());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is a UNC path,
					             but it was https://www.awexpect.com/
					             """);
			}

			[Test]
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
