using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAnEnum
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeIsAnEnum_ShouldSucceed()
			{
				Type subject = typeof(PublicEnum);

				async Task Act()
					=> await That(subject).IsAnEnum();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(NonEnumType))]
			public async Task WhenTypeIsNotAnEnum_ShouldFail(Type? subject, string name)
			{
				async Task Act()
					=> await That(subject).IsAnEnum();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              is an enum,
					              but it was {name}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
					=> await That(subject).IsAnEnum();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is an enum,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type?, string> NonEnumType() => new()
			{
				{
					null, "<null>"
				},
				{
					typeof(IPublicInterface), "interface IPublicInterface"
				},
				{
					typeof(PublicStruct), "struct PublicStruct"
				},
				{
					typeof(PublicClass), "class PublicClass"
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
			public async Task WhenTypeIsAnEnum_ShouldFail()
			{
				Type subject = typeof(PublicEnum);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAnEnum());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not an enum,
					             but it was enum PublicEnum
					             """);
			}

			[Theory]
			[MemberData(nameof(NonEnumTypeForNegated))]
			public async Task WhenTypeIsNotAnEnum_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAnEnum());

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonEnumTypeForNegated() => new()
			{
				typeof(IPublicInterface),
				typeof(PublicStruct),
				typeof(PublicClass),
				typeof(PublicRecord),
				typeof(PublicRecordStruct),
			};

			public static TheoryData<Type?, string> NonEnumType() => new()
			{
				{
					null, "<null>"
				},
				{
					typeof(IPublicInterface), "interface IPublicInterface"
				},
				{
					typeof(PublicStruct), "struct PublicStruct"
				},
				{
					typeof(PublicClass), "class PublicClass"
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
