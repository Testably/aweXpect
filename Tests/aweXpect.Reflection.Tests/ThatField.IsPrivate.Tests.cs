using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsPrivate
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedField", "protected")]
			[Arguments("PublicField", "public")]
			[Arguments("InternalField", "internal")]
			public async Task WhenFieldInfoIsNotPrivate_ShouldFail(string fieldName, string expectedAccessModifier)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).IsPrivate();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsPrivate();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is private,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldInfoIsPrivate_ShouldSucceed()
			{
				FieldInfo? subject = GetField("PrivateField");

				async Task Act()
				{
					await That(subject).IsPrivate();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("InternalField")]
			public async Task WhenFieldInfoIsNotPrivate_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivate());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsPrivate_ShouldFail()
			{
				FieldInfo? subject = GetField("PrivateField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivate());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private,
					             but it was
					             """);
			}
		}
	}
}
