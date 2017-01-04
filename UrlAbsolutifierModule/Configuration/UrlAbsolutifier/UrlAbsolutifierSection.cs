using System;
using System.Configuration;

namespace Paraesthesia.Web.Configuration.UrlAbsolutifier
{
	/// <summary>
	/// Defines the configuration settings for the <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This configuration section type is used by the <see cref="Paraesthesia.Web.UrlAbsolutifierModule"/>
	/// to define the rules used for determining whether to filter response output
	/// and convert URL references to absolute.  It consists solely of a collection
	/// of <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement"/>
	/// objects that each define an individual <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule"/>.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// &lt;?xml version="1.0"?&gt;
	/// &lt;configuration&gt;
	///   &lt;configSections&gt; 
	///     &lt;section name="urlabsolutifier" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlAbsolutifierSection, Paraesthesia.Web.UrlAbsolutifierModule" /&gt; 
	///   &lt;/configSections&gt; 
	///   &lt;urlabsolutifier&gt;
	///     &lt;add process="Include" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlRegexRule, Paraesthesia.Web.UrlAbsolutifierModule" value="[a-z]{6}\.aspx" /&gt;
	///     &lt;add process="Exclude" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.UrlRegexRule, Paraesthesia.Web.UrlAbsolutifierModule" value="abcdef.aspx" /&gt;
	///     &lt;add process="Include" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.HandlerTypeRule, Paraesthesia.Web.UrlAbsolutifierModule" value="System.Web.UI.Page, System.Web" /&gt;
	///     &lt;add process="Exclude" type="Paraesthesia.Web.Configuration.UrlAbsolutifier.HandlerTypeRule, Paraesthesia.Web.UrlAbsolutifierModule" value="MyCustomHandler, MyAssembly" /&gt;
	///   &lt;/urlabsolutifier&gt;
	/// &lt;/configuration&gt;
	/// </code>
	/// </example>
	/// <seealso cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement"/>
	/// <seealso cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.Rule"/>
	public sealed class UrlAbsolutifierSection : ConfigurationSection
	{
		/// <summary>
		/// Gets the set of configured rules.
		/// </summary>
		/// <value>
		/// A <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationCollection"/>
		/// with the set of configured rules.
		/// </value>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public RuleConfigurationCollection Rules
		{
			get
			{
				return (RuleConfigurationCollection)this[""];
			}
		}
	}
}
