using System.Collections.Generic;
using System.Linq;

namespace MunicipalServicesApp
{
    public class PriorityQueueManager
    {
        private List<IssueReport> heap = new List<IssueReport>();

        public void Enqueue(IssueReport issue)
        {
            heap.Add(issue);

            heap = heap.OrderByDescending(i => i.Priority).ToList();
        }

        public List<IssueReport> GetAll()
        {
            return heap;
        }
    }
}