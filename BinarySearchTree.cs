using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp
{
    public class BinarySearchTree
    {
        public class Node
        {
            public IssueReport Data;
            public Node Left, Right;

            public Node(IssueReport data)
            {
                Data = data;
            }
        }

        public Node Root;

        public void Insert(IssueReport issue)
        {
            Root = InsertRec(Root, issue);
        }

        private Node InsertRec(Node root, IssueReport issue)
        {
            if (root == null) return new Node(issue);

            if (string.Compare(issue.ReferenceId, root.Data.ReferenceId) < 0)
                root.Left = InsertRec(root.Left, issue);
            else
                root.Right = InsertRec(root.Right, issue);

            return root;
        }

        public IssueReport Search(string id)
        {
            return SearchRec(Root, id);
        }

        private IssueReport SearchRec(Node root, string id)
        {
            if (root == null) return null;

            if (root.Data.ReferenceId == id)
                return root.Data;

            if (string.Compare(id, root.Data.ReferenceId) < 0)
                return SearchRec(root.Left, id);

            return SearchRec(root.Right, id);
        }
    }
}
