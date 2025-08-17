using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldIsNotStatic_ShouldFail()
			{
				FieldInfo subject =
					typeof(TestClassWithStaticMembers).GetField(nameof(TestClassWithStaticMembers.NonStaticField))!;

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
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

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
			public async Task WhenFieldIsStatic_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(TestClassWithStaticMembers).GetField(nameof(TestClassWithStaticMembers.StaticField))!;

				async Task Act()
					=> await That(subject).IsStatic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldIsNotStatic_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(TestClassWithStaticMembers).GetField(nameof(TestClassWithStaticMembers.NonStaticField))!;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsStatic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsStatic_ShouldFail()
			{
				FieldInfo subject =
					typeof(TestClassWithStaticMembers).GetField(nameof(TestClassWithStaticMembers.StaticField))!;

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
