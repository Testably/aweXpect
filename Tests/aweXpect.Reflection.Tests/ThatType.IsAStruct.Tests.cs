using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

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

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsAStruct_ShouldFail()
			{
				Type subject = typeof(PublicStruct);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAStruct());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not a struct,
					             but it was struct PublicStruct
					             """);
			}

			[Theory]
			[MemberData(nameof(NonStructTypeForNegated))]
			public async Task WhenTypeIsNotAStruct_ShouldSucceed(Type subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAStruct());

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> NonStructTypeForNegated() => new()
			{
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
				typeof(PublicRecord),
			};

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
			};
		}
	}
}
