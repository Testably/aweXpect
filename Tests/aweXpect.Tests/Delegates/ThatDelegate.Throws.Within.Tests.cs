using System.Linq;

namespace aweXpect.Tests;

public sealed partial class ThatDelegate
{
	public sealed partial class Throws
	{
		public sealed class Within
		{
			public sealed class Tests
			{
				[Test]
				public async Task ShouldSupportChainedConstraints()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).Throws().Within(5.Seconds()).WithMessage("foo");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:05 with message equal to "foo",
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
						await That(action).Throws().Within(5.Seconds());

					await That(result).IsSameAs(exception);
				}

				[Test]
				public async Task WhenContinuingWithAndOrOr_ShouldNotBeOffered()
				{
					Type[] continuations = typeof(ThatDelegateThrows).GetMethods()
						.SelectMany(method => new[]
						{
							method.ReturnType.GetProperty("And"), method.ReturnType.GetProperty("Or"),
						})
						.Where(property => property is not null)
						.Select(property => property!.PropertyType)
						.ToArray();

					await That(continuations).IsNotEmpty().And
						.All().Satisfy(type => type.GetMember("Within").Length == 0)
						.Because("Within limits the whole Throws expectation and must not read like a further condition");
				}

				[Test]
				public async Task WhenDelegateReturnsNullTask_ShouldFail()
				{
					Func<System.Threading.CancellationToken, Task> @delegate = _ => null!;

					async Task<Exception> Act()
						=> await That(@delegate).Throws().Within(5.Seconds());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that @delegate
						             throws an exception within 0:05,
						             but it returned <null> instead of a task
						             """)
						.Because("a null task is not an exception thrown by the delegate");
				}

				[Test]
				public async Task WhenDurationIsInfinite_AndNoExceptionIsThrown_ShouldNotMentionTheDuration()
				{
					Action action = () => { };

					async Task<Exception> Act()
						=> await That(action).Throws().Within(System.Threading.Timeout.InfiniteTimeSpan);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that action
						             throws an exception,
						             but it did not throw any exception
						             """);
				}

				[Test]
				public async Task WhenDurationIsInfinite_ShouldNotLimitTheDuration()
				{
					Action action = () =>
					{
						Task.Delay(20.Milliseconds()).Wait();
						throw new CustomException();
					};

					async Task<Exception> Act()
						=> await That(action).Throws().Within(System.Threading.Timeout.InfiniteTimeSpan);

					await That(Act).DoesNotThrow()
						.Because("an infinite duration imposes no limit");
				}

				[Test]
				public async Task WhenDurationIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).Throws().Within(-5.Milliseconds());

					await That(Act).Throws<ArgumentOutOfRangeException>()
						.WithParamName("timeout").And
						.WithMessage("The timeout must not be negative").AsPrefix();
				}

				[Test]
				public async Task WhenExceptionIsThrownTooLate_ShouldFail()
				{
					Exception exception = new CustomException();
					Action action = () =>
					{
						Task.Delay(50.Milliseconds()).Wait();
						throw exception;
					};

					async Task<Exception> Act()
						=> await That(action).Throws().Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:00.005,
						             but it took *
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenExceptionTypeIsThrownInTime_ShouldSucceed()
				{
					Exception exception = new CustomException();
					Action action = () => throw exception;

					async Task<Exception> Act()
						=> await That(action).Throws().Within(5.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNoExceptionIsThrownAndExecutionTimeIsTooLarge_ShouldFail()
				{
					Action action = () =>
					{
						Task.Delay(50.Milliseconds()).Wait();
					};

					async Task<Exception> Act()
						=> await That(action).Throws().Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:00.005,
						             but it took *
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenNoExceptionIsThrownInTime_ShouldFail()
				{
					Action action = () => { };

					async Task<Exception> Act()
						=> await That(action).Throws().Within(5.Seconds()).Because("it should");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:05, because it should,
						             but it did not throw any exception
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).Throws().Within(0.Seconds());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             throws an exception within 0:00,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSyncDelegateExceedsTheTimeoutAndTheDuration_ShouldReportTheDuration()
				{
					Action action = () =>
					{
						Block(200.Milliseconds());
						throw new CustomException();
					};

					async Task<Exception> Act()
						=> await That(action).Throws().Within(100.Milliseconds()).WithTimeout(50.Milliseconds());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:00.100,
						             but it took *
						             """).AsWildcard()
						.Because("the delegate violates the duration on its own, which the measured duration shows best");
				}

				[Test]
				public async Task WhenSyncDelegateExceedsTheTimeoutButNotTheDuration_ShouldFailWithTheTimeout()
				{
					Action action = () =>
					{
						Block(200.Milliseconds());
						throw new CustomException();
					};

					async Task<Exception> Act()
						=> await That(action).Throws().Within(5.Seconds()).WithTimeout(50.Milliseconds());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:05,
						             but it did not finish within 0:00.050
						             """).And
						.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
						.Because("the tighter timeout wins over the duration");
				}

				[Test]
				public async Task WhenSyncDelegateWithValueExceedsTheTimeoutButNotTheDuration_ShouldFailWithTheTimeout()
				{
					Func<int> action = () =>
					{
						Block(200.Milliseconds());
						throw new CustomException();
					};

					async Task<Exception> Act()
						=> await That(action).Throws().Within(5.Seconds()).WithTimeout(50.Milliseconds());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:05,
						             but it did not finish within 0:00.050
						             """).And
						.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
						.Because("the tighter timeout wins over the duration");
				}

				[Test]
				public async Task WhenTimeoutIsLongerThanTheDuration_ShouldKeepTheDuration()
				{
					Func<System.Threading.CancellationToken, Task> @delegate = token => Task.Delay(60.Seconds(), token);

					async Task<Exception> Act()
						=> await That(@delegate).Throws().Within(50.Milliseconds()).WithTimeout(20.Seconds());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that @delegate
						             throws an exception within 0:00.050,
						             but it did not finish within 0:00.050
						             """)
						.Because("the tighter limit wins, so a longer timeout must not loosen the duration");
				}

				[Test]
				public async Task WhenWithinIsSpecifiedTwice_ShouldThrowInvalidOperationException()
				{
					Action action = () => throw new CustomException();

					async Task<Exception> Act()
						=> await That(action).Throws().Within(1.Seconds()).Within(5.Seconds());

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Within cannot be specified more than once.")
						.Because("a second duration would silently disagree with the timeout of the first one");
				}

				[Test]
				public async Task WhenWithinIsSpecifiedTwice_WithInfiniteDuration_ShouldThrowInvalidOperationException()
				{
					Action action = () => throw new CustomException();

					async Task<Exception> Act()
						=> await That(action).Throws().Within(System.Threading.Timeout.InfiniteTimeSpan)
							.Within(1.Seconds());

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Within cannot be specified more than once.")
						.Because("an infinite duration is specified as well, even though it imposes no limit");
				}

				[Test]
				public async Task WhenWithinIsSpecifiedTwice_WithOnlyIfInBetween_ShouldThrowInvalidOperationException()
				{
					Action action = () => throw new CustomException();

					async Task<Exception?> Act()
						=> await That(action).Throws().Within(1.Seconds()).OnlyIf(true).Within(5.Seconds());

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Within cannot be specified more than once.")
						.Because("the continuation shares the duration of the expectation");
				}

				/// <remarks>
				///     Blocks the calling thread like a synchronous delegate that cannot be interrupted.
				/// </remarks>
				private static void Block(TimeSpan duration)
				{
					using System.Threading.ManualResetEventSlim neverSet = new();
					_ = neverSet.Wait(duration);
				}
			}

			public sealed class GenericTests
			{
				[Test]
				public async Task ShouldSupportChainedConstraints()
				{
					Action action = () => { };

					async Task Act()
						=> await That(action).Throws<ArgumentException>().Within(5.Seconds()).WithMessage("foo");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an ArgumentException within 0:05 with message equal to "foo",
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
						await That(action).Throws<CustomException>().Within(5.Seconds());

					await That(result.Value).IsEqualTo(value);
					await That(result).IsSameAs(exception);
				}

				[Test]
				public async Task WhenDurationIsInfinite_AndNoExceptionIsThrown_ShouldNotMentionTheDuration()
				{
					Action action = () => { };

					async Task<CustomException> Act()
						=> await That(action).Throws<CustomException>().Within(System.Threading.Timeout.InfiniteTimeSpan);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that action
						             throws a ThatDelegate.CustomException,
						             but it did not throw any exception
						             """);
				}

				[Test]
				public async Task WhenDurationIsInfinite_ShouldNotLimitTheDuration()
				{
					Action action = () =>
					{
						Task.Delay(20.Milliseconds()).Wait();
						throw new CustomException();
					};

					async Task<CustomException> Act()
						=> await That(action).Throws<CustomException>().Within(System.Threading.Timeout.InfiniteTimeSpan);

					await That(Act).DoesNotThrow()
						.Because("an infinite duration imposes no limit");
				}

				[Test]
				public async Task WhenDurationIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).Throws<CustomException>().Within(-5.Milliseconds());

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
						=> await That(action).Throws<CustomException>().Within(5.Seconds());

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
						=> await That(action).Throws<CustomException>().Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws a ThatDelegate.CustomException within 0:00.005,
						             but it took *
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenExactExceptionTypeIsThrownTooLate_ShouldNotForwardExceptionAsInnerException()
				{
					Exception exception = new CustomException();
					Action action = () =>
					{
						Task.Delay(50.Milliseconds()).Wait();
						throw exception;
					};

					async Task<CustomException> Act()
						=> await That(action).Throws<CustomException>().Within(5.Milliseconds());

					await That(Act).Throws()
						.Whose(e => e.InnerException, i => i.IsNull());
				}

				[Test]
				public async Task WhenNoExceptionIsThrownAndExecutionTimeIsTooLarge_ShouldFail()
				{
					Action action = () =>
					{
						Task.Delay(50.Milliseconds()).Wait();
					};

					async Task<Exception> Act()
						=> await That(action).Throws<Exception>().Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:00.005,
						             but it took *
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenNoExceptionIsThrownInTime_ShouldFail()
				{
					Action action = () => { };

					async Task<Exception> Act()
						=> await That(action).Throws<Exception>().Within(5.Seconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:05,
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
						=> await That(action).Throws<CustomException>().Within(5.Seconds());

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that action
						              throws a ThatDelegate.CustomException within 0:05,
						              but it did throw a ThatDelegate.OtherException:
						                {message}
						              """);
				}

				[Test]
				public async Task WhenSubCustomExceptionIsThrown_ShouldSucceed()
				{
					Exception exception = new SubCustomException();
					Action action = () => throw exception;

					async Task<CustomException> Act()
						=> await That(action).Throws<CustomException>().Within(5.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).Throws<CustomException>().Within(0.Seconds());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             throws a ThatDelegate.CustomException within 0:00,
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
						=> await That(action).Throws<SubCustomException>().Within(6.Seconds());

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that action
						              throws a ThatDelegate.SubCustomException within 0:06,
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
						=> await That(action).Throws(typeof(ArgumentException)).Within(5.Seconds()).WithMessage("foo");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an ArgumentException within 0:05 with message equal to "foo",
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
						await That(action).Throws(typeof(CustomException)).Within(5.Seconds());

					await That(result).IsSameAs(exception);
				}

				[Test]
				public async Task WhenDurationIsInfinite_AndNoExceptionIsThrown_ShouldNotMentionTheDuration()
				{
					Action action = () => { };

					async Task<Exception> Act()
						=> await That(action).Throws(typeof(CustomException))
							.Within(System.Threading.Timeout.InfiniteTimeSpan);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that action
						             throws a ThatDelegate.CustomException,
						             but it did not throw any exception
						             """);
				}

				[Test]
				public async Task WhenDurationIsInfinite_ShouldNotLimitTheDuration()
				{
					Action action = () =>
					{
						Task.Delay(20.Milliseconds()).Wait();
						throw new CustomException();
					};

					async Task<Exception> Act()
						=> await That(action).Throws(typeof(CustomException))
							.Within(System.Threading.Timeout.InfiniteTimeSpan);

					await That(Act).DoesNotThrow()
						.Because("an infinite duration imposes no limit");
				}

				[Test]
				public async Task WhenDurationIsNegative_ShouldThrowArgumentOutOfRangeException()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).Throws(typeof(CustomException)).Within(-5.Milliseconds());

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
						=> await That(action).Throws(typeof(CustomException)).Within(5.Seconds());

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
						=> await That(action).Throws(typeof(CustomException)).Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws a ThatDelegate.CustomException within 0:00.005,
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
						=> await That(action).Throws(typeof(Exception)).Within(5.Milliseconds());

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:00.005,
						             but it took *
						             """).AsWildcard();
				}

				[Test]
				public async Task WhenNoExceptionIsThrownInTime_ShouldFail()
				{
					Action action = () => { };

					async Task<Exception> Act()
						=> await That(action).Throws(typeof(Exception)).Within(5.Seconds()).Because("it should");

					await That(Act).Throws()
						.WithMessage("""
						             Expected that action
						             throws an exception within 0:05, because it should,
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
						=> await That(action).Throws(typeof(CustomException)).Within(5.Seconds());

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that action
						              throws a ThatDelegate.CustomException within 0:05,
						              but it did throw a ThatDelegate.OtherException:
						                {message}
						              """);
				}

				[Test]
				public async Task WhenSubCustomExceptionIsThrown_ShouldSucceed()
				{
					Exception exception = new SubCustomException();
					Action action = () => throw exception;

					async Task<Exception> Act()
						=> await That(action).Throws(typeof(CustomException)).Within(5.Seconds());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					Action? subject = null;

					async Task Act()
						=> await That(subject!).Throws(typeof(CustomException)).Within(0.Seconds());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             throws a ThatDelegate.CustomException within 0:00,
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
						=> await That(action).Throws(typeof(SubCustomException)).Within(6.Seconds());

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that action
						              throws a ThatDelegate.SubCustomException within 0:06,
						              but it did throw a ThatDelegate.CustomException:
						                {message}
						              """);
				}
			}
#pragma warning restore CA2263
		}
	}
}
