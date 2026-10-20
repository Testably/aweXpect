using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatMethod
{
	public sealed class IsNotStatic
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenMethodIsNotStatic_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(TestClassWithStaticMembers).GetMethod(nameof(TestClassWithStaticMembers.NonStaticMethod))!;

				async Task Act()
				{
					await That(subject).IsNotStatic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMethodIsNull_ShouldFail()
			{
				MethodInfo? subject = null;

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
			public async Task WhenMethodIsStatic_ShouldFail()
			{
				MethodInfo subject =
					typeof(TestClassWithStaticMembers).GetMethod(nameof(TestClassWithStaticMembers.StaticMethod))!;

				async Task Act()
				{
					await That(subject).IsNotStatic();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not static,
					              but it was static void TestClassWithStaticMembers.StaticMethod()
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenMethodIsNotStatic_ShouldFail()
			{
				MethodInfo subject =
					typeof(TestClassWithStaticMembers).GetMethod(nameof(TestClassWithStaticMembers.NonStaticMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotStatic());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is static,
					              but it was non-static void TestClassWithStaticMembers.NonStaticMethod()
					              """);
			}

			[Test]
			public async Task WhenMethodIsStatic_ShouldSucceed()
			{
				MethodInfo subject =
					typeof(TestClassWithStaticMembers).GetMethod(nameof(TestClassWithStaticMembers.StaticMethod))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotStatic());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
