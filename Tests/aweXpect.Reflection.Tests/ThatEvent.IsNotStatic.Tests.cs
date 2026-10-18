using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsNotStatic
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventIsNotStatic_ShouldSucceed()
			{
				EventInfo subject =
					typeof(TestClassWithStaticMembers).GetEvent(nameof(TestClassWithStaticMembers.NonStaticEvent))!;

				async Task Act()
				{
					await That(subject).IsNotStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotStatic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not static,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventIsStatic_ShouldFail()
			{
				EventInfo subject =
					typeof(TestClassWithStaticMembers).GetEvent(nameof(TestClassWithStaticMembers.StaticEvent))!;

				async Task Act()
				{
					await That(subject).IsNotStatic();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not static,
					              but it was static event EventHandler TestClassWithStaticMembers.StaticEvent
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsNotStatic_ShouldFail()
			{
				EventInfo subject =
					typeof(TestClassWithStaticMembers).GetEvent(nameof(TestClassWithStaticMembers.NonStaticEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotStatic());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is static,
					              but it was non-static event EventHandler TestClassWithStaticMembers.NonStaticEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsStatic_ShouldSucceed()
			{
				EventInfo subject =
					typeof(TestClassWithStaticMembers).GetEvent(nameof(TestClassWithStaticMembers.StaticEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotStatic());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
