namespace aweXpect.Tests;

public sealed partial class ThatWeakReference
{
	public sealed class IsCollected
	{
		[NotInParallel]
		public sealed class GenericTests
		{
			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				WeakReference<object>? subject = null;

				async Task Act()
					=> await That(subject).IsCollected();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is collected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTargetIsAlive_ShouldFail()
			{
				object target = new();
				WeakReference<object> subject = new(target);

				async Task Act()
					=> await That(subject).IsCollected();

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
				WeakReference<object> subject = CreateCollectableGenericReference();

				async Task Act()
					=> await That(subject).IsCollected();

				await That(Act).DoesNotThrow();
			}
		}

		[NotInParallel]
		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTargetIsAlive_ShouldSucceed()
			{
				object target = new();
				WeakReference subject = new(target);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsCollected());

				await That(Act).DoesNotThrow();
				GC.KeepAlive(target);
			}

			[Test]
			public async Task WhenTargetIsNoLongerReferenced_ShouldFail()
			{
				WeakReference subject = CreateCollectableReference();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsCollected());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not collected,
					             but it was collected
					             """);
			}
		}

		[NotInParallel]
		public sealed class Tests
		{
			[Test]
			public async Task ShouldReturnTheSubject()
			{
				WeakReference subject = CreateCollectableReference();

				WeakReference result = await That(subject).IsCollected();

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				WeakReference? subject = null;

				async Task Act()
					=> await That(subject).IsCollected();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is collected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTargetIsAlive_ShouldFail()
			{
				object target = new();
				WeakReference subject = new(target);

				async Task Act()
					=> await That(subject).IsCollected().Because("it should no longer be referenced");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is collected, because it should no longer be referenced,
					             but it was still alive
					             """);
				GC.KeepAlive(target);
			}

			[Test]
			public async Task WhenTargetIsNoLongerReferenced_ShouldSucceed()
			{
				WeakReference subject = CreateCollectableReference();

				async Task Act()
					=> await That(subject).IsCollected();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTargetIsOnlyReleasedByAChainOfFinalizers_ShouldSucceed()
			{
				Holder targetHolder = new();
				Holder finalizerHolder = new();
				WeakReference subject = CreateReferenceReleasedByFinalizerChain(targetHolder, finalizerHolder);

				async Task Act()
					=> await That(subject).IsCollected()
						.Because("the collection is repeated while finalizers release further objects");

				await That(Act).DoesNotThrow();
				GC.KeepAlive(targetHolder);
				GC.KeepAlive(finalizerHolder);
			}
		}
	}
}
