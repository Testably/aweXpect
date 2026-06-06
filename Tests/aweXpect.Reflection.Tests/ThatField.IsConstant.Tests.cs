using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsConstant
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldIsConstant_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!;

				async Task Act()
				{
					await That(subject).IsConstant();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldIsNotConstant_ShouldFail()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!;

				async Task Act()
				{
					await That(subject).IsConstant();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is constant,
					              but it was non-constant {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsConstant();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is constant,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldIsConstant_ShouldFail()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsConstant());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is not constant,
					              but it was constant {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenFieldIsNotConstant_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsConstant());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
