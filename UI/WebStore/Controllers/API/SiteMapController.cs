using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleMvcSitemap;
using WebStore.Interfaces.Services;

namespace WebStore.Controllers.API
{
    public class SiteMapController : ControllerBase
    {
        public IActionResult Index([FromServices] IProductService productService)
        {
            //статическая часть
            var nodes = new List<SitemapNode>
            {
                new(Url.Action("Index", "Home")),
                new(Url.Action("ContactUs", "Home")),
                new(Url.Action("Index", "Catalog")),
                new(Url.Action("Index", "WebAPI")),
            };


            //динамическая часть - все секции, бренды и товары
            nodes.AddRange(productService.GetSections().Select(s => new SitemapNode(Url.Action("Index", "Catalog", new { SectionId = s.Id }))));

            //просто как вариант - через foreach
            foreach (var brand in productService.GetBrands())
            {
                nodes.Add(new(Url.Action("Index", "Catalog", new { BrandId = brand.Id })));
            }

            foreach (var product in productService.GetProducts())
            {
                nodes.Add(new(Url.Action("Details", "Catalog", new { product.Id })));
            }

            return new SitemapProvider().CreateSitemap(new SitemapModel(nodes));
        }
    }
}
