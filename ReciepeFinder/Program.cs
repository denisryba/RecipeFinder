using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecipeFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            begining:
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Title = "RecipeFinder";
            Console.Clear();
            
            Console.WriteLine("List the ingredients that you have (using 'Space' as a separator):");

            string requestedIngredients = Console.ReadLine();
            Console.WriteLine();

            string url;

            int index = 0;
            int page = 0;
            int countMax = 999;
            string data;

            for (int i = 1; i <= 10; i++)
            {
                Console.Clear();

                Console.WriteLine("List the ingredients that you have (using 'Space' as a separator):");
                Console.WriteLine(requestedIngredients);

                url = "http://www.recipepuppy.com/api/?i=" + requestedIngredients.Replace(" ", ",") + "&p=" + i;
                Console.WriteLine("\nSearching {0}0%", i);

                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    data = reader.ReadToEnd();
                }

                catch
                {
                    continue;
                }              
                

                RecipesList GottenRecipe = JsonConvert.DeserializeObject<RecipesList>(data);


                if (GottenRecipe.results.Count != 0)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        int count = 0;

                        for (int k = 0; k < GottenRecipe.results[j].ingredients.Length; k++)
                        {
                            if (GottenRecipe.results[j].ingredients[k] == ',')
                                count++;
                        }

                        if ((count + 1) < countMax)
                        {
                            countMax = count + 1;
                            index = j;
                            page = i;
                        }

                    }
                }

                else
                {
                    Console.WriteLine("\nSorry, but nothing was found :c");
                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey();
                    goto begining;
                }

                

            }

            url = "http://www.recipepuppy.com/api/?i=" + requestedIngredients.Replace(" ", ",") + "&p=" + page;

            HttpWebRequest requestOutput = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse responseOutput = (HttpWebResponse)requestOutput.GetResponse();

            StreamReader readerOutput = new StreamReader(responseOutput.GetResponseStream());
            data = readerOutput.ReadToEnd();

            RecipesList GottenRecipeOutput = JsonConvert.DeserializeObject<RecipesList>(data);


            Console.WriteLine("\nSimpliest recipe for requested ingredients is '{0}'.\nIt contains only this ingredients '{1}'.\nLink: {2}", GottenRecipeOutput.results[index].title, GottenRecipeOutput.results[index].ingredients, GottenRecipeOutput.results[index].href);

            Console.Write("\nPress any key to view the recipe...");
            Console.ReadKey();
            Process.Start(GottenRecipeOutput.results[index].href);

            Console.ReadKey();

        }
    }
}
