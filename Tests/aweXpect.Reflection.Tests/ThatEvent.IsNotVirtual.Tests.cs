using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventIsNotVirtual_ShouldSucceed()
			{
				EventInfo subject =
					typeof(BaseClassWithMembers).GetEvent(nameof(BaseClassWithMembers.BaseEvent))!;

				async Task Act()
				{
					await That(subject).IsNotVirtual();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not virtual,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenEventIsVirtual_ShouldFail()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).IsNotVirtual();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not virtual,
					              but it was virtual {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventIsNotVirtual_ShouldFail()
			{
				EventInfo subject =
					typeof(BaseClassWithMembers).GetEvent(nameof(BaseClassWithMembers.BaseEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotVirtual());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is virtual,
					             but it was non-virtual event EventHandler BaseClassWithMembers.BaseEvent
					             """);
			}

			[Fact]
			public async Task WhenEventIsVirtual_ShouldSucceed()
			{
				EventInfo subject =
					typeof(AbstractClassWithMembers).GetEvent(nameof(AbstractClassWithMembers.VirtualEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotVirtual());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
