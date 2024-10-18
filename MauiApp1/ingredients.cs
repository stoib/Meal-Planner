using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    internal class Ingredients
    {
        //Defines the ingredient type (use the fields from the database)

        public int IngredientID { get; set; }
        public string IngredientName { get; set; }
        public int IngredientQuantity { get; set; }

        public bool IngredientMain { get; set; }

    }
}
