using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {
        private LibraryContext _context;
        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(CheckOut newCheckOut)
        {
            _context.Add(newCheckOut);
            _context.SaveChanges();
        }

        public IEnumerable<CheckOut> GetAll()
        {
            return _context.CheckOuts;
        }

        public CheckOut GetById(int checkoutId)
        {
            return GetAll()
                .FirstOrDefault(ci => ci.Id == checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetChekoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            if(checkout == null)
            {
                return "";
            }

            var cardId = checkout.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        private CheckOut GetCheckoutByAssetId(int assetId)
        {
            return _context.CheckOuts
                .Include(c => c.LibraryAsset)
                .Include(c => c.LibraryCard)
                .FirstOrDefault(c => c.LibraryAsset.Id == assetId);
        }

        public IEnumerable<Hold> GetCurrentHold(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public CheckOut GetLatestCheckout(int assetId)
        {
            return _context.CheckOuts
                .Where(c => c.LibraryAsset.Id == assetId)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }

        public bool IsCheckedOut(int assetId)
        {
            return _context.CheckOuts
                .Where(co => co.LibraryAsset.Id == assetId)
                .Any();
        }

        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;

            UpdateAssetStatus(assetId, "Available");
            RemoveExistingCheckout(assetId);
            CloseExistingCheckout(assetId, now);

            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string newAsset)
        {
            
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            _context.Update(item);

            item.Status = _context.Statuses
               .FirstOrDefault(s => s.Name == newAsset);
        }

        private void CloseExistingCheckout(int assetId, DateTime now)
        {
            //Close Existing C.Outs

            var history = _context.CheckoutHistories
                .FirstOrDefault(h => h.LibraryAsset.Id == assetId
                && h.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckout(int assetId)
        {
            // remove existing C.Outs
            var checkout = _context.CheckOuts
                 .FirstOrDefault(co => co.LibraryAsset.Id == assetId);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId,int libraryCardId)
        {
            var now = DateTime.Now;

            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            //remove existing C.Out
            RemoveExistingCheckout(assetId);
            //Close Existing C.Out
            CloseExistingCheckout(assetId, now);
            //Look Existing Holds
            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == assetId);
            //if hold != null Checkout out to earliest hold
            if (currentHolds.Any())
            {
                CheckoutToEarliestHolds(assetId, currentHolds);
            }
            //otherwise, update to available
            UpdateAssetStatus(assetId, "Available");
            _context.SaveChanges();
        }

        private void CheckoutToEarliestHolds(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHolds = currentHolds
                .OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault();

            var card = earliestHolds.LibraryCard;

            _context.Remove(earliestHolds);
            _context.SaveChanges();
            CheckOutItem(assetId, card.Id);
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId))
            {
                return;
                //give feedback for users
            }

            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            UpdateAssetStatus(assetId, "Checked Out");

            var libraryCard = _context.LibraryCards
                .Include(lc => lc.CheckOuts)
                .FirstOrDefault(lc => lc.Id == libraryCardId);
            var now = DateTime.Now;

            var checkout = new CheckOut
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };
            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .FirstOrDefault(la => la.Id == assetId);

            var card = _context.LibraryCards
                .FirstOrDefault(c => c.Id == libraryCardId);

            if (asset.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }
            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };
            _context.Add(hold);
            _context.SaveChanges();
        }

        public string GetCurrentHoldPatronName(int holdId)
        {
            var hold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId);

            var cardId = hold?.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return _context.Holds
               .Include(h => h.LibraryAsset)
               .Include(h => h.LibraryCard)
               .FirstOrDefault(h => h.Id == holdId)
               .HoldPlaced;
        }


    }

}
