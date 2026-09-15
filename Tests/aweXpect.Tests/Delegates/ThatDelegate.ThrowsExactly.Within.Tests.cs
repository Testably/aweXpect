namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class ThrowsExactly
	{
		public sealed class Within
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task ShouldSupportChainedConstraints()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).ThrowsExactly<ArgumentException>().Within(5.Seconds()).WithMessage("foo");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an ArgumentException within 0:05 with Message equal to "foo",
						             but it did not throw any exception
						             """);
				}

				[Theory]
				[AutoData]
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

				[Fact]
				public async Task WhenDurationIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).ThrowsExactly<CustomException>().Within(-1.Milliseconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("duration").And
						.WithMessage("The duration must not be negative").AsPrefix();
				}

				[Fact]
				public async Task WhenExactExceptionTypeIsThrownInTime_ShouldSucceed()
				{
					Exception exception = new CustomException();
					Action action = () => throw exception;

					async Task<CustomException> Act()
						=> await That(action).ThrowsExactly<CustomException>().Within(5.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Theory]
				[AutoData]
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

				[Fact]
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

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).ThrowsExactly<CustomException>().Within(0.Seconds());

					await That(Act).ThrowsExactly<XunitException>()
						.WithMessage("""
						             Expected that subject
						             throws exactly a ThatDelegate.CustomException within 0:00,
						             but it was <null>
						             """);
				}

				[Theory]
				[AutoData]
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

			public sealed class TypeTests
			{
				[Fact]
				public async Task ShouldSupportChainedConstraints()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).ThrowsExactly(typeof(ArgumentException)).Within(5.Seconds())
							.WithMessage("foo");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws exactly an ArgumentException within 0:05 with Message equal to "foo",
						             but it did not throw any exception
						             """);
				}

				[Theory]
				[AutoData]
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

				[Fact]
				public async Task WhenDurationIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).ThrowsExactly(typeof(CustomException)).Within(-1.Milliseconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("duration").And
						.WithMessage("The duration must not be negative").AsPrefix();
				}

				[Fact]
				public async Task WhenExactExceptionTypeIsThrownInTime_ShouldSucceed()
				{
					Exception exception = new CustomException();
					Action action = () => throw exception;

					async Task<Exception> Act()
						=> await That(action).ThrowsExactly(typeof(CustomException)).Within(5.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Fact]
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

				[Fact]
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

				[Fact]
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

				[Theory]
				[AutoData]
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

				[Fact]
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

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).ThrowsExactly(typeof(CustomException)).Within(0.Seconds());

					await That(Act).ThrowsExactly<XunitException>()
						.WithMessage("""
						             Expected that subject
						             throws exactly a ThatDelegate.CustomException within 0:00,
						             but it was <null>
						             """);
				}

				[Theory]
				[AutoData]
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
		}
	}
}
