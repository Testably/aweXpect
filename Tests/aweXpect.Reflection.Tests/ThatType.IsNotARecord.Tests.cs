using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsNotARecord
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsARecord_ShouldFail()
			{
				Type subject = typeof(PublicRecord);

				async Task Act()
					=> await That(subject).IsNotARecord();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a record,
					             but it was record PublicRecord
					             """);
			}

			[Theory]
			[MemberData(nameof(NonRecordTypes))]
			public async Task WhenTypeIsNotARecord_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).IsNotARecord();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsNotARecord();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not a record,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> NonRecordTypes()
				=>
				[
					typeof(IPublicInterface),
					typeof(PublicEnum),
					typeof(PublicClass),
					typeof(PublicStruct),
					typeof(PublicRecordStruct),
				];
		}
	}
}
