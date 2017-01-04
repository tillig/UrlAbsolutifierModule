using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Paraesthesia.Web
{
	/// <summary>
	/// Stream filter that converts URLs to absolute.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This <see cref="System.IO.MemoryStream"/> derivative locates known HTML
	/// 4.01 attributes that take URI values and, for each URI value, converts
	/// any relative references to absolute.  For example, the filter will find
	/// all occurrences of <c>href="value"</c> and convert it if needed.  The tag
	/// associated with the attribute is not checked.
	/// </para>
	/// <para>
	/// The exception to this behavior is if the <c>&lt;base href="..." /&gt;</c>
	/// tag is found in the stream - if this is found, then only the <c>base href</c>
	/// tag is converted to absolute and other tags are left alone (allowing the
	/// <c>base href</c> tag to do its job).
	/// </para>
	/// <para>
	/// The filter is aggressive, not checking that the content of the stream is
	/// HTML and not limiting itself only to attributes in tags.  The drawback to
	/// this is that edge cases - like content that has encoded code snippets -
	/// will be potentially incorrectly updated, or that non-text content will
	/// be incorrectly processed.  The advantage is that select non-HTML content
	/// (like RSS feeds, which usually contain XML-encoded HTML that needs processing)
	/// will get processed.
	/// </para>
	/// <para>
	/// The attributes that get processed are:
	/// </para>
	/// <list type="bullet">
	/// <item><description>action</description></item>
	/// <item><description>background</description></item>
	/// <item><description>cite</description></item>
	/// <item><description>classid</description></item>
	/// <item><description>codebase</description></item>
	/// <item><description>data</description></item>
	/// <item><description>datasrc</description></item>
	/// <item><description>for</description></item>
	/// <item><description>href</description></item>
	/// <item><description>longdesc</description></item>
	/// <item><description>profile</description></item>
	/// <item><description>src</description></item>
	/// <item><description>usemap</description></item>
	/// </list>
	/// <para>
	/// The work in the filter occurs during the <see cref="Paraesthesia.Web.UrlAbsolutifierFilter.Close"/>
	/// method because the entire contents of the stream need to be present for
	/// processing.
	/// </para>
	/// </remarks>
	/// <seealso cref="Paraesthesia.Web.UrlAbsolutifierModule" />
	public class UrlAbsolutifierFilter : MemoryStream
	{
		/*
		 * When we see a base href it should take precedence over the request URL.
		 * 
		 * Based on HTML 4.01 transitional spec, the following tag/attribute pairs
		 * take URI values:
		 * 
		 * * datasrc
		 * a href
		 * applet codebase
		 * area href
		 * base href
		 * blockquote cite
		 * body background
		 * del cite
		 * form action
		 * frame longdesc
		 * frame src
		 * head profile
		 * iframe longdesc
		 * iframe src
		 * img longdesc
		 * img src
		 * img usemap
		 * input src
		 * input usemap
		 * ins cite
		 * link href
		 * object classid
		 * object codebase
		 * object data
		 * object usemap
		 * q cite
		 * script for
		 * script src
		 * 
		 * We can short-circuit this by processing all tags but "base" and the
		 * following attributes:
		 * action
		 * background
		 * cite
		 * classid
		 * codebase
		 * data
		 * datasrc
		 * for
		 * href
		 * longdesc
		 * profile
		 * src
		 * usemap
		 * 
		 */

		/// <summary>
		/// Regular expression that matches tags and supported attributes/values
		/// for substitution.
		/// </summary>
		private static Regex SupportedAttributeParser = new Regex(
			@"(?<attributevaluepair>(?<attribute>action|background|cite|classid|codebase|data|for|href|longdesc|profile|src|usemap)\s*=\s*""(?<value>[^""]+)""\s*)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
		/*
		 * To match a tag AND attributes, the following works...
		 * @"<\s*(?<tag>\w+)\s+(.*?(?<attributevaluepair>(?<attribute>action|background|cite|classid|codebase|data|for|href|longdesc|profile|src|usemap)\s*=\s*""(?<value>[^""]+)""\s*)*[^>]*?)*>",
		 * ...but that won't work with encoded content like RSS feeds, and since
		 * we don't use the tag anyway, we can blindly process.  A future idea
		 * might be to have an encoded and non-encoded version and have the
		 * rule configuration determine also which filter to apply.
		 */

		/// <summary>
		/// Regular expression that matches the base href tag so we can re-base
		/// given the base href.
		/// </summary>
		private static Regex BaseHrefParser = new Regex(
			@"<\s*base\s+.*href\s*=\s*""(?<value>[^""]+)"".*>",
			RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

		/// <summary>
		/// The previous filter in the response filtering chain.
		/// </summary>
		private Stream _previousFilter;

		/// <summary>
		/// The URL used to convert relative paths to absolute.
		/// </summary>
		private Uri _requestUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Web.UrlAbsolutifierFilter" /> class.
		/// </summary>
		/// <param name="filter">
		/// The previous filter in the filter chain.  <see langword="null" /> if
		/// there is no previous filter.
		/// </param>
		/// <param name="requestUrl">
		/// The URL of the request, used to convert relative URLs into absolute.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if <paramref name="requestUrl" /> is <see langword="null" />.
		/// </exception>
		public UrlAbsolutifierFilter(Stream filter, Uri requestUrl)
		{
			if (requestUrl == null)
			{
				throw new ArgumentNullException("requestUrl");
			}
			this._previousFilter = filter;
			this._requestUrl = requestUrl;
		}

		/// <summary>
		/// Closes the current stream and releases any resources (such as sockets and file handles)
		/// associated with the current stream.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is where the filter stream rewrites URL references in the page
		/// to be absolute references.
		/// </para>
		/// </remarks>
		public override void Close()
		{
			if (this.CanRead && this.Length > 0)
			{
				// Get the current content into a string so we can work with it.
				string content = Encoding.UTF8.GetString(this.ToArray());

				Match baseHrefMatch = BaseHrefParser.Match(content);
				if (baseHrefMatch != null && baseHrefMatch.Success)
				{
					// If there's a base href, absolutify it only and change our request URL to be that.
					content = BaseHrefParser.Replace(content, new MatchEvaluator(this.ProcessSupportedAttributes));
				}
				else
				{
					// For each supported attribute that isn't base href, absolutify.
					content = SupportedAttributeParser.Replace(content, new MatchEvaluator(this.ProcessSupportedAttributes));
				}

				// Convert the modified content back to bytes so it can be written to the output stream.
				byte[] bytes = new UTF8Encoding().GetBytes(content);

				if (this._previousFilter != null && this._previousFilter.CanWrite)
				{
					// Write the updated content directly to the previous filter.
					this._previousFilter.Write(bytes, 0, bytes.Length);
				}
				else if (this.CanWrite)
				{
					// Clear the current stream and write it back to itself.
					this.SetLength(0);
					this.Write(bytes, 0, bytes.Length);
				}
			}

			if (this._previousFilter != null)
			{
				this._previousFilter.Close();
			}
			base.Close();
		}

		/// <summary>
		/// Processes the supported attributes.
		/// </summary>
		/// <param name="match">The match containing the matched attributes.</param>
		/// <returns>The replacement text with URLs absolutified.</returns>
		private string ProcessSupportedAttributes(Match match)
		{
			// Process supported attribute values - we don't really care about the tag or attribute.
			string replacement = match.Value;
			foreach (Capture capturedValue in match.Groups["value"].Captures)
			{
				string originalValue = capturedValue.Value.Trim();
				string rebasedValue = this.RebaseUrl(originalValue);
				if (originalValue == rebasedValue)
				{
					continue;
				}
				replacement = replacement.Replace(originalValue, rebasedValue);
			}
			return replacement;
		}

		/// <summary>
		/// Rebases a URL with respect to the current request URL.
		/// </summary>
		/// <param name="pathToRebase">The URL path to rebase given the current request.</param>
		/// <returns>A rebased URL if it needed to be updated, or the original <paramref name="pathToRebase" /> if not.</returns>
		private string RebaseUrl(string pathToRebase)
		{
			// Parse the relative URL - if we can't, just return the original string with no substitution.
			if (String.IsNullOrEmpty(pathToRebase) || Uri.IsWellFormedUriString(pathToRebase, UriKind.Absolute) || !Uri.IsWellFormedUriString(pathToRebase, UriKind.Relative))
			{
				return pathToRebase;
			}
			Uri originalUri = new Uri(pathToRebase, UriKind.Relative);

			// Rebase the URI given our request URL.
			Uri rebasedUri = new Uri(this._requestUrl, originalUri);
			return rebasedUri.ToString();
		}
	}
}
