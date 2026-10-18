using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class HasADefaultConstructor
	{
		public sealed class Tests
		{
			[Test]
			[MethodDataSource(nameof(TypesWithoutDefaultConstructor))]
			public async Task WhenTypeDoesNotHaveADefaultConstructor_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).HasADefaultConstructor();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              has a default constructor,
					              but it did not have a default constructor {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[MethodDataSource(nameof(TypesWithDefaultConstructor))]
			public async Task WhenTypeHasADefaultConstructor_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).HasADefaultConstructor();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasADefaultConstructor();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             has a default constructor,
					             but it was <null>
					             """);
			}

			public static IEnumerable<Type> TypesWithDefaultConstructor()
				=> new List<Type>
				{
					typeof(PublicClass),
					typeof(PublicSealedClass),
					typeof(PublicStruct),
				};

			public static IEnumerable<Type> TypesWithoutDefaultConstructor()
				=> new List<Type>
				{
					typeof(ClassWithoutDefaultConstructor),
					typeof(PublicStaticClass),
					typeof(IPublicInterface),
					typeof(PublicAbstractClass),
				};
		}

		public sealed class NegatedTests
		{
			[Test]
			[MethodDataSource(nameof(TypesWithoutDefaultConstructor))]
			public async Task WhenTypeDoesNotHaveADefaultConstructor_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasADefaultConstructor());
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(nameof(TypesWithDefaultConstructor))]
			public async Task WhenTypeHasADefaultConstructor_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasADefaultConstructor());
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that subject
					              does not have a default constructor,
					              but it had a default constructor {Formatter.Format(subject)}
					              """);
			}

			public static IEnumerable<Type> TypesWithDefaultConstructor()
				=> new List<Type>
				{
					typeof(PublicClass),
					typeof(PublicSealedClass),
					typeof(PublicStruct),
				};

			public static IEnumerable<Type> TypesWithoutDefaultConstructor()
				=> new List<Type>
				{
					typeof(ClassWithoutDefaultConstructor),
					typeof(PublicStaticClass),
					typeof(IPublicInterface),
					typeof(PublicAbstractClass),
				};
		}
	}
}
