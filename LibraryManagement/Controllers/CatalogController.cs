using LibraryData;
using LibraryManagement.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LibraryManagement.Controllers
{
    public class CatalogController : Controller
    {
        private  ILibraryAsset _asset;
        private ICheckout _checkouts;

        public CatalogController (ILibraryAsset asset, ICheckout checkout)
        {
            _asset = asset;
            _checkouts = checkout;
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

        public IActionResult Detail(int id)
        {
            var asset = _asset.GetById(id);

            var currentHolds = _checkouts.GetCurrentHold(id)
                .Select(a => new AssetHoldModel
                {
                    HoldPlaced = _checkouts.GetCurrentHoldPlaced(a.Id),
                    PatronName = _checkouts.GetCurrentHoldPatronName(a.Id)

                });

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _asset.GetAuthorOrDirector(id),
                CurrentLocation = _asset.GetCurrentLocation(id).Name,
                DeweyCallNumber = _asset.GetDeweyIndex(id),
                CheckoutHistory = _checkouts.GetChekoutHistory(id),
                ISBN = _asset.GetIsbn(id),
                LatestCheckout = _checkouts.GetLatestCheckout(id),
                PatronName = _checkouts.GetCurrentCheckoutPatron(id),
                CurrentHolds = currentHolds
            };

            return View(model);
        }
    }
}
