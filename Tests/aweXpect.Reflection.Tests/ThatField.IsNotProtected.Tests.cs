using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("InternalField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			[Arguments("ProtectedInternalField")]
			[Arguments("PrivateProtectedField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldInfoIsProtected_ShouldFail()
			{
				FieldInfo? subject = GetField("ProtectedField");

				async Task Act()
				{
					await That(subject).IsNotProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not protected,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("InternalField")]
			[Arguments("PublicField")]
			[Arguments("PrivateField")]
			[Arguments("ProtectedInternalField")]
			[Arguments("PrivateProtectedField")]
			public async Task WhenFieldInfoIsNotProtected_ShouldFail(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is protected,
					              but it was {GetExpectedAccessModifier(fieldName)}
					              """);
			}

			[Test]
			public async Task WhenFieldInfoIsProtected_ShouldSucceed()
			{
				FieldInfo? subject = GetField("ProtectedField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotProtected());
				}

				await That(Act).DoesNotThrow();
			}

			private static string GetExpectedAccessModifier(string fieldName) => fieldName switch
			{
				"PublicField" => "public",
				"PrivateField" => "private",
				"InternalField" => "internal",
				"ProtectedInternalField" => "protected internal",
				"PrivateProtectedField" => "private protected",
				_ => throw new ArgumentException($"Unknown field name: {fieldName}"),
			};
		}
	}
}
