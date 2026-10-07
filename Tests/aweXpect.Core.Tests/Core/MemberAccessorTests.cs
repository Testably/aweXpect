using System.Collections.Generic;
using System.Linq;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core;

public sealed class MemberAccessorTests
{
	private readonly string _text = "a";

	[Test]
	[Arguments("Length ", true)]
	[Arguments(".SomethingElse ", false)]
	public async Task Equals_ShouldCompareStringRepresentation(string otherStringRepresentation, bool expectedResult)
	{
		MemberAccessor<string, int> sut = MemberAccessor<string, int>.FromExpression(x => x.Length);
		MemberAccessor<string, int> other =
			MemberAccessor<string, int>.FromFunc(x => x.Length, otherStringRepresentation);

		bool result = sut.Equals(other);

		await That(result).IsEqualTo(expectedResult);
	}

	[Test]
	public async Task Equals_ToNull_ShouldBeFalse()
	{
		MemberAccessor<string, int> sut = MemberAccessor<string, int>.FromExpression(x => x.Length);

		bool result = sut.Equals(null);

		await That(result).IsFalse();
	}

	[Test]
	public async Task Equals_ToOtherObject_ShouldBeFalse()
	{
		MemberAccessor<string, int> sut = MemberAccessor<string, int>.FromExpression(x => x.Length);
		object other = MemberAccessor<string, string>.FromExpression(x => x);

		bool result = sut.Equals(other);

		await That(result).IsFalse();
	}

	[Test]
	public async Task FromExpression_ShouldCompileExpression()
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromExpression(x => x.Length);

		await That(subject.AccessMember("foo")).IsEqualTo(3);
	}

	[Test]
	public async Task FromExpression_ShouldGetMemberPath()
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromExpression(x => x.Length);

		await That(subject.ToString()).IsEqualTo("Length ");
	}

	[Test]
	public async Task FromExpression_WithCastInMemberPath_ShouldKeepTheWholePath()
	{
		MemberAccessor<MyClass, int> subject = MemberAccessor<MyClass, int>
			.FromExpression(x => ((MyClass)x.Untyped!).Value);

		await That(subject.ToString()).IsEqualTo("Untyped.Value ");
	}

	[Test]
	public async Task FromExpression_WithConvertedMember_ShouldGetMemberPath()
	{
		MemberAccessor<MyClass, int?> subject = MemberAccessor<MyClass, int?>
			.FromExpression(x => x.Value);

		await That(subject.ToString()).IsEqualTo("Value ");
	}

	[Test]
	public async Task FromExpression_WithDifferentMethodCalls_ShouldNotBeEqual()
	{
		MemberAccessor<string, int> sut = MemberAccessor<string, int>.FromExpression(x => x.GetHashCode());
		MemberAccessor<string, int> other = MemberAccessor<string, int>.FromExpression(x => x.IndexOf("a"));

		await That(sut.Equals(other)).IsFalse();
		await That(sut.GetHashCode()).IsNotEqualTo(other.GetHashCode());
	}

	[Test]
	public async Task FromExpression_WithNestedMembers_ShouldKeepInnerDots()
	{
		MemberAccessor<Exception, int> subject = MemberAccessor<Exception, int>
			.FromExpression(x => x.Message.Length);

		await That(subject.ToString()).IsEqualTo("Message.Length ");
	}

	[Test]
	public async Task FromExpression_WithoutMemberPath_ShouldBeNamedInTheExpectation()
	{
		ManualExpectationBuilder<string> sut = new();
		sut.ForMember(MemberAccessor<string, int>.FromExpression(x => x.IndexOf("a")))
			.AddExpectations(expectationBuilder => expectationBuilder.AddConstraint((_, _)
				=> new DummyConstraint<int>(v => v == 2, "equal to 2")));

		ConstraintResult constraintResult = await sut.IsMetBy("bar", null!, CancellationToken.None);

		await That(constraintResult.Outcome).IsEqualTo(Outcome.Failure);
		await That(constraintResult.GetExpectationText()).IsEqualTo("IndexOf(\"a\") equal to 2");
	}

	[Test]
	public async Task FromExpression_WithoutMemberPath_ShouldDescribeTheExpression()
	{
		string text = "a";
		List<(MemberAccessor Accessor, string Expected)> accessors =
		[
			(MemberAccessor<MyClass, MyClass>.FromExpression(x => x), "it "),
			(MemberAccessor<MyClass, int>.FromExpression(_ => 42), "42 "),
			(MemberAccessor<MyClass, double>.FromExpression(_ => 1.5), "1.5 "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Value + 1), "(x.Value + 1) "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.GetHashCode()), "GetHashCode() "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Name.IndexOf("a")), "Name.IndexOf(\"a\") "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Name.IndexOf(text)), "Name.IndexOf(text) "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Name.IndexOf(x.Name)), "Name.IndexOf(x.Name) "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Name.IndexOf(_text)), "Name.IndexOf(_text) "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Name.Length + text.Length + _text.Length),
				"((x.Name.Length + text.Length) + _text.Length) "),
			(MemberAccessor<MyClass, int>.FromExpression(x => "abc".IndexOf(x.Name)), "\"abc\".IndexOf(x.Name) "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.GetInner().Value), "GetInner().Value "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Items.Count()), "Items.Count() "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Items[0].Length), "Items[0].Length "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x[1, 2]), "[1, 2] "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Numbers[1]), "Numbers[1] "),
			(MemberAccessor<MyClass, int>.FromExpression(x => x.Numbers.Length), "Numbers.Length "),
			(MemberAccessor<MyClass, Task<int>>.FromExpression(x => Task.FromResult(x.Value)),
				"Task.FromResult(x.Value) "),
			(MemberAccessor<MyClass, string>.FromExpression(_ => string.Empty), "string.Empty "),
		];

		foreach ((MemberAccessor accessor, string expected) in accessors)
		{
			await That(accessor.ToString()).IsEqualTo(expected);
		}
	}

	[Test]
	public async Task FromExpression_WithSameMethodCall_ShouldBeEqual()
	{
		MemberAccessor<string, int> sut = MemberAccessor<string, int>.FromExpression(x => x.IndexOf("a"));
		MemberAccessor<string, int> other = MemberAccessor<string, int>.FromExpression(y => y.IndexOf("a"));

		await That(sut.Equals(other)).IsTrue();
		await That(sut.GetHashCode()).IsEqualTo(other.GetHashCode());
	}

	[Test]
	[Arguments("Foo")]
	[Arguments("  x => x.Foo")]
	[Arguments("x => x.Foo  ")]
	public async Task FromFunc_ShouldKeepNameUnchanged(string expression)
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromFunc(x => x.Length, expression);

		await That(subject.ToString()).IsEqualTo(expression);
	}

	[Test]
	[Arguments("Foo", "Foo ")]
	[Arguments(".Foo", ".Foo ")]
	[Arguments("x => x.Foo", "Foo ")]
	[Arguments("  x => x.Foo", "Foo ")]
	[Arguments("x => x.Foo  ", "Foo ")]
	[Arguments("itIs => itIs.Foo  ", "Foo ")]
	[Arguments("x => x.Message.Length", "Message.Length ")]
	[Arguments("x => x[0]", "[0] ")]
	[Arguments("x => x.Items[0].Name", "Items[0].Name ")]
	[Arguments("x => x.Items.Count()", "Items.Count() ")]
	[Arguments("x => (int)x.Value", "(int)x.Value ")]
	[Arguments("x => (Exception)x.Inner", "(Exception)x.Inner ")]
	[Arguments("x => ((Foo)x).Bar", "((Foo)x).Bar ")]
	[Arguments("x => x?.Value", "Value ")]
	[Arguments("x => x", "it ")]
	[Arguments("(x) => x.Foo", "Foo ")]
	[Arguments("( x ) => x", "it ")]
	[Arguments("x => xy.Foo", "xy.Foo ")]
	[Arguments("x => { return x.Value; }", "{ return x.Value; } ")]
	[Arguments("_ => _.Value", "Value ")]
	[Arguments("_ => 42", "42 ")]
	[Arguments("@class => @class.Value", "Value ")]
	[Arguments("async => async.Value", "Value ")]
	[Arguments("(x, y) => x.Value", "(x, y) => x.Value ")]
	[Arguments("GetSelector(x => x.Value)", "GetSelector(x => x.Value) ")]
	[Arguments("selector", "selector ")]
	[Arguments("1 => 1.Value", "1 => 1.Value ")]
	public async Task FromFuncAsMemberAccessor_ShouldTryToExtractMemberAccessor(string expression, string expected)
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromFuncAsMemberAccessor(x => x.Length, expression);

		await That(subject.ToString()).IsEqualTo(expected);
	}

	[Test]
	[Arguments("async o => await Task.FromResult(o.Value + 1)")]
	[Arguments("async o => await o.Value + 1")]
	[Arguments("async o => await o.GetAsync() + await o.GetAsync()")]
	[Arguments("async o => await o.GetAsync(\"(\") + o.Other(\")\")")]
	[Arguments("async o => await o.Value < 1")]
	[Arguments("async o => await o.A(/*(*/) + o.B(/*)*/)")]
	[Arguments("async o => await o.A(// (\r\n) + o.B(// )\r\n)")]
	[Arguments("async o => await o")]
	[Arguments("async o => await o.ConfigureAwait(false)")]
	[Arguments("async o => await o.GetAsync()?.ConfigureAwait(false)")]
	[Arguments("async o => o.Value")]
	[Arguments("async o => await ox.Value")]
	[Arguments("async o => await (o.Value)")]
	[Arguments("async o => await o .Value")]
	[Arguments("async o => await o.Value /* comment */")]
	[Arguments("async o => { return await o.Value; }")]
	[Arguments("async (x, y) => await y.Value")]
	[Arguments("async o => awaito.Value")]
	[Arguments("static async o => await o.Value")]
	[Arguments("async ( ) => await o.Value")]
	[Arguments("async o => await o.")]
	[Arguments("async o => await o.1")]
	[Arguments("async o => await o.Value<int>")]
	[Arguments("async o => await o.Get<int+1>()")]
	[Arguments("async o => await o.Get<int")]
	[Arguments("async o => await o.GetAsync(")]
	[Arguments("async o => await o.GetAsync(\"a)")]
	[Arguments("async o => await o.GetAsync(/* )")]
	[Arguments("async o => await o.GetAsync(// )")]
	public async Task FromFuncAsMemberAccessor_WithAsyncLambdaWithoutMemberPath_ShouldKeepExpression(
		string expression)
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromFuncAsMemberAccessor(x => x.Length, expression);

		await That(subject.ToString()).IsEqualTo($"{expression} ")
			.Because("only an awaited member path of the parameter can be reduced without changing its meaning");
	}

	[Test]
	[Arguments("async o => await o.Value", "Value ")]
	[Arguments("async o => await o.GetValueAsync()", "GetValueAsync() ")]
	[Arguments("async (o) => await o.Inner.Value", "Inner.Value ")]
	[Arguments("async(o) => await o.Value", "Value ")]
	[Arguments("async ( o ) => await o.Value", "Value ")]
	[Arguments("async (Foo o) => await o.Value", "Value ")]
	[Arguments("async (Dictionary<int, string> o) => await o.Value", "Value ")]
	[Arguments("async @class => await @class.Value", "Value ")]
	[Arguments("async o => await o.GetAsync().ConfigureAwait(false)", "GetAsync() ")]
	[Arguments("async o => await o.Inner.GetAsync(1).ConfigureAwait(continueOnCapturedContext: true)",
		"Inner.GetAsync(1) ")]
	[Arguments("async o => await o.GetAsync<int>(1)", "GetAsync<int>(1) ")]
	[Arguments("async o => await o.GetAsync<Dictionary<int, string>>()", "GetAsync<Dictionary<int, string>>() ")]
	[Arguments("async o => await o.GetAsync(\"a)\", 'b', x => x.Value)",
		"GetAsync(\"a)\", 'b', x => x.Value) ")]
	[Arguments("async o => await o.GetAsync(/* ) */ 1)", "GetAsync(/* ) */ 1) ")]
	[Arguments("async o => await o?.GetAsync()", "GetAsync() ")]
	[Arguments("async o => await o[0]", "[0] ")]
	[Arguments("async o => await o.Items?[0].Value", "Items?[0].Value ")]
	[Arguments("async o => await o.Tasks[0]", "Tasks[0] ")]
	[Arguments("  async  o  =>\r\n\tawait\r\n  o.Value  ", "Value ")]
	[Arguments("async o => await o.@class", "@class ")]
	[Arguments("async o => await o._value", "_value ")]
	[Arguments("async o => await o.GetAsync<List<int?>[], My_Type.Inner>()", "GetAsync<List<int?>[], My_Type.Inner>() ")]
	[Arguments("async o => await o.GetAsync(\"a\\\")\")", "GetAsync(\"a\\\")\") ")]
	[Arguments("async o => await o.GetAsync(1 / 2)", "GetAsync(1 / 2) ")]
	public async Task FromFuncAsMemberAccessor_WithAsyncMemberLambda_ShouldExtractMemberPath(
		string expression, string expected)
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromFuncAsMemberAccessor(x => x.Length, expression);

		await That(subject.ToString()).IsEqualTo(expected);
	}

	[Test]
	public async Task GetHashCode_DifferentConstraint_ShouldNotBeEqual()
	{
		MemberAccessor<string, int> sut1 = MemberAccessor<string, int>.FromFunc(x => x.Length, "foo");
		MemberAccessor<string, int> sut2 = MemberAccessor<string, int>.FromFunc(x => x.Length, "bar");

		await That(sut1.GetHashCode()).IsNotEqualTo(sut2.GetHashCode());
	}

	[Test]
	public async Task GetHashCode_SameConstraint_ShouldBeEqual()
	{
		MemberAccessor<string, int> sut1 = MemberAccessor<string, int>.FromFunc(x => x.Length, "foo");
		MemberAccessor<string, int> sut2 = MemberAccessor<string, int>.FromFunc(x => x.Length, "foo");

		await That(sut1.GetHashCode()).IsEqualTo(sut2.GetHashCode());
	}

	private sealed class MyClass
	{
		public int this[int row, int column] => row + column;
		public List<string> Items { get; } = [];
		public string Name { get; } = "";
		public int[] Numbers { get; } = [];
		public object? Untyped { get; set; }
		public int Value { get; set; }

		public MyClass GetInner() => this;
	}
}
