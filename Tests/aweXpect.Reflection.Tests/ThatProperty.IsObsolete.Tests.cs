using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CS0612, CS0618 // Intentional reference to an obsolete test fixture member
public sealed partial class ThatProperty
{
	public sealed class IsObsolete
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsNotObsolete_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!;

				async Task Act()
				{
					await That(subject).IsObsolete();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete public int ClassWithObsoleteMembers.NonObsoleteProperty { get; set; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsObsolete_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!;

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
			public async Task WhenPropertyIsNotObsolete_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.NonObsoleteProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsObsolete_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithObsoleteMembers).GetProperty(nameof(ClassWithObsoleteMembers.ObsoleteProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsObsolete());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete public int ClassWithObsoleteMembers.ObsoleteProperty { get; set; }
					              """);
			}
		}
	}
}
