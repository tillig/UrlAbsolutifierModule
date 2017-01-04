using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Paraesthesia.Web.Configuration.UrlAbsolutifier
{
	/// <summary>
	/// Rule that matches the current request URL with a regular expression.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This rule is used by the <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/>
	/// to include or exclude processing of a given response based on the current
	/// request's <see cref="System.Web.HttpRequest.Url"/>.
	/// </para>
	/// </remarks>
	/// <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/>
	/// <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule"/>
	public class UrlRegexRule : Rule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlRegexRule"/> class.
		/// </summary>
		/// <param name="process">The manner in which this rule should be processed.</param>
		/// <param name="ruleValue">The value of the rule. Will be converted by <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.ValueConverter"/>.</param>
		/// <remarks>
		/// If <paramref name="ruleValue"/> is <see langword="null"/> then <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Value"/>
		/// will be <see langword="null"/>.  If <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.ValueConverter"/>
		/// is <see langword="null"/>, <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Value"/> will be
		/// <paramref name="ruleValue"/>.  Otherwise, <see cref="System.ComponentModel.TypeConverter.ConvertFromString(string)"/>
		/// will be called on
		/// <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.ValueConverter"/>
		/// to convert <paramref name="ruleValue"/> into an <see cref="System.Object"/>
		/// and that value will be stored in <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Value"/>.
		/// </remarks>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="ruleValue" /> is <see langword="null" /> or <see cref="System.String.Empty"/>.
		/// </exception>
		public UrlRegexRule(RuleProcess process, string ruleValue)
			: base(process, ruleValue)
		{
			Regex value = this.Value as Regex;
			if (value == null || String.IsNullOrEmpty(value.ToString()))
			{
				throw new ArgumentException("A non-empty/null value must be specified for the rule.", "ruleValue");
			}
		}

		/// <summary>
		/// Gets the <see cref="System.ComponentModel.TypeConverter"/> used to
		/// parse the rule value.
		/// </summary>
		/// <value>
		/// Always returns a new <see cref="System.ComponentModel.StringConverter"/>.
		/// </value>
		public override TypeConverter ValueConverter
		{
			get { return new RegexConverter(); }
		}

		/// <summary>
		/// Determines if the given request context matches the rule criteria.  The
		/// result, combined with <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Process"/>,
		/// determines if the output stream should be absolutified.
		/// </summary>
		/// <param name="context">The context to check the rule against.</param>
		/// <returns>
		/// <see langword="true"/> if the current request path matches the regular expression specified in <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Value"/>;
		/// <see langword="false"/> if not.
		/// </returns>
		public override bool ContextMatchesRule(System.Web.HttpContext context)
		{
			if (context == null)
			{
				return false;
			}
			string requestUrl = context.Request.Path;
			return ((Regex)this.Value).IsMatch(requestUrl);
		}
	}
}
