using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Paraesthesia.Web.Configuration
{
	/// <summary>
	/// Type converter that converts objects into <see cref="System.Text.RegularExpressions.Regex"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This converter supports converting <see cref="System.String"/> and <see cref="System.Text.RegularExpressions.Regex"/>.
	/// All other types are not directly supported.
	/// </para>
	/// </remarks>
	public class RegexConverter : TypeConverter
	{
		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="System.Type"/> that represents the type you want to convert from.</param>
		/// <returns>
		/// <see langword="true" /> if this converter can perform the conversion; otherwise, <see langword="false" />.
		/// </returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return ((sourceType == typeof(string)) || (sourceType == typeof(Regex)) || base.CanConvertFrom(context, sourceType));
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.</param>
		/// <param name="value">The <see cref="System.Object"/> to convert.</param>
		/// <returns>
		/// An <see cref="System.Object"/> that represents the converted value.
		/// </returns>
		/// <exception cref="System.NotSupportedException">The conversion could not be performed.</exception>
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string)
			{
				return new Regex((string)value);
			}
			if (value is Regex)
			{
				return (Regex)value;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
