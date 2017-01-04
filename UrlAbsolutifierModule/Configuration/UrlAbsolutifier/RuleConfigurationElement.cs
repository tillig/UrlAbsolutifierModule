using System;
using System.Configuration;
using System.ComponentModel;

namespace Paraesthesia.Web.Configuration.UrlAbsolutifier
{
	/// <summary>
	/// Defines the configuration for one <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/> rule.
	/// </summary>
	/// <remarks>
	/// <para>
	/// An individual element defines the setup for a UrlAbsolutifier rule.  When
	/// used in conjunction with the <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationCollection"/>
	/// and <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection"/>,
	/// a set of these elements defines the configuration for the <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/>.
	/// </para>
	/// <para>
	/// See <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection"/>
	/// for more information and an example configuration.
	/// </para>
	/// </remarks>
	/// <seealso cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection"/>
	/// <seealso cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationCollection"/>
	public class RuleConfigurationElement : ConfigurationElement
	{
		/// <summary>
		/// Gets or sets the manner in which this rule should be processed.
		/// </summary>
		/// <value>
		/// A <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleProcess"/>
		/// that indicates how a request should be handled with respect to
		/// this rule.
		/// </value>
		[ConfigurationProperty("process", IsRequired = true)]
		public RuleProcess Process
		{
			get
			{
				return (RuleProcess)this["process"];
			}
			set
			{
				this["process"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of the rule to process.
		/// </summary>
		/// <value>
		/// A <see cref="System.Type"/>, subclassed from <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule"/>,
		/// that will perform the action of determining if a request meets the
		/// criteria specified in <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement.Value"/>.
		/// </value>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown if the value is set to <see langword="null" />.
		/// </exception>
		[ConfigurationProperty("type", IsRequired = true)]
		[SubclassTypeValidator(typeof(Rule))]
		[TypeConverter(typeof(TypeNameConverter))]
		public Type Type
		{
			get
			{
				return (Type)this["type"];
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this["type"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of the rule.
		/// </summary>
		/// <value>
		/// A <see cref="System.String"/> that outlines the criteria by which
		/// the rule will determine if it applies to a request.  The value of the
		/// criteria and its significance will depend on the <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement.Type"/>
		/// of rule being applied.
		/// </value>
		[ConfigurationProperty("value", IsRequired = true, DefaultValue = "none")] // DefaultValue set because the StringValidator fires before the constructor gets the opportunity to set the value.
		[StringValidator(MinLength = 1)]
		public string Value
		{
			get
			{
				return (string)this["value"];
			}
			set
			{
				this["value"] = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement" /> class.
		/// </summary>
        public RuleConfigurationElement()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement"/> class.
		/// </summary>
		/// <param name="process">The <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleProcess"/> that determines how the rule should behave.</param>
		/// <param name="type">The <see cref="System.Type"/> of rule to apply.</param>
		/// <param name="value">The value/criteria that helps the rule determine whether it applies to a given request.</param>
		public RuleConfigurationElement(RuleProcess process, Type type, string value)
		{
			this.Process = process;
			this.Type = type;
			this.Value = value;
		}
	}
}
