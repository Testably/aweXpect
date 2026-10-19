using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsInternal
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenFieldInfoIsInternal_ShouldSucceed()
			{
				FieldInfo? subject = GetField("InternalField");

				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("ProtectedField", "protected")]
			[Arguments("PublicField", "public")]
			[Arguments("PrivateField", "private")]
			public async Task WhenFieldInfoIsNotInternal_ShouldFail(string fieldName, string expectedAccessModifier)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsInternal();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is internal,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenFieldInfoIsInternal_ShouldFail()
			{
				FieldInfo? subject = GetField("InternalField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsInternal());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was
					             """);
			}

			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotInternal_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsInternal());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
