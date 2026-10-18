using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsAClass
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenTypeIsAClass_ShouldSucceed()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).IsAClass();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(NonClassType))]
			public async Task WhenTypeIsNotAClass_ShouldFail(Type? subject, string name)
			{
				async Task Act()
				{
					await That(subject).IsAClass();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              is a class,
					              but it was {name}
					              """);
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsAClass();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is a class,
					             but it was <null>
					             """);
			}

			public static IEnumerable<(Type?, string)> NonClassType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicRecord), "record PublicRecord"),
				(typeof(PublicRecordStruct), "record struct PublicRecordStruct"),
			};
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenTypeIsAClass_ShouldFail()
			{
				Type subject = typeof(PublicClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAClass());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not a class,
					             but it was class PublicClass
					             """);
			}

			[Test]
			[MethodDataSource(nameof(NonClassTypeForNegated))]
			public async Task WhenTypeIsNotAClass_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsAClass());
				}

				await That(Act).DoesNotThrow();
			}

			public static IEnumerable<Type> NonClassTypeForNegated() => new List<Type>
			{
				typeof(PublicStruct),
				typeof(IPublicInterface),
				typeof(PublicEnum),
				typeof(PublicRecord),
			};

			public static IEnumerable<(Type?, string)> NonClassType() => new List<(Type?, string)>
			{
				(null, "<null>"),
				(typeof(PublicStruct), "struct PublicStruct"),
				(typeof(IPublicInterface), "interface IPublicInterface"),
				(typeof(PublicEnum), "enum PublicEnum"),
				(typeof(PublicRecord), "record PublicRecord"),
			};
		}
	}
}
