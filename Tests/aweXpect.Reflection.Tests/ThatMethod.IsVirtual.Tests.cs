using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsVirtual
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodIsNotVirtual_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.RegularMethod))!;

				async Task Act()
				{
					await That(subject).IsVirtual();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is virtual,
					              but it was non-virtual void AbstractClassWithMembers.RegularMethod()
					              """);
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsVirtual();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is virtual,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodIsVirtual_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).IsVirtual();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodIsNotVirtual_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.RegularMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsVirtual());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsVirtual_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsVirtual());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not virtual,
					             but it was virtual void AbstractClassWithMembers.VirtualMethod()
					             """);
			}
		}
	}
}
