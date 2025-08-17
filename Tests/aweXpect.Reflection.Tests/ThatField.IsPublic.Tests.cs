using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsPublic
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedField", "protected")]
			[InlineData("InternalField", "internal")]
			[InlineData("PrivateField", "private")]
			public async Task WhenFieldInfoIsNotPublic_ShouldFail(string fieldName, string expectedAccessModifier)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is public,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is public,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenFieldInfoIsPublic_ShouldSucceed()
			{
				FieldInfo? subject = GetField("PublicField");

				async Task Act()
					=> await That(subject).IsPublic();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("InternalField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotPublic_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsPublic());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsPublic_ShouldFail()
			{
				FieldInfo? subject = GetField("PublicField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsPublic());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not public,
					             but it was
					             """);
			}
		}
	}
}
