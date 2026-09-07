using System.Collections.Generic;
using aweXpect.Equivalency;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsNotEquivalentTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task BasicEquivalentObjects_ShouldFail()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
				};
				OuterClass unexpected = new()
				{
					Value = "Foo",
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to ThatObject.OuterClass {
					                 Inner = <null>,
					                 Value = "Foo"
					               },
					             but it was considered equivalent for ThatObject.OuterClass {
					                 Inner = <null>,
					                 Value = "Foo"
					               }
					             
					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Fact]
			public async Task MismatchedObjects_ShouldSucceed()
			{
				OuterClass subject = new();
				OuterClass unexpected = new()
				{
					Value = "Foo",
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task
				ObjectsWithNestedEnumerableMismatch_WithIgnoreRule_ShouldFail()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
							Collection = ["1", "2", "3",],
						},
					},
				};

				OuterClass unexpected = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
							Collection = ["1", "2", "3", "4",],
						},
					},
				};

				async Task Act()
					=> await That(subject)
						.IsNotEquivalentTo(unexpected, o => o.IgnoringMember("Inner.Inner.Collection[3]"));

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to ThatObject.OuterClass {
					                 Inner = ThatObject.InnerClass {
					                   Collection = <null>,
					                   Inner = ThatObject.InnerClass {
					                     Collection = [
					                     "1",
					                     "2",
					                     "3",
					                     "4"
					                   ],
					                     Inner = <null>,
					                     IntValue = 0,
					                     Value = "Baz"
					                   },
					                   IntValue = 0,
					                   Value = "Bar"
					                 },
					                 Value = "Foo"
					               },
					             but it was considered equivalent for ThatObject.OuterClass {
					                 Inner = ThatObject.InnerClass {
					                   Collection = <null>,
					                   Inner = ThatObject.InnerClass {
					                     Collection = [
					                     "1",
					                     "2",
					                     "3"
					                   ],
					                     Inner = <null>,
					                     IntValue = 0,
					                     Value = "Baz"
					                   },
					                   IntValue = 0,
					                   Value = "Bar"
					                 },
					                 Value = "Foo"
					               }
					             
					             Equivalency options:
					              - include public fields and properties
					              - ignore members: ["Inner.Inner.Collection[3]"]
					             """);
			}

			[Fact]
			public async Task
				ObjectsWithNestedEnumerableMismatch_WithIncorrectIgnoreRule_ShouldSucceed()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
							Collection = ["1", "2", "3",],
						},
					},
				};

				OuterClass unexpected = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Bart",
							Collection = ["1", "2", "3", "4",],
						},
					},
				};

				async Task Act()
					=> await That(subject)
						.IsNotEquivalentTo(unexpected, o => o.IgnoringMember("Inner.Inner.Collection.[3]"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ObjectsWithNestedMatches_ShouldFail()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
						},
					},
				};
				OuterClass unexpected = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
						},
					},
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to ThatObject.OuterClass {
					                 Inner = ThatObject.InnerClass {
					                   Collection = <null>,
					                   Inner = ThatObject.InnerClass {
					                     Collection = <null>,
					                     Inner = <null>,
					                     IntValue = 0,
					                     Value = "Baz"
					                   },
					                   IntValue = 0,
					                   Value = "Bar"
					                 },
					                 Value = "Foo"
					               },
					             but it was considered equivalent for ThatObject.OuterClass {
					                 Inner = ThatObject.InnerClass {
					                   Collection = <null>,
					                   Inner = ThatObject.InnerClass {
					                     Collection = <null>,
					                     Inner = <null>,
					                     IntValue = 0,
					                     Value = "Baz"
					                   },
					                   IntValue = 0,
					                   Value = "Bar"
					                 },
					                 Value = "Foo"
					               }
					             
					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Fact]
			public async Task ObjectsWithNestedMatchingEnumerable_ShouldFail()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
							Collection = ["1", "2", "3",],
						},
					},
				};
				OuterClass unexpected = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
							Collection = ["1", "2", "3",],
						},
					},
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to ThatObject.OuterClass {
					                 Inner = ThatObject.InnerClass {
					                   Collection = <null>,
					                   Inner = ThatObject.InnerClass {
					                     Collection = [
					                     "1",
					                     "2",
					                     "3"
					                   ],
					                     Inner = <null>,
					                     IntValue = 0,
					                     Value = "Baz"
					                   },
					                   IntValue = 0,
					                   Value = "Bar"
					                 },
					                 Value = "Foo"
					               },
					             but it was considered equivalent for ThatObject.OuterClass {
					                 Inner = ThatObject.InnerClass {
					                   Collection = <null>,
					                   Inner = ThatObject.InnerClass {
					                     Collection = [
					                     "1",
					                     "2",
					                     "3"
					                   ],
					                     Inner = <null>,
					                     IntValue = 0,
					                     Value = "Baz"
					                   },
					                   IntValue = 0,
					                   Value = "Bar"
					                 },
					                 Value = "Foo"
					               }
					             
					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Fact]
			public async Task ObjectsWithNestedMismatch_ShouldSucceed()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass(),
					},
				};
				OuterClass unexpected = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
						},
					},
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ObjectsWithNestedMismatchingEnumerable_ShouldSucceed()
			{
				OuterClass subject = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
							Collection = ["1", "2", "3",],
						},
					},
				};

				OuterClass unexpected = new()
				{
					Value = "Foo",
					Inner = new InnerClass
					{
						Value = "Bar",
						Inner = new InnerClass
						{
							Value = "Baz",
							Collection = ["1", "2", "3", "4",],
						},
					},
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class CollectionTests
		{
			[Fact]
			public async Task WhenDifferentValues_ShouldSucceed()
			{
				int[] subject = [1, 2, 3,];
				int[] unexpected = [4, 3, 2,];

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected, o => o.IgnoringCollectionOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInDifferentOrder_ShouldSucceed()
			{
				int[] subject = [1, 2, 3,];
				int[] unexpected = [1, 3, 2,];

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenInDifferentOrder_WhenIgnoringCollectionOrder_ShouldFail()
			{
				int[] subject = [1, 2, 3,];
				int[] unexpected = [1, 3, 2,];

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected, o => o.IgnoringCollectionOrder());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to [
					               1,
					               3,
					               2
					             ],
					             but it was considered equivalent for [
					               1,
					               2,
					               3
					             ]
					             
					             Equivalency options:
					              - include public fields and properties
					              - ignore collection order
					             """);
			}

			[Fact]
			public async Task WhenInSameOrder_ShouldFail()
			{
				int[] subject = [1, 2, 3,];
				int[] unexpected = [1, 2, 3,];

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to [
					               1,
					               2,
					               3
					             ],
					             but it was considered equivalent for [
					               1,
					               2,
					               3
					             ]
					             
					             Equivalency options:
					              - include public fields and properties
					             """);
			}
		}

		public sealed class DictionaryTests
		{
			[Fact]
			public async Task WhenDifferentKeys_ShouldSucceed()
			{
				Dictionary<int, int> subject = new()
				{
					{
						1, 2
					},
					{
						2, 3
					},
					{
						3, 4
					},
				};
				Dictionary<int, int> unexpected = new()
				{
					{
						1, 2
					},
					{
						3, 3
					},
					{
						4, 4
					},
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected, o => o.IgnoringCollectionOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenDifferentValues_ShouldSucceed()
			{
				Dictionary<int, int> subject = new()
				{
					{
						1, 2
					},
					{
						2, 3
					},
					{
						3, 4
					},
				};
				Dictionary<int, int> unexpected = new()
				{
					{
						1, 2
					},
					{
						2, 4
					},
					{
						3, 5
					},
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected, o => o.IgnoringCollectionOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSameEntries_ShouldFail()
			{
				Dictionary<int, int> subject = new()
				{
					{
						2, 3
					},
					{
						1, 4
					},
				};

				Dictionary<int, int> unexpected = new()
				{
					{
						2, 3
					},
					{
						1, 4
					},
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);
				
				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to {
					               [2] = 3,
					               [1] = 4
					             },
					             but it was considered equivalent for {
					               [2] = 3,
					               [1] = 4
					             }
					             
					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Fact]
			public async Task WhenSameEntriesInDifferentOrder_ShouldFail()
			{
				Dictionary<string, string> subject = new(StringComparer.OrdinalIgnoreCase)
				{
					{
						"A", "A"
					},
					{
						"B", "B"
					},
				};

				Dictionary<string, string> unexpected = new(StringComparer.OrdinalIgnoreCase)
				{
					{
						"B", "B"
					},
					{
						"A", "A"
					},
				};

				async Task Act()
					=> await That(subject).IsNotEquivalentTo(unexpected);
				
				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to {
					               ["B"] = "B",
					               ["A"] = "A"
					             },
					             but it was considered equivalent for {
					               ["A"] = "A",
					               ["B"] = "B"
					             }
					             
					             Equivalency options:
					              - include public fields and properties
					             """);
			}
		}
	}
}
