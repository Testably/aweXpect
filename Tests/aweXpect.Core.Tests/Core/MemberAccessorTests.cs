namespace aweXpect.Core.Tests.Core;

public sealed class MemberAccessorTests
{
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
	public async Task FromExpression_WithNestedMembers_ShouldKeepInnerDots()
	{
		MemberAccessor<Exception, int> subject = MemberAccessor<Exception, int>
			.FromExpression(x => x.Message.Length);

		await That(subject.ToString()).IsEqualTo("Message.Length ");
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
}
