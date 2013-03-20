using System;
using System.IO;

namespace GroceryRegisterSimulation
{
    public class GroceryCustomerProcessing
    {
        /// <summary>
        /// Program entry point. Create a grocery store object with the parameter-file-supplied
        /// number of cash registers, add customers from parameter file lines. Process all of the
        /// customers in the store. Display the finish time.
        /// </summary>
        /// <param name="args">Parameter file name with path</param>
        public static void Main(string[] args)
        {
            // check argument
            if (!args.Length.Equals(1))
            {
                Console.WriteLine("Usage: GroceryRegisterSimulation <input file fullname>");
                return;
            }

            // Create, initialize grocery store.
            GroceryStore store;
            try
            {
                InputFile.InFileInstance(args[0]);
                store = new GroceryStore(int.Parse(InputFile.NextLine()));
                string custLine;
                while ((custLine = InputFile.NextLine()) != null)
                {
                    store.AddCustomer(StoreCustomer.CreateStoreCustomer(custLine));
                }
            }
            catch (FormatException fex)
            {
                Console.WriteLine("Contents of input file are invalid. " + fex.Message);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error with input file. " + ex.Message);
                return;
            }
            finally
            {
                InputFile.Dispose();
            }

            // Process customers and print result.
            try
            {
                store.ProcessAllCustomers();
                Console.WriteLine("Finished at: " + store.GetTimeOffsetOfCompletion());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program error. " + ex.Message);
            }
        }

    }

    /// <summary>
    /// Singleton pattern for the resource input file
    /// </summary>
    public class InputFile
    {
        private static InputFile _inFileInstance;

        private static StreamReader _inFileReader;

        private InputFile(string fileName)
        {
            if (fileName != null) _inFileReader = new StreamReader(fileName);
        }

        public static InputFile InFileInstance(string fileName)
        {
            return _inFileInstance ?? (_inFileInstance = new InputFile(fileName)); 
        }

        public static string NextLine()
        {
            return _inFileReader.ReadLine();
        }

        public static void Dispose()
        {
            if (_inFileReader != null)
            {
                _inFileReader.Close();
            }
        }
    }

}
