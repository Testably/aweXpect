using System.Reflection;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotPublic
	{
		public sealed class Tests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("InternalField")]
			[Arguments("PrivateField")]
			[Arguments("ProtectedInternalField")]
			[Arguments("PrivateProtectedField")]
			public async Task WhenFieldInfoIsNotPublic_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).IsNotPublic();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
				{
					await That(subject).IsNotPublic();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenFieldInfoIsPublic_ShouldFail()
			{
				FieldInfo? subject = GetField("PublicField");

				async Task Act()
				{
					await That(subject).IsNotPublic();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments("ProtectedField")]
			[Arguments("InternalField")]
			[Arguments("PrivateField")]
			[Arguments("ProtectedInternalField")]
			[Arguments("PrivateProtectedField")]
			public async Task WhenFieldInfoIsNotPublic_ShouldFail(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPublic());
				}

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {GetExpectedAccessModifier(fieldName)}
					              """);
			}

			[Test]
			public async Task WhenFieldInfoIsPublic_ShouldSucceed()
			{
				FieldInfo? subject = GetField("PublicField");

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsNotPublic());
				}

				await That(Act).DoesNotThrow();
			}

			private static string GetExpectedAccessModifier(string fieldName) => fieldName switch
			{
				"ProtectedField" => "protected",
				"PrivateField" => "private",
				"InternalField" => "internal",
				"ProtectedInternalField" => "protected internal",
				"PrivateProtectedField" => "private protected",
				_ => throw new ArgumentException($"Unknown field name: {fieldName}"),
			};
		}
	}
}
