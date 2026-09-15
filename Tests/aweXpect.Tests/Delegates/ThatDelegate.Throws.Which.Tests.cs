namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class Which
		{
			public sealed class Tests
			{
				[Theory]
				[AutoData]
				public async Task ShouldGiveAccessToThrowsException(int hResult)
				{
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.Which.IsSameAs(exception);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[AutoData]
				public async Task ShouldIncludeWhichInErrorMessage(int hResult)
				{
					int expectedHResult = hResult + 1;
					Exception exception = new HResultException(hResult);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.Which.Whose(e => e.HResult, h => h.IsEqualTo(expectedHResult));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an exception which whose .HResult is equal to {expectedHResult},
						              but .HResult was {hResult} which differs by -1
						              """);
				}
			}

			public sealed class GenericTests
			{
				[Fact]
				public async Task ShouldGiveAccessToThrowsException()
				{
					MyException exception = new();
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<MyException>()
							.Which.IsSameAs(exception);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldIncludeWhichInErrorMessage()
				{
					MyException exception = new();
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<MyException>()
							.Which.Whose(h => h.Message, r => r.IsEqualTo("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws a MyException which whose .Message is equal to "foo",
						             but .Message was "ShouldIncludeWhichInErrorMessa…" which differs at index 0:
						                ↓ (actual)
						               "ShouldIncludeWhichInErrorMessage"
						               "foo"
						                ↑ (expected)

						             Actual:
						             ShouldIncludeWhichInErrorMessage

						             Expected:
						             foo
						             """);
				}

				[Fact]
				public async Task ShouldSupportWhichSatisfies()
				{
					MyException exception = new();
					void Act() => throw exception;
					await That(Act)
						.Throws<MyException>().Which
						.Satisfies(x => x.Message == nameof(ShouldSupportWhichSatisfies));
				}
			}
		}
	}
}
