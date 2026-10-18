using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotAbstract
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodIsAbstract_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.AbstractMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not abstract,
					              but it was abstract void AbstractClassWithMembers.AbstractMethod()
					              """);
			}

			[Test]
			public async Task WhenMethodIsNotAbstract_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not abstract,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodIsAbstract_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.AbstractMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNotAbstract_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is abstract,
					             but it was non-abstract void AbstractClassWithMembers.VirtualMethod()
					             """);
			}
		}
	}
}
