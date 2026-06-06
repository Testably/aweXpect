using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class DoesNotOverride
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventDoesNotOverride_ShouldSucceed()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotOverride();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotOverride();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             does not override a base event,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventOverrides_ShouldFail()
			{
				EventInfo subject =
					typeof(ClassWithSealedMembers).GetEvent(nameof(ClassWithSealedMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotOverride();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not override a base event,
					              but it did override a base event {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventDoesNotOverride_ShouldFail()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotOverride());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             overrides a base event,
					             but it did not override a base event event EventHandler AbstractClassWithMembers.VirtualEvent
					             """);
			}

			[Fact]
			public async Task WhenEventOverrides_ShouldSucceed()
			{
				EventInfo subject =
					typeof(ClassWithSealedMembers).GetEvent(nameof(ClassWithSealedMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotOverride());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
