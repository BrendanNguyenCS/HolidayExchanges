using System.Web.Mvc;

namespace HolidayExchanges
{
    /// <summary>
    /// MVC HTML Button helper method
    /// </summary>
    public static class HtmlExtensionsButton
    {
        /// <summary>
        /// Represents the 3 button types (regular, submit, reset)
        /// </summary>
        public enum HtmlButtonTypes
        {
            submit,
            button,
            reset
        }

        /// <summary>
        /// HTML helper method that creates a input button
        /// </summary>
        /// <param name="helper">The <seealso cref="HtmlHelper"/></param>
        /// <param name="buttonText">Translates to the HTML value attribute</param>
        /// <param name="cssClass">CSS classes to apply to button</param>
        /// <param name="id">The id given to the particular button</param>
        /// <param name="buttonType">
        /// The type of button from the <seealso cref="HtmlButtonTypes"/> enum
        /// </param>
        /// <param name="htmlAttributes"></param>
        /// <returns>A MVC HTML button with CSS styling applied</returns>
        public static MvcHtmlString Button(this HtmlHelper helper, string buttonText, string cssClass, string id, HtmlButtonTypes buttonType, object htmlAttributes = null)
        {
            // Starts tab builder
            TagBuilder tb = new TagBuilder("input");

            // Check any extra styling is being applied to button
            if (!string.IsNullOrWhiteSpace(cssClass))
            {
                if (!cssClass.Contains("btn-"))
                {
                    cssClass = "btn-primary " + cssClass;
                }
            }
            else
            {
                cssClass = "btn-primary";
            }

            // Adds btn styling to tag builder
            tb.AddCssClass(cssClass);
            tb.AddCssClass("btn");

            // Adds button text (that shows on the page) to button and tag builder if present
            if (!string.IsNullOrWhiteSpace(buttonText))
            {
                tb.MergeAttribute("value", buttonText);
            }

            // Adds id attribute to button if applicable
            if (!string.IsNullOrWhiteSpace(id))
            {
                tb.GenerateId(id);
            }

            // Gives name same text as id and adds it to tagbuilder
            tb.MergeAttribute("name", TagBuilder.CreateSanitizedId(id));

            // Adds any additional HTML attributes if any
            tb.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            // Calculates which button is wanted
            switch (buttonType)
            {
                case HtmlButtonTypes.button:
                    tb.MergeAttribute("type", "button");
                    break;

                case HtmlButtonTypes.submit:
                    tb.MergeAttribute("type", "submit");
                    break;

                case HtmlButtonTypes.reset:
                    tb.MergeAttribute("type", "reset");
                    break;

                default:
                    break;
            }

            // Creates button object
            return MvcHtmlString.Create(tb.ToString());
        }
    }
}