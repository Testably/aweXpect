using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsRequired
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsNotRequired_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.OptionalProperty))!;

				async Task Act()
				{
					await That(subject).IsRequired();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is required,
					              but it was non-required public string ClassWithRequiredMembers.OptionalProperty { get; set; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsRequired();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is required,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyIsRequired_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.RequiredProperty))!;

				async Task Act()
				{
					await That(subject).IsRequired();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsNotRequired_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.OptionalProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsRequired());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsRequired_ShouldFail()
			{
				PropertyInfo subject =
					typeof(ClassWithRequiredMembers).GetProperty(nameof(ClassWithRequiredMembers.RequiredProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsRequired());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					              Expected that subject
					              is not required,
					              but it was required public string ClassWithRequiredMembers.RequiredProperty { get; set; }
					              """);
			}
		}
	}
}
