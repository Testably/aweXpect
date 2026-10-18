using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CS0612, CS0618 // Intentional reference to an obsolete test fixture member
public sealed partial class ThatField
{
	public sealed class IsObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldIsNotObsolete_ShouldFail()
			{
				FieldInfo subject =
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete int ClassWithObsoleteMembers.NonObsoleteField
					              """);
			}

			[Test]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

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
			public async Task WhenFieldIsObsolete_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!;

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
			public async Task WhenFieldIsNotObsolete_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldIsObsolete_ShouldFail()
			{
				FieldInfo subject =
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete int ClassWithObsoleteMembers.ObsoleteField
					              """);
			}
		}
	}
}
