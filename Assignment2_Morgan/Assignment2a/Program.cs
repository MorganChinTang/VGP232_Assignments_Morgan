using System;
using System.Collections.Generic;
using System.IO;

// TODO: Fill in your name and student number.
// Assignment 1
// NAME: Morgan Chin Tang
// STUDENT NUMBER: 2042513

namespace Assignment1
{
    internal class MainClass
    {
        public static void Main(string[] args)
        {
            // Variables and flags

            // The path to the input file to load.
            string inputFile = string.Empty;

            // The path of the output file to save.
            string outputFile = string.Empty;

            // The flag to determine if we overwrite the output file or append to it.
            bool appendToFile = false;

            // The flag to determine if we need to display the number of entries
            bool displayCount = false;

            // The flag to determine if we need to sort the results via name.
            bool sortEnabled = false;

            // The column name to be used to determine which sort comparison function to use.
            string sortColumnName = string.Empty;

            // The results to be output to a file or to the console
            WeaponCollection results = new WeaponCollection();

            for (int i = 0; i < args.Length; i++)
            {
                // h or --help for help to output the instructions on how to use it
                if (args[i] == "-h" || args[i] == "--help")
                {
                    Console.WriteLine("-i <path> or --input <path> : loads the input file path specified (required)");
                    Console.WriteLine("-o <path> or --output <path> : saves result in the output file path specified (optional)");

                    // TODO: include help info for count
                    Console.WriteLine("-c or --count : displays the number of entries in the input file (optional).");

                    // TODO: include help info for append
                    Console.WriteLine("-a or --append : enables append mode when writing to an existing output file (optional).");

                    // TODO: include help info for sort
                    Console.WriteLine("-s or --sort <column name> : outputs the results sorted by column name (optional).");

                    break;
                }
                else if (args[i] == "-i" || args[i] == "--input")
                {
                    // Check to make sure there's a second argument for the file name.
                    if (args.Length > i + 1)
                    {
                        // stores the file name in the next argument to inputFile
                        ++i;
                        inputFile = args[i];

                        if (string.IsNullOrEmpty(inputFile))
                        {
                            // TODO: print no input file specified.
                            Console.WriteLine("Error: No input file specified.");
                        }
                        else if (!File.Exists(inputFile))
                        {
                            // TODO: print the file specified does not exist.
                            Console.WriteLine("Error: The file specified does not exist: {0}", inputFile);
                        }
                        else
                        {
                            // Load weapons from the file using WeaponCollection
                            if (!results.Load(inputFile))
                            {
                                Console.WriteLine("Error: Failed to load weapons from file: {0}", inputFile);
                            }
                        }
                    }
                }
                else if (args[i] == "-s" || args[i] == "--sort")
                {
                    // TODO: set the sortEnabled flag and see if the next argument is set for the column name
                    sortEnabled = true;
                    // TODO: set the sortColumnName string used for determining if there's another sort function.
                    if (args.Length > i + 1)
                    {
                        ++i;
                        sortColumnName = args[i];
                    }
                }
                else if (args[i] == "-c" || args[i] == "--count")
                {
                    displayCount = true;
                }
                else if (args[i] == "-a" || args[i] == "--append")
                {
                    // TODO: set the appendToFile flag
                    appendToFile = true;
                }
                else if (args[i] == "-o" || args[i] == "--output")
                {
                    // validation to make sure we do have an argument after the flag
                    if (args.Length > i + 1)
                    {
                        // increment the index.
                        ++i;
                        string filePath = args[i];
                        if (string.IsNullOrEmpty(filePath))
                        {
                            // TODO: print No output file specified.
                            Console.WriteLine("Error: No output file specified.");
                        }
                        else
                        {
                            // TODO: set the output file to the outputFile
                            outputFile = filePath;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("The argument Arg[{0}] = [{1}] is invalid", i, args[i]);
                }
            }

            if (sortEnabled)
            {
                // TODO: add implementation to determine the column name to trigger a different sort. (Hint: column names are the 4 properties of the weapon class)
                Console.WriteLine("Sorting by {0}", sortColumnName);

                // print: Sorting by <column name> e.g. BaseAttack
                try
                {
                    results.SortBy(sortColumnName);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }

            if (displayCount)
            {
                Console.WriteLine("There are {0} entries", results.Count);
            }

            if (results.Count > 0)
            {
                if (!string.IsNullOrEmpty(outputFile))
                {
                    if (appendToFile && File.Exists(outputFile))
                    {
                        // For append mode, we need to read existing data, add new data, and save
                        WeaponCollection existingWeapons = new WeaponCollection();
                        if (existingWeapons.Load(outputFile))
                        {
                            existingWeapons.AddRange(results);
                            if (existingWeapons.Save(outputFile))
                            {
                                string fullPath = Path.GetFullPath(outputFile);
                                Console.WriteLine("Results appended to {0}", fullPath);
                            }
                            else
                            {
                                Console.WriteLine("Error: Failed to save results to {0}", outputFile);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: Failed to read existing file for append: {0}", outputFile);
                        }
                    }
                    else
                    {
                        if (results.Save(outputFile))
                        {
                            string fullPath = Path.GetFullPath(outputFile);
                            Console.WriteLine("Results saved to {0}", fullPath);
                        }
                        else
                        {
                            Console.WriteLine("Error: Failed to save results to {0}", outputFile);
                        }
                    }
                }
                else
                {
                    // prints out each entry in the weapon list results.
                    for (int i = 0; i < results.Count; i++)
                    {
                        Console.WriteLine(results[i]);
                    }
                }
            }

            Console.WriteLine("Done!");
        }
    }
}
