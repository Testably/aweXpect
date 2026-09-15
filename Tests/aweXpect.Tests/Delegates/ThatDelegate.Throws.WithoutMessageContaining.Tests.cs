namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithoutMessageContaining
		{
			public sealed class Tests
			{
				[Fact]
				public async Task CanCompareCaseInsensitive()
				{
					string message = "_FOO_BAR";
					Exception exception =
						new OuterException(message, new CustomException());
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.WithoutMessageContaining("foo").IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message not containing matching "foo" ignoring case,
						             but it was "_FOO_BAR"

						             Message:
						             _FOO_BAR
						             """);
				}

				[Fact]
				public async Task CanUseWildcardCheck()
				{
					string message = "_foo-BAR";
					Exception exception =
						new OuterException(message, new CustomException());
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.WithoutMessageContaining("f?o-");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message not containing matching "f?o-",
						             but it was "_foo-BAR"

						             Message:
						             _foo-BAR
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
							.WithoutMessageContaining("foo");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldIgnorePrecedingText()
				{
					string message = "some text before foo";
					Exception exception =
						new OuterException(message, new CustomException());
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.WithoutMessageContaining("foo");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message not containing matching "foo",
						             but it was "some text before foo"

						             Message:
						             some text before foo
						             """);
				}

				[Fact]
				public async Task ShouldIgnoreSucceedingText()
				{
					string message = "foo and some other text";
					Exception exception =
						new OuterException(message, new CustomException());
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.WithoutMessageContaining("foo");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message not containing matching "foo",
						             but it was "foo and some other text"

						             Message:
						             foo and some other text
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
							.WithoutMessageContaining("foo");

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
						.Throws().WithoutMessageContaining("foo");

					await That(result).IsSameAs(exception);
				}

				[Theory]
				[AutoData]
				public async Task WhenExpectedIsNull_ShouldFail(string message)
				{
					Exception exception = new(message);
					void Delegate() => throw exception;

					async Task Act()
						=> await That(Delegate).Throws()
							.WithoutMessageContaining(null);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that Delegate
						              throws an exception with Message not containing matching <null>,
						              but it was "{message}"

						              Message:
						              {message}
						              """);
				}

				[Fact]
				public async Task WhenMessagesAreDifferent_ShouldFail()
				{
					string actual = "expected actual text";
					string expected = "expected other text";
					Action action = () => throw new CustomException(actual);

					async Task Act()
						=> await That(action).Throws().WithoutMessageContaining(expected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
