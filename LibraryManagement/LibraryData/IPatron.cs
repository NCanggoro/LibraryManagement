using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface IPatron
    {
        IPatron Get(int id);
        IEnumerable<Patron> GetAll();
        void Add(Patron newPatron);

        IEnumerable<Checkout> GetCheckouts(int patronId);
        IEnumerable<CheckoutHistory> GetCheckoutHistories(int patronId);
        IEnumerable<Hold> GetHold(int patronId);
    }
}
