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
            
            output.Attributes.RemoveAll(_attributeName);
        }
    }
}
