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
			NullabilityWarning("CS8602", false)
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
			NullabilityWarning("CS8602", true)
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
			NullabilityWarning("CS8602", false)
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
			NullabilityWarning("CS8602", false)
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
			NullabilityWarning("CS8602", false)
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
			NullabilityWarning("CS8602", false)
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
			NullabilityWarning("CS8602", false)
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
			NullabilityWarning("CS8602", true)
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
			NullabilityWarning("CS8600", true)
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
			NullabilityWarning("CS8629", true)
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
			NullabilityWarning("CS8604", true)
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
			NullabilityWarning("CS8602", false)
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
			NullabilityWarning("CS8602", false)
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
			NullabilityWarning("CS8602", true)
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
			NullabilityWarning("CS8602", true)
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
			NullabilityWarning("CS8602", false)
		);

	private static DiagnosticResult NullabilityWarning(string diagnosticId, bool isSuppressed)
		=> DiagnosticResult.CompilerWarning(diagnosticId).WithLocation(0).WithIsSuppressed(isSuppressed);
}
