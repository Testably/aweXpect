using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpSuppressorVerifier<aweXpect.Analyzers.IsNotNullSuppressor>;

namespace aweXpect.Analyzers.Tests;

public class IsNotNullSuppressorTests
{
	[Test]
	public async Task WhenAwaitedExpectationIsAssigned_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        var result = await Expect.That(subject).IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenAwaiterResultIsNotRequested_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using aweXpect;

			public class MyClass
			{
			    public void MyTest(string? subject)
			    {
			        Expect.That(subject).IsNotNull().GetAwaiter();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenControlFlowIsBetweenExpectationAndUsage_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, bool condition)
			    {
			        await Expect.That(subject).IsNotNull();
			        if (condition)
			        {
			            subject = null;
			        }

			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationAllowsNullCollection_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			// A null collection fulfils `IsNotEqualTo`, even though its result type is the not-nullable collection.
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(int[]? subject)
			    {
			        await Expect.That(subject).IsNotEqualTo([1, 2]);
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationAllowsNullString_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			// A null string fulfils `IsNotEqualTo`, even though its result type is the not-nullable string.
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotEqualTo("foo");
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationHasLookAlikeAttribute_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			// The attribute must come from aweXpect itself, not from a look-alike in the user code.
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			[AttributeUsage(AttributeTargets.Method)]
			public sealed class GuaranteesNotNullAttribute : Attribute;

			public static class MyExpectations
			{
			    [GuaranteesNotNull]
			    public static IThat<string?> IsDefinitelyNotNull(this IThat<string?> source) => source;
			}

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Task.Yield();
			        _ = Expect.That(subject).IsDefinitelyNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsABareStatement_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using aweXpect;

			public class MyClass
			{
			    public void MyTest(string? subject)
			    {
			        Expect.That(subject).IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsCombinedWithAnd_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsEqualTo("foo").And.IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsCombinedWithThatAll_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, string? other)
			    {
			        await Expect.ThatAll(Expect.That(subject).IsNotNull(), Expect.That(other).IsNotNull());
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsCombinedWithThatAny_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, string? other)
			    {
			        await Expect.ThatAny(Expect.That(subject).IsNotNull(), Expect.That(other).IsNotNull());
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsDeclaredInCore_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			// `IsExactly` is an instance member of `IThatSubject<T>` rather than an `IThat<T>` extension.
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(object? subject)
			    {
			        await Expect.That(subject).IsExactly<string>();
			        _ = {|#0:subject|}.ToString();
			    }
			}
			""",
			SuppressedNullabilityWarning("CS8602")
		);

	[Test]
	public async Task WhenExpectationIsDiscarded_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Task.Yield();
			        _ = Expect.That(subject).IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsEvaluatedThroughTheAwaiter_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using aweXpect;

			public class MyClass
			{
			    public void MyTest(string? subject)
			    {
			        Expect.That(subject).IsNotNull().GetAwaiter().GetResult();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsFollowedByOr_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNull().Or.IsEqualTo("foo");
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsInConditionalBlock_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, bool condition)
			    {
			        if (condition)
			        {
			            await Expect.That(subject).IsNotNull();
			        }

			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsInConditionalExpression_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, string? other, bool condition)
			    {
			        await (condition ? Expect.That(other).IsNotNull() : Expect.That(subject).IsNotNull());
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsInsideLambda_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        Func<Task> expectation = async () => { await Expect.That(subject).IsNotNull(); };
			        await expectation();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsPassedAsArgument_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Collections.Generic;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        var list = new List<object>();
			        list.Add(Expect.That(subject).IsNotNull());
			        await Helper(Expect.That(subject).IsNotNull());
			        _ = {|#0:subject|}.Length;
			    }

			    private static Task Helper(object expectation) => Task.CompletedTask;
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsPrecededByOr_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsEqualTo("foo").Or.IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsStoredAndAwaitedAfterUsage_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        var expectation = Expect.That(subject).IsNotNull();
			        _ = {|#0:subject|}.Length;
			        await expectation;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationIsVerifiedStatically_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public void MyTest(string? subject)
			    {
			        Synchronously.Verify(Expect.That(subject).IsNotNull());
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationRequiresNotEmptyCollection_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(int[]? subject)
			    {
			        await Expect.That(subject).IsNotEmpty();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning("CS8602")
		);

	[Test]
	public async Task WhenExpectationRequiresNotEmptyString_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotEmpty();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationRequiresNotExactlyOfType_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(object? subject)
			    {
			        await Expect.That(subject).IsNotExactly<string>();
			        _ = {|#0:subject|}.ToString();
			    }
			}
			""",
			SuppressedNullabilityWarning("CS8602")
		);

	[Test]
	public async Task WhenExpectationRequiresNotNullOrEmpty_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNullOrEmpty();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationRequiresNotNullOrEmptyGuid_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(Guid? subject)
			    {
			        await Expect.That(subject).IsNotNullOrEmpty();
			        _ = {|#0:subject|}.Value;
			    }
			}
			""",
			SuppressedNullabilityWarning("CS8629")
		);

	[Test]
	public async Task WhenExpectationRequiresNotNullOrWhiteSpace_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNullOrWhiteSpace();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExpectationRequiresNotOfType_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(object? subject)
			    {
			        await Expect.That(subject).IsNot<string>();
			        _ = {|#0:subject|}.ToString();
			    }
			}
			""",
			SuppressedNullabilityWarning("CS8602")
		);

	[Test]
	public async Task WhenExtensionExpectationFromReferencedAssemblyGuaranteesNotNull_ShouldSuppressWarning()
		=> await Verifier
			.VerifySuppressorWithExtensionAsync(
				"""
				using System.Threading.Tasks;
				using aweXpect;
				using MyExtension;

				public class MyClass
				{
				    public async Task MyTest(string? subject)
				    {
				        await Expect.That(subject).IsAbsolutePath();
				        _ = {|#0:subject|}.Length;
				    }
				}
				""",
				"""
				using System;
				using aweXpect.Core;
				using aweXpect.Results;

				namespace MyExtension;

				public static class MyExpectations
				{
				    [GuaranteesNotNull]
				    public static AndOrResult<string, IThat<string?>> IsAbsolutePath(this IThat<string?> subject)
				        => throw new NotSupportedException();
				}
				""",
				SuppressedNullabilityWarning()
			);

	[Test]
	public async Task WhenExtensionExpectationGuaranteesNotNull_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;
			using aweXpect.Results;

			public static class MyExpectations
			{
			    [GuaranteesNotNull]
			    public static AndOrResult<string, IThat<string?>> IsAbsolutePath(this IThat<string?> subject)
			        => throw new NotSupportedException();
			}

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsAbsolutePath();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExtensionExpectationIsFollowedByOr_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;
			using aweXpect.Results;

			public static class MyExpectations
			{
			    [GuaranteesNotNull]
			    public static AndOrResult<string, IThat<string?>> IsAbsolutePath(this IThat<string?> subject)
			        => throw new NotSupportedException();
			}

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsAbsolutePath().Or.IsNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExtensionExpectationWithoutAttribute_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;
			using aweXpect.Results;

			public static class MyExpectations
			{
			    public static AndOrResult<string, IThat<string?>> IsAbsolutePath(this IThat<string?> subject)
			        => throw new NotSupportedException();
			}

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsAbsolutePath();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExtensionKeepsSubjectBeforeAnd_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;
			using aweXpect.Results;

			public static class MyExpectations
			{
			    public static AndOrResult<string?, IThat<string?>> IsAbsolutePath(this IThat<string?> subject)
			        => throw new NotSupportedException();
			}

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsAbsolutePath().And.IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExtensionReturnsThatOfSameTypeBeforeExpectation_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			// The returned `IThat<string?>` could be a member of the same type, e.g. the file name of a path.
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public static class MyExpectations
			{
			    public static IThat<string?> WhoseFileName(this IThat<string?> subject)
			        => throw new NotSupportedException();
			}

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).WhoseFileName().IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenExtensionSwitchesSubjectBeforeExpectation_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			// The extension continues with a member of the subject, so `IsNotNull` verifies the member.
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;
			using aweXpect.Core;

			public class Album
			{
			    public string? Title { get; set; }
			}

			public static class MyExpectations
			{
			    public static IThat<string?> WhoseTitle(this IThat<Album?> subject)
			        => throw new NotSupportedException();
			}

			public class MyClass
			{
			    public async Task MyTest(Album? subject)
			    {
			        await Expect.That(subject).WhoseTitle().IsNotNull();
			        _ = {|#0:subject|}.Title;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenGotoSkipsTheExpectation_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, bool condition)
			    {
			        if (condition) goto skip;
			        await Expect.That(subject).IsNotNull();
			        skip:
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenLabelIsBetweenExpectationAndUsage_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, bool condition)
			    {
			        if (condition) goto skip;
			        await Expect.That(subject).IsNotNull();
			        skip: ;
			        if (condition)
			        {
			            _ = {|#0:subject|}.Length;
			        }
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenOtherSubjectIsExpectedNotNull_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, string? other)
			    {
			        await Expect.That(other).IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsArray_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string[]? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsAssignedToNonNullable_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        string value = {|#0:subject|};
			    }
			}
			""",
			SuppressedNullabilityWarning("CS8600")
		);

	[Test]
	public async Task WhenSubjectIsDeconstructedAfterExpectation_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, string? other)
			    {
			        await Expect.That(subject).IsNotNull();
			        (subject, other) = GetValues();
			        _ = {|#0:subject|}.Length;
			    }

			    private static (string?, string?) GetValues() => (null, null);
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsField_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    private string? _subject = "foo";

			    public async Task MyTest()
			    {
			        await Expect.That(_subject).IsNotNull();
			        _ = {|#0:_subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsNullableValueType_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(int? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        _ = {|#0:subject|}.Value;
			    }
			}
			""",
			SuppressedNullabilityWarning("CS8629")
		);

	[Test]
	public async Task WhenSubjectIsPassedAsArgument_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        Consume({|#0:subject|});
			    }

			    private static void Consume(string value)
			    {
			    }
			}
			""",
			SuppressedNullabilityWarning("CS8604")
		);

	[Test]
	public async Task WhenSubjectIsPrimaryConstructorParameter_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass(string? subject)
			{
			    private void Reset() => subject = null;

			    public async Task MyTest()
			    {
			        await Expect.That(subject).IsNotNull();
			        Reset();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsProperty_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    private static int _count;

			    private string? Subject => _count++ % 2 == 0 ? "foo" : null;

			    public async Task MyTest()
			    {
			        await Expect.That(Subject).IsNotNull();
			        _ = {|#0:Subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsReassignedAfterExpectation_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, string? other)
			    {
			        await Expect.That(subject).IsNotNull();
			        subject = other;
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsReassignedInLoop_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, int[] items)
			    {
			        await Expect.That(subject).IsNotNull();
			        foreach (int item in items)
			        {
			            _ = {|#0:subject|}.Length;
			            subject = null;
			        }
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsReassignedInNestedBlock_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, string? other, bool condition)
			    {
			        await Expect.That(subject).IsNotNull();
			        if (condition)
			        {
			            subject = other;
			            _ = {|#0:subject|}.Length;
			        }
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsReassignedInSwitchSection_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, int mode)
			    {
			        await Expect.That(subject).IsNotNull();
			        switch (mode)
			        {
			            case 1:
			                subject = null;
			                _ = {|#0:subject|}.Length;
			                break;
			        }
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsReassignedInTryBlock_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        try
			        {
			            subject = null;
			        }
			        finally
			        {
			            _ = {|#0:subject|}.Length;
			        }
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsRefLocal_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    private string? _field = "foo";

			    public void MyTest()
			    {
			        ref string? subject = ref _field;
			        Expect.That(subject).IsNotNull().VerifySynchronously();
			        Reset();
			        _ = {|#0:subject|}.Length;
			    }

			    private void Reset() => _field = null;
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsRefParameter_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    private string? _field = "foo";

			    public void MyTest() => Verify(ref _field);

			    private void Verify(ref string? subject)
			    {
			        Expect.That(subject).IsNotNull().VerifySynchronously();
			        _field = null;
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsUsedAfterExpectation_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsUsedAtTopLevel_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(OutputKind.ConsoleApplication,
			"""
			using aweXpect;

			string? subject = args.Length > 0 ? args[0] : null;
			await Expect.That(subject).IsNotNull();
			_ = {|#0:subject|}.Length;
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsUsedInNestedBlock_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, bool condition)
			    {
			        await Expect.That(subject).IsNotNull();
			        if (condition)
			        {
			            _ = {|#0:subject|}.Length;
			        }
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsUsedInsideLambda_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        Func<int> length = () => {|#0:subject|}.Length;
			        _ = length();
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsVerifiedInsideLoop_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject, int[] items)
			    {
			        foreach (int item in items)
			        {
			            await Expect.That(subject).IsNotNull();
			            _ = {|#0:subject|}.Length;
			            subject = null;
			        }
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsWrittenInCapturedLambda_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System;
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        Action clear = () => subject = null;
			        await Expect.That(subject).IsNotNull();
			        clear();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsWrittenInIfCondition_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        if (TryGet(out subject))
			        {
			            _ = {|#0:subject|}.Length;
			        }
			    }

			    private static bool TryGet(out string? value)
			    {
			        value = null;
			        return true;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsWrittenInSameStatement_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class MyClass
			{
			    public async Task MyTest(string? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        Consume(Clear(out subject), {|#0:subject|}.Length);
			    }

			    private static int Clear(out string? value)
			    {
			        value = null;
			        return 0;
			    }

			    private static void Consume(int a, int b)
			    {
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsWrittenInTopLevelLambda_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(OutputKind.ConsoleApplication,
			"""
			using System;
			using aweXpect;

			string? subject = args.Length > 0 ? args[0] : null;
			Action clear = () => subject = null;
			await Expect.That(subject).IsNotNull();
			clear();
			_ = {|#0:subject|}.Length;
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectIsWrittenThroughARefAlias_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public void MyTest(string? subject)
			    {
			        ref string? alias = ref subject;
			        Expect.That(subject).IsNotNull().VerifySynchronously();
			        alias = null;
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			NotSuppressedNullabilityWarning()
		);

	[Test]
	public async Task WhenSubjectMemberIsDereferenced_ShouldSuppressOnlySubjectWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using System.Threading.Tasks;
			using aweXpect;

			public class Holder
			{
			    public string? Value = "foo";
			}

			public class MyClass
			{
			    public async Task MyTest(Holder? subject)
			    {
			        await Expect.That(subject).IsNotNull();
			        _ = subject.Value.Length;
			    }
			}
			""",
			// Only the subject was verified, its member was not.
			DiagnosticResult.CompilerWarning("CS8602").WithSpan(14, 13, 14, 20).WithIsSuppressed(true),
			DiagnosticResult.CompilerWarning("CS8602").WithSpan(14, 13, 14, 26).WithIsSuppressed(false)
		);

	[Test]
	public async Task WhenSynchronouslyVerifiedExpectationIsAssigned_ShouldSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			"""
			using aweXpect;
			using aweXpect.Synchronous;

			public class MyClass
			{
			    public void MyTest(string? subject)
			    {
			        var result = Expect.That(subject).IsNotNull().VerifySynchronously();
			        _ = {|#0:subject|}.Length;
			    }
			}
			""",
			SuppressedNullabilityWarning()
		);

	private static DiagnosticResult NotSuppressedNullabilityWarning()
		=> DiagnosticResult.CompilerWarning("CS8602").WithLocation(0).WithIsSuppressed(false);

	private static DiagnosticResult SuppressedNullabilityWarning()
		=> DiagnosticResult.CompilerWarning("CS8602").WithLocation(0).WithIsSuppressed(true);

	private static DiagnosticResult SuppressedNullabilityWarning(string diagnosticId)
		=> DiagnosticResult.CompilerWarning(diagnosticId).WithLocation(0).WithIsSuppressed(true);
}
