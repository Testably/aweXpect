using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAClass
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAClass_ShouldSucceed()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
					=> await That(subject).IsAClass();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonClassType))]
			public async Task WhenTypeIsNotAClass_ShouldFail(Type? subject, string name)
			{
				async Task Act()
					=> await That(subject).IsAClass();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is a class,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsAClass();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is a class,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonClassType() => new()
			{
				{
					null, "<null>"
				},
				{
					typeof(PublicStruct), "struct PublicStruct"
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
