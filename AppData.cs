using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp
{
    public static class AppData
    {
        public static List<IssueReport> Issues = new List<IssueReport>();
        public static BinarySearchTree Tree = new BinarySearchTree();
        public static PriorityQueueManager PriorityQueue = new PriorityQueueManager();
        public static GraphManager Graph = new GraphManager();
    }
}
