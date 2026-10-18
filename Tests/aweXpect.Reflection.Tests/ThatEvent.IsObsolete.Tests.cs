using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatEvent
{
	public sealed class IsObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventIsNotObsolete_ShouldFail()
			{
				EventInfo subject =
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete event EventHandler ClassWithObsoleteMembers.NonObsoleteEvent
					              """);
			}

			[Test]
			public async Task WhenEventIsNull_ShouldFail()
			{
				EventInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is obsolete,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenEventIsObsolete_ShouldSucceed()
			{
				EventInfo subject =
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsNotObsolete_ShouldSucceed()
			{
				EventInfo subject =
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.NonObsoleteEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsObsolete_ShouldFail()
			{
				EventInfo subject =
					typeof(ClassWithObsoleteMembers).GetEvent(nameof(ClassWithObsoleteMembers.ObsoleteEvent))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete event EventHandler ClassWithObsoleteMembers.ObsoleteEvent
					              """);
			}
		}
	}
}
