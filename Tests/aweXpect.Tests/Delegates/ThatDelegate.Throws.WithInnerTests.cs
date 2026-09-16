namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class WithInner
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenAwaited_WithoutExpectations_ShouldReturnThrownException()
				{
					Exception exception = new OuterException(innerException: new CustomException());
					void Delegate() => throw exception;

					Exception result = await That(Delegate)
						.Throws().WithInner<CustomException>();

					await That(result).IsSameAs(exception);
				}

				[Theory]
				[AutoData]
				public async Task WhenInnerExceptionHasSuperType_ShouldFail(string message)
				{
					Action action = ()
						=> throw new OuterException(innerException: new CustomException(message));

					async Task Act()
						=> await That(action).Throws().WithInner<SubCustomException>();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that action
						              throws an exception with an inner ThatDelegate.SubCustomException,
						              but it was a ThatDelegate.CustomException:
						                {message}
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenInnerExceptionHasWrongType_ShouldFail(string message)
				{
					Action action = ()
						=> throw new OuterException(innerException: new OtherException(message));

					async Task Act()
						=> await That(action).Throws().WithInner<CustomException>();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that action
						              throws an exception with an inner ThatDelegate.CustomException,
						              but it was a ThatDelegate.OtherException:
						                {message}
						              """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotPresent_ShouldFail()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws().WithInner<Exception>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsPresent_ShouldSucceed()
				{
					Action action = () => throw new OuterException(innerException: new CustomException());

					async Task Act()
						=> await That(action).Throws().WithInner<CustomException>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionIsSubType_ShouldSucceed()
				{
					Action action = ()
						=> throw new OuterException(innerException: new SubCustomException());

					async Task Act()
						=> await That(action).Throws().WithInner<CustomException>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNoInnerExceptionIsPresent_ShouldFail()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws().WithInner<CustomException>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner ThatDelegate.CustomException,
						             but it was <null>
						             """);
				}
			}

			public sealed class GenericWithExpectationTests
			{
				[Fact]
				public async Task CustomException_WhenInnerExceptionDoesNotMatchCriteria_ShouldFail()
				{
					string message = "bar";
					Action action = ()
						=> throw new OuterException(innerException: new CustomException(message));

					async Task Act()
						=> await That(action).Throws().WithInner<CustomException>(x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner ThatDelegate.CustomException whose Message is equal to "foo",
						             but it was "bar" which differs at index 0:
						                ↓ (actual)
						               "bar"
						               "foo"
						                ↑ (expected)

						             Message:
						             bar
						             """);
				}

				[Fact]
				public async Task Exception_WhenInnerExceptionDoesNotMatchCriteria_ShouldFail()
				{
					string message = "bar";
					Action action = ()
						=> throw new OuterException(innerException: new CustomException(message));

					async Task Act()
						=> await That(action).Throws().WithInner<Exception>(x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception whose Message is equal to "foo",
						             but it was "bar" which differs at index 0:
						                ↓ (actual)
						               "bar"
						               "foo"
						                ↑ (expected)

						             Message:
						             bar
						             """);
				}

				[Theory]
				[AutoData]
				public async Task WhenAwaited_WithExpectations_ShouldReturnThrownException(
					string message)
				{
					Exception exception = new OuterException(innerException: new CustomException(message));
					void Delegate() => throw exception;

					Exception result = await That(Delegate)
						.Throws().WithInner<CustomException>(e => e.HasMessage(message));

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenExpectationsAreCombinedWithAnd_ShouldApplyAllOfThem()
				{
					Action action = ()
						=> throw new OuterException(innerException: new CustomException("bar"));

					async Task Act()
						=> await That(action).Throws()
							.WithInner<CustomException>(x => x.HasMessage("bar").And.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner ThatDelegate.CustomException whose Message is equal to "bar" and Message is equal to "foo",
						             but it was "bar" which differs at index 0:
						                ↓ (actual)
						               "bar"
						               "foo"
						                ↑ (expected)

						             Message:
						             bar
						             """)
						.Because("both expectations inspect the same Message, which is only appended once");
				}

				[Fact]
				public async Task WhenExpectationsAreCombinedWithOr_ShouldApplyEitherOfThem()
				{
					Action action = ()
						=> throw new OuterException(innerException: new CustomException("bar"));

					async Task Act()
						=> await That(action).Throws()
							.WithInner<CustomException>(x => x.HasMessage("foo").Or.HasMessage("bar"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectationsAreTypedAtTheInnerExceptionType_ShouldSucceed()
				{
					Action action = ()
						=> throw new OuterException(innerException: new CustomException("foo")
						{
							Value = "bar",
						});

					async Task Act()
						=> await That(action).Throws()
							.WithInner<CustomException>(x => x.Satisfies(e => e?.Value == "bar"));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionDoesNotMatchCriteria_ShouldFail()
				{
					string message = "bar";
					Action action = ()
						=> throw new OuterException(innerException: new CustomException(message));

					async Task Act()
						=> await That(action).Throws().WithInner<Exception>(x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception whose Message is equal to "foo",
						             but it was "bar" which differs at index 0:
						                ↓ (actual)
						               "bar"
						               "foo"
						                ↑ (expected)

						             Message:
						             bar
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionDoesNotMatchType_ShouldFail()
				{
					Action action = ()
						=> throw new OuterException(innerException: new CustomException("foo"));

					async Task Act()
						=> await That(action).Throws()
							.WithInner<MyException>(x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner MyException whose Message is equal to "foo",
						             but it was a ThatDelegate.CustomException:
						               foo
						             """);
				}

				[Fact]
				public async Task
					WhenInnerExceptionHasUnexpectedTypeAndExpectationsAreTypedAtTheInnerExceptionType_ShouldFail()
				{
					Action action = ()
						=> throw new OuterException(innerException: new OtherException("foo"));

					async Task Act()
						=> await That(action).Throws()
							.WithInner<CustomException>(x => x.Satisfies(e => e?.Value == "bar"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner ThatDelegate.CustomException whose satisfies e => e?.Value == "bar",
						             but it was a ThatDelegate.OtherException:
						               foo
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotPresent_ShouldFail()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws()
							.WithInner<Exception>(x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception whose Message is equal to "foo",
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenNoExceptionIsThrown_ShouldFail()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).Throws()
							.WithInner<CustomException>(x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner ThatDelegate.CustomException whose Message is equal to "foo",
						             but it did not throw any exception
						             """);
				}
			}

			public sealed class TypeTests
			{
				[Theory]
				[AutoData]
				public async Task WhenAwaited_WithExpectations_ShouldReturnThrownException(
					string message)
				{
					Exception exception = new OuterException(innerException: new CustomException(message));
					void Delegate() => throw exception;

					Exception result = await That(Delegate)
						.Throws().WithInner(typeof(CustomException),
							e => e.HasMessage(message));

					await That(result).IsSameAs(exception);
				}

				[Fact]
				public async Task WhenAwaited_WithoutExpectations_ShouldReturnThrownException()
				{
					Exception exception = new OuterException(innerException: new CustomException());
					void Delegate() => throw exception;

					Exception result = await That(Delegate)
						.Throws().WithInner(typeof(CustomException));

					await That(result).IsSameAs(exception);
				}

				[Theory]
				[AutoData]
				public async Task WhenInnerExceptionHasSuperType_ShouldFail(string message)
				{
					Action action = ()
						=> throw new OuterException(innerException: new CustomException(message));

					async Task Act()
						=> await That(action).Throws()
							.WithInner(typeof(SubCustomException));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that action
						              throws an exception with an inner ThatDelegate.SubCustomException,
						              but it was a ThatDelegate.CustomException:
						                {message}
						              """);
				}

				[Theory]
				[AutoData]
				public async Task WhenInnerExceptionHasWrongType_ShouldFail(string message)
				{
					Action action = ()
						=> throw new OuterException(innerException: new OtherException(message));

					async Task Act()
						=> await That(action).Throws().WithInner(typeof(CustomException));

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that action
						              throws an exception with an inner ThatDelegate.CustomException,
						              but it was a ThatDelegate.OtherException:
						                {message}
						              """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotPresent_ShouldFail()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws().WithInner(typeof(Exception));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsPresent_ShouldSucceed()
				{
					Action action = () => throw new OuterException(innerException: new CustomException());

					async Task Act()
						=> await That(action).Throws().WithInner(typeof(CustomException));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenInnerExceptionIsSubType_ShouldSucceed()
				{
					Action action = ()
						=> throw new OuterException(innerException: new SubCustomException());

					async Task Act()
						=> await That(action).Throws().WithInner(typeof(CustomException));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenNoInnerExceptionIsPresent_ShouldFail()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws().WithInner(typeof(CustomException));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner ThatDelegate.CustomException,
						             but it was <null>
						             """);
				}
			}

			public sealed class TypeWithExpectationTests
			{
				[Fact]
				public async Task CustomException_WhenInnerExceptionDoesNotMatchCriteria_ShouldFail()
				{
					string message = "bar";
					Action action = ()
						=> throw new OuterException(innerException: new CustomException(message));

					async Task Act()
						=> await That(action).Throws()
							.WithInner(typeof(CustomException), x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner ThatDelegate.CustomException whose Message is equal to "foo",
						             but it was "bar" which differs at index 0:
						                ↓ (actual)
						               "bar"
						               "foo"
						                ↑ (expected)

						             Message:
						             bar
						             """);
				}

				[Fact]
				public async Task Exception_WhenInnerExceptionDoesNotMatchCriteria_ShouldFail()
				{
					string message = "bar";
					Action action = ()
						=> throw new OuterException(innerException: new CustomException(message));

					async Task Act()
						=> await That(action).Throws()
							.WithInner(typeof(Exception), x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception whose Message is equal to "foo",
						             but it was "bar" which differs at index 0:
						                ↓ (actual)
						               "bar"
						               "foo"
						                ↑ (expected)

						             Message:
						             bar
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionDoesNotMatchCriteria_ShouldFail()
				{
					string message = "bar";
					Action action = ()
						=> throw new OuterException(innerException: new CustomException(message));

					async Task Act()
						=> await That(action).Throws()
							.WithInner(typeof(Exception), x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception whose Message is equal to "foo",
						             but it was "bar" which differs at index 0:
						                ↓ (actual)
						               "bar"
						               "foo"
						                ↑ (expected)

						             Message:
						             bar
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionDoesNotMatchType_ShouldFail()
				{
					Action action = ()
						=> throw new OuterException(innerException: new CustomException("foo"));

					async Task Act()
						=> await That(action).Throws()
							.WithInner(typeof(MyException), x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner MyException whose Message is equal to "foo",
						             but it was a ThatDelegate.CustomException:
						               foo

						             Message:
						             foo
						             """);
				}

				[Fact]
				public async Task WhenInnerExceptionIsNotPresent_ShouldFail()
				{
					Action action = () => throw new OuterException();

					async Task Act()
						=> await That(action).Throws()
							.WithInner(typeof(Exception), x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner exception whose Message is equal to "foo",
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenNoExceptionIsThrown_ShouldFail()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).Throws()
							.WithInner(typeof(CustomException), x => x.HasMessage("foo"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that action
						             throws an exception with an inner ThatDelegate.CustomException whose Message is equal to "foo",
						             but it did not throw any exception
						             """);
				}
			}
		}
	}
}
