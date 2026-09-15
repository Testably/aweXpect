namespace aweXpect.Tests;

public sealed partial class ThatGeneric
{
	public sealed class Whose
	{
		public sealed class Tests
		{
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
					=> await That(subject).Whose(o => o.Item, it => it.Is<Derived>()
						.Whose(d => d.Name, it => it.IsEqualTo("foo")));

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(true, true, true)]
			[InlineData(true, false, false)]
			[InlineData(false, true, false)]
			[InlineData(false, false, false)]
			public async Task AndCombination_ShouldVerifyAllExpectations(bool a, bool b, bool expectSuccess)
			{
				MyCombinationClass subject = new()
				{
					A = a,
					B = b,
				};

				async Task Act()
					=> await That(subject)
						.Whose(o => o.A, v => v.IsTrue()).And
						.Whose(o => o.B, v => v.IsTrue());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              whose .A is True and whose .B is True,
					              but {(a ? "" : ".A was False")}{(!a && !b ? " and " : "")}{(b ? "" : ".B was False")}
					              """);
			}

			[Theory]
			[InlineData(true, true, true)]
			[InlineData(true, false, true)]
			[InlineData(false, true, true)]
			[InlineData(false, false, false)]
			public async Task OrCombination_ShouldVerifyAnyExpectations(bool a, bool b, bool expectSuccess)
			{
				MyCombinationClass subject = new()
				{
					A = a,
					B = b,
				};

				async Task Act()
					=> await That(subject)
						.Whose(o => o.A, v => v.IsTrue()).Or
						.Whose(o => o.B, v => v.IsTrue());

				await That(Act).Throws<XunitException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             whose .A is True or whose .B is True,
					             but .A was False and .B was False
					             """);
			}

			[Theory]
			[AutoData]
			public async Task WhenConditionIsNotSatisfied_ShouldFail(int value)
			{
				int expectedValue = value + 1;
				MyClass subject = new()
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject)
						.Whose(o => o.Value, v => v.IsEqualTo(expectedValue));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              whose .Value is equal to {Formatter.Format(expectedValue)},
					              but .Value was {Formatter.Format(value)} which differs by -1
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenConditionIsSatisfied_ShouldSucceed(int value)
			{
				MyClass subject = new()
				{
					Value = value,
				};

				async Task Act()
					=> await That(subject).Whose(o => o.Value, v => v.IsEqualTo(value));

				await That(Act).DoesNotThrow();
			}

			private sealed class MyClass
			{
				public int Value { get; set; }
			}

			private sealed class MyCombinationClass
			{
				public bool A { get; set; }
				public bool B { get; set; }
			}
		}
	}
}
