using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsARecord
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsARecord_ShouldSucceed()
			{
				Type subject = typeof(PublicRecord);

				async Task Act()
				{
					await That(subject).IsARecord();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonRecordType))]
			public async Task WhenTypeIsNotARecord_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsARecord();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is a record,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsARecord();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is a record,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonRecordType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(PublicRecordStruct), "record struct PublicRecordStruct"),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsARecord_ShouldFail()
			{
				Type subject = typeof(PublicRecord);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsARecord());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a record,
					             but it was record PublicRecord
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonRecordTypeForNegated))]
			public async Task WhenTypeIsNotARecord_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsARecord());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonRecordTypeForNegated() => new List<Type>
			{
				typeof(PublicClass),
				typeof(IPublicInterface),
				typeof(PublicEnum),
				typeof(PublicStruct),
			};

			public static IEnumerable<(Type?, string)> NonRecordType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicClass), "class PublicClass"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicStruct), "struct PublicStruct"),
			};
		}
	}
}
