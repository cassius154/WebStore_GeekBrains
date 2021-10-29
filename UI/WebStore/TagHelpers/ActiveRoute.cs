using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebStore.TagHelpers
{
    //[HtmlTargetElement("QWE")]
    //[HtmlTargetElement(Attributes = "qwe,asd,zxc")]
    [HtmlTargetElement(Attributes = _attributeName)]
    public class ActiveRoute : TagHelper
    {
        private const string _attributeName = "ws-is-active-route";

        [HtmlAttributeName("ws-active-route-class")]
        public string ActiveCssClass { get; set; } = "active";

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        //так можно получить итоговую генерируемую ссылку
        //[HtmlAttributeName("href")]
        //public string Href { get; set; }

        [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
        public Dictionary<string, string> RouteValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        //HtmlAttributeNotBound указывает на то, что свойство ViewContext не связано с разметкой
        [ViewContext, HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (_isActive())
            {
                _makeActive(output);
            }

            output.Attributes.RemoveAll(_attributeName);
        }

        private bool _isActive()
        {
            //здесь то, что приходит из адресной строки браузера
            var routeValues = ViewContext.RouteData.Values;
            var routeController = routeValues["controller"]?.ToString();
            var routeAction = routeValues["action"]?.ToString();

            //если действие в ссылке указано (HtmlAttributeName("asp-action"))
            //и оно не совпадает с тем, что указано в пришедшем маршруте (адресной строке) - false
            //if (!string.IsNullOrEmpty(Action) && !string.Equals(Action, routeAction))
            //{
            //    return false;
            //}

            //то же в новом синтаксисе
            if (Action is { Length: > 0 } action && !string.Equals(action, routeAction))
            {
                return false;
            }

            //если контроллер в ссылке указан  HtmlAttributeName("asp-controller")
            //и он не совпадает с тем, что указан в пришедшем маршруте (адресной строке) - false
            if (Controller is { Length: > 0 } controller && !string.Equals(controller, routeController))
            {
                return false;
            }

            //теперь мы должны взять все содержимое словаря, составленного из атрибутов asp-route-
            //и сравнить его с тем, что указано в пришедшем из адресной строки маршруте (ViewContext - routeValues) -  
            //в RouteValues те части запроса, что указано в ссылке в атрибутах asp-route-
            foreach (var (key, value) in RouteValues)
            {
                //если пришедший маршрут не включает key из атрибутов asp-route-
                //или значения в пришедшем маршруте и прописанном в текущей ссылке не равны - false
                if (!routeValues.ContainsKey(key) || routeValues[key]?.ToString() != value)
                {
                    return false;
                }
            }

            return true;
        }

        private void _makeActive(TagHelperOutput output)
        {
            var classAttribute = output.Attributes.FirstOrDefault(attr => attr.Name == "class");

            if (classAttribute is null)
            {
                output.Attributes.Add("class", ActiveCssClass);
            }
            else
            {
                if (classAttribute.Value?.ToString()?.Contains(ActiveCssClass) ?? false)
                {
                    return;
                }

                output.Attributes.SetAttribute("class", $"{classAttribute.Value} {ActiveCssClass}");
            }
        }
    }
}
