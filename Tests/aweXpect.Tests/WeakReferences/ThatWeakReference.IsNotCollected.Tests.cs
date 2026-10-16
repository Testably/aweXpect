namespace aweXpect.Tests;

public sealed partial class ThatWeakReference
{
	public sealed class IsNotCollected
	{
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				WeakReference<object>? subject = null;

				async Task Act()
					=> await That(subject).IsNotCollected();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not collected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTargetIsAlive_ShouldSucceed()
			{
				object target = new();
				WeakReference<object> subject = new(target);

				async Task Act()
					=> await That(subject).IsNotCollected();

				await That(Act).DoesNotThrow();
				GC.KeepAlive(target);
			}

			[Test]
			public async Task WhenTargetIsNoLongerReferenced_ShouldFail()
			{
				WeakReference<object> subject = CreateCollectableGenericReference();

				async Task Act()
					=> await That(subject).IsNotCollected();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not collected,
					             but it was collected
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTargetIsAlive_ShouldFail()
			{
				object target = new();
				WeakReference subject = new(target);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotCollected());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is collected,
					             but it was still alive
					             """);
				GC.KeepAlive(target);
			}

			[Test]
			public async Task WhenTargetIsNoLongerReferenced_ShouldSucceed()
			{
				WeakReference subject = CreateCollectableReference();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotCollected());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				WeakReference? subject = null;

				async Task Act()
					=> await That(subject).IsNotCollected();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not collected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTargetIsAlive_ShouldSucceed()
			{
				object target = new();
				WeakReference subject = new(target);

				async Task Act()
					=> await That(subject).IsNotCollected();

				await That(Act).DoesNotThrow();
				GC.KeepAlive(target);
			}

			[Test]
			public async Task WhenTargetIsNoLongerReferenced_ShouldFail()
			{
				WeakReference subject = CreateCollectableReference();

				async Task Act()
					=> await That(subject).IsNotCollected().Because("it should still be cached");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not collected, because it should still be cached,
					             but it was collected
					             """);
			}
		}
	}
}
