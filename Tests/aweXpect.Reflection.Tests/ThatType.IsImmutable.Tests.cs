using aweXpect.Reflection.Tests.TestHelpers.Types;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatType
{
	public sealed class IsImmutable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenTypeHasMultipleMutableMembers_ShouldFail()
			{
				Type subject = typeof(ClassWithMutableFieldAndSettableProperty);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithMutableFieldAndSettableProperty

					             Mutable members:
					             [
					               int ClassWithMutableFieldAndSettableProperty.Field,
					               public int ClassWithMutableFieldAndSettableProperty.Property { get; set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeHasMutableField_ShouldFail()
			{
				Type subject = typeof(ClassWithMutableField);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithMutableField

					             Mutable members:
					             [
					               int ClassWithMutableField.Value
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeHasPropertyWithPrivateSetter_ShouldFail()
			{
				Type subject = typeof(ClassWithPrivateSettableProperty);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithPrivateSettableProperty

					             Mutable members:
					             [
					               public int ClassWithPrivateSettableProperty.Value { get; private set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeHasSettableIndexer_ShouldFail()
			{
				Type subject = typeof(ClassWithSettableIndexer);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithSettableIndexer

					             Mutable members:
					             [
					               public int ClassWithSettableIndexer.Item { get; set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeHasSettableProperty_ShouldFail()
			{
				Type subject = typeof(ClassWithSettableProperty);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassWithSettableProperty

					             Mutable members:
					             [
					               public int ClassWithSettableProperty.Value { get; set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsMutableField_ShouldFail()
			{
				Type subject = typeof(ClassInheritingMutableField);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassInheritingMutableField

					             Mutable members:
					             [
					               int MutableBaseClass._value
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTypeInheritsProtectedMutableField_ShouldFail()
			{
				Type subject = typeof(ClassInheritingProtectedMutableField);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable ClassInheritingProtectedMutableField

					             Mutable members:
					             [
					               int MutableBaseClassWithProtectedField.ProtectedValue
					             ]
					             """);
			}

			[Theory]
			[MemberData(nameof(ImmutableTypes))]
			public async Task WhenTypeIsImmutable_ShouldSucceed(Type subject)
			{
				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTypeIsNull_ShouldFail()
			{
				Type? subject = null;

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTypeIsPositionalRecordStruct_ShouldFail()
			{
				Type subject = typeof(MutableRecordStruct);

				async Task Act()
				{
					await That(subject).IsImmutable();
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable,
					             but it was mutable MutableRecordStruct

					             Mutable members:
					             [
					               public int MutableRecordStruct.Value { get; set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenUsedForItems_ShouldLabelTheMutableMembersWithTheItem()
			{
				Type[] subject =
				[
					typeof(ImmutableClass), typeof(ClassWithMutableField), typeof(ClassWithSettableProperty),
				];

				async Task Act()
				{
					await That(subject).All().ComplyWith(type => type.IsImmutable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             is immutable for all items,
					             but only 1 of 3 were

					             Not matching items:
					             [
					               ClassWithMutableField,
					               ClassWithSettableProperty
					             ]

					             Collection:
					             [
					               ImmutableClass,
					               ClassWithMutableField,
					               ClassWithSettableProperty
					             ]

					             Mutable members (item [1]):
					             [
					               int ClassWithMutableField.Value
					             ]
					             """);
			}

			[Fact]
			public async Task WhenUsedInThatAll_ShouldListTheMutableMembersOfEachFailure()
			{
				async Task Act()
				{
					await ThatAll(
						That(typeof(ClassWithMutableField)).IsImmutable(),
						That(typeof(ImmutableClass)).IsImmutable(),
						That(typeof(ClassWithSettableProperty)).IsImmutable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected all of the following to succeed:
					              [01] Expected that typeof(ClassWithMutableField) is immutable
					              [02] Expected that typeof(ImmutableClass) is immutable
					              [03] Expected that typeof(ClassWithSettableProperty) is immutable
					             but
					              [01] it was mutable ClassWithMutableField
					              [03] it was mutable ClassWithSettableProperty

					             [01] Mutable members:
					             [
					               int ClassWithMutableField.Value
					             ]

					             [03] Mutable members:
					             [
					               public int ClassWithSettableProperty.Value { get; set; }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenUsedInWhose_ShouldLabelTheMutableMembersWithTheMember()
			{
				TypeHolder subject = new(typeof(ClassWithMutableField));

				async Task Act()
				{
					await That(subject).Whose(holder => holder.Type, type => type.IsImmutable());
				}

				await That(Act).Throws()
					.WithMessage("""
					             Expected that subject
					             whose Type is immutable,
					             but Type was mutable ClassWithMutableField

					             Mutable members (Type):
					             [
					               int ClassWithMutableField.Value
					             ]
					             """);
			}

			public static TheoryData<Type> ImmutableTypes() => new()
			{
				typeof(ImmutableClass),
				typeof(ImmutableClassWithInitProperty),
				typeof(ImmutableDerivedClass),
				typeof(PublicRecord),
				typeof(PublicSealedClass),
				typeof(PositionalRecord),
				typeof(ImmutableReadOnlyStruct),
				typeof(ImmutableRecordStruct),
				typeof(IImmutableInterface),
				typeof(PublicEnum),
				typeof(PublicStaticClass),
				typeof(GenericImmutableClass<>),
			};

			private sealed class TypeHolder(Type type)
			{
				public Type Type { get; } = type;
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenTypeIsImmutable_ShouldFail()
			{
				Type subject = typeof(ImmutableClass);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsImmutable());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not immutable,
					             but it was immutable ImmutableClass
					             """);
			}

			[Fact]
			public async Task WhenTypeIsMutable_ShouldSucceed()
			{
				Type subject = typeof(ClassWithMutableField);

				async Task Act()
				{
					await That(subject).DoesNotComplyWith(it => it.IsImmutable());
				}

				await That(Act).DoesNotThrow();
			}
		}
	}
}
