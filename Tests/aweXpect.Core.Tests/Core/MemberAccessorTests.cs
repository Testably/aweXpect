namespace aweXpect.Core.Tests.Core;

public sealed class MemberAccessorTests
{
	[Theory]
	[InlineData("Length ", true)]
	[InlineData(".SomethingElse ", false)]
	public async Task Equals_ShouldCompareStringRepresentation(string otherStringRepresentation, bool expectedResult)
	{
		MemberAccessor<string, int> sut = MemberAccessor<string, int>.FromExpression(x => x.Length);
		MemberAccessor<string, int> other =
			MemberAccessor<string, int>.FromFunc(x => x.Length, otherStringRepresentation);

		bool result = sut.Equals(other);

		await That(result).IsEqualTo(expectedResult);
	}

	[Fact]
	public async Task Equals_ToNull_ShouldBeFalse()
	{
		MemberAccessor<string, int> sut = MemberAccessor<string, int>.FromExpression(x => x.Length);

		bool result = sut.Equals(null);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task Equals_ToOtherObject_ShouldBeFalse()
	{
		MemberAccessor<string, int> sut = MemberAccessor<string, int>.FromExpression(x => x.Length);
		object other = MemberAccessor<string, string>.FromExpression(x => x);

		bool result = sut.Equals(other);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task FromExpression_ShouldCompileExpression()
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromExpression(x => x.Length);

		await That(subject.AccessMember("foo")).IsEqualTo(3);
	}

	[Fact]
	public async Task FromExpression_ShouldGetMemberPath()
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromExpression(x => x.Length);

		await That(subject.ToString()).IsEqualTo("Length ");
	}

	[Fact]
	public async Task FromExpression_WithNestedMembers_ShouldKeepInnerDots()
	{
		MemberAccessor<Exception, int> subject = MemberAccessor<Exception, int>
			.FromExpression(x => x.Message.Length);

		await That(subject.ToString()).IsEqualTo("Message.Length ");
	}

	[Theory]
	[InlineData("Foo")]
	[InlineData("  x => x.Foo")]
	[InlineData("x => x.Foo  ")]
	public async Task FromFunc_ShouldKeepNameUnchanged(string expression)
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromFunc(x => x.Length, expression);

		await That(subject.ToString()).IsEqualTo(expression);
	}

	[Theory]
	[InlineData("Foo", "Foo ")]
	[InlineData(".Foo", ".Foo ")]
	[InlineData("x => x.Foo", "Foo ")]
	[InlineData("  x => x.Foo", "Foo ")]
	[InlineData("x => x.Foo  ", "Foo ")]
	[InlineData("itIs => itIs.Foo  ", "Foo ")]
	[InlineData("x => x.Message.Length", "Message.Length ")]
	[InlineData("x => x[0]", "[0] ")]
	[InlineData("x => x.Items[0].Name", "Items[0].Name ")]
	[InlineData("x => x.Items.Count()", "Items.Count() ")]
	[InlineData("x => (int)x.Value", "(int)x.Value ")]
	[InlineData("x => (Exception)x.Inner", "(Exception)x.Inner ")]
	[InlineData("x => ((Foo)x).Bar", "((Foo)x).Bar ")]
	[InlineData("x => x?.Value", "Value ")]
	[InlineData("x => x", "it ")]
	[InlineData("(x) => x.Foo", "Foo ")]
	[InlineData("( x ) => x", "it ")]
	[InlineData("x => xy.Foo", "xy.Foo ")]
	[InlineData("x => { return x.Value; }", "{ return x.Value; } ")]
	[InlineData("_ => _.Value", "Value ")]
	[InlineData("_ => 42", "42 ")]
	[InlineData("@class => @class.Value", "Value ")]
	[InlineData("async => async.Value", "Value ")]
	[InlineData("(x, y) => x.Value", "(x, y) => x.Value ")]
	[InlineData("GetSelector(x => x.Value)", "GetSelector(x => x.Value) ")]
	[InlineData("selector", "selector ")]
	public async Task FromFuncAsMemberAccessor_ShouldTryToExtractMemberAccessor(string expression, string expected)
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromFuncAsMemberAccessor(x => x.Length, expression);

		await That(subject.ToString()).IsEqualTo(expected);
	}

	[Theory]
	[InlineData("async o => await Task.FromResult(o.Value + 1)")]
	[InlineData("async o => await o.Value + 1")]
	[InlineData("async o => await o.GetAsync() + await o.GetAsync()")]
	[InlineData("async o => await o.GetAsync(\"(\") + o.Other(\")\")")]
	[InlineData("async o => await o.Value < 1")]
	[InlineData("async o => await o.A(/*(*/) + o.B(/*)*/)")]
	[InlineData("async o => await o.A(// (\r\n) + o.B(// )\r\n)")]
	[InlineData("async o => await o")]
	[InlineData("async o => await o.ConfigureAwait(false)")]
	[InlineData("async o => await o.GetAsync()?.ConfigureAwait(false)")]
	[InlineData("async o => o.Value")]
	[InlineData("async o => await ox.Value")]
	[InlineData("async o => await (o.Value)")]
	[InlineData("async o => await o .Value")]
	[InlineData("async o => await o.Value /* comment */")]
	[InlineData("async o => { return await o.Value; }")]
	[InlineData("async (x, y) => await y.Value")]
	[InlineData("async o => awaito.Value")]
	[InlineData("static async o => await o.Value")]
	public async Task FromFuncAsMemberAccessor_WithAsyncLambdaWithoutMemberPath_ShouldKeepExpression(
		string expression)
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromFuncAsMemberAccessor(x => x.Length, expression);

		await That(subject.ToString()).IsEqualTo($"{expression} ")
			.Because("only an awaited member path of the parameter can be reduced without changing its meaning");
	}

	[Theory]
	[InlineData("async o => await o.Value", "Value ")]
	[InlineData("async o => await o.GetValueAsync()", "GetValueAsync() ")]
	[InlineData("async (o) => await o.Inner.Value", "Inner.Value ")]
	[InlineData("async(o) => await o.Value", "Value ")]
	[InlineData("async ( o ) => await o.Value", "Value ")]
	[InlineData("async (Foo o) => await o.Value", "Value ")]
	[InlineData("async (Dictionary<int, string> o) => await o.Value", "Value ")]
	[InlineData("async @class => await @class.Value", "Value ")]
	[InlineData("async o => await o.GetAsync().ConfigureAwait(false)", "GetAsync() ")]
	[InlineData("async o => await o.Inner.GetAsync(1).ConfigureAwait(continueOnCapturedContext: true)",
		"Inner.GetAsync(1) ")]
	[InlineData("async o => await o.GetAsync<int>(1)", "GetAsync<int>(1) ")]
	[InlineData("async o => await o.GetAsync<Dictionary<int, string>>()", "GetAsync<Dictionary<int, string>>() ")]
	[InlineData("async o => await o.GetAsync(\"a)\", 'b', x => x.Value)",
		"GetAsync(\"a)\", 'b', x => x.Value) ")]
	[InlineData("async o => await o.GetAsync(/* ) */ 1)", "GetAsync(/* ) */ 1) ")]
	[InlineData("async o => await o?.GetAsync()", "GetAsync() ")]
	[InlineData("async o => await o[0]", "[0] ")]
	[InlineData("async o => await o.Items?[0].Value", "Items?[0].Value ")]
	[InlineData("async o => await o.Tasks[0]", "Tasks[0] ")]
	[InlineData("  async  o  =>\r\n\tawait\r\n  o.Value  ", "Value ")]
	public async Task FromFuncAsMemberAccessor_WithAsyncMemberLambda_ShouldExtractMemberPath(
		string expression, string expected)
	{
		MemberAccessor<string, int> subject = MemberAccessor<string, int>
			.FromFuncAsMemberAccessor(x => x.Length, expression);

		await That(subject.ToString()).IsEqualTo(expected);
	}

	[Fact]
	public async Task GetHashCode_DifferentConstraint_ShouldNotBeEqual()
	{
		MemberAccessor<string, int> sut1 = MemberAccessor<string, int>.FromFunc(x => x.Length, "foo");
		MemberAccessor<string, int> sut2 = MemberAccessor<string, int>.FromFunc(x => x.Length, "bar");

		await That(sut1.GetHashCode()).IsNotEqualTo(sut2.GetHashCode());
	}

	[Fact]
	public async Task GetHashCode_SameConstraint_ShouldBeEqual()
	{
		MemberAccessor<string, int> sut1 = MemberAccessor<string, int>.FromFunc(x => x.Length, "foo");
		MemberAccessor<string, int> sut2 = MemberAccessor<string, int>.FromFunc(x => x.Length, "foo");

		await That(sut1.GetHashCode()).IsEqualTo(sut2.GetHashCode());
	}
}
