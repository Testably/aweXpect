using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotPrivateProtected
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("InternalField")]
			[Arguments("PrivateField")]
			[Arguments("ProtectedInternalField")]
			public async Task WhenFieldInfoIsNotPrivateProtected_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldInfoIsPrivateProtected_ShouldFail()
			{
				FieldInfo? subject = GetField("PrivateProtectedField");

				async Task Act()
				{
					await That(subject).IsNotPrivateProtected();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not private protected,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("PublicField")]
			[Arguments("InternalField")]
			[Arguments("PrivateField")]
			[Arguments("ProtectedInternalField")]
			public async Task WhenFieldInfoIsNotPrivateProtected_ShouldFail(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is private protected,
					              but it was {GetExpectedAccessModifier(fieldName)}
					              """);
			}

			[Test]
			public async Task WhenFieldInfoIsPrivateProtected_ShouldSucceed()
			{
				FieldInfo? subject = GetField("PrivateProtectedField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPrivateProtected());
				}

				await That(Act).DoesNotThrow();
			}

			private static string GetExpectedAccessModifier(string fieldName) => fieldName switch
			{
				"ProtectedField" => "protected",
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
