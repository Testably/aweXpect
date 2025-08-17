using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

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

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsAnInterface_ShouldFail()
			{
				Type subject = typeof(IPublicInterface);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAnInterface());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an interface,
					             but it was interface IPublicInterface
					             """);
			}

			[Theory]
			[MemberData(nameof(NonInterfaceTypeForNegated))]
			public async Task WhenTypeIsNotAnInterface_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAnInterface());

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonInterfaceTypeForNegated() => new()
			{
				typeof(PublicStruct),
				typeof(PublicClass),
				typeof(PublicEnum),
				typeof(PublicRecord),
				typeof(PublicRecordStruct),
			};

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
