using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class ComplyWith
		{
			public sealed class Tests
			{
				[Fact]
				public async Task AllowsNestedIs()
				{
					List<Base> subject =
					[
						new Derived
						{
							Name = "foo",
						},
						new Derived
						{
							Name = "bar",
						},
					];

					async Task Act()
						=> await That(subject).All().ComplyWith(it => it.Is<Derived>());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsLessThan(6)).WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             is less than 6 for all items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [
						               0,
						               1,
						               2,
						               3,
						               4,
						               5,
						               6,
						               7,
						               8,
						               9,
						               (… and maybe more)
						             ]
						             """);
				}

				[Fact]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsGreaterThan(-1))
							.And.All().ComplyWith(x => x.IsGreaterThan(-1));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<int> subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [2, (… and maybe more)]

						             Collection:
						             [
						               1,
						               1,
						               2,
						               3,
						               5,
						               8,
						               13,
						               21,
						               34,
						               55,
						               (… and maybe more)
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					int[] subject = [1, 1, 1, 1, 2, 2, 3,];

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 4 of 7 were

						             Not matching items:
						             [2, 2, 3]

						             Collection:
						             [1, 1, 1, 1, 2, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable((int[])[]);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(0));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).All().ComplyWith(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expectations").And
						.WithMessage("The 'expectations' cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenInnerExpectationHasAsyncReason_ShouldIncludeReasonInExpectation()
				{
					int[] subject = [1,];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.IsEqualTo(2).Because(Task.FromResult<string?>("of reasons")));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 2, because of reasons for all items,
						             but none of 1 were

						             Not matching items:
						             [1]

						             Collection:
						             [1]
						             """);
				}

				[Fact]
				public async Task WhenInnerExpectationHasReason_ShouldIncludeReasonInExpectation()
				{
					int[] subject = [1,];

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(2).Because("of reasons"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 2, because of reasons for all items,
						             but none of 1 were

						             Not matching items:
						             [1]

						             Collection:
						             [1]
						             """);
				}

				[Fact]
				public async Task WhenItemsAreCollections_ShouldVerifyEachItem()
				{
					int[][] subject = [[1, 2,], [1, 3,],];

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo([1, 2,]));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to collection [1, 2,] in order for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [
						               [
						                 1,
						                 3
						               ]
						             ]

						             Collection:
						             [
						               [
						                 1,
						                 2
						               ],
						               [
						                 1,
						                 3
						               ]
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItemsAreLazyCollections_ShouldVerifyEachItem()
				{
					IEnumerable<IEnumerable<int>> subject =
						ToEnumerable<IEnumerable<int>>(ToEnumerable([1, 2,]), ToEnumerable([1, 3,]));

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.HasItem(2));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item equal to 2 for all items,
						             but only 1 of at least 2 did

						             Not matching items:
						             [
						               [
						                 1,
						                 3
						               ],
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               [
						                 1,
						                 2
						               ],
						               [
						                 1,
						                 3
						               ]
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItemsAreLazyCollectionsThatComply_ShouldSucceed()
				{
					IEnumerable<IEnumerable<int>> subject =
						ToEnumerable<IEnumerable<int>>(ToEnumerable([1, 2,]), ToEnumerable([2, 3,]));

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.HasItem(2));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsUseAndWhose_ShouldSeparateTheMembersWithSingleSpaces()
				{
					object[] subject = [new MyClass(1, "foo"),];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.Is<MyClass>()
								.Whose(o => o.Value, v => v.IsEqualTo(1))
								.AndWhose(o => o.StringValue, s => s.IsEqualTo("bar")));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is of type MyClass whose Value is equal to 1 and whose StringValue is equal to "bar" for all items,
						             but none of 1 were

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "foo",
						                 Value = 1
						               }
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "foo",
						                 Value = 1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItemsUseNestedWhose_ShouldIncludeAllMembersInExpectation()
				{
					MyClass[] subject = [new(1, "foo"),];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.Whose(o => o.StringValue, s => s.Whose(v => v.Length, l => l.IsEqualTo(5))));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             whose StringValue has Length that is equal to 5 for all items,
						             but none of 1 did

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "foo",
						                 Value = 1
						               }
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "foo",
						                 Value = 1
						               }
						             ]
						             """)
						.Because("the nested member text must survive the node tree rendering");
				}

				[Fact]
				public async Task WhenItemsUseWhose_ShouldIncludeMemberInExpectation()
				{
					MyClass[] subject = [new(1), new(2),];

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.Whose(o => o.Value, v => v.IsEqualTo(5)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             whose Value is equal to 5 for all items,
						             but none of 2 did

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               }
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               }
						             ]
						             """)
						.Because("the member text must survive the node tree rendering");
				}

				[Fact]
				public async Task WhenItemsUseWhoseAfterAWhichMember_ShouldNotRenderWhichWhose()
				{
					Exception[] subject = [new("a", new InvalidOperationException("b")),];

					async Task Act()
						=> await That(subject).All().ComplyWith(x
							=> x.HasInner<InvalidOperationException>(i => i.Whose(e => e.Message, m => m.IsEqualTo("x"))));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an inner InvalidOperationException whose Message is equal to "x" for all items,
						             but none of 1 did

						             Not matching items:
						             [
						               Exception: a
						             ]

						             Collection:
						             [
						               Exception: a
						             ]
						             """)
						.Because("a member separated by \"which\" must drop it before a nested \"whose\"");
				}

				[Fact]
				public async Task WhenItemsUseWhoseWithAsyncMember_ShouldIncludeMemberInExpectation()
				{
					MyClass[] subject = [new(1),];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.Whose(o => Task.FromResult(o.Value), v => v.IsEqualTo(5)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             whose Task.FromResult(o.Value) is equal to 5 for all items,
						             but none of 1 did

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               }
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               }
						             ]
						             """)
						.Because("the async member text must survive the node tree rendering");
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(0));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 0 for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenAllItemsComply_ShouldFail()
				{
					int[] subject = [1, 2, 3, 4, 5,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.All().ComplyWith(x => x.IsGreaterThan(0)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is greater than 0 not for all items,
						             but all 5 were

						             Collection:
						             [1, 2, 3, 4, 5]
						             """);
				}

				[Fact]
				public async Task WhenAsyncReasonFollowsDoesNotComplyWith_ShouldIncludeReasonOnceInExpectation()
				{
					int[] subject = [3, 1,];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.DoesNotComplyWith(it => it.IsEqualTo(1))
								.Because(Task.FromResult<string?>("of reasons")));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 1, because of reasons for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [1]

						             Collection:
						             [3, 1]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.DoesNotComplyWith(it => it.IsEqualTo(1)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 1 for all items,
						             but none of at least 1 were

						             Not matching items:
						             [1, (… and maybe more)]

						             Collection:
						             [1, 1, 1, 1, 1, 1, 1]
						             """);
				}

				[Fact]
				public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.All().ComplyWith(null!));

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("expectations").And
						.WithMessage("The 'expectations' cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenItemsDoNotComplyWithAndCombination_ShouldNegateEachItem()
				{
					int[] subject = [20, -5,];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.DoesNotComplyWith(it => it.IsGreaterThan(0).And.IsLessThan(10)));

					await That(Act).DoesNotThrow()
						.Because("no item is between 0 and 10");
				}

				[Fact]
				public async Task WhenItemsDoNotComplyWithAndCombination_ShouldUseOrInExpectation()
				{
					int[] subject = [20, 5,];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.DoesNotComplyWith(it => it.IsGreaterThan(0).And.IsLessThan(10)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not greater than 0 or is not less than 10 for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [5]

						             Collection:
						             [20, 5]
						             """);
				}

				[Fact]
				public async Task WhenItemsDoNotComplyWithAndWhose_ShouldUseOrInExpectation()
				{
					object[] subject = [new MyClass(1, "foo"),];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.DoesNotComplyWith(it => it.Is<MyClass>()
								.Whose(o => o.Value, v => v.IsEqualTo(1))
								.AndWhose(o => o.StringValue, s => s.IsEqualTo("foo"))));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not of type MyClass whose Value is equal to 1 or whose StringValue is not equal to "foo" for all items,
						             but none of 1 were

						             Not matching items:
						             [
						               MyClass {
						                 StringValue = "foo",
						                 Value = 1
						               }
						             ]

						             Collection:
						             [
						               MyClass {
						                 StringValue = "foo",
						                 Value = 1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenItemsDoNotComplyWithNestedDoesNotComplyWith_ShouldCancelOutTheNegation()
				{
					int[] subject = [3, 1,];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.DoesNotComplyWith(it
								=> it.DoesNotComplyWith(y => y.IsEqualTo(1).Or.IsEqualTo(2))));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 or is equal to 2 for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [3]

						             Collection:
						             [3, 1]
						             """);
				}

				[Fact]
				public async Task WhenItemsDoNotComplyWithOrCombination_ShouldNegateEachItem()
				{
					int[] subject = [3, 1,];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.DoesNotComplyWith(it => it.IsEqualTo(1).Or.IsEqualTo(2)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 1 and is not equal to 2 for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [1]

						             Collection:
						             [3, 1]
						             """);
				}

				[Fact]
				public async Task WhenReasonFollowsDoesNotComplyWith_ShouldIncludeReasonInExpectation()
				{
					int[] subject = [3, 1,];

					async Task Act()
						=> await That(subject).All()
							.ComplyWith(x => x.DoesNotComplyWith(it => it.IsEqualTo(1)).Because("of reasons"));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 1, because of reasons for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [1]

						             Collection:
						             [3, 1]
						             """);
				}
			}
		}
	}
}
