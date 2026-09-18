using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class Is
	{
		public sealed class WhoseTests
		{
			[Fact]
			public async Task WhenPropertyDoesNotMatch_ShouldSucceed()
			{
				object subject = new MyClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).Is<MyClass>()
						.Whose(it => it.Value, value => value.IsLessThan(42));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is type ThatObject.MyClass whose Value is less than 42,
					             but Value was 42
					             """);
			}

			[Fact]
			public async Task WhenPropertyMatches_ShouldSucceed()
			{
				object subject = new MyClass
				{
					Value = 42,
				};

				async Task Act()
					=> await That(subject).Is<MyClass>().Whose(it => it.Value, value => value.IsEqualTo(42));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AllowsNestedIs()
			{
				Outer subject = new()
				{
					Item = new Derived
					{
						Name = "foo",
					},
				};

				async Task Act()
					=> await That(subject).Is<Outer>()
						.Whose(o => o.Item, it => it.Is<Derived>()
							.Whose(d => d.Name, it => it.IsEqualTo("foo")));

				await That(Act).DoesNotThrow();
			}
			[Fact]
			public async Task Whose_AllowsNestedIs_FailsWhenInnerTypeMismatches()
			{
				Outer subject = new()
				{
					Item = new OtherDerived(),
				};

				async Task Act()
					=> await That(subject).Is<Outer>()
						.Whose(o => o.Item, it => it.Is<Derived>());

				await That(Act).Throws<XunitException>();
			}
		}

		public sealed class AndWhoseTests
		{
			[Fact]
			public async Task AndWhose_AllowsNestedIs()
			{
				Outer subject = new()
				{
					Item = new Derived
					{
						Name = "foo",
					},
					Other = new Derived
					{
						Name = "bar",
					},
				};

				async Task Act()
					=> await That(subject).Is<Outer>()
						.Whose(o => o.Item, it => it.Is<Derived>().Whose(d => d.Name, it => it.IsEqualTo("foo")))
						.AndWhose(o => o.Other, it => it.Is<Derived>().Whose(d => d.Name, it => it.IsEqualTo("bar")));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task AndWhose_WhenBothMembersAreCollections_ShouldVerifyEachMember()
			{
				TwoCollections subject = new()
				{
					First = [1, 2,],
					Second = [1, 3,],
				};

				async Task Act()
					=> await That(subject).Is<TwoCollections>()
						.Whose(o => o.First, it => it.IsEqualTo([1, 2,]))
						.AndWhose(o => o.Second, it => it.IsEqualTo([1, 2,]));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is type ThatObject.Is.AndWhoseTests.TwoCollections whose First is equal to collection [1, 2,] in order and whose Second is equal to collection [1, 2,] in order,
					             but Second
					               contained item 3 at index 1 instead of 2 and
					               lacked 1 of 2 expected items: 2
					             *
					             """).AsWildcard();
			}

			private sealed class TwoCollections
			{
				public List<int> First { get; set; } = [];
				public List<int> Second { get; set; } = [];
			}
		}
	}
}
