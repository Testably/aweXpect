using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodIsNotObsolete_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete void ClassWithObsoleteMembers.NonObsoleteMethod()
					              """);
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

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
			public async Task WhenMethodIsObsolete_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!;

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
			public async Task WhenMethodIsNotObsolete_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.NonObsoleteMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsObsolete_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithObsoleteMembers).GetMethod(nameof(ClassWithObsoleteMembers.ObsoleteMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete void ClassWithObsoleteMembers.ObsoleteMethod()
					              """);
			}
		}
	}
}
