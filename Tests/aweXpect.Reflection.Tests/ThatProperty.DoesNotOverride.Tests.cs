using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class DoesNotOverride
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyDoesNotOverride_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotOverride();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).DoesNotOverride();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             does not override a base property,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyOverrides_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithSealedMembers).GetProperty(nameof(ClassWithSealedMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotOverride();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              does not override a base property,
					              but it did override a base property public string ClassWithSealedMembers.VirtualProperty { get; set; }
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyDoesNotOverride_ShouldFail()
			{
				PropertyInfo subject =
					typeof(AbstractClassWithMembers).GetProperty(nameof(AbstractClassWithMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotOverride());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             overrides a base property,
					             but it did not override a base property public string AbstractClassWithMembers.VirtualProperty { get; set; }
					             """);
			}

			[Test]
			public async Task WhenPropertyOverrides_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithSealedMembers).GetProperty(nameof(ClassWithSealedMembers.VirtualProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.DoesNotOverride());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
