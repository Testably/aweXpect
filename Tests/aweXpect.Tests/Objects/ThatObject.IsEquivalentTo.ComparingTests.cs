using aweXpect.Equivalency;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class ComparingTests
		{
			[Test]
			public async Task WhenCollectionOrderIsIgnored_ShouldPairTheItemsByThePredicate()
			{
				Dto[] subject = [new(1, "a"), new(2, "b"), new(3, "c"),];
				Entity[] expected = [new(3, "c"), new(1, "a"), new(2, "b"),];

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key)
						.IgnoringCollectionOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenElementDoesNotMatch_ShouldFailWithThePathAndThePredicate()
			{
				Dto[] subject = [new(1, "a"), new(2, "b"),];
				Entity[] expected = [new(1, "a"), new(3, "b"),];

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Element [1] did not match (dto, entity) => dto.Id == entity.Key:
					                   Actual: ThatObject.IsEquivalentTo.ComparingTests.Dto { Id = 2, Name = "b" }
					                 Expected: ThatObject.IsEquivalentTo.ComparingTests.Entity { FullName = "b", Key = 3 }

					             Equivalency options:
					              - include public fields and properties
					              - compare ThatObject.IsEquivalentTo.ComparingTests.Dto with ThatObject.IsEquivalentTo.ComparingTests.Entity by (dto, entity) => dto.Id == entity.Key
					             """);
			}

			[Test]
			public async Task WhenMemberMeetsTheTypes_ShouldUseThePredicate()
			{
				OrderDto subject = new(7, new Dto(1, "a"));
				OrderEntity expected = new(7, new Entity(1, "a"));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key));

				await That(Act).DoesNotThrow()
					.Because("the predicate applies wherever the types meet in the compared objects");
			}

			[Test]
			public async Task WhenMemberOfTheSameTypeIsComparedByValue_ShouldUseThePredicate()
			{
				Track subject = new(7);
				Track expected = new(8);

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<int, int>((_, _) => true));

				await That(Act).DoesNotThrow()
					.Because("the predicate decides also for a member whose values are compared without reading them as objects");
			}

			[Test]
			public async Task WhenMemberValueIsNull_ShouldNotCallThePredicate()
			{
				OrderDto subject = new(7, null);
				OrderEntity expected = new(7, new Entity(1, "a"));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Customer differed:
					                   Actual: <null>
					                 Expected: ThatObject.IsEquivalentTo.ComparingTests.Entity { FullName = "a", Key = 1 }

					             Equivalency options:
					              - include public fields and properties
					              - compare ThatObject.IsEquivalentTo.ComparingTests.Dto with ThatObject.IsEquivalentTo.ComparingTests.Entity by (dto, entity) => dto.Id == entity.Key
					             """);
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFailWithTheException()
			{
				Dto[] subject = [new(1, "a"),];
				Entity[] expected = [new(1, "a"),];

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<Dto, Entity>((_, _) => throw new InvalidOperationException("broken")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but the comparison of Element [0] did throw an InvalidOperationException:
					               broken
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenRegisteredForAnotherType_ShouldNotApply()
			{
				Dto subject = new(1, "a");
				Entity expected = new(1, "a");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.For<OrderEntity>(x => x.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property FullName was missing on the actual object and
					               Property Key was missing on the actual object

					             Equivalency options:
					              - include public fields and properties
					              - for ThatObject.IsEquivalentTo.ComparingTests.OrderEntity:
					                - include public fields and properties
					                - compare ThatObject.IsEquivalentTo.ComparingTests.Dto with ThatObject.IsEquivalentTo.ComparingTests.Entity by (dto, entity) => dto.Id == entity.Key
					             """)
					.Because("a registered predicate only applies within the type it is registered for");
			}

			[Test]
			public async Task WhenRegisteredForAType_ShouldApplyToItsMembers()
			{
				OrderDto subject = new(7, new Dto(1, "a"));
				OrderEntity expected = new(7, new Entity(1, "b"));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.For<OrderEntity>(x => x.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key)));

				await That(Act).DoesNotThrow()
					.Because("the members of a type are compared with the options registered for it");
			}

			[Test]
			public async Task WhenSeveralPredicatesApply_ShouldUseTheLastOne()
			{
				Dto subject = new(1, "a");
				Entity expected = new(2, "a");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key)
						.Comparing<Dto, Entity>((dto, entity) => dto.Name == entity.FullName));

				await That(Act).DoesNotThrow()
					.Because("a later predicate overrides an earlier one for the same types");
			}

			[Test]
			public async Task WhenSubjectDoesNotMatch_ShouldFailForIt()
			{
				Dto subject = new(1, "a");
				Entity expected = new(2, "a");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               It did not match (dto, entity) => dto.Id == entity.Key:
					                   Actual: ThatObject.IsEquivalentTo.ComparingTests.Dto { Id = 1, Name = "a" }
					                 Expected: ThatObject.IsEquivalentTo.ComparingTests.Entity { FullName = "a", Key = 2 }

					             Equivalency options:
					              - include public fields and properties
					              - compare ThatObject.IsEquivalentTo.ComparingTests.Dto with ThatObject.IsEquivalentTo.ComparingTests.Entity by (dto, entity) => dto.Id == entity.Key
					             """);
			}

			[Test]
			public async Task WhenTypeIsDerived_ShouldUseThePredicate()
			{
				Dto subject = new DerivedDto(1, "a");
				Entity expected = new(1, "b");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTypesDoNotMeet_ShouldCompareByMembers()
			{
				OrderDto subject = new(7, new Dto(1, "a"));
				OrderEntity expected = new(8, new Entity(1, "a"));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected,
						o => o.Comparing<Dto, Entity>((dto, entity) => dto.Id == entity.Key));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Number differed:
					                   Actual: 7
					                 Expected: 8

					             Equivalency options:
					              - include public fields and properties
					              - compare ThatObject.IsEquivalentTo.ComparingTests.Dto with ThatObject.IsEquivalentTo.ComparingTests.Entity by (dto, entity) => dto.Id == entity.Key
					             """)
					.Because("the enclosing objects are not of the compared types");
			}

			[Test]
			public async Task WhenValuesAreComparedByValue_ShouldUseThePredicate()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).IsEquivalentTo("1",
						o => o.Comparing<int, string>((number, text) => number.ToString() == text));

				await That(Act).DoesNotThrow()
					.Because("the predicate decides before the values are compared by value");
			}

			private class Dto(int id, string name)
			{
				public int Id { get; } = id;
				public string Name { get; } = name;
			}

			private sealed class DerivedDto(int id, string name) : Dto(id, name);

			private sealed class Entity(int key, string fullName)
			{
				public string FullName { get; } = fullName;
				public int Key { get; } = key;
			}

			private sealed class OrderDto(int number, Dto? customer)
			{
				public Dto? Customer { get; } = customer;
				public int Number { get; } = number;
			}

			private sealed class OrderEntity(int number, Entity? customer)
			{
				public Entity? Customer { get; } = customer;
				public int Number { get; } = number;
			}

			internal sealed class Track(int number)
			{
				public int Number { get; } = number;
			}
		}
	}
}
