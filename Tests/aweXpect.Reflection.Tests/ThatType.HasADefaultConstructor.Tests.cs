using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class HasADefaultConstructor
	{
		public sealed class Tests
		{
			[Theory]
			[MemberData(nameof(TypesWithDefaultConstructor))]
			public async Task WhenTypeHasADefaultConstructor_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).HasADefaultConstructor();
				}

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[MemberData(nameof(TypesWithoutDefaultConstructor))]
			public async Task WhenTypeDoesNotHaveADefaultConstructor_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).HasADefaultConstructor();
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              has a default constructor,
					              but it did not have a default constructor {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).HasADefaultConstructor();
				}

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             has a default constructor,
					             but it was <null>
					             """);
			}

			public static TheoryData<Type> TypesWithDefaultConstructor()
				=> new()
				{
					typeof(PublicClass),
					typeof(PublicSealedClass),
					typeof(PublicStruct),
				};

			public static TheoryData<Type> TypesWithoutDefaultConstructor()
				=> new()
				{
					typeof(ClassWithoutDefaultConstructor),
					typeof(PublicStaticClass),
					typeof(IPublicInterface),
					typeof(PublicAbstractClass),
				};
		}

		public sealed class NegatedTests
		{
			[Theory]
			[MemberData(nameof(TypesWithDefaultConstructor))]
			public async Task WhenTypeHasADefaultConstructor_ShouldFail(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasADefaultConstructor());
				}

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that subject
					              does not have a default constructor,
					              but it had a default constructor {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[MemberData(nameof(TypesWithoutDefaultConstructor))]
			public async Task WhenTypeDoesNotHaveADefaultConstructor_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.HasADefaultConstructor());
				}

				await That(Act).DoesNotThrow();
			}

			public static TheoryData<Type> TypesWithDefaultConstructor()
				=> new()
				{
					typeof(PublicClass),
					typeof(PublicSealedClass),
					typeof(PublicStruct),
				};

			public static TheoryData<Type> TypesWithoutDefaultConstructor()
				=> new()
				{
					typeof(ClassWithoutDefaultConstructor),
					typeof(PublicStaticClass),
					typeof(IPublicInterface),
					typeof(PublicAbstractClass),
				};
		}
	}
}
