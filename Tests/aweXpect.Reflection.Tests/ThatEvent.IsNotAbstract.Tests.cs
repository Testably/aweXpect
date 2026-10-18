using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotAbstract
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventIsAbstract_ShouldFail()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.AbstractEvent))!;

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not abstract,
					              but it was abstract event EventHandler AbstractClassWithMembers.AbstractEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsNotAbstract_ShouldSucceed()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsAbstract_ShouldSucceed()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.AbstractEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsNotAbstract_ShouldFail()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is abstract,
					             but it was non-abstract event EventHandler AbstractClassWithMembers.VirtualEvent
					             """);
			}
		}
	}
}
