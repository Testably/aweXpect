using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAStruct
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAStruct_ShouldSucceed()
			{
				Type subject = typeof(PublicStruct);

				async Task Act()
					=> await That(subject).IsAStruct();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonStructType))]
			public async Task WhenTypeIsNotAStruct_ShouldFail(Type? subject, string name)
			{
				async Task Act()
					=> await That(subject).IsAStruct();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is a struct,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsAStruct();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is a struct,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonStructType() => new()
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
					typeof(PublicRecordStruct), "record struct PublicRecordStruct"
				},
			};
		}
	}
}
