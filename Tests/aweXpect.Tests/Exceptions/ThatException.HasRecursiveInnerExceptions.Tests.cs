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
					             has recursive inner exceptions which all satisfy e => e.Message != "inner3A",
					             but not all did
					             
					             Not matching items:
					             [
					               System.Exception: inner3A,
					               (… and maybe others)
					             ]
					             
					             Collection:
					             [
					               System.Exception: inner1*,
					               System.AggregateException:*,
					               System.Exception: inner3A*,
					               System.Exception: inner3B*
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
					             has recursive inner exceptions which none satisfy e => e.Message != "inner3A",
					             but at least one did
					             
					             Matching items:
					             [
					               System.Exception: inner1*,
					               (… and maybe others)
					             ]
					             
					             Collection:
					             [
					               System.Exception: inner1*,
					               System.AggregateException:*,
					               System.Exception: inner3A*,
					               System.Exception: inner3B*
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
					             does not have recursive inner exceptions which all satisfy e => e.Message.StartsWith("inner"),
					             but it had

					             Collection:
					             [
					               System.Exception: inner1*,
					               System.AggregateException:*,
					               System.Exception: inner3A*,
					               System.Exception: inner3B*
					             ]
					             """).AsWildcard();
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
					               System.ArgumentException: inner
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
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				Exception? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasRecursiveInnerExceptions(c => c.IsEmpty()));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
