using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

#pragma warning disable CS0612, CS0618 // Intentional reference to an obsolete test fixture member
public sealed partial class ThatField
{
	public sealed class IsNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldIsNotObsolete_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!;

				async Task Act()
				{
					await That(subject).IsNotObsolete();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not obsolete,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenFieldIsObsolete_ShouldFail()
			{
				FieldInfo subject =
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!;

				async Task Act()
				{
					await That(subject).IsNotObsolete();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not obsolete,
					              but it was obsolete {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldIsNotObsolete_ShouldFail()
			{
				FieldInfo subject =
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.NonObsoleteField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotObsolete());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is obsolete,
					              but it was non-obsolete {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenFieldIsObsolete_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(ClassWithObsoleteMembers).GetField(nameof(ClassWithObsoleteMembers.ObsoleteField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotObsolete());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
