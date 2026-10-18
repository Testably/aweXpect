using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNullable
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventIsNonNullable_ShouldFail()
			{
				EventInfo subject = typeof(ClassWithNonNullableEvents)
					.GetEvent(nameof(ClassWithNonNullableEvents.NonNullableEvent))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable event EventHandler ClassWithNonNullableEvents.NonNullableEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsNonNullableGenericType_ShouldFail()
			{
				EventInfo subject = typeof(ClassWithNonNullableEvents)
					.GetEvent(nameof(ClassWithNonNullableEvents.NonNullableGenericEvent))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable event EventHandler<string> ClassWithNonNullableEvents.NonNullableGenericEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsNonNullableInMixedClass_ShouldFail()
			{
				EventInfo subject = typeof(ClassWithMixedNullableEvents)
					.GetEvent(nameof(ClassWithMixedNullableEvents.NonNullableEvent))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable event EventHandler ClassWithMixedNullableEvents.NonNullableEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsNonNullableWithCustomAccessors_ShouldFail()
			{
				EventInfo subject = typeof(ClassWithCustomNonNullableEvent)
					.GetEvent(nameof(ClassWithCustomNonNullableEvent.NonNullableCustomEvent))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable event EventHandler ClassWithCustomNonNullableEvent.NonNullableCustomEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is nullable,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventIsNullable_ShouldSucceed()
			{
				EventInfo subject = typeof(ClassWithNullableEvents)
					.GetEvent(nameof(ClassWithNullableEvents.NullableEvent))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsNullableGenericType_ShouldSucceed()
			{
				EventInfo subject = typeof(ClassWithNullableEvents)
					.GetEvent(nameof(ClassWithNullableEvents.NullableGenericEvent))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsNullableInMixedClass_ShouldSucceed()
			{
				EventInfo subject = typeof(ClassWithMixedNullableEvents)
					.GetEvent(nameof(ClassWithMixedNullableEvents.NullableEvent))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsNullableWithCustomAccessors_ShouldSucceed()
			{
				EventInfo subject = typeof(ClassWithCustomNullableEvent)
					.GetEvent(nameof(ClassWithCustomNullableEvent.NullableCustomEvent))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsOblivious_ShouldFail()
			{
				EventInfo subject = typeof(ClassWithObliviousEvents)
					.GetEvent(nameof(ClassWithObliviousEvents.ObliviousEvent))!;

				async Task Act()
				{
					await That(subject).IsNullable();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is nullable,
					              but it was non-nullable event EventHandler ClassWithObliviousEvents.ObliviousEvent
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsNotNullable_ShouldSucceed()
			{
				EventInfo subject = typeof(ClassWithNonNullableEvents)
					.GetEvent(nameof(ClassWithNonNullableEvents.NonNullableEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNullable());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsNullable_ShouldFail()
			{
				EventInfo subject = typeof(ClassWithNullableEvents)
					.GetEvent(nameof(ClassWithNullableEvents.NullableEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNullable());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not nullable,
					              but it was nullable event EventHandler ClassWithNullableEvents.NullableEvent
					              """);
			}
		}
	}
}
