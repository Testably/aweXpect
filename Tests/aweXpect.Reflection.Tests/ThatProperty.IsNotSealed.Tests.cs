using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotSealed
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsNotSealed_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).IsNotSealed();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsSealed_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithSealedMembers).GetProperty(nameof(ClassWithSealedMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).IsNotSealed();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not sealed,
					              but it was sealed public string ClassWithSealedMembers.VirtualProperty { get; set; }
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsNotSealed_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotSealed());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is sealed,
					             but it was non-sealed public string AbstractClassWithMembers.VirtualProperty { get; set; }
					             """);
			}

			[Test]
			public async Task WhenPropertyIsSealed_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithSealedMembers).GetProperty(nameof(ClassWithSealedMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotSealed());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
