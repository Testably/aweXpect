using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsVirtual
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventIsNotVirtual_ShouldFail()
			{
				EventInfo subject =
					typeof(BaseClassWithMembers).GetEvent(nameof(BaseClassWithMembers.BaseEvent))!;

				async Task Act()
				{
					await That(subject).IsVirtual();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is virtual,
					              but it was non-virtual event EventHandler BaseClassWithMembers.BaseEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsVirtual();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is virtual,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventIsVirtual_ShouldSucceed()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).IsVirtual();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsNotVirtual_ShouldSucceed()
			{
				EventInfo subject =
					typeof(BaseClassWithMembers).GetEvent(nameof(BaseClassWithMembers.BaseEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsVirtual());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsVirtual_ShouldFail()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsVirtual());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not virtual,
					             but it was virtual event EventHandler AbstractClassWithMembers.VirtualEvent
					             """);
			}
		}
	}
}
