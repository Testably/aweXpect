using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotAbstract
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventIsAbstract_ShouldFail()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.AbstractEvent))!;

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not abstract,
					              but it was abstract {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEventIsNotAbstract_ShouldSucceed()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventIsAbstract_ShouldSucceed()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.AbstractEvent))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsNotAbstract_ShouldFail()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is abstract,
					             but it was non-abstract System.EventHandler VirtualEvent
					             """);
			}
		}
	}
}
