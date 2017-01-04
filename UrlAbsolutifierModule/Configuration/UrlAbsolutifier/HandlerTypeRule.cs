using System;
using System.ComponentModel;

namespace Paraesthesia.Web.Configuration.UrlAbsolutifier
{
	/// <summary>
	/// Rule that matches the current request handler by type.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This rule is used by the <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/>
	/// to include or exclude processing of a given response based on the current
	/// request's handler <see cref="System.Type"/>.
	/// </para>
	/// </remarks>
	/// <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/>
	/// <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule"/>
	public class HandlerTypeRule : Rule
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.HandlerTypeRule"/> class.
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
		public HandlerTypeRule(RuleProcess process, string ruleValue)
			: base(process, ruleValue)
		{
			if (this.Value == null)
			{
				throw new ArgumentException("A value must be specified for the rule.", "ruleValue");
			}
		}

		/// <summary>
		/// Gets the <see cref="System.ComponentModel.TypeConverter"/> used to
		/// parse the rule value.
		/// </summary>
		/// <value>
		/// Always returns a new <see cref="System.Configuration.TypeNameConverter"/>.
		/// </value>
		public override TypeConverter ValueConverter
		{
			get { return new System.Configuration.TypeNameConverter(); }
		}

		/// <summary>
		/// Determines if the given request context matches the rule criteria.  The
		/// result, combined with <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Process"/>,
		/// determines if the output stream should be absolutified.
		/// </summary>
		/// <param name="context">The context to check the rule against.</param>
		/// <returns>
		/// <see langword="true"/> if the current request handler is of the type specified by <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Value"/>;
		/// <see langword="false"/> if not (or if <paramref name="context" /> is <see langword="null" />,
		/// or if there is no current request handler).
		/// </returns>
		public override bool ContextMatchesRule(System.Web.HttpContext context)
		{
			if (context == null || context.Handler == null)
			{
				return false;
			}

			Type expected = (Type)this.Value;
			return expected.IsAssignableFrom(context.Handler.GetType());
		}
	}
}
