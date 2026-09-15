namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithoutMessage
		{
			public sealed class Tests
			{
				[Fact]
				public async Task CanCompareCaseInsensitive()
				{
					string message = "FOO";
					Exception exception =
						new OuterException(message, new CustomException());
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.WithoutMessage("foo").IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message not equal to "foo" ignoring case,
						             but it was "FOO"

						             Message:
						             FOO
						             """);
				}

				[Fact]
				public async Task CanUseWildcardCheck()
				{
					string message = "foo-bar";
					Exception exception =
						new OuterException(message, new CustomException());
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.WithoutMessage("foo*").AsWildcard();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message not matching "foo*",
						             but it was "foo-bar"

						             Message:
						             foo-bar
						             """);
				}

				[Fact]
				public async Task ShouldCompareCaseSensitive()
				{
					string message = "FOO";
					Exception exception =
						new OuterException(message, new CustomException());
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.WithoutMessage("foo");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldIncludeExceptionType()
				{
					string message = "FOO";
					Exception exception = new CustomException(message);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<CustomException>()
							.WithoutMessage("foo");

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[AutoData]
				public async Task WhenAwaited_ShouldReturnThrownException(string message)
				{
					Exception exception =
						new OuterException(message, new CustomException());
					void Delegate() => throw exception;

					Exception result = await That(Delegate)
						.Throws().WithoutMessage("foo");

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenMessagesAreDifferent_ShouldFail()
				{
					string actual = "actual text";
					string expected = "expected other text";
					Action action = () => throw new CustomException(actual);

					async Task Act()
						=> await That(action).Throws().WithoutMessage(expected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
