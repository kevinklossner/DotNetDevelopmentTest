using System.Collections.Generic;
using System.Linq;

namespace GroceryRegisterSimulation
{
    public interface ICashRegister
    {
        bool IsTrainingRegister();
        void SetIsTrainingRegister();
        int GetNbrCustomersInLine();
        int GetNbrItemsOfLastCustomer();
        int GetRegisterNumber();
        void PutCustomerInLine(IStoreCustomer customer);
        void RemoveFirstCustInLine();
        IStoreCustomer GetFirstCustInLine();
        IStoreCustomer GetLastCustInLine();
    }

    public class CashRegister : ICashRegister
    {
        private int RegisterNumber { get; set; }

        private bool _trainingRegister;

        private readonly Queue<IStoreCustomer> _customersInLine;

        public bool IsTrainingRegister()
        {
            return _trainingRegister;
        }

        public void SetIsTrainingRegister()
        {
            _trainingRegister = true;
        }

        public CashRegister(int registerNumber)
        {
            RegisterNumber = registerNumber;
            _customersInLine = new Queue<IStoreCustomer>();
        }

        public int GetRegisterNumber()
        {
            return RegisterNumber;
        }

        public int GetNbrCustomersInLine()
        {
            return _customersInLine.Count;
        }

        public int GetNbrItemsOfLastCustomer()
        {
            return GetLastCustInLine().GetNumberOfItems();
        }

        public IStoreCustomer GetLastCustInLine()
        {
            return _customersInLine.LastOrDefault();
        }

        public IStoreCustomer GetFirstCustInLine()
        {
            return _customersInLine.FirstOrDefault();
        }

        public void PutCustomerInLine(IStoreCustomer customer)
        {
            _customersInLine.Enqueue(customer);
        }

        public void RemoveFirstCustInLine()
        {
            _customersInLine.Dequeue();
        }
    }
}
