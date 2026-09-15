namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithMessage
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
							.WithMessage("foo").IgnoringCase();

					await That(Act).DoesNotThrow();
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
							.WithMessage("foo*").AsWildcard();

					await That(Act).DoesNotThrow();
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
							.WithMessage("foo");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message equal to "foo",
						             but it was "FOO" which differs at index 0:
						                ↓ (actual)
						               "FOO"
						               "foo"
						                ↑ (expected)

						             Message:
						             FOO
						             """);
				}

				[Fact]
				public async Task ShouldIncludeExceptionType()
				{
					string message = "FOO";
					Exception exception = new CustomException(message);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws<CustomException>()
							.WithMessage("foo");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws a ThatDelegate.CustomException with Message equal to "foo",
						             but it was "FOO" which differs at index 0:
						                ↓ (actual)
						               "FOO"
						               "foo"
						                ↑ (expected)

						             Message:
						             FOO
						             """);
				}

				[Theory]
				[AutoData]
				public async Task WhenAwaited_ShouldReturnThrownException(string message)
				{
					Exception exception =
						new OuterException(message, new CustomException());
					void Delegate() => throw exception;

					Exception result = await That(Delegate)
						.Throws().WithMessage(message);

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenMessagesAreDifferent_ShouldFail()
				{
					string actual = "actual text";
					string expected = "expected other text";
					Action action = () => throw new CustomException(actual);

					async Task Act()
						=> await That(action).Throws().WithMessage(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with Message equal to "expected other text",
						             but it was "actual text" which differs at index 0:
						                ↓ (actual)
						               "actual text"
						               "expected other text"
						                ↑ (expected)

						             Message:
						             actual text
						             """);
				}
			}
		}
	}
}
