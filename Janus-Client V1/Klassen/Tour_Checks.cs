using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Windows;

namespace TrucksLOG.Klassen
{
    class Tour_Checks
    {
        public static readonly string __version = "1.0.0.0";
        public static readonly string __supported_net_framework_version = "4.7.1";
        public static readonly int __supported_net_framework_regkey = 461308;
        public static readonly string __supported_automation_language_version = "0.1";
        public static string[] lines;
        public static ArrayList unitList;
        public static StreamReader file;

        public static string Suche_Money()
        {
            string path = REG.Lesen("Pfade", "Autosave_Path");

            return File.ReadAllLines(path).ToString();
        }

       

        public static bool checkDecryptState()
        {
            if (!Tour_Checks.lines[0].StartsWith("ScsC"))
            {
                return true;
            }
            return false;
        }

        public static class Input
        {
            public static string path = Directory.GetCurrentDirectory() + @"\game.sii";

            public static void readSavegame()
            {
                Tour_Checks.lines = File.ReadAllLines(path);

            }

            
        }


        public static string path = Directory.GetCurrentDirectory() + @"\game.sii";
        

        public static void readSavegame()
        {
            Tour_Checks.lines = File.ReadAllLines(path);
        }


        public static void changeAttribute(int unitIndex, int attrIndex, string newValue)
        {
            ((ArrayList)unitList[unitIndex])[attrIndex] = newValue;
        }


        public static void resetSystem()
        {
            Input.path = Directory.GetCurrentDirectory() + @"\game.sii";
            Array.Clear(Tour_Checks.lines, 0, Tour_Checks.lines.Length);
            unitList.Clear();
        }

        private int[] getID(TreeNode node)
        {
            int[] indexes = new int[2];
            int[] tagArray = (int[])node.DataItem;

            for (int i = 0; i < 2; ++i)
            {
                indexes[i] = tagArray[i];
            }

            foreach (int inn in indexes)
            {
                Console.WriteLine("INN: " + inn);
            }

            Console.WriteLine("----------");

            return indexes;
        }

        public static void addUnit(ArrayList values)
        {
            unitList.Add(values);
        }


        public static ArrayList findTerm(string searchTerm)
        {
            ArrayList matchList = new ArrayList();
            int countUnit = 0;
            int countAttr = 0;

            foreach (ArrayList attrList in unitList)
            {
                foreach (string attr in attrList)
                {
                    Regex regex = new Regex(searchTerm, RegexOptions.IgnoreCase);
                    Match match = regex.Match(attr);
                    while (match.Success)
                    {
                        matchList.Add(countUnit);
                        match = match.NextMatch();
                    }
                    ++countAttr;
                }
                ++countUnit;
            }

            return matchList;
        }

        public static void insertAttribute(int unitIndex, string attrValue, int attrIndex = 1)  // Index = 0 is the unit (name) itself
        {
            ((ArrayList)unitList[unitIndex]).Insert(attrIndex, attrValue);
        }


        


    }
}
