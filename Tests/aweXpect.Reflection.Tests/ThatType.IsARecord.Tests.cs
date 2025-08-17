using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

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

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsARecord_ShouldFail()
			{
				Type subject = typeof(PublicRecord);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsARecord());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a record,
					             but it was record PublicRecord
					             """);
			}

			[Theory]
			[MemberData(nameof(NonRecordTypeForNegated))]
			public async Task WhenTypeIsNotARecord_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsARecord());

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonRecordTypeForNegated() => new()
			{
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
				typeof(PublicStruct),
			};

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
			};
		}
	}
}
