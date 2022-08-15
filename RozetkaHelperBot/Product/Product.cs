using System;
using System.Collections.Generic;
using System.Text;

namespace RozetkaHelperBot.Product
{
    class Product
    {
        public string Title { get; set; }
        public int Price { get; set; }

        public override string ToString()
        {
            return $"Title:  {Title}     Price:    {Price}";
        }
    }
}
