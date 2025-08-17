using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatProperty
{
	public sealed class IsStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenPropertyIsNotStatic_ShouldFail()
			{
				PropertyInfo subject =
					typeof(TestClassWithStaticMembers).GetProperty(nameof(TestClassWithStaticMembers.NonStaticProperty))
					!;

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
			public async Task WhenPropertyIsNull_ShouldFail()
			{
				PropertyInfo? subject = null;

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
			public async Task WhenPropertyIsStatic_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(TestClassWithStaticMembers).GetProperty(nameof(TestClassWithStaticMembers.StaticProperty))!;

				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenPropertyIsNotStatic_ShouldSucceed()
			{
				PropertyInfo subject =
					typeof(TestClassWithStaticMembers).GetProperty(nameof(TestClassWithStaticMembers.NonStaticProperty))
					!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsStatic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyIsStatic_ShouldFail()
			{
				PropertyInfo subject =
					typeof(TestClassWithStaticMembers).GetProperty(nameof(TestClassWithStaticMembers.StaticProperty))!;

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
