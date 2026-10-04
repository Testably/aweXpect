using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Results;

public class AndOrWhoseResultTests
{
	[Fact]
	public async Task AndWhose_AfterAConvertingConstraint_ShouldVerifyTheConvertedValue()
	{
		string sut = "10";

		async Task Act()
			=> await IsNumeric(That(sut))
				.Whose(x => x + 1, x => x.IsEqualTo(11))
				.AndWhose(x => x * 2, x => x.IsEqualTo(40));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is numeric whose x + 1 is equal to 11 and whose x * 2 is equal to 40,
			             but x * 2 was 20, which differs by -20
			             """)
			.Because("every member continues from the value that the converting constraint stores");
	}

	[Fact]
	public async Task AndWhose_WhenAsyncMemberAccessorIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsFalse())
				.AndWhose((Func<MyClass, Task<bool>>)null!, f => f.IsTrue());

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberAccessor").And
			.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task AndWhose_WhenAsyncMemberExpectationsIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsFalse())
				.AndWhose(f => f.GetValue1Async(), null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("expectations").And
			.WithMessage("The 'expectations' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task AndWhose_WhenAsyncMemberFaults_ShouldFail()
	{
		ThrowingClass sut = new("async member failed");

		async Task Act()
			=> await That(sut).Is<ThrowingClass>()
				.Whose(f => f.Value, f => f.IsFalse())
				.AndWhose(f => f.FaultedAsync(), f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.ThrowingClass whose Value is False and whose FaultedAsync() is True,
			             but FaultedAsync() did throw an InvalidOperationException:
			               async member failed
			             """)
			.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
	}

	[Fact]
	public async Task AndWhose_WhenExpectationsIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsFalse())
				.AndWhose(f => f.Value2, null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("expectations").And
			.WithMessage("The 'expectations' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task AndWhose_WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsFalse())
				.AndWhose((Func<MyClass, bool>)null!, f => f.IsFalse());

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberAccessor").And
			.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task AndWhose_WhenTheMemberIsACollection_ShouldUseThePluralForm()
	{
		ListClass sut = new();

		async Task Act()
			=> await That(sut).Is<ListClass>()
				.Whose(f => f.Name, f => f.Get().ExpectationBuilder.AddConstraint((_, g)
					=> new DummyConstraint<string?>(_ => true, g.Verb("has a value", "have a value"))))
				.AndWhose(f => f.Items, f => f.Get().ExpectationBuilder.AddConstraint((_, g)
					=> new DummyConstraint<int[]?>(_ => false, g.Verb("has one item", "have one item"))));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.ListClass whose Name has a value and whose Items have one item,
			             *
			             """).AsWildcard()
			.Because("the number of the member follows its type, like in the other Whose overloads");
	}

	[Fact]
	public async Task AndWhose_WhenValueTaskMemberAccessorIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsFalse())
				.AndWhose((Func<MyClass, ValueTask<bool>>)null!, f => f.IsTrue());

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberAccessor").And
			.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task AndWhose_WithAsyncMember_ShouldVerifyAwaitedValue()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value2, f => f.IsFalse())
				.AndWhose(f => f.GetValue1Async(), f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose Value2 is False and whose GetValue1Async() is True,
			             but GetValue1Async() was False
			             """);
	}

	[Fact]
	public async Task AndWhose_WithBecause_ShouldIncludeTheReason()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value2, f => f.IsFalse())
				.AndWhose(f => f.Value1, f => f.IsTrue())
				.Because("we want to test the reason");

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose Value2 is False and whose Value1 is True, because we want to test the reason,
			             but Value1 was False
			             """);
	}

	[Fact]
	public async Task MultipleWhose_ShouldAllowChaining()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsTrue())
				.AndWhose(f => f.Value2, f => f.IsTrue())
				.And.IsSameAs(sut);

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose Value1 is True and whose Value2 is True and refers to AndOrWhoseResultTests.MyClass {
			                 Value1 = False,
			                 Value2 = False
			               },
			             but Value1 was False and Value2 was False
			             """);
	}

	[Theory]
	[InlineData(true, true, true)]
	[InlineData(true, false, false)]
	[InlineData(false, true, false)]
	[InlineData(false, false, false)]
	public async Task MultipleWhose_ShouldVerifyAll(bool value1, bool value2, bool expectSuccess)
	{
		MyClass sut = new()
		{
			Value1 = value1,
			Value2 = value2,
		};

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsTrue())
				.AndWhose(f => f.Value2, f => f.IsTrue());

		await That(Act).Throws().OnlyIf(!expectSuccess)
			.WithMessage($"""
			              Expected that sut
			              is of type AndOrWhoseResultTests.MyClass whose Value1 is True and whose Value2 is True,
			              but {(value1 ? "" : "Value1 was False")}{(!value1 && !value2 ? " and " : "")}{(value2 ? "" : "Value2 was False")}
			              """);
	}

	[Fact]
	public async Task MultipleWhose_WithNamedMemberAccessor_ShouldSucceed()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(memberAccessor: f => f.Value1, expectations: f => f.IsFalse())
				.AndWhose(memberAccessor: f => f.Value2, expectations: f => f.IsFalse());

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task Whose_AfterAConvertingConstraint_ShouldVerifyTheConvertedValue()
	{
		string sut = "10";

		async Task Act()
			=> await IsNumeric(That(sut)).Whose(x => x * 2, x => x.IsEqualTo(40));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is numeric whose x * 2 is equal to 40,
			             but x * 2 was 20, which differs by -20
			             """)
			.Because("the member continues from the value that the converting constraint stores");
	}

	[Fact]
	public async Task Whose_AfterAConvertingConstraint_WhenMet_ShouldReturnTheConvertedValue()
	{
		string sut = "20";

		int result = await IsNumeric(That(sut)).Whose(x => x * 2, x => x.IsEqualTo(40));

		await That(result).IsEqualTo(20);
	}

	[Fact]
	public async Task Whose_AfterAConvertingConstraint_WhenNegated_ShouldVerifyTheConvertedValue()
	{
		string sut = "20";

		async Task Act()
			=> await That(sut).DoesNotComplyWith(it => IsNumeric(it).Whose(x => x * 2, x => x.IsEqualTo(40)));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is not numeric whose x * 2 is equal to 40,
			             but it was "20"
			             """)
			.Because("the negation is met only when the converted value does not meet the member expectation");
	}

	[Fact]
	public async Task Whose_AfterAConvertingConstraint_WhenTheConversionFails_ShouldOnlyRenderTheMember()
	{
		string sut = "ten";

		async Task Act()
			=> await IsNumeric(That(sut)).Whose(x => x * 2, x => x.IsEqualTo(40));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is numeric whose x * 2 is equal to 40,
			             but it was "ten"
			             """)
			.Because("without a converted value there is no member to verify");
	}

	[Fact]
	public async Task Whose_AfterAConvertingConstraint_WithAsyncMember_ShouldVerifyTheConvertedValue()
	{
		string sut = "10";

		async Task Act()
			=> await IsNumeric(That(sut)).Whose(x => Task.FromResult(x * 2), x => x.IsEqualTo(40));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is numeric whose Task.FromResult(x * 2) is equal to 40,
			             but Task.FromResult(x * 2) was 20, which differs by -20
			             """)
			.Because("the awaited member continues from the value that the converting constraint stores");
	}

	[Fact]
	public async Task Whose_WhenAsyncMemberAccessorIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose((Func<MyClass, Task<bool>>)null!, f => f.IsTrue());

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberAccessor").And
			.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task Whose_WhenAsyncMemberExpectationsIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.GetValue1Async(), null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("expectations").And
			.WithMessage("The 'expectations' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task Whose_WhenAsyncMemberFaults_ShouldFail()
	{
		ThrowingClass sut = new("async member failed");

		async Task Act()
			=> await That(sut).Is<ThrowingClass>()
				.Whose(f => f.FaultedAsync(), f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.ThrowingClass whose FaultedAsync() is True,
			             but FaultedAsync() did throw an InvalidOperationException:
			               async member failed
			             """)
			.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("async member failed"));
	}

	[Fact]
	public async Task Whose_WhenExpectationsIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("expectations").And
			.WithMessage("The 'expectations' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task Whose_WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose((Func<MyClass, bool>)null!, f => f.IsFalse());

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberAccessor").And
			.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task Whose_WhenMemberThrows_ShouldFail()
	{
		ThrowingClass sut = new("member failed");

		async Task Act()
			=> await That(sut).Is<ThrowingClass>()
				.Whose(f => f.Throwing(), f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.ThrowingClass whose Throwing() is True,
			             but Throwing() did throw an InvalidOperationException:
			               member failed
			             """)
			.And.WithInner<InvalidOperationException>(inner => inner.HasMessage("member failed"));
	}

	[Fact]
	public async Task Whose_WhenTheAsyncMemberIsACollection_ShouldUseThePluralForm()
	{
		ListClass sut = new();

		async Task Act()
			=> await That(sut).Is<ListClass>()
				.Whose(f => f.GetItemsAsync(), f => f.Get().ExpectationBuilder.AddConstraint((_, g)
					=> new DummyConstraint<int[]?>(_ => false, g.Verb("has one item", "have one item"))));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.ListClass whose GetItemsAsync() have one item,
			             *
			             """).AsWildcard();
	}

	[Fact]
	public async Task Whose_WhenTheMemberIsACollection_ShouldUseThePluralForm()
	{
		ListClass sut = new();

		async Task Act()
			=> await That(sut).Is<ListClass>()
				.Whose(f => f.Items, f => f.Get().ExpectationBuilder.AddConstraint((_, g)
					=> new DummyConstraint<int[]?>(_ => false, g.Verb("has one item", "have one item"))));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.ListClass whose Items have one item,
			             *
			             """).AsWildcard()
			.Because("the number of the member follows its type, like in the other Whose overloads");
	}

	[Fact]
	public async Task Whose_WhenValueTaskMemberAccessorIsNull_ShouldThrowArgumentNullException()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose((Func<MyClass, ValueTask<bool>>)null!, f => f.IsTrue());

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("memberAccessor").And
			.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
	}

	[Fact]
	public async Task Whose_WithAsyncMember_ShouldVerifyAwaitedValue()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.GetValue1Async(), f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose GetValue1Async() is True,
			             but GetValue1Async() was False
			             """);
	}

	[Fact]
	public async Task Whose_WithBecause_ShouldAllowAndWhoseAndKeepTheReasonLast()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value2, f => f.IsFalse())
				.Because("we want to test the reason")
				.AndWhose(f => f.Value1, f => f.IsTrue());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose Value2 is False and whose Value1 is True, because we want to test the reason,
			             but Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithBecause_ShouldIncludeTheReason()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsTrue())
				.Because("we want to test the reason");

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose Value1 is True, because we want to test the reason,
			             but Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithBecauseTask_ShouldIncludeTheReason()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1, f => f.IsTrue())
				.Because(Task.FromResult<string?>("we want to test the reason"));

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose Value1 is True, because we want to test the reason,
			             but Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithCancellation_WhenMet_ShouldReturnTheValue()
	{
		MyClass sut = new();
		using CancellationTokenSource cts = new();

		MyClass result = await That(sut).Is<MyClass>()
			.Whose(f => f.Value1, f => f.IsFalse())
			.WithCancellation(cts.Token);

		await That(result).IsSameAs(sut);
	}

	[Fact]
	public async Task Whose_WithCast_ShouldKeepWholeSelectorBody()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(o => (bool?)o.Value1, f => f.IsEqualTo(true));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose (bool?)o.Value1 is True,
			             but (bool?)o.Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithCastParameter_ShouldKeepWholeSelectorBody()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => ((MyClass)f).Value1, f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose ((MyClass)f).Value1 is True,
			             but ((MyClass)f).Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithIdentity_ShouldRenderIt()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f, f => f.IsNull());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose it is null,
			             but it was AndOrWhoseResultTests.MyClass {
			                 Value1 = False,
			                 Value2 = False
			               }
			             """);
	}

	[Fact]
	public async Task Whose_WithNestedMemberPath_ShouldOmitLeadingDot()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.Value1.ToString().Length, f => f.IsLessThan(5));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose Value1.ToString().Length is less than 5,
			             but Value1.ToString().Length was 5
			             """);
	}

	[Fact]
	public async Task Whose_WithNullConditional_ShouldOmitParameter()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f?.Value1, f => f.IsEqualTo(true));

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose Value1 is True,
			             but Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithParenthesizedParameter_ShouldOmitParameter()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose((f) => f.Value1, f => f.IsTrue());

		await That(Act).Throws()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose Value1 is True,
			             but Value1 was False
			             """);
	}

	[Fact]
	public async Task Whose_WithTimeout_WhenAsyncMemberDoesNotFinish_ShouldFail()
	{
		MyClass sut = new();

		async Task Act()
			=> await That(sut).Is<MyClass>()
				.Whose(f => f.NeverCompletesAsync(), f => f.IsTrue())
				.WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that sut
			             is of type AndOrWhoseResultTests.MyClass whose NeverCompletesAsync() is True,
			             but it did not finish within 0:00.050
			             """)
			.And.WithInner<TimeoutException>(inner
				=> inner.HasMessage("The operation did not finish within 0:00.050."));
	}

	private static AndOrWhoseResult<int, IThat<string?>> IsNumeric(IThat<string?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
			=> new IsNumericConstraint(it, grammars)), subject);

	private sealed class IsNumericConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<string>(it, grammars), IValueConstraint<string?>
	{
		private int _number;

		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			Outcome = int.TryParse(actual, out _number) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_number is TValue number)
			{
				value = number;
				return true;
			}

			value = default;
			return false;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is numeric");

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was \"").Append(Actual).Append('"');

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is not numeric");

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was \"").Append(Actual).Append('"');
	}

	private sealed class ListClass
	{
		public int[] Items { get; } = [];
		public string Name { get; } = "";

		public Task<int[]> GetItemsAsync() => Task.FromResult(Items);
	}

	private sealed class MyClass
	{
		public bool Value1 { get; set; }
		public bool Value2 { get; set; }

		public Task<bool> GetValue1Async() => Task.FromResult(Value1);

		public Task<bool> NeverCompletesAsync() => new TaskCompletionSource<bool>().Task;
	}

	private sealed class ThrowingClass(string message)
	{
		public bool Value { get; set; }

		public async Task<bool> FaultedAsync()
		{
			await Task.Yield();
			throw new InvalidOperationException(message);
		}

		public bool Throwing() => throw new InvalidOperationException(message);
	}
}
