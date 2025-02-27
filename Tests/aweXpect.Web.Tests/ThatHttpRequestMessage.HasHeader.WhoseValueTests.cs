using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed partial class HasHeader
	{
		public sealed class WhoseValueTests
		{
			[Fact]
			public async Task WhenHeaderDoesNotExist_ShouldFail()
			{
				string name = "x-my-header";
				string value = "some header";
				string otherKey = "x-some-other-key";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(otherKey).WhoseValue(v => v.IsEqualTo(value));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a `x-some-other-key` header whose value is equal to "some header",
					             but it did not contain the expected header

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: some header
					             """);
			}

			[Fact]
			public async Task WhenHeaderExistsAndValueDoesNotSatisfyTheExpectations_ShouldFail()
			{
				string name = "x-my-header";
				string value = "some header";
				string expectedValue = "some other header";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(name).WhoseValue(v => v.IsEqualTo(expectedValue));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a `x-my-header` header whose value is equal to "some other header",
					             but the value was "some header" which differs at index 5:
					                     ↓ (actual)
					               "some header"
					               "some other header"
					                     ↑ (expected)
					             """);
			}

			[Fact]
			public async Task WhenHeaderExistsAndValueSatisfiesTheExpectations_ShouldSucceed()
			{
				string name = "x-my-header";
				string value = "some header";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(name).WhoseValue(v => v.IsEqualTo(value));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader("x-my-key").WhoseValue(v => v.IsEmpty());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a `x-my-key` header whose value is empty,
					             but it was <null>
					             """);
			}
		}
	}
}
