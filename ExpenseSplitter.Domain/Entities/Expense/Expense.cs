using ExpenseSplitter.Domain.Entities.ExpesneCategory;

namespace ExpenseSplitter.Domain.Entities.Expense
{
    public class Expense
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public ExpenseCategory Cattegory { get; private set; } = null!;
        public DateTime Date { get; private set; }
        
        public Expense(string name, Guid categoryId, DateTime? dateTime)
        {


            if (dateTime == null)
            {
                Date = DateTime.Now;
            }    


        }

        public void Update()
        {

        }
    }
}
