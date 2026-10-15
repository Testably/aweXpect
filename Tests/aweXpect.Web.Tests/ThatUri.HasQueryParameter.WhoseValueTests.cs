namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed partial class HasQueryParameter
	{
		public sealed class WhoseValueTests
		{
			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WhoseValue(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenQueryParameterDoesNotExist_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WhoseValue(v => v.IsEqualTo("jazz"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "genre" whose value is equal to "jazz",
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Fact]
			public async Task WhenValueDoesNotSatisfyTheExpectations_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WhoseValue(v => v.StartsWith("ja"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "genre" whose value starts with "ja",
					             but the value was "rock", which differs at index 0:
					                ↓ (actual)
					               "rock"
					               "ja"
					                ↑ (expected prefix)
					             """);
			}

			[Fact]
			public async Task WhenValueSatisfiesTheExpectations_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=jazz");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WhoseValue(v => v.StartsWith("ja"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WhoseValue(v => v.IsEmpty());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page" whose value is empty,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?page=2", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WhoseValue(v => v.IsEqualTo("2"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page" whose value is equal to "2",
					             but it was relative
					             """);
			}
		}

		public sealed class NegatedWhoseValueTests
		{
			[Fact]
			public async Task WhenQueryParameterDoesNotExist_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("genre").WhoseValue(v => v.IsEqualTo("jazz")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?page=2", UriKind.Relative);

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("page").WhoseValue(v => v.IsEqualTo("2")));

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it was relative").AsWildcard()
					.Because("a relative URI has no query to inspect, so it fails the expectation and its negation alike");
			}

			[Fact]
			public async Task WhenValueDoesNotSatisfyTheExpectations_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=rock");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("genre").WhoseValue(v => v.IsEqualTo("jazz")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueSatisfiesTheExpectations_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=jazz");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("genre").WhoseValue(v => v.IsEqualTo("jazz")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "genre" whose value is equal to "jazz",
					             but it was https://awexpect.com/tracks?genre=jazz
					             """);
			}
		}
	}
}
