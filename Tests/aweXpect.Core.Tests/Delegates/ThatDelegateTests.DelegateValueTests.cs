using aweXpect.Chronology;
using aweXpect.Core.Sources;
using aweXpect.Core.Tests.TestHelpers;
using WithoutValue = aweXpect.Delegates.ThatDelegate.WithoutValue;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class DelegateValueTests
	{
		[Test]
		public async Task DoesNotThrow_Generic_WhenNegatedAndOtherExceptionIsThrown_ShouldFail()
		{
			DelegateValue value = new(new ArgumentException("foo"), TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).DoesNotThrow<MyException>());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             throws a MyException,
				             but it did throw an ArgumentException:
				               foo
				             """);
		}

		[Test]
		public async Task DoesNotThrow_WhenDelegateIsNull_ShouldFail()
		{
			DelegateValue value = new(null, TimeSpan.Zero, true);

			async Task Act()
				=> await WithoutValue(That(value)).DoesNotThrow();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             does not throw any exception,
				             but it was <null>
				             """);
		}

		[Test]
		public async Task DoesNotThrow_WhenNegatedAndDelegateDoesNotThrow_ShouldFail()
		{
			DelegateValue value = new(null, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).DoesNotThrow());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             throws an exception,
				             but it did not throw any exception
				             """);
		}

		[Test]
		public async Task DoesNotThrow_WhenNullTaskWasReturned_ShouldFail()
		{
			DelegateValue value = new(null, TimeSpan.Zero)
			{
				NullKind = NullSubjectKind.NullTaskReturned,
			};

			async Task Act()
				=> await WithoutValue(That(value)).DoesNotThrow();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             does not throw any exception,
				             but it returned <null> instead of a task
				             """);
		}

		[Test]
		public async Task DoesNotThrow_WhenTaskSubjectIsNull_ShouldFail()
		{
			DelegateValue value = new(null, TimeSpan.Zero)
			{
				NullKind = NullSubjectKind.NullTaskSubject,
			};

			async Task Act()
				=> await WithoutValue(That(value)).DoesNotThrow();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             does not throw any exception,
				             but it was a <null> task
				             """);
		}

		[Test]
		public async Task ExecutesIn_AllowingExceptions_WhenDelegateThrowsTooEarly_ShouldFail()
		{
			DelegateValue value = new(new MyException("foo"), 2.Seconds());

			async Task Act()
				=> await WithoutValue(That(value)).ExecutesIn().AllowingExceptions().AtLeast(5.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             executes in at least 0:05 allowing exceptions,
				             but it took only 0:02 and did throw a MyException:
				               foo
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull())
				.Because("the duration, not the allowed exception, caused the failure");
		}

		[Test]
		public async Task ExecutesIn_WhenDelegateIsNull_ShouldFail()
		{
			DelegateValue value = new(null, TimeSpan.Zero, true);

			async Task Act()
				=> await WithoutValue(That(value)).ExecutesIn().AtMost(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             executes in at most 0:01,
				             but it was <null>
				             """);
		}

		[Test]
		public async Task ExecutesIn_WhenLateResultExceedsTheMaximum_ShouldReportTheDuration()
		{
			DelegateValue value = new(null, 2.Seconds());

			async Task Act()
				=> await WithoutValue(That(value)).ExecutesIn().AtMost(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             executes in at most 0:01,
				             but it took 0:02
				             """)
				.Because("a synchronous delegate that returned late is judged by its measured duration");
		}

		[Test]
		public async Task ExecutesIn_WhenLateResultIsWithinTheMaximumButNotWithinTheTimeout_ShouldFailWithTheTimeout()
		{
			DelegateValue value = new(null, 2.Seconds());

			async Task Act()
				=> await WithoutValue(That(value)).ExecutesIn().AtMost(5.Seconds()).WithTimeout(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             executes in at most 0:05,
				             but it did not finish within 0:01
				             """).And
				.WithInner<TimeoutException>()
				.Because("the tighter timeout wins over the maximum, which the duration does not violate");
		}

		[Test]
		public async Task ExecutesIn_WhenNegated_ShouldThrowNotSupportedException()
		{
			DelegateValue value = new(null, TimeSpan.Zero);

			async Task Act()
				=> await That(value).DoesNotComplyWith(it => WithoutValue(it).ExecutesIn().AtMost(1.Seconds()));

			await That(Act).Throws<NotSupportedException>()
				.WithMessage("Negation of ExecutesIn is not supported.");
		}

		[Test]
		public async Task Throws_WhenDelegateIsNull_ShouldFail()
		{
			DelegateValue value = new(null, TimeSpan.Zero, true);

			async Task Act()
				=> await WithoutValue(That(value)).Throws<MyException>();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             throws a MyException,
				             but it was <null>
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull());
		}

		[Test]
		public async Task Throws_WhenLateResultExceedsWithin_ShouldReportTheDuration()
		{
			DelegateValue value = new(new MyException("foo"), 2.Seconds());

			async Task Act()
				=> await WithoutValue(That(value)).Throws<MyException>().Within(1.Seconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that value
				             throws a MyException within 0:01,
				             but it took 0:02
				             """).And
				.Whose(e => e.InnerException, i => i.IsNull())
				.Because("the exception was thrown too late, so it did not cause the failure");
		}

		private static WithoutValue WithoutValue(IThat<DelegateValue> subject)
			=> new(((IExpectThat<DelegateValue>)subject).ExpectationBuilder);
	}
}
