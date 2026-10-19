using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalField", "internal")]
			[Arguments("PublicField", "public")]
			[Arguments("PrivateField", "private")]
			public async Task WhenFieldInfoIsNotProtected_ShouldFail(string fieldName, string expectedAccessModifier)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Test]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldInfoIsProtected_ShouldSucceed()
			{
				FieldInfo? subject = GetField("ProtectedField");

				async Task Act()
				{
					await That(subject).IsProtected();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtected());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsProtected_ShouldFail()
			{
				FieldInfo? subject = GetField("ProtectedField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}
		}
	}
}
