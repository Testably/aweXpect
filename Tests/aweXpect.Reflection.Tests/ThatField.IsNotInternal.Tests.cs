using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenFieldInfoIsInternal_ShouldFail()
			{
				FieldInfo? subject = GetField("InternalField");

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was
					             """);
			}

			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotInternal_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
					=> await That(subject).IsNotInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not internal,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenFieldInfoIsInternal_ShouldSucceed()
			{
				FieldInfo? subject = GetField("InternalField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInternal());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotInternal_ShouldFail(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is internal,
					              but it was {GetExpectedAccessModifier(fieldName)}
					              """);
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
