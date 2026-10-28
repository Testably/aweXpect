using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasHeader
	{
		public sealed class WhoseValueTests
		{
			[Test]
			public async Task WhenHeaderDoesNotExist_ShouldFail()
			{
				string name = "x-my-header";
				string value = "some header";
				string otherKey = "x-some-other-key";
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(otherKey).WhoseValue(v => v.IsEqualTo(value));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-some-other-key" header whose value is equal to "some header",
					             but it did not contain the expected header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenHeaderExistsAndValueDoesNotSatisfyTheExpectations_ShouldFail()
			{
				string name = "x-my-header";
				string value = "some header";
				string expectedValue = "some other header";
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(name).WhoseValue(v => v.IsEqualTo(expectedValue));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose value is equal to "some other header",
					             but the value was "some header", which differs at index 5:
					                     ↓ (actual)
					               "some header"
					               "some other header"
					                     ↑ (expected)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenHeaderExistsAndValueIsOfType_ShouldSucceed()
			{
				string name = "x-my-header";
				string value = "some header";
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(name)
						.WhoseValue(v => v.Is<string>().Whose(s => s.Length, l => l.IsEqualTo(11)));

				await That(Act).DoesNotThrow()
					.Because("the nested expectations offer the type checks of every subject");
			}

			[Test]
			public async Task WhenHeaderExistsAndValueSatisfiesTheExpectations_ShouldSucceed()
			{
				string name = "x-my-header";
				string value = "some header";
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(name).WhoseValue(v => v.IsEqualTo(value));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderHasMultipleValues_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeaders("x-genre", "rock", "jazz");

				async Task Act()
					=> await That(subject).HasHeader("x-genre").WhoseValue(v => v.IsNotEqualTo("rock"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-genre" header whose value is not equal to "rock",
					             but it had 2 header values ["rock", "jazz"]

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-genre: rock
					                 x-genre: jazz
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader("x-my-key").WhoseValue(v => v.IsEmpty());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-key" header whose value is empty,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedWhoseValueTests
		{
			[Test]
			public async Task WhenHeaderHasMultipleValues_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeaders("x-genre", "rock", "jazz");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasHeader("x-genre").WhoseValue(v => v.IsEqualTo("rock")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an "x-genre" header whose value is equal to "rock",
					             but it did contain the "x-genre" header: ["rock", "jazz"] and it had 2 header values ["rock", "jazz"]

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-genre: rock
					                 x-genre: jazz
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """)
					.Because("a header with several values has no single value to check, so it fails the expectation and its negation alike");
			}
		}
	}
}
