using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class Overrides
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventDoesNotOverride_ShouldFail()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).Overrides();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              overrides a base event,
					              but it did not override a base event event EventHandler AbstractClassWithMembers.VirtualEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).Overrides();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             overrides a base event,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventOverrides_ShouldSucceed()
			{
				EventInfo subject =
					typeof(ClassWithSealedMembers).GetEvent(nameof(ClassWithSealedMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).Overrides();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventDoesNotOverride_ShouldSucceed()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Overrides());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventOverrides_ShouldFail()
			{
				EventInfo subject =
					typeof(ClassWithSealedMembers).GetEvent(nameof(ClassWithSealedMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.Overrides());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not override a base event,
					             but it did override a base event event EventHandler ClassWithSealedMembers.VirtualEvent
					             """);
			}
		}
	}
}
