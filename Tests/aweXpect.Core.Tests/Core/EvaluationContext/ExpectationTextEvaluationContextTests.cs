using System.Threading;
using aweXpect.Core.EvaluationContext;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public sealed class ExpectationTextEvaluationContextTests
{
	[Test]
	public async Task Cancellation_WithInner_ShouldBeTheOneOfTheInnerContext()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = EvaluationCancellation.Create(null, cts.Token);
		Context inner = new()
		{
			Cancellation = cancellation,
		};
		IEvaluationContext sut = ExpectationTextEvaluationContext.For(inner);

		await That(sut.Cancellation).IsSameAs(cancellation);
	}

	[Test]
	public async Task Cancellation_WithoutInner_ShouldBeNone()
	{
		IEvaluationContext sut = ExpectationTextEvaluationContext.For(null);

		await That(sut.Cancellation).IsSameAs(EvaluationCancellation.None);
	}

	[Test]
	public async Task Store_WithInner_ShouldStoreTheValueInTheInnerContext()
	{
		Context inner = new();
		IEvaluationContext sut = ExpectationTextEvaluationContext.For(inner);

		sut.Store("foo", "foo-value");

		await That(inner.TryReceive("foo", out string? innerValue)).IsTrue();
		await That(innerValue).IsEqualTo("foo-value");
		await That(sut.TryReceive("foo", out string? value)).IsTrue();
		await That(value).IsEqualTo("foo-value");
	}

	[Test]
	public async Task Store_WithoutInner_ShouldNotStoreTheValue()
	{
		IEvaluationContext sut = ExpectationTextEvaluationContext.For(null);

		sut.Store("foo", "foo-value");

		await That(sut.TryReceive("foo", out string? value)).IsFalse()
			.Because("there is no evaluation in which the value could be stored");
		await That(value).IsNull();
	}
}
