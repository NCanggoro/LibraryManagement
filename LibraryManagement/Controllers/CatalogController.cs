using LibraryData;
using LibraryManagement.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LibraryManagement.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAsset _asset;

        public CatalogController (ILibraryAsset asset)
        {
            _asset = asset;
        }

        public IActionResult Index()
        {
            var assetModels = _asset.GetAll();

            var listingResult = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id = result.Id,
                    ImageUrl = result.ImageUrl,
                    AuthorOrDirector = _asset.GetAuthorOrDirector(result.Id),
                    DeweyCallNumber = _asset.GetDeweyIndex(result.Id),
                    Title = result.Title,
                    Type = _asset.GetType(result.Id)
                });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };

            return View(model);
        }
    }
}
