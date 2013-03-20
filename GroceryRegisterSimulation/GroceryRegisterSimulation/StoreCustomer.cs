using System;
using System.Collections.Generic;
using System.Linq;

namespace GroceryRegisterSimulation
{

    public interface IStoreCustomer
    {
        int GetStartTime();
        void SetStartTime(int startTime);
        int GetArrivalTime();
        int GetNumberOfItems();
        int GetCompletionTime(bool isTrainingRegister);
        int GetSelectedRegister();
        char GetCustomerType();
        ICashRegister RegisterSelection(List<ICashRegister> registers, List<IStoreCustomer> customers);
    }
    
    public abstract class StoreCustomer : IStoreCustomer
    {
        private int NumberOfItems { get; set; }
        private int ArrivalTime { get; set; }
        private int StartTime { get; set; }
        protected int SelectedRegister { get; set; }

        protected StoreCustomer(int arrivalTime, int numberOfItems)
        {
            ArrivalTime = arrivalTime;
            NumberOfItems = numberOfItems;
        }

        public void SetStartTime(int startTime)
        {
            StartTime = startTime;
        }

        public int GetStartTime()
        {
            return StartTime;
        }

        public int GetArrivalTime()
        {
            return ArrivalTime;
        }

        public int GetNumberOfItems()
        {
            return NumberOfItems;
        }

        public int GetSelectedRegister()
        {
            return SelectedRegister;
        }

        /// <summary>
        /// Calculates completion time based on this customers start time, number of items, and register training status.
        /// </summary>
        /// <param name="isTrainingRegister">Boolean indicating register training clerk</param>
        /// <returns>Integer of actual completion time</returns>
        public int GetCompletionTime(bool isTrainingRegister)
        {
            // Requirement: 4.Regular registers take one minute to process each customer's item.
            // Requirement: The register staffed by the cashier in training takes two minutes for each item. 
            return isTrainingRegister ? (GetNumberOfItems() * 2) + GetStartTime() : GetNumberOfItems() + GetStartTime();
        }

        /// <summary>
        /// Factory pattern method to create a new Customer based on the input file line. 
        /// </summary>
        /// <exception cref="FormatException">If customer type not equal to 'A' or 'B'</exception>
        /// <param name="inputLine">String of the input line</param>
        /// <returns>object of type IStoreCustomer</returns>
        /// <remarks>Throws error for blank line.</remarks>
        public static IStoreCustomer CreateStoreCustomer(string inputLine)
        {
            var inputItems = inputLine.Split(null);
            if (inputItems[0].Equals("A"))
            {
                return new CustomerTypeA(int.Parse(inputItems[1]), int.Parse(inputItems[2]));
            }
            if (inputItems[0].Equals("B"))
            {
                return new CustomerTypeB(int.Parse(inputItems[1]), int.Parse(inputItems[2]));
            }
            throw new FormatException("Invalid customer type specification, must be 'A' or 'B'.");
        }

        /// <summary>
        /// Customer type specification
        /// </summary>
        /// <returns>Character 'A' or 'B' indicating customer type</returns>
        public abstract char GetCustomerType();

        /// <summary>
        /// Perform selection of cash register. Set the local variable 'SelectedRegister' to the number of the selected register.
        /// </summary>
        /// <param name="registers">list of cash registers</param>
        /// <param name="customers">list of customers</param>
        /// <returns>object of type ICashRegister representing the selected cash register</returns>
        public abstract ICashRegister RegisterSelection(List<ICashRegister> registers, List<IStoreCustomer> customers);

    }

    public class CustomerTypeA : StoreCustomer
    {
        public CustomerTypeA(int arrivalTime, int numberOfItems) : base(arrivalTime, numberOfItems) { }

        public override char GetCustomerType()
        {
            return 'A';
        }

        // Requirement: Customer Type A always chooses the register with the shortest line (least number of customers in line).
        public override ICashRegister RegisterSelection(List<ICashRegister> registers, List<IStoreCustomer> customers )
        {
            var numberOfCustomersInShortestLine = registers.Min(cashRegister => cashRegister.GetNbrCustomersInLine());
            var cashRegisterSelect =
                registers.Find(
                    cashRegister => cashRegister.GetNbrCustomersInLine().Equals(numberOfCustomersInShortestLine));
            SelectedRegister = cashRegisterSelect.GetRegisterNumber();
            return cashRegisterSelect;
        }
    }

    public class CustomerTypeB : StoreCustomer
    {
        public CustomerTypeB(int arrivalTime, int numberOfItems) : base(arrivalTime, numberOfItems) { }

        public override char GetCustomerType()
        {
            return 'B';
        }

        public override ICashRegister RegisterSelection(List<ICashRegister> registers, List<IStoreCustomer> customers)
        {
            ICashRegister cashRegister;

            // Requirement: Customer Type B will always choose an empty line before a line with any customers in it.
            cashRegister = registers.Find(register => register.GetNbrCustomersInLine().Equals(0));

            // Requirement: Customer Type B looks at the last customer in each line, and always chooses to be behind the customer with the 
            // fewest number of items left to check out.
            if (cashRegister == null)
            {
                // get a list of the last customers in line
                var lastCustomersInLine = (from register in registers 
                                           where register.GetLastCustInLine() != null 
                                           select register.GetLastCustInLine()).ToList();
                // get the first customer with the fewest items
                var customerInLineWithFewest =
                    lastCustomersInLine.Find(
                        customerFewest =>
                        customerFewest.GetNumberOfItems()
                                      .Equals(
                                          lastCustomersInLine.Min(customerInLine => customerInLine.GetNumberOfItems())));
                // get the register for the customer with the fewest items
                cashRegister =
                    registers.Find(
                        register => register.GetRegisterNumber().Equals(customerInLineWithFewest.GetSelectedRegister()));
            }
            // set the selected register number
            SelectedRegister = cashRegister.GetRegisterNumber();
            return cashRegister;
        }
    }

}
