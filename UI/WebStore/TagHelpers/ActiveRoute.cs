using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebStore.TagHelpers
{
    //[HtmlTargetElement("QWE")]
    //[HtmlTargetElement(Attributes = "qwe,asd,zxc")]
    [HtmlTargetElement(Attributes = _attributeName)]
    public class ActiveRoute : TagHelper
    {
        private const string _attributeName = "ws-is-active-route";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            
            output.Attributes.RemoveAll(_attributeName);
        }
    }
}
