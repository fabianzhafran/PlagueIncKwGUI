﻿using System;
using System.Collections.Generic;
using System.IO;

namespace PlagueIncAlgorithm
{
    class PlagueInc
    {
        static readonly string inputFile = Path.Combine(Environment.CurrentDirectory, "..\\..\\txt\\InputFile.txt");
        static readonly string populationFile = Path.Combine(Environment.CurrentDirectory, "..\\..\\txt\\PopulationFile.txt");
        public static Dictionary<string, int> cityPopulationList = new Dictionary<string, int>();
        public static Dictionary<string, Dictionary<string, float>> connectedCityList = new Dictionary<string, Dictionary<string, float>>();
        public static Dictionary<string, bool> visited = new Dictionary<string, bool>();
        public static Dictionary<string, int> dayCityGotInfected = new Dictionary<string, int>();
        public static Dictionary<string, List<string>> cityInfectsOthers = new Dictionary<string, List<string>>();

        // AddConnectedCity()
        // Adds the connection from {sourceCity} to {targetCity} with weight = {weight} to the dictionary {connectedCityList}
        public static void AddConnectedCity(string sourceCity, string targetCity, float weight, Dictionary<string, Dictionary<string, float>> connectedCityList)
        {
            if (connectedCityList.ContainsKey(sourceCity))
            {
                if (!connectedCityList[sourceCity].ContainsKey(targetCity))
                {
                    connectedCityList[sourceCity].Add(targetCity, weight);
                }
                else
                {
                    Console.WriteLine("Connection already established.");
                }
            }
            else
            {
                Dictionary<string, float> tempDict = new Dictionary<string, float>();
                tempDict.Add(targetCity, weight);
                connectedCityList.Add(sourceCity, tempDict);
            }
        }


        // Function for I(A, t(A))
        public static float InfectedPopulationInCity(string plaguedCity, Dictionary<string, int> cityPopulationList, Dictionary<string, int> dayCityGotInfected, int totalDays)
        {
            float T_city = dayCityGotInfected[plaguedCity]; // T(plaguedCity)
            Console.WriteLine("T({0}) = {1}", plaguedCity, T_city);
            float t_city = totalDays - T_city; // t(plaguedCity)
            Console.WriteLine("t({0}) = {1}", plaguedCity, t_city);
            float cityPopulation = cityPopulationList[plaguedCity]; // P(plaguedCity)
            Console.WriteLine("P({0}) = {1}", plaguedCity, cityPopulation);
            // Console.WriteLine("{0}", (float)Math.Exp(0.25 * t_city));

            return (cityPopulation / (1 + ((cityPopulation - 1) / (float)Math.Exp(0.25 * t_city))));
        }

        // Function for finding how many days adjacentCity got infected after plaguedCity got infected
        // (t)
        public static int WhenCityGotInfected(string plaguedCity, string adjacentCity, Dictionary<string, int> cityPopulationList, Dictionary<string, int> dayCityGotInfected, Dictionary<string, Dictionary<string, float>> connectedCityList, int totalDays)
        {
            float cityPopulation = cityPopulationList[plaguedCity]; // P(A)
            float travelChance = connectedCityList[plaguedCity][adjacentCity]; // Tr(A,B)
            double tempDays = (Math.Log((cityPopulation - 1) / (travelChance * cityPopulation - 1)) * 4);
            // Karena di spek > 1,
            if (tempDays % 1 == 0)
            {
                tempDays += 1;
            }
            else
            {
                tempDays = Math.Ceiling(tempDays);
            }

            return (int)tempDays;
        }

        // Function to check if an adjacentCity will get infected by a plagueCity
        public static bool IsSpread(string plaguedCity, string adjacentCity, Dictionary<string, Dictionary<string, float>> connectedCityList, Dictionary<string, int> cityPopulationList, Dictionary<string, int> dayCityGotInfected, int totalDays)
        {
            float travelChance = connectedCityList[plaguedCity][adjacentCity]; // Tr(PlaguedCity, adjacentCity)
            Console.WriteLine("From City {0}", plaguedCity);
            Console.WriteLine("To City {0}", adjacentCity);
            float infectedPopulation = InfectedPopulationInCity(plaguedCity, cityPopulationList, dayCityGotInfected, totalDays); // I(plaguedCity, t(plaguedCity))
            float spreadChance = infectedPopulation * travelChance; // S(plaguedCity, adjacentCity)
            Console.WriteLine("Tr({0}, {1}) = {2}", plaguedCity, adjacentCity, travelChance);
            Console.WriteLine("I({0}) = {1}", plaguedCity, infectedPopulation);
            Console.WriteLine("S({0}, {1}) = {2}", plaguedCity, adjacentCity, spreadChance);

            return (spreadChance > 1);
        }
        public static void BFS(string startingCity, Dictionary<string, bool> visited, Dictionary<string, Dictionary<string, float>> connectedCityList, Dictionary<string, int> cityPopulationList, List<string> result, int totalDays, Dictionary<string, List<String>> cityInfectsOthers)
        {
            Queue<string> bfsQueue = new Queue<string>();
            Dictionary<string, int> dayCityGotInfected = new Dictionary<string, int>(); // To keep track of the start day when a city got infected
            bfsQueue.Enqueue(startingCity);
            dayCityGotInfected[startingCity] = 0;
            result.Add(startingCity);
            while (bfsQueue.Count > 0)
            {
                string plaguedCity = bfsQueue.Peek();
                visited[plaguedCity] = true;
                bfsQueue.Dequeue();
                if (!cityInfectsOthers.ContainsKey(plaguedCity))
                {
                    List<string> tempList = new List<string>();
                    cityInfectsOthers.Add(plaguedCity, tempList);
                }
                    
                foreach (KeyValuePair<string, float> adjacentCity in connectedCityList[plaguedCity])
                {
                    if ((!visited[adjacentCity.Key]) && (IsSpread(plaguedCity, adjacentCity.Key, connectedCityList, cityPopulationList, dayCityGotInfected, totalDays)))
                    {
                        
                        
                        cityInfectsOthers[plaguedCity].Add(adjacentCity.Key);
                   
                        bfsQueue.Enqueue(adjacentCity.Key);
                        visited[adjacentCity.Key] = true;
                        result.Add(adjacentCity.Key);
                        int dayAdjacentCityInfected = WhenCityGotInfected(plaguedCity, adjacentCity.Key, cityPopulationList, dayCityGotInfected, connectedCityList, totalDays);
                        dayCityGotInfected.Add(adjacentCity.Key, dayAdjacentCityInfected + dayCityGotInfected[plaguedCity]); // Add an infected city to the dict

                        Console.WriteLine("City {0} got infected from city {1}!", adjacentCity.Key, plaguedCity);
                    }
                }
            }

            foreach (KeyValuePair<string, int> city in dayCityGotInfected)
            {
                Console.WriteLine("City {0} Infected Since Day: {1}", city.Key, city.Value);
            }
        }
        public static List<string> PlagueIncResult()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~");
            Console.WriteLine("Mulaigan");
            List<string> result = new List<string>();
            string[] inputLines = { };
            string[] populationLines = { };
            int cityLength = 0;
            string startingCity = "";
            //int amtOfCityConnected;

            if (File.Exists(inputFile))
            {
                inputLines = File.ReadAllLines(inputFile);
                int.TryParse(inputLines[0], out cityLength);
            }

            // INPUT CONNECTED CITY
            // If already listed, then do nothing.
            if (connectedCityList.Count == 0)
            {
                foreach (string inputLine in inputLines)
                {
                    // Console.WriteLine(inputLine);
                    string[] inputLineSplit;
                    float weight;
                    if (inputLine != inputLines[0])
                    {
                        inputLineSplit = inputLine.Split(' ');
                        // Console.WriteLine(inputLineSplit[2]);
                        float.TryParse(inputLineSplit[2].Replace('.', ','), out weight);
                        // Console.WriteLine(weight);
                        AddConnectedCity(inputLineSplit[0], inputLineSplit[1], weight, connectedCityList);
                        if (!visited.ContainsKey(inputLineSplit[0]))
                        {
                            visited.Add(inputLineSplit[0], false);
                        }
                        if (!visited.ContainsKey(inputLineSplit[1]))
                        {
                            visited.Add(inputLineSplit[1], false);
                        }
                    }
                }
            }
            else
            {
                try
                {
                    foreach (KeyValuePair<string, bool> visitedEl in visited)
                    {
                        visited[visitedEl.Key] = false;
                    }
                }
                catch
                {

                }
            }

            // INPUT CITY POPULATION
            if (cityPopulationList.Count == 0)
            {
                if (File.Exists(populationFile))
                {
                    populationLines = File.ReadAllLines(populationFile);
                    startingCity = populationLines[0].Split(' ')[1];
                    foreach (string populationLine in populationLines)
                    {
                        Console.WriteLine(populationLine);
                        if (populationLine != populationLines[0])
                        {
                            string[] populationLineSplit = populationLine.Split(' ');
                            if (visited.ContainsKey(populationLineSplit[0]))
                            {
                                int tempPopulation;
                                int.TryParse(populationLineSplit[1], out tempPopulation);
                                cityPopulationList.Add(populationLineSplit[0], tempPopulation);
                                Console.WriteLine(tempPopulation);
                            }
                        }
                    }
                }
            }

            // Print List
            Console.WriteLine("-----------------CITY CONNECTED LIST-----------------");
            foreach(KeyValuePair<string, Dictionary<string, float>> connectedCity in connectedCityList) {
                Console.WriteLine("City {0} to", connectedCity.Key);
                foreach(KeyValuePair<string, float> city in connectedCity.Value) {
                    Console.WriteLine("{0} => {1}", city.Key, city.Value);
                }
            }

            // TEST BFS
            int inputDays;
            inputDays = PlagueIncGUI.Form1.inputDays;
            Console.WriteLine("Days as Input : {0}",inputDays);
            BFS("A", visited, connectedCityList, cityPopulationList, result, inputDays, cityInfectsOthers);


            return result;
        }
    }
}