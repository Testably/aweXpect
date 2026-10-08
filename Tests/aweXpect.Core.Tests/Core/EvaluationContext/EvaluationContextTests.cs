using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Core.Helpers;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Core.TimeSystem;
using aweXpect.Results;
using Context = aweXpect.Core.EvaluationContext.EvaluationContext;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public class EvaluationContextTests
{
	[Test]
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

	[Test]
	public async Task ReleaseMaterializations_ShouldAlsoReleaseTheSourcesOfTheCurrentAttempt()
	{
		Context context = new();
		Context attempt = await context.StartAttempt();
		DisposeTrackingEnumerable source = new(null, 1, 2);
		_ = attempt.UseMaterializedEnumerable(source).First();

		await context.ReleaseMaterializations();

		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task ResolvePendingReasons_ShouldAlsoResolveTheReasonsOfTheCurrentAttempt()
	{
		Context context = new();
		Context attempt = await context.StartAttempt();
		AsyncBecauseReason reason = new(Task.FromResult<string?>("of a"));
		attempt.ResolveOnFailure(reason);

		await context.ResolvePendingReasons(CancellationToken.None);

		await That(reason.ToString()).IsEqualTo(", because of a")
			.Because("the failure message of the evaluation is created from the result of its last attempt");
	}

	[Test]
	public async Task ResolvePendingReasons_WhenCancellationIsRequested_ShouldAbandonAPendingReason()
	{
		TaskCompletionSource<string?> becauseSource = new();
		Context context = new();
		AsyncBecauseReason reason = new(becauseSource.Task);
		context.ResolveOnFailure(reason);

		Task resolve = context.ResolvePendingReasons(new CancellationToken(true));
		await Task.WhenAny(resolve, Task.Delay(TimeSpan.FromSeconds(10)));
		bool isCompleted = resolve.IsCompleted;
		becauseSource.SetResult("of a");

		await That(isCompleted).IsTrue()
			.Because("the cancellation must stop waiting for a reason that does not arrive");
		await That(reason.ToString()).IsEqualTo(", because the reason was not available in time");
	}

	[Test]
	public async Task StartAttempt_ShouldKeepTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		Context context = new()
		{
			TimeSystem = timeSystem,
		};

		Context attempt = await context.StartAttempt();

		await That(attempt.TimeSystem).IsSameAs(timeSystem)
			.Because("a repeated check in another attempt measures and waits like the evaluation");
	}

	[Test]
	public async Task StartAttempt_ShouldReleaseTheMaterializedSourcesAndReturnAnEmptyContext()
	{
		Context context = new();
		DisposeTrackingEnumerable source = new(null, 1, 2);
		_ = context.UseMaterializedEnumerable(source).First();
		context.Store("foo", "foo-value");

		Context attempt = await context.StartAttempt();

		await That(source.DisposeCount).IsEqualTo(1);
		await That(attempt).IsNotSameAs(context)
			.Because("each attempt is a separate evaluation");
		await That(attempt.TryReceive("foo", out string? _)).IsFalse()
			.Because("another attempt starts with an empty context");
	}

	[Test]
	public async Task StartCheck_ShouldKeepTheResourcesOfThePreviousCheckUntilTheEvaluationIsReleased()
	{
		Context context = new();
		Context previous = await context.StartCheck(null);
		int releaseCount = 0;
		previous.ReleaseWithEvaluation(() => releaseCount++);

		await context.StartCheck(previous);
		int releaseCountInTheNextCheck = releaseCount;
		await context.ReleaseMaterializations();

		await That(releaseCountInTheNextCheck).IsEqualTo(0)
			.Because("the next check of the same evaluation still uses the resource");
		await That(releaseCount).IsEqualTo(1)
			.Because("the resource is released together with the evaluation");
	}

	[Test]
	public async Task StartCheck_ShouldKeepTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		Context context = new()
		{
			TimeSystem = timeSystem,
		};

		Context check = await context.StartCheck(null);

		await That(check.TimeSystem).IsSameAs(timeSystem)
			.Because("a repeated check within another check measures and waits like the evaluation");
	}

	[Test]
	public async Task Store_AfterAnItemEvaluation_ShouldNotReceiveTheValuesOfTheItem()
	{
		Context context = new();
		ManualExpectationBuilder<string> builder = new();
		builder.AddConstraint((_, _) => new SharesValueConstraint<string>(s => s));

		await builder.IsMetBy("a", context, CancellationToken.None);

		await That(context.TryReceive(SharesValueConstraint<string>.Key, out string? _)).IsFalse()
			.Because("the values stored for an item are only visible while the item is evaluated");
	}

	[Test]
	public async Task Store_InAMemberOfWhich_ShouldNotReceiveTheValuesOfTheSubject()
	{
		Pair sut = new("a", "b");
		IThat<Pair> that = That(sut);

		async Task Act()
			=> await new ExpectationResult(that.Get().ExpectationBuilder
				.AddConstraint((_, _) => new SharesValueConstraint<Pair>(p => p.First))
				.ForWhich<Pair, string>(p => p.Second, " which ")
				.AddConstraint((_, _) => new SharesValueConstraint<string>(s => s)));

		await That(Act).DoesNotThrow()
			.Because("the member is evaluated with its own stored values");
	}

	[Test]
	public async Task Store_InAMemberOfWhose_ShouldNotReceiveTheValuesOfTheSubject()
	{
		Pair sut = new("a", "b");
		IThat<Pair> that = That(sut);

		async Task Act()
			=> await new AndOrWhoseResult<Pair, IThat<Pair>>(that.Get().ExpectationBuilder
					.AddConstraint((_, _) => new SharesValueConstraint<Pair>(p => p.First)), that)
				.Whose(p => p.Second, s => s.Get().ExpectationBuilder
					.AddConstraint((_, _) => new SharesValueConstraint<string?>(x => x!)));

		await That(Act).DoesNotThrow()
			.Because("the member is evaluated with its own stored values");
	}

	[Test]
	public async Task Store_InItemEvaluations_ShouldNotShareTheValuesBetweenTheItems()
	{
		Context context = new();
		ManualExpectationBuilder<string> builder = new();
		builder.AddConstraint((_, _) => new SharesValueConstraint<string>(s => s));

		ConstraintResult first = await builder.IsMetBy("a", context, CancellationToken.None);
		ConstraintResult second = await builder.IsMetBy("b", context, CancellationToken.None);

		await That(first.Outcome).IsEqualTo(Outcome.Success);
		await That(second.Outcome).IsEqualTo(Outcome.Success)
			.Because("the second item must not receive the value stored for the first one");
	}

	[Test]
	public async Task TimeSystem_ShouldDefaultToTheRealTimeSystem()
	{
		Context context = new();

		await That(context.TimeSystem).IsSameAs(RealTimeSystem.Instance);
	}

	[Test]
	public async Task UseMaterializedEnumerable_InAnItemEvaluation_ShouldShareTheMaterializationOfTheEvaluation()
	{
		Context context = new();
		DisposeTrackingEnumerable source = new(null, 1, 2);
		IEnumerable<int> materialized = context.UseMaterializedEnumerable(source);
		_ = materialized.First();
		MaterializesConstraint constraint = new(source);
		ManualExpectationBuilder<int> builder = new();
		builder.AddConstraint((_, _) => constraint);

		await builder.IsMetBy(1, context, CancellationToken.None);
		await context.ReleaseMaterializations();

		await That(constraint.Materialized).IsSameAs(materialized)
			.Because("the items share the materialized collections of the evaluation");
		await That(source.DisposeCount).IsEqualTo(1);
	}

	[Test]
	public async Task WhenNotStoredPreviously_ShouldReturnFalse()
	{
		IEvaluationContext context = await GetSut();

		bool result = context.TryReceive("foo", out string? fooResult);
		await That(result).IsFalse();
		await That(fooResult).IsNull();
	}

	[Test]
	public async Task WhenTypeDoesNotMatch_ShouldReturnFalse()
	{
		IEvaluationContext context = await GetSut();

		context.Store("foo", 42);

		bool result = context.TryReceive("foo", out string? fooResult);
		await That(result).IsFalse();
		await That(fooResult).IsNull();
	}

	[Test]
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

	private sealed class MaterializesConstraint(IEnumerable<int> source) : IContextConstraint<int>
	{
		public IEnumerable<int>? Materialized { get; private set; }

		public ConstraintResult IsMetBy(int actual, IEvaluationContext context)
		{
			Materialized = context.UseMaterializedEnumerable(source);
			return new DummyConstraintResult<int>(Outcome.Success, actual, "materializes the source");
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("materializes the source");
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

	private sealed class Pair(string first, string second)
	{
		public string First { get; } = first;
		public string Second { get; } = second;
	}

	/// <summary>
	///     Stores the selected value, or compares it with the value that is already stored, like a constraint that caches
	///     what it parsed from the subject for the following constraints.
	/// </summary>
	private sealed class SharesValueConstraint<T>(Func<T, string> selector) : IContextConstraint<T>
	{
		public const string Key = "SharesValue";

		public ConstraintResult IsMetBy(T actual, IEvaluationContext context)
		{
			string value = selector(actual);
			if (!context.TryReceive(Key, out string? stored))
			{
				context.Store(Key, value);
				stored = value;
			}

			return new DummyConstraintResult<T>(stored == value ? Outcome.Success : Outcome.Failure, actual,
				"shares the value", $"it received {stored}");
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("shares the value");
	}
}
