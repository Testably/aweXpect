using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsARecordStruct
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsARecordStruct_ShouldSucceed()
			{
				Type subject = typeof(PublicRecordStruct);

				async Task Act()
					=> await That(subject).IsARecordStruct();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonRecordStructType))]
			public async Task WhenTypeIsNotARecordStruct_ShouldFail(Type? subject, string name)
			{
				async Task Act()
					=> await That(subject).IsARecordStruct();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is a record struct,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsARecordStruct();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is a record struct,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonRecordStructType() => new()
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
					typeof(PublicRecord), "record PublicRecord"
				},
				{
					typeof(PublicStruct), "struct PublicStruct"
				},
			};
		}
	}
}
