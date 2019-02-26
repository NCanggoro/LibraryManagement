using LibraryData;
using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {
        private readonly LibraryContext _context;

        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll().FirstOrDefault(x => x.Id == checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistories(int id)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentHoldPatronName(int id)
        {
            throw new NotImplementedException();
        }

        public DateTime GetCurrentHoldPlaced(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            throw new NotImplementedException();
        }

        public Checkout GetLatestCheckout(int assetId)
        {
            throw new NotImplementedException();
        }

        public bool IsCheckedout(int id)
        {
            throw new NotImplementedException();
        }

        public void MarkFound(int assetId)
        {
            throw new NotImplementedException();
        }

        public void MarkLost(int assetId)
        {
            throw new NotImplementedException();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public void CheckInItem(int assetId)
        {
            throw new NotImplementedException();
        }

        public void CheckoutItem(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

    }
}
