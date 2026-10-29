using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using TUnit.Core.Exceptions;

namespace aweXpect.Reflection.Tests;

public sealed class CollectionConstraintTests
{
	[Test]
	public async Task WhenCanceled_ShouldNotBeVerified()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		IEnumerable<Type?> subject = [typeof(string), typeof(int),];

		async Task Act()
		{
			await That(subject).HaveNamespace("Foo").WithCancellation(cts.Token);
		}

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that subject
			             all have namespace equal to "Foo",
			             but it could not be verified, because the evaluation was already canceled

			             Collection:
			             [
			               string,
			               int
			             ]
			             """);
	}

	[Test]
	public async Task WhenCombinedWithAnd_ShouldEnumerateTheSubjectOnlyOnce()
	{
		int enumerations = 0;

		IEnumerable<Type?> Counted()
		{
			enumerations++;
			yield return typeof(string);
			yield return typeof(CollectionConstraintTests);
		}

		await That(Counted()).AreSealed().And.AreNotAbstract().And.AreClasses().And.HaveNamespace("*")
			.AsWildcard().And.HaveNoDependencyCycles();

		await That(enumerations).IsEqualTo(1)
			.Because("all expectations on the subject share one materialized copy of it");
	}

	[Test]
	public async Task WhenCombinedWithAnd_ShouldNotRepeatTheSubject()
	{
		IEnumerable<Type?> subject = [typeof(string), typeof(int),];

		async Task Act()
		{
			await That(subject).AreAbstract().And.AreInterfaces();
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all are abstract and all are interfaces,
			             but none of 2 were

			             Not matching items:
			             [
			               string,
			               int
			             ]

			             Collection:
			             [
			               string,
			               int
			             ]
			             """);
	}

	[Test]
	public async Task WhenCombinedWithOr_ShouldNotRepeatTheSubject()
	{
		IEnumerable<Type?> subject = [typeof(string), typeof(int),];

		async Task Act()
		{
			await That(subject).AreAbstract().Or.AreInterfaces();
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all are abstract or all are interfaces,
			             but none of 2 were

			             Not matching items:
			             [
			               string,
			               int
			             ]

			             Collection:
			             [
			               string,
			               int
			             ]
			             """);
	}

	[Test]
	public async Task WhenItemIsNull_ShouldFailBothWays()
	{
		IEnumerable<Type?> subject = [null, typeof(string),];

		async Task Act()
		{
			await That(subject).AreSealed();
		}

		async Task ActNegated()
		{
			await That(subject).DoesNotComplyWith(they => they.AreSealed());
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all are sealed,
			             but for the item at index 0, it was <null>

			             Collection:
			             [
			               <null>,
			               string
			             ]
			             """);
		await That(ActNegated).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             not all are sealed,
			             but for the item at index 0, it was <null>

			             Collection:
			             [
			               <null>,
			               string
			             ]
			             """).Because("a null item cannot be inspected, like a null subject");
	}

	[Test]
	public async Task WhenNegatedAndCombinedWithAnd_ShouldNotRepeatTheSubject()
	{
		IEnumerable<MethodInfo> subject = [typeof(string).GetMethod(nameof(string.Clone))!,];

		async Task Act()
		{
			await That(subject).DoesNotComplyWith(they => they.ArePublic().And.AreNotPrivate());
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             not all are public or at least one is private,
			             but all 1 were and none of 1 were

			             Collection:
			             [
			               object string.Clone()
			             ]
			             """);
	}

	[Test]
	public async Task WhenNegatedAndCombinedWithOr_ShouldStartWithTheSubject()
	{
		IEnumerable<MethodInfo> subject = [typeof(string).GetMethod(nameof(string.Clone))!,];

		async Task Act()
		{
			await That(subject).DoesNotComplyWith(they => they.ArePublic().Or.AreNotPrivate());
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             not all are public and at least one is private,
			             but all 1 were

			             Collection:
			             [
			               object string.Clone()
			             ]
			             """);
	}

	[Test]
	public async Task WhenSubjectIsNull_ShouldFail()
	{
		IEnumerable<Type?>? subject = null;

		async Task Act()
		{
			await That(subject).AreSealed();
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             all are sealed,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task WhenMemberIsNull_ShouldUseThePluralVerb()
	{
		Holder subject = new(null!);

		async Task Act()
		{
			await That(subject).Whose(h => h.Types, t => t.AreSealed());
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             whose Types all are sealed,
			             but Types were <null>
			             """);
	}

	[Test]
	public async Task WhenSubjectIsNull_ShouldFailForDependencyCycles()
	{
		IEnumerable<Type?>? subject = null;

		async Task Act()
		{
			await That(subject).HaveNoDependencyCycles();
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             have no dependency cycles,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task WhenSubjectIsNull_ShouldFailWhenNegated()
	{
		IEnumerable<Type?>? subject = null;

		async Task Act()
		{
			await That(subject).DoesNotComplyWith(they => they.AreSealed());
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             not all are sealed,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task WhenUsingAQuantifier_ShouldOnlyRequireTheSelectedItems()
	{
		IEnumerable<Type?> subject = [typeof(string), typeof(object), typeof(int),];

		async Task Act()
		{
			await That(subject).AtLeast(3).AreSealed();
		}

		await That(subject).AtLeast(2).AreSealed();
		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is sealed for at least 3 items,
			             but only 2 of 3 were

			             Not matching items:
			             [
			               object
			             ]

			             Collection:
			             [
			               string,
			               object,
			               int
			             ]
			             """);
	}

	[Test]
	public async Task WhenUsingAQuantifierOnDerivedTypes_ShouldApplyTheExpectationOfTheBaseType()
	{
		IEnumerable<System.Reflection.TypeInfo> subject =
			[typeof(string).GetTypeInfo(), typeof(object).GetTypeInfo(),];

		async Task Act()
		{
			await That(subject).All().AreSealed();
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is sealed for all items,
			             but only 1 of 2 were

			             Not matching items:
			             [
			               object
			             ]

			             Collection:
			             [
			               string,
			               object
			             ]
			             """).Because("the expectation on the items of Type applies to the items of the derived TypeInfo");
	}

	[Test]
	public async Task WhenUsingAQuantifierOnMembers_ShouldFormatTheMembers()
	{
		IEnumerable<MethodInfo> subject =
		[
			typeof(string).GetMethod(nameof(string.Clone))!,
			typeof(string).GetMethod(nameof(string.IsNullOrEmpty))!,
		];

		async Task Act()
		{
			await That(subject).All().AreStatic();
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is static for all items,
			             but only 1 of 2 were

			             Not matching items:
			             [
			               object string.Clone()
			             ]

			             Collection:
			             [
			               object string.Clone(),
			               bool string.IsNullOrEmpty(string value)
			             ]
			             """);
	}

#if NET8_0_OR_GREATER
	[Test]
	public async Task WhenAsyncSubjectIsCanceled_ShouldNotBeVerified()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		IAsyncEnumerable<Type?> subject = ToAsyncEnumerable(typeof(string), typeof(int));

		async Task Act()
		{
			await That(subject).AreSealed().WithCancellation(cts.Token);
		}

		await That(Act).Throws<InconclusiveTestException>()
			.WithMessage("""
			             Expected that subject
			             all are sealed,
			             but it could not be verified, because the evaluation was already canceled
			             """);
	}

	[Test]
	public async Task WhenAsyncSubjectIsCombinedWithAnd_ShouldEnumerateTheSubjectOnlyOnce()
	{
		int enumerations = 0;

		async IAsyncEnumerable<Type?> Counted()
		{
			enumerations++;
			await Task.Yield();
			yield return typeof(string);
			yield return typeof(CollectionConstraintTests);
		}

		await That(Counted()).AreSealed().And.AreNotAbstract().And.AreClasses().And.HaveNamespace("*")
			.AsWildcard().And.HaveNoDependencyCycles();

		await That(enumerations).IsEqualTo(1)
			.Because("all expectations on the subject share one materialized copy of it");
	}

	[Test]
	public async Task WhenAsyncSubjectIsNull_ShouldFailWhenNegated()
	{
		IAsyncEnumerable<Type?>? subject = null;

		async Task Act()
		{
			await That(subject).DoesNotComplyWith(they => they.AreSealed());
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             not all are sealed,
			             but it was <null>
			             """);
	}

	private static async IAsyncEnumerable<Type?> ToAsyncEnumerable(params Type?[] types)
	{
		foreach (Type? type in types)
		{
			await Task.Yield();
			yield return type;
		}
	}
#endif

	private sealed record Holder(IEnumerable<Type?> Types);
}
