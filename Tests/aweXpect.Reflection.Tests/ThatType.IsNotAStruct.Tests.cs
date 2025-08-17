using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotAStruct
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAStruct_ShouldFail()
			{
				Type subject = typeof(PublicStruct);

				async Task Act()
					=> await That(subject).IsNotAStruct();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a struct,
					             but it was struct PublicStruct
					             """);
			}

			[Theory]
			[MemberData(nameof(StructTypesForNegated))]
			public async Task WhenTypeIsAStruct_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAStruct());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotAStruct_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAStruct());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a struct,
					             but it was class PublicClass
					             """);
			}

			[Theory]
			[MemberData(nameof(NonStructTypes))]
			public async Task WhenTypeIsNotAStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).IsNotAStruct();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotAStruct();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a struct,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> StructTypesForNegated() => new()
			{
				typeof(PublicStruct),
			};

			public static TheoryData<Type> NonStructTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicClass),
					typeof(PublicRecord),
					typeof(PublicRecordStruct),
				];
		}
	}
}
