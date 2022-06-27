using System;
using System.Collections.Generic;
using System.Text;

namespace PMPal.Backend
{
    public class CariesQuestion
    {
        int Number { get; set; }

        string Category { get; set; }

        string Name { get; set; }

        string Question { get; set; }
        //int Value { get; set; }
        public CariesQuestion(int number, string category, string name,string question)
        {
            this.Number = number;
            this.Category = category;
            this.Name = name;
            this.Question = question;
        }

    }
    static class CariesFormQuestions
    {
        static List<string> CariesFormQuestionsGroups = new List<string>
        {
            "Contributing Conditions",
            "General Health Conditions",
            "Clinical Conditions"
        };
        
    }

    public class CariesForm
    {
        List<CariesQuestion> cariesQuestions = new List<CariesQuestion>();

        CariesForm()
        {
            cariesQuestions = new List<CariesQuestion>();
            cariesQuestions.Add(new CariesQuestion(1, "Contributing Conditions", "FlourideExp", "Fluoride Exposure(drinking water, supplements, professional applications, toothpaste"));
            cariesQuestions.Add(new CariesQuestion(2, "Contributing Conditions", "Sugary Foods", "Sugary Foods or Drinks (juice, carbonated or noncarbonated soft drinks, energy drinks, medicinal syrups)"));
        }
    }

    
}
