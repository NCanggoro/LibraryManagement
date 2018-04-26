using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface ICheckout
    {
        void Add(CheckOut newCheckOut);

        IEnumerable<CheckOut> GetAll();
        IEnumerable<CheckoutHistory> GetChekoutHistory(int id);
        IEnumerable<Hold> GetCurrentHold(int id);

        string GetCurrentHoldPatronName(int id);
        string GetCurrentCheckoutPatron(int assetId);
        DateTime GetCurrentHoldPlaced(int id);
        CheckOut GetById(int checkoutId);
        CheckOut GetLatestCheckout(int assetId);
        bool IsCheckedOut(int id);


        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId, int libraryCardId);
        void PlaceHold(int assetId, int libraryCardId);
        void MarkLost(int assetId);
        void MarkFound(int assetId);      

    }
}
