using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotARecordStruct
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsARecordStruct_ShouldFail()
			{
				Type subject = typeof(PublicRecordStruct);

				async Task Act()
					=> await That(subject).IsNotARecordStruct();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a record struct,
					             but it was record struct PublicRecordStruct
					             """);
			}

			[Theory]
			[MemberData(nameof(RecordStructTypesForNegated))]
			public async Task WhenTypeIsARecordStruct_ShouldSucceedWithNegatedAssertion(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotARecordStruct());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNotARecordStruct_ShouldFailWithNegatedAssertion()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotARecordStruct());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is a record struct,
					             but it was class PublicClass
					             """);
			}

			[Theory]
			[MemberData(nameof(NonRecordStructTypes))]
			public async Task WhenTypeIsNotARecordStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).IsNotARecordStruct();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotARecordStruct();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a record struct,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> RecordStructTypesForNegated() => new()
			{
				typeof(PublicRecordStruct),
			};

			public static TheoryData<Type> NonRecordStructTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicClass),
					typeof(PublicRecord),
					typeof(PublicStruct),
				];
		}
	}
}
