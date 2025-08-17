using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsARecord
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsARecord_ShouldSucceed()
			{
				Type subject = typeof(PublicRecord);

				async Task Act()
					=> await That(subject).IsARecord();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonRecordType))]
			public async Task WhenTypeIsNotARecord_ShouldFail(Type? subject, string name)
			{
				async Task Act()
					=> await That(subject).IsARecord();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is a record,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsARecord();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is a record,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonRecordType() => new()
			{
				{
					null, "<null>"
				},
				{
					typeof(PublicClass), "class PublicClass"
				},
				{
					typeof(IPublicInterface), "interface IPublicInterface"
				},
				{
					typeof(PublicEnum), "enum PublicEnum"
				},
				{
					typeof(PublicStruct), "struct PublicStruct"
				},
				{
					typeof(PublicRecordStruct), "record struct PublicRecordStruct"
				},
			};
		}
	}
}
