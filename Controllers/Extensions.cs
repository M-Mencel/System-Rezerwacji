using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace System_Rezerwacji.Extensions
{
    public static class HtmlHelpers
    {
        public static IHtmlContent ActiveClass(this IHtmlHelper html, string controller, string? action)
        {
            var routeData = html.ViewContext.RouteData.Values;
            var currentController = routeData["controller"]?.ToString();
            var currentAction = routeData["action"]?.ToString();

            bool ctrlMatch = string.Equals(currentController, controller, System.StringComparison.OrdinalIgnoreCase);
            bool actionMatch = action is null || string.Equals(currentAction, action, System.StringComparison.OrdinalIgnoreCase);

            return new HtmlString(ctrlMatch && actionMatch ? "active" : "");
        }
    }
}
