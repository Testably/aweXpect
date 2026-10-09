using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests;

public sealed class CollectionConstraintTests
{
	[Fact]
	public async Task WhenCanceled_ShouldNotBeVerified()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		IEnumerable<Type?> subject = [typeof(string), typeof(int),];

		async Task Act()
		{
			await That(subject).HaveNamespace("Foo").WithCancellation(cts.Token);
		}

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected that subject
			             all have namespace equal to "Foo",
			             but it could not be verified, because the evaluation was already canceled
			             """);
	}

	[Fact]
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

	[Fact]
	public async Task WhenCombinedWithAnd_ShouldNotRepeatTheSubject()
	{
		IEnumerable<Type?> subject = [typeof(string), typeof(int),];

		async Task Act()
		{
			await That(subject).AreAbstract().And.AreInterfaces();
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             are all abstract and are all interfaces,
			             but it contained non-abstract types and contained other types

			             Not matching items:
			             [
			               string,
			               int
			             ]
			             """);
	}

	[Fact]
	public async Task WhenCombinedWithOr_ShouldNotRepeatTheSubject()
	{
		IEnumerable<Type?> subject = [typeof(string), typeof(int),];

		async Task Act()
		{
			await That(subject).AreAbstract().Or.AreInterfaces();
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             are all abstract or are all interfaces,
			             but it contained non-abstract types and contained other types

			             Not matching items:
			             [
			               string,
			               int
			             ]
			             """);
	}

	[Fact]
	public async Task WhenNegatedAndCombinedWithAnd_ShouldNotRepeatTheSubject()
	{
		IEnumerable<MethodInfo> subject = [typeof(string).GetMethod(nameof(string.Clone))!,];

		async Task Act()
		{
			await That(subject).DoesNotComplyWith(they => they.ArePublic().And.AreNotPrivate());
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             not all are public or at least one is private,
			             but it only contained public items and contained no private items

			             Collection:
			             [
			               object string.Clone()
			             ]
			             """);
	}

	[Fact]
	public async Task WhenNegatedAndCombinedWithOr_ShouldStartWithTheSubject()
	{
		IEnumerable<MethodInfo> subject = [typeof(string).GetMethod(nameof(string.Clone))!,];

		async Task Act()
		{
			await That(subject).DoesNotComplyWith(they => they.ArePublic().Or.AreNotPrivate());
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             not all are public and at least one is private,
			             but it only contained public items

			             Collection:
			             [
			               object string.Clone()
			             ]
			             """);
	}

	[Fact]
	public async Task WhenSubjectIsNull_ShouldFail()
	{
		IEnumerable<Type?>? subject = null;

		async Task Act()
		{
			await That(subject).AreSealed();
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             are all sealed,
			             but it was <null>
			             """);
	}

	[Fact]
	public async Task WhenMemberIsNull_ShouldUseThePluralVerb()
	{
		Holder subject = new(null!);

		async Task Act()
		{
			await That(subject).Whose(h => h.Types, t => t.AreSealed());
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             whose Types are all sealed,
			             but Types were <null>
			             """);
	}

	[Fact]
	public async Task WhenSubjectIsNull_ShouldFailForDependencyCycles()
	{
		IEnumerable<Type?>? subject = null;

		async Task Act()
		{
			await That(subject).HaveNoDependencyCycles();
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             have no dependency cycles,
			             but it was <null>
			             """);
	}

	[Fact]
	public async Task WhenSubjectIsNull_ShouldFailWhenNegated()
	{
		IEnumerable<Type?>? subject = null;

		async Task Act()
		{
			await That(subject).DoesNotComplyWith(they => they.AreSealed());
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             are not all sealed,
			             but it was <null>
			             """);
	}

#if NET8_0_OR_GREATER
	[Fact]
	public async Task WhenAsyncSubjectIsCanceled_ShouldNotBeVerified()
	{
		using CancellationTokenSource cts = new();
		cts.Cancel();
		IAsyncEnumerable<Type?> subject = ToAsyncEnumerable(typeof(string), typeof(int));

		async Task Act()
		{
			await That(subject).AreSealed().WithCancellation(cts.Token);
		}

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected that subject
			             are all sealed,
			             but it could not be verified, because the evaluation was already canceled
			             """);
	}

	[Fact]
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

	[Fact]
	public async Task WhenAsyncSubjectIsNull_ShouldFailWhenNegated()
	{
		IAsyncEnumerable<Type?>? subject = null;

		async Task Act()
		{
			await That(subject).DoesNotComplyWith(they => they.AreSealed());
		}

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             are not all sealed,
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
