using System.Reflection;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatField
{
	public sealed class IsProtectedInternal
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("ProtectedField", "protected")]
			[InlineData("PublicField", "public")]
			[InlineData("PrivateField", "private")]
			public async Task WhenFieldInfoIsNotProtectedInternal_ShouldFail(string fieldName,
				string expectedAccessModifier)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
					=> await That(subject).IsProtectedInternal();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is protected internal,
					              but it was {expectedAccessModifier}
					              """);
			}

			[Fact]
			public async Task WhenFieldInfoIsNull_ShouldFail()
			{
				FieldInfo? subject = null;

				async Task Act()
					=> await That(subject).IsProtectedInternal();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is protected internal,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenFieldInfoIsProtectedInternal_ShouldSucceed()
			{
				FieldInfo? subject = GetField("ProtectedInternalField");

				async Task Act()
					=> await That(subject).IsProtectedInternal();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData("ProtectedField")]
			[InlineData("PublicField")]
			[InlineData("PrivateField")]
			public async Task WhenFieldInfoIsNotProtectedInternal_ShouldSucceed(string fieldName)
			{
				FieldInfo? subject = GetField(fieldName);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsProtectedInternal());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenFieldInfoIsProtectedInternal_ShouldFail()
			{
				FieldInfo? subject = GetField("ProtectedInternalField");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsProtectedInternal());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not protected internal,
					             but it was
					             """);
			}
		}
	}
}
