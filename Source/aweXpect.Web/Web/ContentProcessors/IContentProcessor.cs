using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Web.ContentProcessors;

/// <summary>
///     A processor for formatting a <see cref="HttpContent" />.
/// </summary>
public interface IContentProcessor
{
	/// <summary>
	///     Appends the formatted <paramref name="httpContent" /> to the <paramref name="messageBuilder" /> using the
	///     <paramref name="indentation" />.
	/// </summary>
	/// <returns>
	///     <see langword="true" />, when the <paramref name="httpContent" /> could be handled, otherwise
	///     <see langword="false" /> when other processors should be tried.
	/// </returns>
	Task<bool> AppendContentInfo(StringBuilder messageBuilder, HttpContent httpContent, string indentation,
		CancellationToken cancellationToken = default);
}
