using System.Collections.Generic;
using System.Linq;

namespace GroceryRegisterSimulation
{
    public interface IGroceryStore
    {
        void AddCustomer(IStoreCustomer customer);
        string GetTimeOffsetOfCompletion();
        void ProcessAllCustomers();
    }

    public class GroceryStore : IGroceryStore
    {
        private const string TimeOffsetUnits = " minutes";
        
        private readonly List<ICashRegister> _registers;

        private readonly List<IStoreCustomer> _customers;

        /// <summary>
        /// Initialize the store with the number of registers, set each register number,
        /// designate the last register as a training register
        /// </summary>
        /// <param name="numberOfRegisters">Number of cash registers to create</param>
        public GroceryStore(int numberOfRegisters)
        {
            // Requirement: The number of registers is specified by the problem inputs; 
            _registers = new List<ICashRegister>(numberOfRegisters);
            for (var i = 1; i <= numberOfRegisters; i++)

                // Requirement: registers are numbered 1, 2, 3, ..., n for n registers.
                _registers.Add(new CashRegister(i));
            
            // Requirement: The grocery store always has a single cashier in training. 
            // This cashier is always assigned to the highest numbered register.
            _registers.Last().SetIsTrainingRegister();

            _customers = new List<IStoreCustomer>();
        }

        /// <summary>
        /// Add customer to private list of customers
        /// </summary>
        /// <param name="customer">Customer to add to list</param>
        public void AddCustomer(IStoreCustomer customer)
        {
            _customers.Add(customer);
        }

        /// <summary>
        /// Get a string for time in TimeOffsetUnits of the last customer finish.
        /// This will make a list of completion times for each registers' last customer. 
        /// The maximum from this list is defined as the time the last customer finishes.
        /// Assumes all customers are processed (in a line or finished checkout).
        /// </summary>
        /// <returns>String to display as answer in the form: 't=x minutes'</returns>
        public string GetTimeOffsetOfCompletion()
        {
            // make a list of completion times for the last customer in line at each register
            var lastTimes = _registers.Select(register => 
                register.GetLastCustInLine().GetCompletionTime(register.IsTrainingRegister())).ToList();
            // get the maximum time from the list
            var lastTime = lastTimes.Concat(new[] {int.MinValue}).Max();
            return "t=" + lastTime + TimeOffsetUnits;
        }

        /// <summary>
        /// Increment the time offset and process customers according to current time offset.
        /// Process involves removing customers that have finished the checkout, 
        /// then servicing customers that are ready for the checkout.
        /// </summary>
        public void ProcessAllCustomers()
        {
            var timeOffset = 0;
            // a customer is processed when the start time is set, if any are not set then continue
            while (_customers.Any(customer => customer.GetStartTime().Equals(0)))
            {
                // increment the time offset and remove any customers that finished
                RemoveFinishedCustomers(++timeOffset);

                // service all customers that are ready
                IStoreCustomer nextCustomer;
                while((nextCustomer = GetNextReadyCustomer(timeOffset)) != null)
                    ServiceCustomer(nextCustomer, timeOffset);
            }
        }

        private void ServiceCustomer(IStoreCustomer customer, int timeOffset)
        {
            // perform register selection and get the selected register
            var selectedRegister = customer.RegisterSelection(_registers, _customers);
            
            // set the start time
            if (selectedRegister.GetNbrCustomersInLine().Equals(0))
                // start time is current time if line is empty
                customer.SetStartTime(timeOffset);
            else
                // otherwise start time is the end time of the last customer in line
                customer.SetStartTime(
                    selectedRegister.GetLastCustInLine()
                                    .GetCompletionTime(selectedRegister.IsTrainingRegister()));
            
            // put the customer in the queue
            selectedRegister.PutCustomerInLine(customer as StoreCustomer);
        }

        private IStoreCustomer GetNextReadyCustomer(int timeOffset)
        {
            // get an ordered list of the customer that are ready to checkout
            var readyCustomers = _customers.Where(customer =>
                                                 customer.GetArrivalTime().Equals(timeOffset) &&
                                                 customer.GetSelectedRegister().Equals(0))
                                          .OrderBy(customer => customer.GetNumberOfItems())
                                          .ThenBy(customer => customer.GetCustomerType());
            // return the first customer in the list
            return readyCustomers.FirstOrDefault();
        }

        private void RemoveFinishedCustomers(int timeOffset)
        {
            // At each register, see if the current customer (FirstCustInLine) has a completion time
            // that is equal to the current time offset indicating checkout is done. 
            foreach (var cashRegister in 
                from cashRegister in _registers 
                let currentCustomer = cashRegister.GetFirstCustInLine() 
                where currentCustomer != null
                where currentCustomer.GetCompletionTime(cashRegister.IsTrainingRegister()).Equals(timeOffset) 
                select cashRegister)
            {
                // If it is equal, remove the current customer
                cashRegister.RemoveFirstCustInLine();
            }
        }
    }
}
