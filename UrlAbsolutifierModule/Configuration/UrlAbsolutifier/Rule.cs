using System;
using System.ComponentModel;

namespace Paraesthesia.Web.Configuration.UrlAbsolutifier
{
	/// <summary>
	/// Base class for <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/> processing rules.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Derive from this class to create your own custom rules to determine if
	/// a given response needs to be filtered by the <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/>.
	/// Implement the <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.ContextMatchesRule"/>
	/// method to make your determination on whether a given web context matches
	/// your rule's criteria.
	/// </para>
	/// </remarks>
	/// <seealso cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.HandlerTypeRule" />
	/// <seealso cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlRegexRule" />
	public abstract class Rule
	{
		/// <summary>
		/// Internal storage for the
		/// <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Process" />
		/// property.
		/// </summary>
		private RuleProcess _process;

		/// <summary>
		/// Internal storage for the
		/// <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Value" />
		/// property.
		/// </summary>
		private object _value;

		/// <summary>
		/// Gets the manner in which this rule should be processed.
		/// </summary>
		/// <value>
		/// A <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleProcess"/>
		/// that, combined with the outcome of <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.ContextMatchesRule"/>,
		/// indicates how the current request should be handled with respect to
		/// this rule.
		/// </value>
		public RuleProcess Process
		{
			get { return _process; }
		}

		/// <summary>
		/// Gets the parsed rule value.
		/// </summary>
		/// <value>
		/// A <see cref="System.Object"/> that represents the value of the rule
		/// passed in during construction and converted via the
		/// <see cref="System.ComponentModel.TypeConverter.ConvertFromString(string)"/>
		/// method on <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.ValueConverter"/>.
		/// </value>
		public object Value
		{
			get { return _value; }
		}

		/// <summary>
		/// Gets the <see cref="System.ComponentModel.TypeConverter"/> used to
		/// parse the rule value.
		/// </summary>
		/// <value>
		/// A <see cref="System.ComponentModel.TypeConverter"/> that will be used
		/// during construction to convert the <see cref="System.String"/> value
		/// of the rule to a strongly typed <see cref="System.Object"/> via
		/// <see cref="System.ComponentModel.TypeConverter.ConvertFromString(string)"/>
		/// </value>
		public abstract TypeConverter ValueConverter { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Rule"/> class.
		/// </summary>
		/// <param name="process">The manner in which this rule should be processed.</param>
		/// <param name="ruleValue">The value of the rule. Will be converted by <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.ValueConverter"/>.</param>
		/// <remarks>
		/// <para>
		/// If <paramref name="ruleValue" /> is <see langword="null" /> then <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Value"/>
		/// will be <see langword="null" />.  If <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.ValueConverter"/>
		/// is <see langword="null" />, <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Value"/> will be
		/// <paramref name="ruleValue" />.  Otherwise, <see cref="System.ComponentModel.TypeConverter.ConvertFromString(string)"/>
		/// will be called on
		/// <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.ValueConverter"/>
		/// to convert <paramref name="ruleValue" /> into an <see cref="System.Object"/>
		/// and that value will be stored in <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Value"/>.
		/// </para>
		/// </remarks>
		protected Rule(RuleProcess process, string ruleValue)
		{
			this._process = process;
			TypeConverter converter = this.ValueConverter;
			if (ruleValue == null || converter == null)
			{
				this._value = ruleValue;
			}
			else
			{
				this._value = converter.ConvertFromString(ruleValue);
			}
		}

		/// <summary>
		/// Determines if the given request context matches the rule criteria.  The
		/// result, combined with <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule.Process"/>,
		/// determines if the output stream should be absolutified.
		/// </summary>
		/// <param name="context">The context to check the rule against.</param>
		/// <returns>
		/// <see langword="true" /> if the current request matches this rule;
		/// <see langword="false" /> if not.
		/// </returns>
		public abstract bool ContextMatchesRule(System.Web.HttpContext context);

	}
}
