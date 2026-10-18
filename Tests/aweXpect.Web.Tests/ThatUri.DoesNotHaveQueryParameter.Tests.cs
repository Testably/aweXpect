namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed class DoesNotHaveQueryParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				Uri[] subjects = [new Uri("https://awexpect.com/?token=abc"),];

				async Task Act()
					=> await That(new { Uris = subjects, })
						.Whose(x => x.Uris, items => items.All()
							.ComplyWith(item => item.DoesNotHaveQueryParameter("token")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Uris do not have a query parameter "token" for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveQueryParameter(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("name").And
					.WithMessage("The 'name' cannot be null.").AsPrefix();
			}

			[Test]
			[Arguments("")]
			[Arguments(" ")]
			[Arguments("\t")]
			public async Task WhenNameIsEmptyOrWhitespace_ShouldThrowArgumentException(string name)
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveQueryParameter(name);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("name").And
					.WithMessage("The 'name' cannot be empty or whitespace.").AsPrefix()
					.Because("query parameters without a name are ignored, so the expectation could never fail");
			}

			[Test]
			public async Task WhenQueryParameterDoesNotExist_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotHaveQueryParameter("token");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenQueryParameterExists_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2&token=abc");

				async Task Act()
					=> await That(subject).DoesNotHaveQueryParameter("token");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "token",
					             but it was https://awexpect.com/tracks?page=2&token=abc
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveQueryParameter("token");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "token",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotHaveQueryParameter("token");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "token",
					             but it was relative
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenQueryParameterDoesNotExist_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveQueryParameter("token"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "token",
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Test]
			public async Task WhenQueryParameterExists_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?token=abc");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveQueryParameter("token"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotHaveQueryParameter("token"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "token",
					             but it was relative
					             """);
			}
		}
	}
}
