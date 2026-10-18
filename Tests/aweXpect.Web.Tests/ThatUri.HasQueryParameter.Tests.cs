namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed partial class HasQueryParameter
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				Uri[] subjects = [new Uri("https://awexpect.com/"),];

				async Task Act()
					=> await That(new { Uris = subjects, })
						.Whose(x => x.Uris, items => items.All()
							.ComplyWith(item => item.HasQueryParameter("page")));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Uris have a query parameter "page" for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenNameIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter(null!);

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
					=> await That(subject).HasQueryParameter(name);

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("name").And
					.WithMessage("The 'name' cannot be empty or whitespace.").AsPrefix()
					.Because("query parameters without a name are ignored, so they could never be found");
			}

			[Test]
			public async Task WhenQueryParameterDiffersInCase_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?Page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("page");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page",
					             but it was https://awexpect.com/tracks?Page=2
					             """)
					.Because("query parameter names are case-sensitive");
			}

			[Test]
			public async Task WhenQueryParameterDoesNotExist_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("token");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "token",
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Test]
			[Arguments("https://awexpect.com/tracks?page=2", "page")]
			[Arguments("https://awexpect.com/tracks?genre=jazz&page=2", "page")]
			[Arguments("https://awexpect.com/tracks?explicit", "explicit")]
			[Arguments("https://awexpect.com/tracks?page=", "page")]
			[Arguments("https://awexpect.com/tracks?my%20key=1", "my key")]
			[Arguments("https://awexpect.com/tracks?my+key=1", "my key")]
			public async Task WhenQueryParameterExists_ShouldSucceed(string uri, string name)
			{
				Uri subject = new(uri);

				async Task Act()
					=> await That(subject).HasQueryParameter(name);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("page");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?page=2", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasQueryParameter("page");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page",
					             but it was relative
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenQueryParameterDoesNotExist_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("token"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenQueryParameterExists_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("page"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "page",
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("page"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "page",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("page"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "page",
					             but it was relative
					             """)
					.Because("a relative URI has no query to inspect, so it fails the expectation and its negation alike");
			}
		}
	}
}
