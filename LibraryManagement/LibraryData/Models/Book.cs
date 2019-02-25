using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData.Models
{
    public class Book : LibraryAsset
    {
        public string ISBN { get; set; }

        public string Author { get; set; }

        public string IndexNumber { get; set; }
    }
}
