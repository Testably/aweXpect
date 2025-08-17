using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

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

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsARecordStruct_ShouldFail()
			{
				Type subject = typeof(PublicRecordStruct);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsARecordStruct());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a record struct,
					             but it was record struct PublicRecordStruct
					             """);
			}

			[Theory]
			[MemberData(nameof(NonRecordStructTypeForNegated))]
			public async Task WhenTypeIsNotARecordStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsARecordStruct());

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonRecordStructTypeForNegated() => new()
			{
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
				typeof(PublicRecord),
				typeof(PublicStruct),
			};

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
