namespace aweXpect.Tests;

public sealed partial class ThatException
{
	public class HasRecursiveInnerExceptions
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllInnerExceptionsMatchTheCondition_ShouldSucceed()
			{
				Exception subject = new("outer",
					new Exception("inner1",
						new AggregateException("inner2",
							new Exception("inner3A"),
							new Exception("inner3B"))));

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c
						=> c.All().Satisfy(e => e.Message.StartsWith("inner")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExceptionHasNoInnerException_ForAll_ShouldFail()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c => c.All().Satisfy(_ => true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recursive inner exceptions of which all satisfy _ => true,
					             but none of 0 did

					             Collection:
					             []
					             """)
					.Because("an expectation on all inner exceptions requires at least one of them");
			}

			[Fact]
			public async Task WhenExceptionHasNoInnerException_ForAreEqualTo_ShouldFail()
			{
				Exception inner = new("inner");
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c => c.All().AreEqualTo(inner));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recursive inner exceptions of which all are equal to Exception: inner,
					             but none of 0 were

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenExceptionHasNoInnerException_ForAreUnique_ShouldFail()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c => c.All().AreUnique());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recursive inner exceptions of which all are unique,
					             but none of 0 were

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenExceptionHasNoInnerException_ForAtMost_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c => c.AtMost(2).Satisfy(_ => true));

				await That(Act).DoesNotThrow()
					.Because("at most 2 inner exceptions is what an exception without any inner exception has");
			}

			[Fact]
			public async Task WhenExceptionHasNoInnerException_ForComplyWith_ShouldFail()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c => c.All().ComplyWith(e => e.HasMessage("inner")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recursive inner exceptions whose Message is equal to "inner" for all items,
					             but none of 0 were

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenExceptionHasNoInnerException_ForNone_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c => c.None().Satisfy(_ => true));

				await That(Act).DoesNotThrow()
					.Because("no inner exception matches when there is no inner exception");
			}

			[Fact]
			public async Task WhenExceptionHasOneInnerException_ForAll_ShouldSucceed()
			{
				Exception subject = new("outer", new Exception("inner"));

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c => c.All().Satisfy(_ => true));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExceptionIsAnAggregateExceptionWithoutInnerExceptions_ForAll_ShouldFail()
			{
				Exception subject = new AggregateException();

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c => c.All().Satisfy(_ => true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recursive inner exceptions of which all satisfy _ => true,
					             but none of 0 did

					             Collection:
					             []
					             """)
					.Because("an AggregateException without inner exceptions is empty just like any other exception");
			}

			[Fact]
			public async Task WhenExpectationsAreEmpty_ShouldThrowArgumentException()
			{
				Exception subject = new("outer", new Exception("inner"));

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(_ => { });

				await That(Act).Throws<ArgumentException>()
					.WithMessage("You must add at least one expectation in the expectations callback.*").AsWildcard()
					.And.WithParamName("expectations");
			}

			[Fact]
			public async Task WhenInnerExceptionsDoNotMatchTheCondition_ForAll_ShouldFail()
			{
				Exception subject = new("outer",
					new Exception("inner1",
						new AggregateException("inner2",
							new Exception("inner3A"),
							new Exception("inner3B"))));

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(
						c => c.All().Satisfy(e => e.Message != "inner3A"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recursive inner exceptions of which all satisfy e => e.Message != "inner3A",
					             but only 2 of at least 3 did
					             
					             Not matching items:
					             [
					               Exception: inner3A,
					               (… and maybe others)
					             ]
					             
					             Collection:
					             [
					               Exception: inner1*,
					               AggregateException:*,
					               Exception: inner3A*,
					               Exception: inner3B*
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenInnerExceptionsDoNotMatchTheCondition_ForNone_ShouldFail()
			{
				Exception subject = new("outer",
					new Exception("inner1",
						new AggregateException("inner2",
							new Exception("inner3A"),
							new Exception("inner3B"))));

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(
						c => c.None().Satisfy(e => e.Message != "inner3A"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recursive inner exceptions of which none satisfy e => e.Message != "inner3A",
					             but at least 1 of at least 1 did
					             
					             Matching items:
					             [
					               Exception: inner1*,
					               (… and maybe others)
					             ]
					             
					             Collection:
					             [
					               Exception: inner1*,
					               AggregateException:*,
					               Exception: inner3A*,
					               Exception: inner3B*
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).HasRecursiveInnerExceptions(c => c.IsEmpty());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recursive inner exceptions which are empty,
					             but it was <null>
					             """);
			}
		}
		
		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAllInnerExceptionsMatchTheCondition_ShouldFail()
			{
				Exception subject = new("outer",
					new Exception("inner1",
						new AggregateException("inner2",
							new Exception("inner3A"),
							new Exception("inner3B"))));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasRecursiveInnerExceptions(c
							=> c.All().Satisfy(e => e.Message.StartsWith("inner"))));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have recursive inner exceptions of which all satisfy e => e.Message.StartsWith("inner"),
					             but it had

					             Collection:
					             [
					               Exception: inner1*,
					               AggregateException:*,
					               Exception: inner3A*,
					               Exception: inner3B*
					             ]
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenExceptionHasNoInnerException_ForAll_ShouldSucceed()
			{
				Exception subject = new("outer");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasRecursiveInnerExceptions(c => c.All().Satisfy(_ => true)));

				await That(Act).DoesNotThrow()
					.Because("the negated expectation holds whenever the positive one fails");
			}

			[Fact]
			public async Task WhenInnerExceptionCountMatches_ShouldFail()
			{
				Exception subject = new InvalidOperationException("outer", new ArgumentException("inner"));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasRecursiveInnerExceptions(c => c.HasCount(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have recursive inner exceptions which have exactly one item,
					             but it had
					             
					             Collection:
					             [
					               ArgumentException: inner
					             ]
					             """);
			}

			[Fact]
			public async Task WhenInnerExceptionsDoNotMatchTheCondition_ForAll_ShouldSucceed()
			{
				Exception subject = new("outer",
					new Exception("inner1",
						new AggregateException("inner2",
							new Exception("inner3A"),
							new Exception("inner3B"))));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasRecursiveInnerExceptions(c => c.All().Satisfy(e => e.Message != "inner3A")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInnerExceptionsDoNotMatchTheCondition_ForNone_ShouldSucceed()
			{
				Exception subject = new("outer",
					new Exception("inner1",
						new AggregateException("inner2",
							new Exception("inner3A"),
							new Exception("inner3B"))));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasRecursiveInnerExceptions(c => c.None().Satisfy(e => e.Message != "inner3A")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasRecursiveInnerExceptions(c => c.IsEmpty()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have recursive inner exceptions which are empty,
					             but it was <null>
					             """);
			}
		}
	}
}
