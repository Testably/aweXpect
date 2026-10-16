using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Chronology;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	[NotInParallel]
	public sealed class IsCollectedAfterTests
	{
		[Test]
		public async Task WhenAsyncDelegateCompletesAfterTheTimeout_ShouldNotCallTheCleanup()
		{
			EventBus bus = new();
			TaskCompletionSource<Subscriber> factory = new();
			int cleanupCalls = 0;

			async Task Act()
				=> await That(() => factory.Task).IsCollectedAfter(_ => cleanupCalls++)
					.WithTimeout(50.Milliseconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that () => factory.Task
				             is collected after _ => cleanupCalls++,
				             but it did not finish within 0:00.050
				             """);
			factory.SetResult(new Subscriber(bus));
			await Task.Delay(100);
			await That(cleanupCalls).IsEqualTo(0)
				.Because("the cleanup of an abandoned delegate must not run after the expectation already failed");
		}

		[Test]
		public async Task WhenAsyncDelegateCompletesAsynchronously_AndCleanupReleasesTheObject_ShouldSucceed()
		{
			EventBus bus = new();

			async Task Act()
				=> await That(() => CreateAsynchronously(bus)).IsCollectedAfter(subscriber => subscriber.Dispose());

			await That(Act).DoesNotThrow()
				.Because("neither the task of the delegate nor its continuations may keep the object alive");
		}

		[Test]
		public async Task WhenAsyncDelegateCompletesAsynchronously_AndObjectIsStillReferenced_ShouldFail()
		{
			EventBus bus = new();

			async Task Act()
				=> await That(() => CreateAsynchronously(bus)).IsCollectedAfter(_ => { });

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that () => CreateAsynchronously(bus)
				             is collected after _ => { },
				             but it was still alive
				             """);
		}

		[Test]
		public async Task WhenAsyncDelegateCompletesSynchronously_AndCleanupReleasesTheObject_ShouldSucceed()
		{
			EventBus bus = new();

			async Task Act()
				=> await That(() => Task.FromResult(new Subscriber(bus)))
					.IsCollectedAfter(subscriber => subscriber.Dispose());

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenAsyncDelegateReturnsNullTask_ShouldFail()
		{
			Func<Task<Subscriber>> @delegate = () => null!;

			async Task Act()
				=> await That(@delegate).IsCollectedAfter(subscriber => subscriber.Dispose());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that @delegate
				             is collected after subscriber => subscriber.Dispose(),
				             but it returned <null> instead of a task
				             """);
		}

		[Test]
		public async Task WhenAsyncDelegateThrows_ShouldFail()
		{
			async Task Act()
				=> await That(() => ThrowAsynchronously()).IsCollectedAfter(subscriber => subscriber.Dispose());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that () => ThrowAsynchronously()
				             is collected after subscriber => subscriber.Dispose(),
				             but it did throw an InvalidOperationException:
				               factory failed
				             """).And
				.WithInner<InvalidOperationException>(inner => inner.HasMessage("factory failed"));
		}

		[Test]
		public async Task WhenAsyncDelegateTimesOut_ShouldFail()
		{
			async Task Act()
				=> await That((CancellationToken token) => NeverComplete(token))
					.IsCollectedAfter(subscriber => subscriber.Dispose())
					.WithTimeout(50.Milliseconds());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that (CancellationToken token) => NeverComplete(token)
				             is collected after subscriber => subscriber.Dispose(),
				             but it did not finish within 0:00.050
				             """).And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."));
		}

		[Test]
		public async Task WhenCancellationTokenDelegateReturnsCollectableObject_ShouldSucceed()
		{
			EventBus bus = new();

			async Task Act()
				=> await That((CancellationToken _) => new Subscriber(bus))
					.IsCollectedAfter(subscriber => subscriber.Dispose());

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenCleanupIsNull_ShouldThrowArgumentNullException()
		{
			EventBus bus = new();

			async Task Act()
				=> await That(() => new Subscriber(bus)).IsCollectedAfter(null!);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("cleanup").And
				.WithMessage("The 'cleanup' cannot be null.").AsPrefix();
		}

		[Test]
		public async Task WhenCleanupReleasesTheObject_ShouldSucceed()
		{
			EventBus bus = new();

			async Task Act()
				=> await That(() => new Subscriber(bus)).IsCollectedAfter(subscriber => subscriber.Dispose());

			await That(Act).DoesNotThrow();
			await That(bus.HasSubscribers).IsFalse()
				.Because("the cleanup must have been called on the object");
		}

		[Test]
		public async Task WhenCleanupThrows_ShouldFail()
		{
			EventBus bus = new();

			async Task Act()
				=> await That(() => new Subscriber(bus))
					.IsCollectedAfter(_ => throw new InvalidOperationException("cleanup failed"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that () => new Subscriber(bus)
				             is collected after _ => throw new InvalidOperationException("cleanup failed"),
				             but the cleanup did throw an InvalidOperationException:
				               cleanup failed
				             """).And
				.WithInner<InvalidOperationException>(inner => inner.HasMessage("cleanup failed"));
		}

		[Test]
		public async Task WhenDelegateIsNull_ShouldFail()
		{
			Func<Subscriber> @delegate = null!;

			async Task Act()
				=> await That(@delegate).IsCollectedAfter(subscriber => subscriber.Dispose());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that @delegate
				             is collected after subscriber => subscriber.Dispose(),
				             but it was <null>
				             """);
		}

		[Test]
		public async Task WhenDelegateReturnsNull_ShouldFailWithoutCallingTheCleanup()
		{
			int cleanupCalls = 0;

			async Task Act()
				=> await That(() => (Subscriber)null!).IsCollectedAfter(_ => cleanupCalls++);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that () => (Subscriber)null!
				             is collected after _ => cleanupCalls++,
				             but it returned <null>
				             """);
			await That(cleanupCalls).IsEqualTo(0)
				.Because("there is no object to clean up");
		}

		[Test]
		public async Task WhenDelegateThrows_ShouldFail()
		{
			async Task Act()
				=> await That(Subscriber () => throw new InvalidOperationException("factory failed"))
					.IsCollectedAfter(subscriber => subscriber.Dispose());

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that Subscriber () => throw new InvalidOperationException("factory failed")
				             is collected after subscriber => subscriber.Dispose(),
				             but it did throw an InvalidOperationException:
				               factory failed
				             """).And
				.WithInner<InvalidOperationException>(inner => inner.HasMessage("factory failed"));
		}

		[Test]
		public async Task WhenObjectIsStillReferenced_ShouldFail()
		{
			EventBus bus = new();

			async Task Act()
				=> await That(() => new Subscriber(bus)).IsCollectedAfter(_ => { });

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that () => new Subscriber(bus)
				             is collected after _ => { },
				             but it was still alive
				             """);
		}

		[Test]
		public async Task WhenObjectIsStillReferencedFromAStaticList_ShouldFail()
		{
			try
			{
				async Task Act()
					=> await That(() => StaticRegistry.Register(new object())).IsCollectedAfter(_ => { });

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that () => StaticRegistry.Register(new object())
					             is collected after _ => { },
					             but it was still alive
					             """);
			}
			finally
			{
				StaticRegistry.Clear();
			}
		}

		[Test]
		public async Task WhenObjectIsOnlyReleasedByAChainOfFinalizers_ShouldSucceed()
		{
			Holder targetHolder = new();
			Holder finalizerHolder = new();

			async Task Act()
				=> await That(() => CreateObjectReleasedByFinalizerChain(targetHolder, finalizerHolder))
					.IsCollectedAfter(_ => { })
					.Because("the collection is repeated while finalizers release further objects");

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task WhenObjectOnlyReferencesLongLivedObjects_ShouldSucceedWithoutCleanup()
		{
			EventBus bus = new();

			async Task Act()
				=> await That(() => new Publisher(bus)).IsCollectedAfter(_ => { })
					.Because("a reference from the object to a long-lived one does not keep the object alive");

			await That(Act).DoesNotThrow();
		}

		private static async Task<Subscriber> CreateAsynchronously(EventBus bus)
		{
			await Task.Yield();
			return new Subscriber(bus);
		}

		/// <remarks>
		///     The target is only released after two rounds of finalizers, so a single forced collection does not suffice.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static object CreateObjectReleasedByFinalizerChain(Holder targetHolder, Holder finalizerHolder)
		{
			object target = new();
			targetHolder.Value = target;
			finalizerHolder.Value = new ReleasingFinalizer(targetHolder);
			_ = new ReleasingFinalizer(finalizerHolder);
			return target;
		}

		private static async Task<Subscriber> NeverComplete(CancellationToken token)
		{
			await Task.Delay(Timeout.Infinite, token);
			return new Subscriber(new EventBus());
		}

		private static async Task<Subscriber> ThrowAsynchronously()
		{
			await Task.Yield();
			throw new InvalidOperationException("factory failed");
		}

		private sealed class EventBus
		{
			public bool HasSubscribers => Published is not null;
			public event EventHandler? Published;
		}

		private sealed class Holder
		{
			public object? Value { get; set; }
		}

		private sealed class Publisher(EventBus bus)
		{
			public EventBus Bus { get; } = bus;
		}

		private sealed class ReleasingFinalizer(Holder holder)
		{
			~ReleasingFinalizer()
			{
				holder.Value = null;
			}
		}

		private static class StaticRegistry
		{
			private static readonly List<object> Items = [];

			public static void Clear()
			{
				lock (Items)
				{
					Items.Clear();
				}
			}

			public static object Register(object item)
			{
				lock (Items)
				{
					Items.Add(item);
				}

				return item;
			}
		}

		private sealed class Subscriber : IDisposable
		{
			private readonly EventBus _bus;

			public Subscriber(EventBus bus)
			{
				_bus = bus;
				_bus.Published += OnPublished;
			}

			public void Dispose() => _bus.Published -= OnPublished;

			private void OnPublished(object? sender, EventArgs e)
			{
			}
		}
	}
}
