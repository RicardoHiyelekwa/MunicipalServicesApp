using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public partial class StatusForm : Form
    {
        public StatusForm()
        {
            Text = "Status Tracker";
            Size = new Size(900, 600);

            TextBox txt = new TextBox { Top = 20, Left = 20 };
            Button btn = new Button { Text = "Search", Top = 20, Left = 200 };

            Label lbl = new Label { Top = 60, Left = 20, Width = 500 };

            DataGridView grid = new DataGridView
            {
                Top = 100,
                Left = 20,
                Width = 500,
                Height = 200,
                DataSource = AppData.Issues
            };

            ListBox lstPriority = new ListBox
            {
                Top = 320,
                Left = 20,
                Width = 250
            };

            Button btnPriority = new Button
            {
                Text = "Load Priority",
                Top = 480,
                Left = 20
            };

            ListBox lstGraph = new ListBox
            {
                Top = 320,
                Left = 300,
                Width = 250
            };

            Button btnGraph = new Button
            {
                Text = "Show Flow",
                Top = 480,
                Left = 300
            };

            btn.Click += (s, e) =>
            {
                var res = AppData.Tree.Search(txt.Text);

                if (res != null)
                    lbl.Text = $"Found: {res.Category} | Status: {res.Status} | Priority: {res.Priority}";
                else
                    lbl.Text = "Not found";
            };

            btnPriority.Click += (s, e) =>
            {
                lstPriority.Items.Clear();

                foreach (var i in AppData.PriorityQueue.GetAll())
                    lstPriority.Items.Add(i.ReferenceId + " - P:" + i.Priority);
            };

            btnGraph.Click += (s, e) =>
            {
                lstGraph.Items.Clear();

                if (AppData.Issues.Count == 0) return;

                var flow = AppData.Graph.BFS(AppData.Issues.First().ReferenceId);

                foreach (var step in flow)
                    lstGraph.Items.Add(step);
            };

            Controls.AddRange(new Control[]
            {
                txt, btn, lbl, grid,
                lstPriority, btnPriority,
                lstGraph, btnGraph
            });
        }
    }
}