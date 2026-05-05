using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public partial class EventsForm : Form
    {
        List<EventItem> events = new List<EventItem>();
        Queue<EventItem> queue = new Queue<EventItem>();
        Stack<string> history = new Stack<string>();
        HashSet<string> categories = new HashSet<string>();
        SortedDictionary<DateTime, EventItem> sorted = new SortedDictionary<DateTime, EventItem>();

        ListBox list;
        ListBox recommendations;
        TextBox search;

        public EventsForm()
        {
            Text = "Events";
            Size = new System.Drawing.Size(800, 500);

            search = new TextBox { Top = 20, Left = 20 };
            Button btn = new Button { Text = "Search", Top = 20, Left = 200 };

            list = new ListBox { Top = 60, Left = 20, Width = 300, Height = 300 };
            recommendations = new ListBox { Top = 60, Left = 350, Width = 300 };

            btn.Click += Search;

            LoadEvents();

            Controls.AddRange(new Control[] { search, btn, list, recommendations });


            Button btnQueue = new Button { Text = "Show Queue", Top = 400, Left = 20 };

            btnQueue.Click += (s, e) =>
            {
                list.Items.Clear();

                foreach (var ev in queue)
                    list.Items.Add("Next: " + ev.Title);
            };

            Button btnHistory = new Button { Text = "Search History", Top = 400, Left = 150 };

            btnHistory.Click += (s, e) =>
            {
                list.Items.Clear();

                foreach (var item in history)
                    list.Items.Add("Search: " + item);
            };
            Button btnCategories = new Button { Text = "Categories", Top = 400, Left = 300 };

            btnCategories.Click += (s, e) =>
            {
                list.Items.Clear();

                foreach (var cat in categories)
                    list.Items.Add(cat);
            };
        }

        void LoadEvents()
        {
            Add("Cleanup", "Sanitation", DateTime.Now.AddDays(2));
            Add("Road Fix", "Roads", DateTime.Now.AddDays(3));

            foreach (var e in sorted)
                list.Items.Add(e.Value.Title);
        }

        void Add(string title, string cat, DateTime date)
        {
            var e = new EventItem { Title = title, Category = cat, Date = date };

            events.Add(e);
            queue.Enqueue(e);
            categories.Add(cat);
            sorted[date] = e;
        }

        void Search(object s, EventArgs e)
        {
            string q = search.Text.ToLower();
            history.Push(q);

            list.Items.Clear();

            foreach (var ev in events)
                if (ev.Category.ToLower().Contains(q))
                    list.Items.Add(ev.Title);

            Recommend(q);
        }

        void Recommend(string q)
        {
            recommendations.Items.Clear();

            foreach (var ev in events)
            {
                if (ev.Category.ToLower().Contains(q) ||
                    ev.Title.ToLower().Contains(q))
                {
                    recommendations.Items.Add("Recommended: " + ev.Title + " (" + ev.Category + ")");
                }
            }
            foreach (var ev in sorted)
                list.Items.Add(ev.Value.Title + " - " + ev.Key.ToShortDateString());
        }
    }
}
