[assembly: System.Reflection.AssemblyMetadata("RepositoryUrl", "https://github.com/Testably/aweXpect.git")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETStandard,Version=v2.0", FrameworkDisplayName = ".NET Standard 2.0")]
namespace aweXpect
{
    public static class ThatSnapshot
    {
        [aweXpect.Core.GuaranteesNotNull]
        public static aweXpect.Snapshots.Results.BinarySnapshotResult<System.IO.FileInfo, aweXpect.Core.IThat<System.IO.FileInfo?>> MatchesSnapshot(this aweXpect.Core.IThat<System.IO.FileInfo?> subject, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "") { }
        [System.Runtime.CompilerServices.OverloadResolutionPriority(1)]
        [aweXpect.Core.GuaranteesNotNull]
        public static aweXpect.Snapshots.Results.BinarySnapshotWithoutExtension<System.IO.Stream, aweXpect.Core.IThat<System.IO.Stream?>> MatchesSnapshot(this aweXpect.Core.IThat<System.IO.Stream?> subject, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "") { }
        [aweXpect.Core.GuaranteesNotNull]
        public static aweXpect.Snapshots.Results.BinarySnapshotWithoutExtension<byte[], aweXpect.Core.IThat<byte[]?>> MatchesSnapshot(this aweXpect.Core.IThat<byte[]?> subject, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "") { }
        [aweXpect.Core.GuaranteesNotNull]
        public static aweXpect.Snapshots.Results.ObjectSnapshotResult<T, aweXpect.Core.IThat<T>> MatchesSnapshot<[aweXpect.Core.Metadata.RequiresMemberMetadata] T>(this aweXpect.Core.IThat<T> subject, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "") { }
        [aweXpect.Core.GuaranteesNotNull]
        public static aweXpect.Snapshots.Results.StringSnapshotResult<string, aweXpect.Core.IThat<string?>> MatchesSnapshot(this aweXpect.Core.IThat<string?> subject, [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "") { }
        public static aweXpect.Snapshots.Results.BinarySnapshotResult<System.IO.FileInfo, aweXpect.Core.IThat<System.IO.FileInfo?>> WithExtension(this aweXpect.Snapshots.Results.BinarySnapshotResult<System.IO.FileInfo, aweXpect.Core.IThat<System.IO.FileInfo?>> result, string extension) { }
        public static aweXpect.Snapshots.Results.StringSnapshotResult<TType, aweXpect.Core.IThat<string?>> WithExtension<TType>(this aweXpect.Snapshots.Results.StringSnapshotResult<TType, aweXpect.Core.IThat<string?>> result, string extension) { }
    }
}
namespace aweXpect.Snapshots
{
    public interface IFileSnapshotStore
    {
        string GetPendingFile(aweXpect.Snapshots.SnapshotId id);
        string GetSnapshotFile(aweXpect.Snapshots.SnapshotId id);
    }
    public sealed class ObjectSnapshotFor<TResult> : aweXpect.Snapshots.SnapshotTextFor<TResult>
    {
        public aweXpect.Snapshots.SnapshotMembers<TResult, object> Members() { }
        public aweXpect.Snapshots.SnapshotValues<TResult, object> Values() { }
    }
    public static class OrphanedSnapshots
    {
        public static System.Threading.Tasks.Task CheckAsync(bool force = false, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { }
    }
    public enum SnapshotAcceptance
    {
        None = 0,
        New = 1,
        All = 2,
    }
    public abstract class SnapshotComparer
    {
        protected SnapshotComparer() { }
        public abstract System.Threading.Tasks.ValueTask<aweXpect.Snapshots.SnapshotComparison> CompareAsync(aweXpect.Snapshots.SnapshotComparisonContext context, System.Threading.CancellationToken cancellationToken);
        public static aweXpect.Snapshots.SnapshotComparerRegistration ForExtension(string extension, aweXpect.Snapshots.SnapshotComparer comparer) { }
        public static aweXpect.Snapshots.SnapshotComparerRegistration When(System.Func<aweXpect.Snapshots.SnapshotComparisonContext, bool> predicate, aweXpect.Snapshots.SnapshotComparer comparer) { }
    }
    public sealed class SnapshotComparerRegistration
    {
    }
    public sealed class SnapshotComparison
    {
        public string? Description { get; }
        public bool IsEqual { get; }
        public static aweXpect.Snapshots.SnapshotComparison Equal { get; }
        public static aweXpect.Snapshots.SnapshotComparison Different(string description) { }
    }
    public sealed class SnapshotComparisonContext
    {
        public SnapshotComparisonContext(aweXpect.Snapshots.SnapshotId id, aweXpect.Snapshots.SnapshotContent snapshot, aweXpect.Snapshots.SnapshotContent actual) { }
        public aweXpect.Snapshots.SnapshotContent Actual { get; }
        public aweXpect.Snapshots.SnapshotId Id { get; }
        public aweXpect.Snapshots.SnapshotContent Snapshot { get; }
    }
    public sealed class SnapshotContent
    {
        public byte[] Bytes { get; }
        public bool IsText { get; }
        public string? Text { get; }
        public static aweXpect.Snapshots.SnapshotContent FromBytes(byte[] bytes) { }
        public static aweXpect.Snapshots.SnapshotContent FromText(string text) { }
    }
    public sealed class SnapshotEmptyCollections<TResult>
    {
        public TResult Ignore() { }
    }
    public sealed class SnapshotFor<TResult> : aweXpect.Snapshots.SnapshotTextFor<TResult>
    {
        public aweXpect.Snapshots.SnapshotLines<TResult> Lines() { }
        public aweXpect.Snapshots.SnapshotMembers<TResult, object> Members() { }
        public aweXpect.Snapshots.SnapshotValues<TResult, object> Values() { }
    }
    public sealed record SnapshotId : System.IEquatable<aweXpect.Snapshots.SnapshotId>
    {
        public SnapshotId(string SourceFile, string MemberName, string? Name = null) { }
        public string Extension { get; init; }
        public string MemberName { get; init; }
        public string? Name { get; init; }
        public string SourceFile { get; init; }
        public string? Variant { get; init; }
        public void Deconstruct(out string SourceFile, out string MemberName, out string? Name) { }
        public bool Equals(aweXpect.Snapshots.SnapshotId? other) { }
        public override bool Equals(object? obj) { }
        public override int GetHashCode() { }
        public override string ToString() { }
        public static bool operator ==(aweXpect.Snapshots.SnapshotId? left, aweXpect.Snapshots.SnapshotId? right) { }
        public static bool operator !=(aweXpect.Snapshots.SnapshotId? left, aweXpect.Snapshots.SnapshotId? right) { }
    }
    public sealed class SnapshotKnownText<TResult>
    {
        public TResult Mask() { }
        public TResult Mask(string template) { }
    }
    public class SnapshotLines<TResult>
    {
        public aweXpect.Snapshots.SnapshotLinesContaining<TResult> Containing(string text) { }
        public TResult Convert(System.Func<string, string?> convert, [System.Runtime.CompilerServices.CallerArgumentExpression("convert")] string doNotPopulateThisValue = "") { }
        public aweXpect.Snapshots.SnapshotLines<TResult> Except(System.Func<string, bool> predicate, [System.Runtime.CompilerServices.CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "") { }
        public TResult Ignore() { }
        public TResult Mask(string template) { }
        public aweXpect.Snapshots.SnapshotLines<TResult> Which(System.Func<string, bool> predicate, [System.Runtime.CompilerServices.CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "") { }
    }
    public sealed class SnapshotLinesContaining<TResult> : aweXpect.Snapshots.SnapshotLines<TResult>, aweXpect.Core.ICopyOnOption<aweXpect.Options.StringEqualityOptions, aweXpect.Snapshots.SnapshotLinesContaining<TResult>>, aweXpect.Core.IOptionsProvider<aweXpect.Options.StringEqualityOptions>, aweXpect.Results.IStringPatternMatchTypeOptions
    {
    }
    public sealed class SnapshotMatch
    {
        public aweXpect.Snapshots.SnapshotId Id { get; }
        public aweXpect.Snapshots.SnapshotStore Store { get; }
    }
    public class SnapshotMembers<TResult, TValue>
    {
        public TResult Convert<[aweXpect.Core.Metadata.RequiresMemberMetadata] TConverted>(System.Func<TValue, TConverted> convert, [System.Runtime.CompilerServices.CallerArgumentExpression("convert")] string doNotPopulateThisValue = "") { }
        public aweXpect.Snapshots.SnapshotMembers<TResult, TValue> DeclaredIn<T>() { }
        public aweXpect.Snapshots.SnapshotMembers<TResult, TValue> Except(System.Func<aweXpect.Formatting.FormattedValue, bool> predicate, [System.Runtime.CompilerServices.CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "") { }
        public TResult Ignore() { }
        public TResult Mask() { }
        public TResult Mask(System.Func<aweXpect.Formatting.FormattedValue, string?> template, [System.Runtime.CompilerServices.CallerArgumentExpression("template")] string doNotPopulateThisValue = "") { }
        public TResult Mask(string template) { }
        public aweXpect.Snapshots.SnapshotMembersOfType<TResult, T> OfExactType<T>() { }
        public aweXpect.Snapshots.SnapshotMembersOfType<TResult, T> OfType<T>() { }
        public aweXpect.Snapshots.SnapshotMembers<TResult, TValue> Which(System.Func<aweXpect.Formatting.FormattedValue, bool> predicate, [System.Runtime.CompilerServices.CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "") { }
        public aweXpect.Snapshots.SnapshotEmptyCollections<TResult> WhichAreEmptyCollections() { }
        public aweXpect.Snapshots.SnapshotMembersWithName<TResult, TValue> WithName(string name) { }
        public aweXpect.Snapshots.SnapshotMembers<TResult, TValue> WithPath(string path) { }
    }
    public sealed class SnapshotMembersOfType<TResult, TValue> : aweXpect.Snapshots.SnapshotMembers<TResult, TValue>
    {
        public aweXpect.Snapshots.SnapshotMembersOfType<TResult, object> OrOfExactType<T>() { }
        public aweXpect.Snapshots.SnapshotMembersOfType<TResult, object> OrOfType<T>() { }
    }
    public sealed class SnapshotMembersWithName<TResult, TValue> : aweXpect.Snapshots.SnapshotMembers<TResult, TValue>, aweXpect.Core.ICopyOnOption<aweXpect.Options.StringEqualityOptions, aweXpect.Snapshots.SnapshotMembersWithName<TResult, TValue>>, aweXpect.Core.IOptionsProvider<aweXpect.Options.StringEqualityOptions>, aweXpect.Results.IStringMatchTypeOptions, aweXpect.Results.IStringPatternMatchTypeOptions
    {
    }
    public sealed class SnapshotMismatch
    {
        public aweXpect.Snapshots.SnapshotContent Actual { get; }
        public aweXpect.Snapshots.SnapshotId Id { get; }
        public aweXpect.Snapshots.SnapshotContent? Snapshot { get; }
        public aweXpect.Snapshots.SnapshotStore Store { get; }
    }
    public sealed class SnapshotPaths<TResult>
    {
        public TResult Keep() { }
    }
    public class SnapshotPattern<TResult>
    {
        public TResult Mask(string template) { }
    }
    public abstract class SnapshotReporter
    {
        protected SnapshotReporter() { }
        public virtual System.Threading.Tasks.ValueTask OnMatchAsync(aweXpect.Snapshots.SnapshotMatch match, System.Threading.CancellationToken cancellationToken) { }
        public virtual System.Threading.Tasks.ValueTask OnMismatchAsync(aweXpect.Snapshots.SnapshotMismatch mismatch, System.Threading.CancellationToken cancellationToken) { }
    }
    public sealed class SnapshotRule
    {
        public static aweXpect.Snapshots.SnapshotFor<aweXpect.Snapshots.SnapshotRule> For { get; }
    }
    public abstract class SnapshotStore
    {
        protected SnapshotStore() { }
        public abstract System.Threading.Tasks.ValueTask AcceptAsync(aweXpect.Snapshots.SnapshotId id, byte[] actual, System.Threading.CancellationToken cancellationToken);
        public virtual System.Threading.Tasks.ValueTask DeleteAsync(aweXpect.Snapshots.SnapshotId id, System.Threading.CancellationToken cancellationToken) { }
        public virtual string Describe(aweXpect.Snapshots.SnapshotId id) { }
        public virtual string? DescribeMismatch(aweXpect.Snapshots.SnapshotId id) { }
        public virtual System.Threading.Tasks.ValueTask<System.Collections.Generic.IReadOnlyList<aweXpect.Snapshots.SnapshotId>?> ListAsync(System.Threading.CancellationToken cancellationToken) { }
        public abstract System.Threading.Tasks.ValueTask<byte[]?> ReadAsync(aweXpect.Snapshots.SnapshotId id, System.Threading.CancellationToken cancellationToken);
        public virtual System.Threading.Tasks.ValueTask RecordMatchAsync(aweXpect.Snapshots.SnapshotId id, System.Threading.CancellationToken cancellationToken) { }
        public abstract System.Threading.Tasks.ValueTask RecordMismatchAsync(aweXpect.Snapshots.SnapshotId id, byte[] actual, System.Threading.CancellationToken cancellationToken);
    }
    public sealed class SnapshotText<TResult> : aweXpect.Snapshots.SnapshotPattern<TResult>
    {
        public aweXpect.Snapshots.SnapshotText<TResult> AsRegex() { }
        public aweXpect.Snapshots.SnapshotText<TResult> AsWildcard() { }
        public aweXpect.Snapshots.SnapshotText<TResult> IgnoringCase(bool ignoreCase = true) { }
    }
    public class SnapshotTextFor<TResult>
    {
        public aweXpect.Snapshots.SnapshotKnownText<TResult> Dates() { }
        public aweXpect.Snapshots.SnapshotKnownText<TResult> Guids() { }
        public aweXpect.Snapshots.SnapshotKnownText<TResult> MachineName() { }
        public aweXpect.Snapshots.SnapshotPaths<TResult> Paths() { }
        public aweXpect.Snapshots.SnapshotPattern<TResult> Regex(System.Text.RegularExpressions.Regex pattern) { }
        public aweXpect.Snapshots.SnapshotText<TResult> Text(string text) { }
        public aweXpect.Snapshots.SnapshotKnownText<TResult> UserName() { }
    }
    public class SnapshotValues<TResult, TValue>
    {
        public TResult Convert<[aweXpect.Core.Metadata.RequiresMemberMetadata] TConverted>(System.Func<TValue, TConverted> convert, [System.Runtime.CompilerServices.CallerArgumentExpression("convert")] string doNotPopulateThisValue = "") { }
        public aweXpect.Snapshots.SnapshotValues<TResult, TValue> DeclaredIn<T>() { }
        public aweXpect.Snapshots.SnapshotValues<TResult, TValue> Except(System.Func<aweXpect.Formatting.FormattedValue, bool> predicate, [System.Runtime.CompilerServices.CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "") { }
        public TResult Ignore() { }
        public TResult Mask() { }
        public TResult Mask(System.Func<aweXpect.Formatting.FormattedValue, string?> template, [System.Runtime.CompilerServices.CallerArgumentExpression("template")] string doNotPopulateThisValue = "") { }
        public TResult Mask(string template) { }
        public aweXpect.Snapshots.SnapshotValuesOfType<TResult, T> OfExactType<T>() { }
        public aweXpect.Snapshots.SnapshotValuesOfType<TResult, T> OfType<T>() { }
        public aweXpect.Snapshots.SnapshotValues<TResult, TValue> Which(System.Func<aweXpect.Formatting.FormattedValue, bool> predicate, [System.Runtime.CompilerServices.CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "") { }
        public aweXpect.Snapshots.SnapshotValuesWithName<TResult, TValue> WithName(string name) { }
        public aweXpect.Snapshots.SnapshotValues<TResult, TValue> WithPath(string path) { }
    }
    public sealed class SnapshotValuesOfType<TResult, TValue> : aweXpect.Snapshots.SnapshotValues<TResult, TValue>
    {
        public aweXpect.Snapshots.SnapshotValuesOfType<TResult, object> OrOfExactType<T>() { }
        public aweXpect.Snapshots.SnapshotValuesOfType<TResult, object> OrOfType<T>() { }
    }
    public sealed class SnapshotValuesWithName<TResult, TValue> : aweXpect.Snapshots.SnapshotValues<TResult, TValue>, aweXpect.Core.ICopyOnOption<aweXpect.Options.StringEqualityOptions, aweXpect.Snapshots.SnapshotValuesWithName<TResult, TValue>>, aweXpect.Core.IOptionsProvider<aweXpect.Options.StringEqualityOptions>, aweXpect.Results.IStringMatchTypeOptions, aweXpect.Results.IStringPatternMatchTypeOptions
    {
    }
    public static class SnapshotsCustomizationExtensions
    {
        public static aweXpect.Snapshots.SnapshotsCustomizationExtensions.SnapshotsCustomization Snapshots(this aweXpect.Customization.AwexpectCustomization awexpectCustomization) { }
        public class SnapshotsCustomization
        {
            public aweXpect.Customization.ICustomizationValueSetter<aweXpect.Snapshots.SnapshotAcceptance?> Acceptance { get; }
            public aweXpect.Customization.ICustomizationValueSetter<bool> AttachPendingFiles { get; }
            public aweXpect.Customization.ICustomizationValueSetter<System.Collections.Generic.IReadOnlyList<aweXpect.Snapshots.SnapshotComparerRegistration>> Comparers { get; }
            public aweXpect.Customization.ICustomizationValueSetter<bool> NamedAfterTestArguments { get; }
            public aweXpect.Customization.ICustomizationValueSetter<System.Collections.Generic.IReadOnlyList<aweXpect.Snapshots.SnapshotReporter>> Reporters { get; }
            public aweXpect.Customization.ICustomizationValueSetter<System.Collections.Generic.IReadOnlyList<aweXpect.Snapshots.SnapshotRule>> Rules { get; }
            public aweXpect.Customization.ICustomizationValueSetter<aweXpect.Snapshots.SnapshotStore> Store { get; }
        }
    }
    public sealed class StringSnapshotFor<TResult> : aweXpect.Snapshots.SnapshotTextFor<TResult>
    {
        public aweXpect.Snapshots.SnapshotLines<TResult> Lines() { }
    }
}
namespace aweXpect.Snapshots.Results
{
    public class BinarySnapshotResult<TType, TThat> : aweXpect.Snapshots.Results.SnapshotResult<TType, TThat, aweXpect.Snapshots.Results.BinarySnapshotResult<TType, TThat>>
    {
    }
    public sealed class BinarySnapshotWithoutExtension<TType, TThat>
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Obsolete("A snapshot of binary data needs a file extension, e.g. .MatchesSnapshot().WithExtension(\"png\").", true)]
        public System.Runtime.CompilerServices.TaskAwaiter GetAwaiter() { }
        public aweXpect.Snapshots.Results.BinarySnapshotResult<TType, TThat> WithExtension(string extension) { }
    }
    public class ObjectSnapshotResult<TType, TThat> : aweXpect.Snapshots.Results.SnapshotResult<TType, TThat, aweXpect.Snapshots.Results.ObjectSnapshotResult<TType, TThat>>
    {
        public aweXpect.Snapshots.ObjectSnapshotFor<aweXpect.Snapshots.Results.ObjectSnapshotResult<TType, TThat>> For { get; }
    }
    public class SnapshotResult<TType, TThat, TSelf> : aweXpect.Results.AndOrResult<TType, TThat, TSelf> where TSelf : aweXpect.Snapshots.Results.SnapshotResult<TType, TThat, TSelf>
    {
        public aweXpect.Snapshots.Results.SnapshotUniqueFor<TSelf> UniqueFor { get; }
        public aweXpect.Results.AndOrResult<TType, TThat> Accepting(aweXpect.Snapshots.SnapshotAcceptance mode) { }
        public TSelf Named(string name) { }
        public TSelf NamedAfter(object? argument1) { }
        public TSelf NamedAfter(object? argument1, object? argument2) { }
        public TSelf NamedAfter(object? argument1, object? argument2, object? argument3) { }
        public TSelf NamedAfter(object? argument1, object? argument2, object? argument3, object? argument4) { }
        public TSelf NamedAfter(object? argument1, object? argument2, object? argument3, object? argument4, params object?[]? moreArguments) { }
        public TSelf Using(aweXpect.Snapshots.SnapshotComparer comparer) { }
    }
    public sealed class SnapshotUniqueFor<TResult>
    {
        public TResult Architecture() { }
        public TResult OperatingSystem() { }
        public TResult TargetFramework() { }
    }
    public class StringSnapshotResult<TType, TThat> : aweXpect.Snapshots.Results.SnapshotResult<TType, TThat, aweXpect.Snapshots.Results.StringSnapshotResult<TType, TThat>>
    {
        public aweXpect.Snapshots.StringSnapshotFor<aweXpect.Snapshots.Results.StringSnapshotResult<TType, TThat>> For { get; }
    }
}

