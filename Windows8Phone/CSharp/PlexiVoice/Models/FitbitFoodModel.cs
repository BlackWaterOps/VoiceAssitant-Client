using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexiVoice.Models
{
    public class FitbitFoodUnit
    {
        public string plural { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class FitbitLoggedFood
    {
        public int mealTypeId { get; set; }
        public int foodId { get; set; }
        public string locale { get; set; }
        public string brand { get; set; }
        public int calories { get; set; }
        public int amount { get; set; }
        public List<int> units { get; set; }
        public string accessLevel { get; set; }
        public FitbitFoodUnit unit { get; set; }
        public string name { get; set; }
    }

    public class FitbitNutritionalValues
    {
        public double carbs { get; set; }
        public double fiber { get; set; }
        public double sodium { get; set; }
        public int calories { get; set; }
        public double fat { get; set; }
        public double protein { get; set; }
    }

    public class FitbitFood
    {
        public int logId { get; set; }
        public FitbitLoggedFood loggedFood { get; set; }
        public FitbitNutritionalValues nutritionalValues { get; set; }
        public string logDate { get; set; }
        public bool isFavorite { get; set; }
    }

    public class FitbitFoodGoals
    {
        public int calories { get; set; }
        public int estimatedCaloriesOut { get; set; }
    }

    public class FitbitFoodSummary
    {
        public double carbs { get; set; }
        public double fiber { get; set; }
        public double sodium { get; set; }
        public int calories { get; set; }
        public double fat { get; set; }
        public int water { get; set; }
        public double protein { get; set; }
    }

    public class FitbitFoodModel
    {
        public List<FitbitFood> foods { get; set; }
        public FitbitFoodGoals goals { get; set; }
        public FitbitFoodSummary summary { get; set; }
    }
}
