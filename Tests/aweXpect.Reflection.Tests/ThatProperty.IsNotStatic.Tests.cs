using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsNotStatic
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenPropertyIsNotStatic_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(TestClassWithStaticMembers).GetProperty(nameof(TestClassWithStaticMembers.NonStaticProperty))
					!;

				async Task Act()
				{
					await That(subject).IsNotStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotStatic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not static,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenPropertyIsStatic_ShouldFail()
			{
				PropertyInfo subject =
					typeof(TestClassWithStaticMembers).GetProperty(nameof(TestClassWithStaticMembers.StaticProperty))!;

				async Task Act()
				{
					await That(subject).IsNotStatic();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not static,
					              but it was static public string TestClassWithStaticMembers.StaticProperty { get; set; }
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenPropertyIsNotStatic_ShouldFail()
			{
				PropertyInfo subject =
					typeof(TestClassWithStaticMembers).GetProperty(nameof(TestClassWithStaticMembers.NonStaticProperty))
					!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotStatic());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is static,
					              but it was non-static public string TestClassWithStaticMembers.NonStaticProperty { get; set; }
					              """);
			}

			[Test]
			public async Task WhenPropertyIsStatic_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(TestClassWithStaticMembers).GetProperty(nameof(TestClassWithStaticMembers.StaticProperty))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotStatic());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
