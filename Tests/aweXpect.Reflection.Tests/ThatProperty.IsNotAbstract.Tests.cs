using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotAbstract
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsAbstract_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.AbstractProperty))!;

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not abstract,
					              but it was abstract public string AbstractClassWithMembers.AbstractProperty { get; set; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsNotAbstract_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).IsNotAbstract();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsAbstract_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.AbstractProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNotAbstract_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotAbstract());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is abstract,
					             but it was non-abstract public string AbstractClassWithMembers.VirtualProperty { get; set; }
					             """);
			}
		}
	}
}
