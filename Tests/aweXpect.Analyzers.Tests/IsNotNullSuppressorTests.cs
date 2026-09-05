using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verifier = aweXpect.Analyzers.Tests.Verifiers.CSharpSuppressorVerifier<aweXpect.Analyzers.IsNotNullSuppressor>;

namespace aweXpect.Analyzers.Tests;

public class IsNotNullSuppressorTests
{
	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task WhenExpectationAllowsNullString_ShouldNotSuppressWarning() => await Verifier
		.VerifySuppressorAsync(
			// A null string fulfils `IsNotEmpty`, even though its result type is the not-nullable string.
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
			NotSuppressedNullabilityWarning()
		);

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	[Fact]
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

	private static DiagnosticResult NotSuppressedNullabilityWarning()
		=> DiagnosticResult.CompilerWarning("CS8602").WithLocation(0).WithIsSuppressed(false);

	private static DiagnosticResult SuppressedNullabilityWarning()
		=> DiagnosticResult.CompilerWarning("CS8602").WithLocation(0).WithIsSuppressed(true);

	private static DiagnosticResult SuppressedNullabilityWarning(string diagnosticId)
		=> DiagnosticResult.CompilerWarning(diagnosticId).WithLocation(0).WithIsSuppressed(true);
}
