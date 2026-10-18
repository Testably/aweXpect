using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotSealed
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodIsNotSealed_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).IsNotSealed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not sealed,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenMethodIsSealed_ShouldFail()
			{
				MethodInfo subject =
					typeof(ClassWithSealedMembers).GetMethod(nameof(ClassWithSealedMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).IsNotSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not sealed,
					              but it was sealed void ClassWithSealedMembers.VirtualMethod()
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodIsNotSealed_ShouldFail()
			{
				MethodInfo subject =
					typeof(AbstractClassWithMembers).GetMethod(nameof(AbstractClassWithMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotSealed());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was non-sealed void AbstractClassWithMembers.VirtualMethod()
					             """);
			}

			[Test]
			public async Task WhenMethodIsSealed_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(ClassWithSealedMembers).GetMethod(nameof(ClassWithSealedMembers.VirtualMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotSealed());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
