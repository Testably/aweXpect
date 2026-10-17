using System;
using System.Linq.Expressions;
using aweXpect.Formatting;
using aweXpect.Snapshots;

namespace aweXpect.Results;

/// <summary>
///     The maskings that can be added to a snapshot expectation, with the same names and semantics as the factories of
///     <see cref="SnapshotMasking" /> for all snapshots.
/// </summary>
/// <remarks>
///     The maskings of the expectation are applied in the order in which they are added, then the maskings for all
///     snapshots, so a specific one goes before a general one. Only the maskings of the environment come first: the
///     paths, see <see cref="Paths" />, then <see cref="MachineName" /> and <see cref="UserName" />. The snapshot and the
///     failure contain the masked subject.
/// </remarks>
public sealed class SnapshotMaskings<TType, TThat>
{
	private readonly SnapshotOptions _options;
	private readonly SnapshotResult<TType, TThat> _result;

	internal SnapshotMaskings(SnapshotResult<TType, TThat> result, SnapshotOptions options)
	{
		_result = result;
		_options = options;
	}

	/// <inheritdoc cref="SnapshotMasking.Dates()" />
	public SnapshotResult<TType, TThat> Dates()
		=> Add(SnapshotMasking.Dates());

	/// <inheritdoc cref="SnapshotMasking.Guids()" />
	public SnapshotResult<TType, TThat> Guids()
		=> Add(SnapshotMasking.Guids());

	/// <inheritdoc cref="SnapshotMasking.MachineName()" />
	public SnapshotResult<TType, TThat> MachineName()
		=> Add(SnapshotMasking.MachineName());

	/// <inheritdoc cref="SnapshotMasking.Member(string)" />
	public SnapshotResult<TType, TThat> Member(string memberName)
		=> AddStructural(SnapshotMasking.Member(memberName));

	/// <inheritdoc cref="SnapshotMasking.Member(string, string)" />
	public SnapshotResult<TType, TThat> Member(string memberName, string replacement)
		=> AddStructural(SnapshotMasking.Member(memberName, replacement));

	/// <inheritdoc cref="SnapshotMasking.Member(Func{FormattedValue, bool})" />
	public SnapshotResult<TType, TThat> Member(Func<FormattedValue, bool> predicate)
		=> AddStructural(SnapshotMasking.Member(predicate));

	/// <inheritdoc cref="SnapshotMasking.Member(Func{FormattedValue, bool}, string)" />
	public SnapshotResult<TType, TThat> Member(Func<FormattedValue, bool> predicate, string replacement)
		=> AddStructural(SnapshotMasking.Member(predicate, replacement));

	/// <inheritdoc cref="SnapshotMasking.MemberOf{T}(Expression{Func{T, object}})" />
	public SnapshotResult<TType, TThat> MemberOf<T>(Expression<Func<T, object?>> member)
		=> AddStructural(SnapshotMasking.MemberOf(member));

	/// <inheritdoc cref="SnapshotMasking.MemberOf{T}(Expression{Func{T, object}}, string)" />
	public SnapshotResult<TType, TThat> MemberOf<T>(Expression<Func<T, object?>> member, string replacement)
		=> AddStructural(SnapshotMasking.MemberOf(member, replacement));

	/// <summary>
	///     Replaces the paths of the environment with placeholders when <paramref name="mask" /> is
	///     <see langword="true" />, e.g. <c>C:\Music\Discography\Tests\albums.json</c> with
	///     <c>{ProjectDirectory}\albums.json</c>.
	/// </summary>
	/// <remarks>
	///     It overrides <c>Customize.aweXpect.Snapshots().MaskPaths</c>, which masks the paths by default: the directory
	///     of the test project with <c>{ProjectDirectory}</c>, the source root with <c>{SourceRoot}</c>,
	///     <see cref="System.AppContext.BaseDirectory" /> with <c>{OutputDirectory}</c>,
	///     <see cref="System.IO.Path.GetTempPath()" /> with <c>{TempPath}</c> and the profile of the user with
	///     <c>{UserProfile}</c>.<br />
	///     The longest path is replaced first, as they are nested in each other. On Windows, a path matches with <c>\</c>
	///     or <c>/</c> as separator, also with escaped backslashes (<c>\\</c>) as in a string of an object snapshot, and
	///     elsewhere with <c>/</c>. It only matches as whole segments, so <c>C:\Music\Album</c> does not match inside
	///     <c>C:\Music\Albums</c>; the rest of the path keeps its separators. The case is ignored on Windows and macOS. A
	///     root of the file system, such as <c>C:\</c> or <c>/</c>, is never masked. The paths are replaced before the
	///     other maskings, so that they see the placeholders.
	/// </remarks>
	public SnapshotResult<TType, TThat> Paths(bool mask = true)
	{
		_options.MaskPaths = mask;
		return _result;
	}

	/// <inheritdoc cref="SnapshotMasking.Regex(string, string)" />
	public SnapshotResult<TType, TThat> Regex(string pattern, string replacement)
		=> Add(SnapshotMasking.Regex(pattern, replacement));

	/// <inheritdoc cref="SnapshotMasking.Regex(System.Text.RegularExpressions.Regex, string)" />
	public SnapshotResult<TType, TThat> Regex(System.Text.RegularExpressions.Regex pattern, string replacement)
		=> Add(SnapshotMasking.Regex(pattern, replacement));

	/// <inheritdoc cref="SnapshotMasking.Text(string, string)" />
	public SnapshotResult<TType, TThat> Text(string value, string replacement)
		=> Add(SnapshotMasking.Text(value, replacement));

	/// <inheritdoc cref="SnapshotMasking.Type{T}()" />
	public SnapshotResult<TType, TThat> Type<T>()
		=> AddStructural(SnapshotMasking.Type<T>());

	/// <inheritdoc cref="SnapshotMasking.Type{T}(string)" />
	public SnapshotResult<TType, TThat> Type<T>(string replacement)
		=> AddStructural(SnapshotMasking.Type<T>(replacement));

	/// <inheritdoc cref="SnapshotMasking.UserName()" />
	public SnapshotResult<TType, TThat> UserName()
		=> Add(SnapshotMasking.UserName());

	private SnapshotResult<TType, TThat> Add(SnapshotMasking masking)
	{
		_options.Maskings.Add(masking);
		return _result;
	}

	private SnapshotResult<TType, TThat> AddStructural(SnapshotMasking masking)
	{
		StructuralRules.ThrowIfString<TType>();
		return Add(masking);
	}
}
