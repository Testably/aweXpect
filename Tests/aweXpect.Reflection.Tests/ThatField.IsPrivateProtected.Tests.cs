using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsPrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedField", "protected")]
			[Arguments("PublicField", "public")]
			[Arguments("InternalField", "internal")]
			public async Task WhenFieldInfoIsNotPrivateProtected_ShouldFail(string fieldName,
				string expectedAccessModifier)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is private protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldInfoIsPrivateProtected_ShouldSucceed()
			{
				FieldInfo? subject = GetField("PrivateProtectedField");

				async Task Act()
				{
					await That(subject).IsPrivateProtected();
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
			public async Task WhenFieldInfoIsNotPrivateProtected_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsPrivateProtected_ShouldFail()
			{
				FieldInfo? subject = GetField("PrivateProtectedField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}
		}
	}
}
