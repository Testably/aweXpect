using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatConstructor
{
	public sealed class IsStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenConstructorIsNotStatic_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClassWithStaticMembers).GetConstructors().First(c => !c.IsStatic);

				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is static,
					              but it was non-static {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenConstructorIsNull_ShouldFail()
			{
				ConstructorInfo? subject = null;

				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is static,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenConstructorIsStatic_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic).First();

				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenConstructorIsNotStatic_ShouldSucceed()
			{
				ConstructorInfo subject = typeof(TestClassWithStaticMembers).GetConstructors().First(c => !c.IsStatic);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsStatic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenConstructorIsStatic_ShouldFail()
			{
				ConstructorInfo subject = typeof(TestClassWithStaticMembers)
					.GetConstructors(BindingFlags.Static | BindingFlags.NonPublic).First();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsStatic());

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not static,
					              but it was static {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
