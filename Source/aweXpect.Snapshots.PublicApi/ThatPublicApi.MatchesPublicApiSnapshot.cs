using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect.Snapshots.PublicApi;

/// <summary>
///     Public API snapshot expectations.
/// </summary>
public static class ThatPublicApi
{
	/// <summary>
	///     Verifies that the public API of the assembly matches the snapshot
	///     <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.txt</c> next to the test file.
	/// </summary>
	/// <remarks>
	///     The public API is written as C# declarations by <see cref="PublicApiDeclarations.GenerateFrom(Assembly)" />. It usually
	///     differs between the target frameworks of a library, so keep a snapshot per target framework of the test project
	///     with <c>.UniqueFor.TargetFramework()</c>; each target framework of the test project loads the matching build of
	///     the library.<br />
	///     Otherwise, it behaves like <c>MatchesSnapshot()</c> for strings, e.g. for the files and accepting changes.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="callerFilePath">The path of the test file, which is set by the compiler.</param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static SnapshotResult<string?, IThat<string?>> MatchesPublicApiSnapshot(
		this IThat<Assembly?> subject,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
		=> AddPublicApiSnapshot(subject, PublicApiDeclarations.GenerateFrom, callerFilePath, callerMemberName);

	/// <summary>
	///     Verifies that the public API of the assembly in the file matches the snapshot
	///     <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.txt</c> next to the test file.
	/// </summary>
	/// <remarks>
	///     The assembly is read without loading it by <see cref="PublicApiDeclarations.GenerateFrom(FileInfo)" />,
	///     e.g. a build for a target framework that the tests cannot run on or a reference assembly.<br />
	///     Otherwise, it behaves like the overload for an <see cref="Assembly" />.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="callerFilePath">The path of the test file, which is set by the compiler.</param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static SnapshotResult<string?, IThat<string?>> MatchesPublicApiSnapshot(
		this IThat<FileInfo?> subject,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
		=> AddPublicApiSnapshot(subject, PublicApiDeclarations.GenerateFrom, callerFilePath,
			callerMemberName);

	[SuppressMessage("Minor Code Smell", "S3236:Caller information arguments should not be provided explicitly",
		Justification = "The snapshot is named after the test that calls the public API expectation.")]
	private static SnapshotResult<string?, IThat<string?>> AddPublicApiSnapshot<TSubject>(
		IThat<TSubject?> subject,
		Func<TSubject, string> write,
		string callerFilePath,
		string callerMemberName)
		where TSubject : class
	{
		SnapshotResult<string?, IThat<string?>>? result = null;
		subject.Get().ExpectationBuilder
			.ForMember(MemberAccessor<TSubject?, string?>.FromFunc(
					value => value is null ? null : write(value), "public API"),
				(_, stringBuilder) => stringBuilder.Append("has a public API which "))
			.AddExpectations(expectationBuilder => result =
				new ThatSubject<string?>(expectationBuilder).MatchesSnapshot(callerFilePath, callerMemberName));
		return result!;
	}
}
