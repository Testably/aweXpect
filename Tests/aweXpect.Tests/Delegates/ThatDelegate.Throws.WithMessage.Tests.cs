namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithMessage
		{
			public sealed class ContainingTests
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
							.WithMessage().Containing("foo").IgnoringCase();

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
							.WithMessage().Containing("foo");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message containing "foo",
						             but it was "FOO"

						             Message:
						             FOO
						             """);
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
							.WithMessage().Containing("foo");

					await That(Act).DoesNotThrow();
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
							.WithMessage().Containing("foo");

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
							.WithMessage().Containing("foo");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws a ThatDelegate.CustomException with Message containing "foo",
						             but it was "FOO"

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
						.Throws().WithMessage().Containing(message);

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenMessagesAreDifferent_ShouldFail()
				{
					string actual = "expected actual text";
					string expected = "expected other text";
					Action action = () => throw new CustomException(actual);

					async Task Act()
						=> await That(action).Throws().WithMessage().Containing(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with Message containing "expected other text",
						             but it was "expected actual text"

						             Message:
						             expected actual text
						             """);
				}
			}

			public sealed class EqualToTests
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
							.WithMessage().EqualTo("foo").IgnoringCase();

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
							.WithMessage().EqualTo("foo*").AsWildcard();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenMessagesAreDifferent_ShouldFail()
				{
					string actual = "actual text";
					string expected = "expected other text";
					Action action = () => throw new CustomException(actual);

					async Task Act()
						=> await That(action).Throws().WithMessage().EqualTo(expected);

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
						             """)
						.Because("the continuation renders exactly like the WithMessage(expected) shorthand");
				}
			}

			public sealed class NotContainingTests
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
							.WithMessage().NotContaining("foo").IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message not containing "foo" ignoring case,
						             but it was "_FOO_BAR"

						             Message:
						             _FOO_BAR
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
							.WithMessage().NotContaining("foo");

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
							.WithMessage().NotContaining("foo");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that Delegate
						             throws an exception with Message not containing "foo",
						             but it was "some text before foo"

						             Message:
						             some text before foo
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
							.WithMessage().NotContaining("foo");

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
						.Throws().WithMessage().NotContaining("foo");

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenMessagesAreDifferent_ShouldSucceed()
				{
					string actual = "expected actual text";
					string expected = "expected other text";
					Action action = () => throw new CustomException(actual);

					async Task Act()
						=> await That(action).Throws().WithMessage().NotContaining(expected);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NotEqualToTests
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
							.WithMessage().NotEqualTo("foo").IgnoringCase();

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
							.WithMessage().NotEqualTo("foo*").AsWildcard();

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
							.WithMessage().NotEqualTo("foo");

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
						.Throws().WithMessage().NotEqualTo("foo");

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenMessagesAreDifferent_ShouldSucceed()
				{
					string actual = "actual text";
					string expected = "expected other text";
					Action action = () => throw new CustomException(actual);

					async Task Act()
						=> await That(action).Throws().WithMessage().NotEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}

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
