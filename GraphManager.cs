using System.Collections.Generic;

namespace MunicipalServicesApp
{
    public class GraphManager
    {
        private Dictionary<string, List<string>> graph = new Dictionary<string, List<string>>();

        public void AddEdge(string from, string to)
        {
            if (!graph.ContainsKey(from))
                graph[from] = new List<string>();

            graph[from].Add(to);
        }

        public List<string> BFS(string start)
        {
            List<string> result = new List<string>();
            Queue<string> queue = new Queue<string>();
            HashSet<string> visited = new HashSet<string>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                string node = queue.Dequeue();
                result.Add(node);

                if (!graph.ContainsKey(node)) continue;

                foreach (string neighbor in graph[node])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return result;
        }
    }
}