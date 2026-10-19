using System.Reflection;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotConstant
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldIsConstant_ShouldFail()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!;

				async Task Act()
				{
					await That(subject).IsNotConstant();
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is not constant,
					              but it was constant int TestClassWithFieldModifiers.ConstantField
					              """);
			}

			[Test]
			public async Task WhenFieldIsNotConstant_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!;

				async Task Act()
				{
					await That(subject).IsNotConstant();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotConstant();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not constant,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldIsConstant_ShouldSucceed()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.ConstantField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotConstant());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldIsNotConstant_ShouldFail()
			{
				FieldInfo subject =
					typeof(TestClassWithFieldModifiers).GetField(nameof(TestClassWithFieldModifiers.MutableField))!;

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotConstant());
				}

				await That(Act).Throws()
					.WithMessage("""
					              Expected that subject
					              is constant,
					              but it was non-constant int TestClassWithFieldModifiers.MutableField
					              """);
			}
		}
	}
}
