using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using TcbInternetSolutions.Vulcan.Core;

namespace Vulcan.Examples.Models.Pages
{
    [ContentType(DisplayName = "General Page", GUID = "ef424896-1ab8-472d-b33d-1191c561e53b", Description = "")]
    public class GeneralPageData : PageData
    {

        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString MainBody { get; set; }


        [VulcanSearchable] // allows text content to be indexed
        [CultureSpecific]
        [Display(
                Name = "Main content",
                Description = "The main will display below the main body.",
                GroupName = SystemTabNames.Content,
                Order = 10)]
        public virtual ContentArea MainContent { get; set; }

    }
}