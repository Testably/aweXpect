using System.Linq;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Results;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public class EvaluationContextTests
{
	[Fact]
	public async Task CanStoreMultipleValueInParallel()
	{
		IEvaluationContext context = await GetSut();

		context.Store("foo", "foo-value");
		context.Store("bar", "bar-value");

		context.TryReceive("foo", out string? fooResult);
		await That(fooResult).IsEqualTo("foo-value");
		context.TryReceive("bar", out string? barResult);
		await That(barResult).IsEqualTo("bar-value");
	}

	[Fact]
	public async Task ReleaseMaterializations_ShouldAlsoReleaseTheSourcesOfTheCurrentAttempt()
	{
		Context context = new();
		Context attempt = await context.StartAttempt();
		DisposeTrackingEnumerable source = new(null, 1, 2);
		_ = attempt.UseMaterializedEnumerable<int>(source).First();

		await context.ReleaseMaterializations();

		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Fact]
	public async Task ResolvePendingReasons_ShouldAlsoResolveTheReasonsOfTheCurrentAttempt()
	{
		Context context = new();
		Context attempt = await context.StartAttempt();
		AsyncBecauseReason reason = new(Task.FromResult<string?>("of a"));
		attempt.ResolveOnFailure(reason);

		await context.ResolvePendingReasons();

		await That(reason.ToString()).IsEqualTo(", because of a")
			.Because("the failure message of the evaluation is created from the result of its last attempt");
	}

	[Fact]
	public async Task StartAttempt_ShouldReleaseTheMaterializedSourcesAndReturnAnEmptyContext()
	{
		Context context = new();
		DisposeTrackingEnumerable source = new(null, 1, 2);
		_ = context.UseMaterializedEnumerable<int>(source).First();
		context.Store("foo", "foo-value");

		Context attempt = await context.StartAttempt();

		await That(source.DisposeCount).IsEqualTo(1);
		await That(attempt).IsNotSameAs(context)
			.Because("each attempt is a separate evaluation");
		await That(attempt.TryReceive("foo", out string? _)).IsFalse()
			.Because("another attempt starts with an empty context");
	}

	[Fact]
	public async Task WhenNotStoredPreviously_ShouldReturnFalse()
	{
		IEvaluationContext context = await GetSut();

		bool result = context.TryReceive("foo", out string? fooResult);
		await That(result).IsFalse();
		await That(fooResult).IsNull();
	}

	[Fact]
	public async Task WhenTypeDoesNotMatch_ShouldReturnFalse()
	{
		IEvaluationContext context = await GetSut();

		context.Store("foo", 42);

		bool result = context.TryReceive("foo", out string? fooResult);
		await That(result).IsFalse();
		await That(fooResult).IsNull();
	}

	[Fact]
	public async Task WhenTypeMatches_ShouldReturnTrue()
	{
		IEvaluationContext context = await GetSut();

		context.Store("foo", "bar");

		bool result = context.TryReceive("foo", out string? fooResult);
		await That(result).IsTrue();
		await That(fooResult).IsEqualTo("bar");
	}

	private static async Task<IEvaluationContext> GetSut()
	{
#pragma warning disable aweXpect0001
		ThatBoolSubject that = That(true);
#pragma warning restore aweXpect0001
		MyContextConstraint constraint = new();
		await new AndOrResult<bool, IExpectThat<bool>>(
			((IExpectThat<bool>)that).ExpectationBuilder
				.AddConstraint((_, _) => constraint),
			that);

		return constraint.Context!;
	}

	private sealed class MyContextConstraint : IContextConstraint<bool>
	{
		public IEvaluationContext? Context { get; private set; }

		/// <inheritdoc />
		public ConstraintResult IsMetBy(bool actual, IEvaluationContext context)
		{
			Context = context;
			return new DummyConstraintResult<bool>(Outcome.Success, actual, "");
		}

		/// <inheritdoc />
		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null) { }
	}
}
