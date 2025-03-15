using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed partial class HasHeader
	{
		public sealed class WhoseValuesTests
		{
			[Fact]
			public async Task WhenHeaderDoesNotExist_ShouldFail()
			{
				string name = "x-my-header";
				string[] value = ["foo", "bar", "baz",];
				string otherKey = "x-some-other-key";
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders(name, value);

				async Task Act()
					=> await That(subject).HasHeader(otherKey).WhoseValues(v => v.HasCount().EqualTo(3));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a `x-some-other-key` header whose values has exactly 3 items,
					             but it did not contain the expected header

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: foo
					                 x-my-header: bar
					                 x-my-header: baz
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
					=> await That(subject).HasHeader(name).WhoseValues(v => v.Contains(expectedValue));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a `x-my-header` header whose values contains "some other header" at least once,
					             but the values contained it 0 times in [
					               "some header"
					             ]

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: some header
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
					=> await That(subject).HasHeader(name).WhoseValues(v => v.Contains(value));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader("x-my-key").WhoseValues(v => v.IsEmpty());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a `x-my-key` header whose values are empty,
					             but it was <null>
					             """);
			}
		}
	}
}
