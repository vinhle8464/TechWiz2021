using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TechWizProject.Services;

namespace TechWizProject.Views.Shared.TagHelpers.Restaurant
{
    [HtmlTargetElement("restaurant",TagStructure = TagStructure.NormalOrSelfClosing)]
    public class RestaurantTag : TagHelper
    {
        [HtmlAttributeNotBound]

        [ViewContext]

        public ViewContext ViewContext { get; set; }

        private readonly IHtmlHelper htmlHelper;
        private readonly IRestaurantService restaurantService;

        public RestaurantTag(IRestaurantService restaurantService, IHtmlHelper htmlHelper)
        {
            this.restaurantService = restaurantService;

            this.htmlHelper = htmlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            (htmlHelper as IViewContextAware).Contextualize(ViewContext);
            output.TagName = "";
            var loadRestaurant = await restaurantService.LoadAllRestaurant();
            htmlHelper.ViewBag.restaurants = loadRestaurant;
            output.Content.SetHtmlContent(await htmlHelper.PartialAsync("TagHelpers/Restaurant/Index"));
        }
    }
}
