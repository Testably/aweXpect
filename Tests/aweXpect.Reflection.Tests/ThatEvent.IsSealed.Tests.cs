using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsSealed
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventIsNotSealed_ShouldFail()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).IsSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is sealed,
					              but it was non-sealed event EventHandler AbstractClassWithMembers.VirtualEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventIsSealed_ShouldSucceed()
			{
				EventInfo subject =
					typeof(ClassWithSealedMembers).GetEvent(nameof(ClassWithSealedMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).IsSealed();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsNotSealed_ShouldSucceed()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsSealed());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsSealed_ShouldFail()
			{
				EventInfo subject =
					typeof(ClassWithSealedMembers).GetEvent(nameof(ClassWithSealedMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsSealed());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was sealed event EventHandler ClassWithSealedMembers.VirtualEvent
					             """);
			}
		}
	}
}
