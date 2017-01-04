using System;
using System.Configuration;

namespace Paraesthesia.Web.Configuration.UrlAbsolutifier
{
	/// <summary>
	/// Contains a collection of <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement"/>
	/// objects defined in a configuration file.
	/// </summary>
	[ConfigurationCollection(typeof(RuleConfigurationElement))]
	public class RuleConfigurationCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Gets a value indicating whether an attempt to add a duplicate
		/// <see cref="System.Configuration.ConfigurationElement"/> to the
		/// <see cref="System.Configuration.ConfigurationElementCollection"/> will cause an exception to be thrown.
		/// </summary>
		/// <value>Always returns <see langword="false" />.</value>
		protected override bool ThrowOnDuplicate
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RuleConfigurationCollection"/> class.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Instantiates using a <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElementComparer"/>
		/// as the <see cref="System.Collections.IComparer"/> for comparing collection
		/// items.
		/// </para>
		/// </remarks>
		public RuleConfigurationCollection() : base(new RuleConfigurationElementComparer()) { }

		/// <summary>
		/// Adds an element to the collection.
		/// </summary>
		/// <param name="element">The element to add.</param>
		/// <remarks>
		/// <para>
		/// If a duplicate rule is found, it is removed from its original location
		/// in the rule collection and added back to the new location.
		/// </para>
		/// </remarks>
		public void Add(RuleConfigurationElement element)
		{
			int index = this.BaseIndexOf(element);
			if (index >= 0)
			{
				this.BaseRemove(element);
			}
			// We have to pass -1 as the index or any element removed in the block
			// above will be marked as "replaced" rather than being re-added to
			// the end of the collection.
			this.BaseAdd(-1, element);
		}

		/// <summary>
		/// Adds an element with the specified properties to the collection.
		/// </summary>
		/// <param name="process">The <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleProcess"/> that determines how the rule should behave.</param>
		/// <param name="type">The <see cref="System.Type"/> of rule to apply.</param>
		/// <param name="value">The value/criteria that helps the rule determine whether it applies to a given request.</param>
		/// <remarks>
		/// <para>
		/// If a duplicate rule is found, it is removed from its original location
		/// in the rule collection and added back to the new location.
		/// </para>
		/// </remarks>
		public void Add(RuleProcess process, Type type, string value)
		{
			this.Add(new RuleConfigurationElement(process, type, value));
		}

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public void Clear()
		{
			this.BaseClear();
		}

		/// <summary>
		/// Creates a new <see cref="System.Configuration.ConfigurationElement"/>.
		/// </summary>
		/// <returns>
		/// A new <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleConfigurationElement"/>.
		/// </returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new RuleConfigurationElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element.  In this
		/// case, the element itself is its own key.
		/// </summary>
		/// <param name="element">The <see cref="System.Configuration.ConfigurationElement"/> to return the key for.</param>
		/// <returns>
		/// The element itself is its own key, so <paramref name="element" />
		/// is returned.
		/// </returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			// The element itself is its own key.
			return element;
		}

		/// <summary>
		/// Removes an element from the collection.
		/// </summary>
		/// <param name="element">The element to remove.</param>
		public void Remove(RuleConfigurationElement element)
		{
			this.BaseRemove(element);
		}

		/// <summary>
		/// Removes an element with the specified properties from the collection.
		/// </summary>
		/// <param name="process">The <see cref="Paraesthesia.Web.Configuration.UrlAbsolutifier.RuleProcess"/> that determines how the rule should behave.</param>
		/// <param name="type">The <see cref="System.Type"/> of rule to apply.</param>
		/// <param name="value">The value/criteria that helps the rule determine whether it applies to a given request.</param>
		public void Remove(RuleProcess process, Type type, string value)
		{
			this.BaseRemove(new RuleConfigurationElement(process, type, value));
		}
	}
}
