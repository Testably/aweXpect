using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedField", "protected")]
			[Arguments("InternalField", "internal")]
			[Arguments("PrivateField", "private")]
			public async Task WhenFieldInfoIsNotPublic_ShouldFail(string fieldName, string expectedAccessModifier)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldInfoIsPublic_ShouldSucceed()
			{
				FieldInfo? subject = GetField("PublicField");

				async Task Act()
				{
					await That(subject).IsPublic();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("InternalField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotPublic_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsPublic_ShouldFail()
			{
				FieldInfo? subject = GetField("PublicField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}
		}
	}
}
