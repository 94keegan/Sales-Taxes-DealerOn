using System.Collections.Generic;

namespace Sales_Taxes
{
    class Item
    {
        public bool Valid { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public string Imported { get; set; }

        public Item()
        {
            Valid = true;
        }
    }
}
