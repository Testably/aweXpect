using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Snapshots.PublicApi;
using aweXpect.Snapshots.PublicApi.Options;
using aweXpect.Snapshots.Results;

namespace aweXpect;

/// <summary>
///     Public API snapshot expectations.
/// </summary>
public static class ThatPublicApi
{
	/// <summary>
	///     Verifies that the public API of the assembly, generated with the optional <paramref name="options" />, matches
	///     the snapshot <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.txt</c> next to the test file.
	/// </summary>
	/// <remarks>
	///     The public API is written as C# declarations by
	///     <see cref="PublicApiDeclarations.GenerateFrom(Assembly, Func{PublicApiOptions, PublicApiOptions})" />. It
	///     usually differs between the target frameworks of a library, so keep a snapshot per target framework of the test
	///     project with <c>.UniqueFor.TargetFramework()</c>; each target framework of the test project loads the matching
	///     build of the library.<br />
	///     The generated text is compared like a string by <c>MatchesSnapshot()</c>, e.g. for the files, the rules and
	///     accepting changes, while <c>.And</c> continues on the assembly and awaiting the result returns it.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="options">
	///     The options for generating the public API, e.g. <c>o => o.Ignoring.ForwardedTypes()</c>, or
	///     <see langword="null" /> for the defaults.
	/// </param>
	/// <param name="callerFilePath">The path of the test file, which is set by the compiler.</param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static StringSnapshotResult<Assembly, IThat<Assembly?>> MatchesPublicApiSnapshot(
		this IThat<Assembly?> subject,
		Func<PublicApiOptions, PublicApiOptions>? options = null,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
		=> AddPublicApiSnapshot(subject, options,
			static (assembly, generateWith) => PublicApiDeclarations.GenerateFrom(assembly, generateWith),
			callerFilePath, callerMemberName);

	/// <summary>
	///     Verifies that the public API of the assembly in the file, generated with the optional
	///     <paramref name="options" />, matches the snapshot <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.txt</c>
	///     next to the test file.
	/// </summary>
	/// <remarks>
	///     The assembly is read without loading it by
	///     <see cref="PublicApiDeclarations.GenerateFrom(FileInfo, Func{PublicApiOptions, PublicApiOptions})" />,
	///     e.g. a build for a target framework that the tests cannot run on or a reference assembly.<br />
	///     Otherwise, it behaves like the overload for an <see cref="Assembly" />, so <c>.And</c> continues on the file
	///     and awaiting the result returns it.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="options">
	///     The options for generating the public API, e.g. <c>o => o.Ignoring.ForwardedTypes()</c>, or
	///     <see langword="null" /> for the defaults.
	/// </param>
	/// <param name="callerFilePath">The path of the test file, which is set by the compiler.</param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static StringSnapshotResult<FileInfo, IThat<FileInfo?>> MatchesPublicApiSnapshot(
		this IThat<FileInfo?> subject,
		Func<PublicApiOptions, PublicApiOptions>? options = null,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
		=> AddPublicApiSnapshot(subject, options,
			static (file, generateWith) => PublicApiDeclarations.GenerateFrom(file, generateWith),
			callerFilePath, callerMemberName);

	/// <remarks>
	///     The snapshot is added on the member of the generated public API, and its options are shared with the result
	///     on the subject, so that e.g. <c>.Named(...)</c> on the result still names the snapshot.<br />
	///     The options that the public API was generated with are kept for the "Public API options" of a failure.
	/// </remarks>
	[SuppressMessage("Minor Code Smell", "S3236:Caller information arguments should not be provided explicitly",
		Justification = "The snapshot is named after the test that calls the public API expectation.")]
	private static StringSnapshotResult<TSubject, IThat<TSubject?>> AddPublicApiSnapshot<TSubject>(
		IThat<TSubject?> subject,
		Func<PublicApiOptions, PublicApiOptions>? options,
		Func<TSubject, Func<PublicApiOptions, PublicApiOptions>, string> write,
		string callerFilePath,
		string callerMemberName)
		where TSubject : class
	{
		PublicApiOptions? usedOptions = null;
		StringSnapshotResult<string, IThat<string?>>? snapshot = null;
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder
			.ForMember(MemberAccessor<TSubject?, string?>.FromFuncWithName(
					value => value is null
						? null
						: write(value, defaults => usedOptions = options?.Invoke(defaults) ?? defaults),
					"public API"),
				(_, stringBuilder) => stringBuilder.Append("has a public API which "))
			.AddExpectations(memberBuilder =>
			{
				memberBuilder.AddSubjectContexts<string>((_, contexts) => usedOptions?.AddContext(contexts));
				snapshot = new ThatSubject<string?>(memberBuilder).MatchesSnapshot(callerFilePath, callerMemberName);
			});
		return new StringSnapshotResult<TSubject, IThat<TSubject?>>(expectationBuilder, subject, snapshot!.Options);
	}
}
