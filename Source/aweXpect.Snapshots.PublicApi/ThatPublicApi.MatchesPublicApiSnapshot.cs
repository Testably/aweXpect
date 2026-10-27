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
	///     The result continues on the generated public API text, not on the assembly, so <c>.And</c> adds expectations on
	///     that text: the text and line rules of the snapshot only apply to a string subject.<br />
	///     Otherwise, it behaves like <c>MatchesSnapshot()</c> for strings, e.g. for the files and accepting changes.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="options">
	///     The options for generating the public API, e.g. <c>o => o.Ignoring.ForwardedTypes()</c>, or
	///     <see langword="null" /> for the defaults.
	/// </param>
	/// <param name="callerFilePath">The path of the test file, which is set by the compiler.</param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static StringSnapshotResult<string?, IThat<string?>> MatchesPublicApiSnapshot(
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
	///     The result continues on the generated public API text, not on the file, so <c>.And</c> adds expectations on
	///     that text: the text and line rules of the snapshot only apply to a string subject.<br />
	///     Otherwise, it behaves like the overload for an <see cref="Assembly" />.
	/// </remarks>
	/// <param name="subject">The subject.</param>
	/// <param name="options">
	///     The options for generating the public API, e.g. <c>o => o.Ignoring.ForwardedTypes()</c>, or
	///     <see langword="null" /> for the defaults.
	/// </param>
	/// <param name="callerFilePath">The path of the test file, which is set by the compiler.</param>
	/// <param name="callerMemberName">The name of the test method, which is set by the compiler.</param>
	[GuaranteesNotNull]
	public static StringSnapshotResult<string?, IThat<string?>> MatchesPublicApiSnapshot(
		this IThat<FileInfo?> subject,
		Func<PublicApiOptions, PublicApiOptions>? options = null,
		[CallerFilePath] string callerFilePath = "",
		[CallerMemberName] string callerMemberName = "")
		=> AddPublicApiSnapshot(subject, options,
			static (file, generateWith) => PublicApiDeclarations.GenerateFrom(file, generateWith),
			callerFilePath, callerMemberName);

	/// <remarks>
	///     The result stays nullable, unlike the one of the string snapshot, as awaiting it does not return the
	///     generated public API.<br />
	///     The options that the public API was generated with are kept for the "Public API options" of a failure.
	/// </remarks>
	[SuppressMessage("Minor Code Smell", "S3236:Caller information arguments should not be provided explicitly",
		Justification = "The snapshot is named after the test that calls the public API expectation.")]
	private static StringSnapshotResult<string?, IThat<string?>> AddPublicApiSnapshot<TSubject>(
		IThat<TSubject?> subject,
		Func<PublicApiOptions, PublicApiOptions>? options,
		Func<TSubject, Func<PublicApiOptions, PublicApiOptions>, string> write,
		string callerFilePath,
		string callerMemberName)
		where TSubject : class
	{
		PublicApiOptions? usedOptions = null;
		StringSnapshotResult<string?, IThat<string?>>? result = null;
		subject.Get().ExpectationBuilder
			.ForMember(MemberAccessor<TSubject?, string?>.FromFuncWithName(
					value => value is null
						? null
						: write(value, defaults => usedOptions = options?.Invoke(defaults) ?? defaults),
					"public API"),
				(_, stringBuilder) => stringBuilder.Append("has a public API which "))
			.AddExpectations(expectationBuilder =>
			{
				expectationBuilder.AddSubjectContexts<string>((_, contexts) => usedOptions?.AddContext(contexts));
				result = new ThatSubject<string?>(expectationBuilder).MatchesSnapshot(callerFilePath,
					callerMemberName)!;
			});
		return result!;
	}
}
