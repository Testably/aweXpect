namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ThrowsExactly
	{
		public sealed class Within
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task ShouldSupportChainedConstraints()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).ThrowsExactly<ArgumentException>().Within(5.Seconds()).WithMessage("foo");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an ArgumentException within 0:05 with message equal to "foo",
						             but it did not throw any exception
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenAwaited_ShouldReturnThrownException(string value)
				{
					Exception exception = new CustomException
					{
						Value = value,
					};
					Action action = () => throw exception;

					CustomException result =
						await That(action).ThrowsExactly<CustomException>().Within(5.Seconds());

					await That(result.Value).IsEqualTo(value);
					await That(result).IsSameAs(exception);
				}

				[Test]
				public async Task WhenDurationIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).ThrowsExactly<CustomException>().Within(-5.Milliseconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("timeout").And
						.WithMessage("The timeout must not be negative").AsPrefix();
				}

				[Test]
				public async Task WhenExactExceptionTypeIsThrownInTime_ShouldSucceed()
				{
					Exception exception = new CustomException();
					Action action = () => throw exception;

					async Task<CustomException> Act()
						=> await That(action).ThrowsExactly<CustomException>().Within(5.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExactExceptionTypeIsThrownTooLate_ShouldFail()
				{
					Exception exception = new CustomException();
					Action action = () =>
					{
						Task.Delay(50.Milliseconds()).Wait();
						throw exception;
					};

					async Task<CustomException> Act()
						=> await That(action).ThrowsExactly<CustomException>().Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly a ThatDelegate.CustomException within 0:00.005,
						             but it took *
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenNoExceptionIsThrownAndExecutionTimeIsTooLarge_ShouldFail()
				{
					Action action = () =>
					{
						Task.Delay(50.Milliseconds()).Wait();
					};

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly<Exception>().Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an Exception within 0:00.005,
						             but it took *
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenNoExceptionIsThrownInTime_ShouldFail()
				{
					Action action = () => { };

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly<Exception>().Within(5.Seconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an Exception within 0:05,
						             but it did not throw any exception
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenOtherExceptionIsThrown_ShouldFail(string message)
				{
					Exception exception = new OtherException(message);
					Action action = () => throw exception;

					async Task<CustomException> Act()
						=> await That(action).ThrowsExactly<CustomException>().Within(5.Seconds());

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that action
						              throws exactly a ThatDelegate.CustomException within 0:05,
						              but it did throw a ThatDelegate.OtherException:
						                {message}
						              """);
				}

				[Test]
				public async Task WhenSubCustomExceptionIsThrown_ShouldFail()
				{
					Exception exception = new SubCustomException();
					Action action = () => throw exception;

					async Task<CustomException> Act()
						=> await That(action).ThrowsExactly<CustomException>().Within(5.Seconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly a ThatDelegate.CustomException within 0:05,
						             but it did throw a ThatDelegate.SubCustomException:
						               WhenSubCustomExceptionIsThrown_ShouldFail
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).ThrowsExactly<CustomException>().Within(0.Seconds());

					await That(Act).ThrowsExactly<FailException>()
						.WithMessage("""
						             Expected that subject
						             throws exactly a ThatDelegate.CustomException within 0:00,
						             but it was <null>
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenSupertypeExceptionIsThrown_ShouldFail(string message)
				{
					Exception exception = new CustomException(message);
					Action action = () => throw exception;

					async Task<SubCustomException> Act()
						=> await That(action).ThrowsExactly<SubCustomException>().Within(6.Seconds());

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that action
						              throws exactly a ThatDelegate.SubCustomException within 0:06,
						              but it did throw a ThatDelegate.CustomException:
						                {message}
						              """);
				}
			}

#pragma warning disable CA2263 // these tests deliberately cover the Type overloads
			public sealed class TypeTests
			{
				[Test]
				public async Task ShouldSupportChainedConstraints()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).ThrowsExactly(typeof(ArgumentException)).Within(5.Seconds())
							.WithMessage("foo");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an ArgumentException within 0:05 with message equal to "foo",
						             but it did not throw any exception
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenAwaited_ShouldReturnThrownException(string value)
				{
					Exception exception = new CustomException
					{
						Value = value,
					};
					Action action = () => throw exception;

					Exception result =
						await That(action).ThrowsExactly(typeof(CustomException)).Within(5.Seconds());

					await That(result).IsSameAs(exception);
				}

				[Test]
				public async Task WhenDurationIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).ThrowsExactly(typeof(CustomException)).Within(-5.Milliseconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("timeout").And
						.WithMessage("The timeout must not be negative").AsPrefix();
				}

				[Test]
				public async Task WhenExactExceptionTypeIsThrownInTime_ShouldSucceed()
				{
					Exception exception = new CustomException();
					Action action = () => throw exception;

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly(typeof(CustomException)).Within(5.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExactExceptionTypeIsThrownTooLate_ShouldFail()
				{
					Exception exception = new CustomException();
					Action action = () =>
					{
						Task.Delay(50.Milliseconds()).Wait();
						throw exception;
					};

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly(typeof(CustomException)).Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly a ThatDelegate.CustomException within 0:00.005,
						             but it took *
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenNoExceptionIsThrownAndExecutionTimeIsTooLarge_ShouldFail()
				{
					Action action = () =>
					{
						Task.Delay(50.Milliseconds()).Wait();
					};

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly(typeof(Exception)).Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an Exception within 0:00.005,
						             but it took *
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenNoExceptionIsThrownInTime_ShouldFail()
				{
					Action action = () => { };

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly(typeof(Exception)).Within(5.Seconds()).Because("it should");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an Exception within 0:05, because it should,
						             but it did not throw any exception
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenOtherExceptionIsThrown_ShouldFail(string message)
				{
					Exception exception = new OtherException(message);
					Action action = () => throw exception;

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly(typeof(CustomException)).Within(5.Seconds());

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that action
						              throws exactly a ThatDelegate.CustomException within 0:05,
						              but it did throw a ThatDelegate.OtherException:
						                {message}
						              """);
				}

				[Test]
				public async Task WhenSubCustomExceptionIsThrown_ShouldFail()
				{
					Exception exception = new SubCustomException();
					Action action = () => throw exception;

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly(typeof(CustomException)).Within(5.Seconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly a ThatDelegate.CustomException within 0:05,
						             but it did throw a ThatDelegate.SubCustomException:
						               WhenSubCustomExceptionIsThrown_ShouldFail
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).ThrowsExactly(typeof(CustomException)).Within(0.Seconds());

					await That(Act).ThrowsExactly<FailException>()
						.WithMessage("""
						             Expected that subject
						             throws exactly a ThatDelegate.CustomException within 0:00,
						             but it was <null>
						             """);
				}

				[Test]
				[AutoArguments]
				public async Task WhenSupertypeExceptionIsThrown_ShouldFail(string message)
				{
					Exception exception = new CustomException(message);
					Action action = () => throw exception;

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly(typeof(SubCustomException)).Within(6.Seconds());

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that action
						              throws exactly a ThatDelegate.SubCustomException within 0:06,
						              but it did throw a ThatDelegate.CustomException:
						                {message}
						              """);
				}
			}
#pragma warning restore CA2263
		}
	}
}
