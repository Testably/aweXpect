using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasProblemDetailsContent
	{
		public sealed class WithInstanceTests
		{
			[Test]
			public async Task WhenInstanceIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithInstance(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenInstanceIsNullAfterStatus_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(400).WithInstance(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenInstanceIsSpecifiedTwice_ShouldUseTheLastInstance()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "instance": "baz"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithInstance("foo").WithInstance("bar");

				await That(Act).Throws<FailException>()
					.WithMessage(
						"*has a ProblemDetails content with any type and instance \"bar\",*but it had instance \"baz\"*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenInstanceIsSpecifiedTwiceAroundStatus_ShouldUseTheLastInstanceInItsPlace()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "instance": "baz",
					               "status": 500
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithInstance("foo").WithStatus(500)
						.WithInstance("bar");

				await That(Act).Throws<FailException>()
					.WithMessage(
						"*has a ProblemDetails content with any type, instance \"bar\" and status 500,*but it had instance \"baz\"*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenInstanceDiffersInCase_WithIgnoringCase_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "instance": "bar"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithInstance("BAR").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("foo", "bar")]
			[Arguments("foo", "FOO")]
			public async Task WhenInstanceDoesNotMatch_ShouldFail(string actualInstance, string expectedInstance)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent($$"""
					               {
					                 "type": "my-type",
					                 "instance": "{{actualInstance}}"
					               }
					               """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithInstance(expectedInstance);

				await That(Act).Throws<FailException>()
					.WithMessage($$"""
					               Expected that subject
					               has a ProblemDetails content with any type and instance "{{expectedInstance}}",
					               but it had instance "{{actualInstance}}", which differs at index 0:
					                  ↓ (actual)
					                 "{{actualInstance}}"
					                 "{{expectedInstance}}"
					                  ↑ (expected)

					               HTTP-Response:
					                 200 OK HTTP/1.1
					                   Content-Type: text/plain; charset=utf-8
					                   Content-Length: *
					                 {
					                   "type": "my-type",
					                   "instance": "{{actualInstance}}"
					                 }
					               """).AsWildcard();
			}

			[Test]
			public async Task WhenInstanceMatches_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "instance": "foo",
					               "status": 200,
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithStatus(200).WithInstance("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenInstanceDoesNotStartWithPrefix_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "instance": "/albums/42"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithInstance("/tracks/").AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a ProblemDetails content with any type and instance starting with "/tracks/",
					             but it had instance "/albums/42", which differs at index 1:
					                 ↓ (actual)
					               "/albums/42"
					               "/tracks/"
					                 ↑ (expected prefix)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: *
					               {
					                 "type": "my-type",
					                 "instance": "/albums/42"
					               }
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenInstanceStartsWithPrefix_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("""
					             {
					               "type": "my-type",
					               "instance": "/albums/42"
					             }
					             """);

				async Task Act()
					=> await That(subject).HasProblemDetailsContent().WithInstance("/albums/").AsPrefix();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
