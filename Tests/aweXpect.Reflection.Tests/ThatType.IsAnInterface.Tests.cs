using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAnInterface
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnInterface_ShouldSucceed()
			{
				Type subject = typeof(IPublicInterface);

				async Task Act()
					=> await That(subject).IsAnInterface();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonInterfaceType))]
			public async Task WhenTypeIsNotAnInterface_ShouldFail(Type? subject, string name)
			{
				async Task Act()
					=> await That(subject).IsAnInterface();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is an interface,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsAnInterface();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is an interface,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonInterfaceType() => new()
			{
				{
					null, "<null>"
				},
				{
					typeof(PublicStruct), "struct PublicStruct"
				},
				{
					typeof(PublicClass), "class PublicClass"
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
