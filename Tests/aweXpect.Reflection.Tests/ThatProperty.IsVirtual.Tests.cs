using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsVirtual
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsNotVirtual_ShouldFail()
			{
				PropertyInfo subject =
					typeof(BaseClassWithMembers).GetProperty(nameof(BaseClassWithMembers.BaseProperty))!;

				async Task Act()
				{
					await That(subject).IsVirtual();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is virtual,
					              but it was non-virtual public int BaseClassWithMembers.BaseProperty { get; set; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsVirtual_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

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
			public async Task WhenPropertyIsNotVirtual_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(BaseClassWithMembers).GetProperty(nameof(BaseClassWithMembers.BaseProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsVirtual());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsVirtual_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsVirtual());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not virtual,
					             but it was virtual public string AbstractClassWithMembers.VirtualProperty { get; set; }
					             """);
			}
		}
	}
}
