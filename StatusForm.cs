using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public partial class StatusForm : Form
    {
        private DataGridView grid;
        private ListBox lstPriority;
        private ListBox lstGraph;
        private Label lblResult;
        private TextBox txtSearch;

        // ── Win32 cue banner (PlaceholderText for .NET Framework 4.8) ──
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wp, string lp);
        private const uint EM_SETCUEBANNER = 0x1501;
        private static void SetPlaceholder(TextBox tb, string text)
        {
            tb.HandleCreated += (s, e) => SendMessage(tb.Handle, EM_SETCUEBANNER, (IntPtr)1, text);
        }

        public StatusForm()
        {
            Text = "Service Request Status";
            Size = new Size(980, 710);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ── Header ──────────────────────────────────────────────
            Panel header = new Panel
            {
                Dock = DockStyle.Top, Height = 70,
                BackColor = Color.FromArgb(136, 0, 27)
            };
            Label lblHead = new Label
            {
                Text = "🔍  Service Request Status Tracker",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true, Top = 18, Left = 20
            };
            header.Controls.Add(lblHead);

            // ── BST Search ──────────────────────────────────────────
            Label lblSearch = MakeLabel("Search by Reference ID (BST lookup):", 20, 90);
            txtSearch = new TextBox
            {
                Top = 113, Left = 20, Width = 280,
                Font = new Font("Segoe UI", 10)
            };
            SetPlaceholder(txtSearch, "e.g. MSA-123456");
            Button btnSearch = MakeButton("🔍 Search", 315, 110, 110, Color.FromArgb(136, 0, 27));
            Button btnRefresh = MakeButton("↻ Refresh List", 440, 110, 130, Color.FromArgb(0, 84, 166));

            lblResult = new Label
            {
                Top = 148, Left = 20, Width = 920, Height = 24,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(50, 100, 50)
            };

            // ── DataGridView — submitted issues ─────────────────────
            Label lblGrid = MakeLabel("All Submitted Requests:", 20, 178);
            grid = new DataGridView
            {
                Top = 200, Left = 20, Width = 920, Height = 185,
                ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };
            SetupGrid();

            // ── Priority Queue ──────────────────────────────────────
            Label lblPriority = MakeLabel("Priority Queue (highest priority first):", 20, 400);
            lstPriority = new ListBox
            {
                Top = 423, Left = 20, Width = 440, Height = 160,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                HorizontalScrollbar = true
            };
            Button btnPriority = MakeButton("Load Priority Queue", 20, 593, 180, Color.FromArgb(90, 60, 150));

            // ── Graph BFS ───────────────────────────────────────────
            Label lblGraph = MakeLabel("Workflow Graph — BFS Traversal:", 490, 400);
            lstGraph = new ListBox
            {
                Top = 423, Left = 490, Width = 450, Height = 160,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };
            Button btnGraph = MakeButton("Show Workflow Flow", 490, 593, 180, Color.FromArgb(136, 0, 27));

            Button btnBack = MakeButton("← Back to Menu", 20, 630, 160, Color.FromArgb(60, 60, 80));

            // ── Wire events ─────────────────────────────────────────
            btnSearch.Click += (s, e) =>
            {
                string id = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(id))
                { MessageBox.Show("Please enter a Reference ID.", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                var res = AppData.Tree.Search(id);
                if (res != null)
                {
                    lblResult.ForeColor = Color.FromArgb(16, 137, 62);
                    lblResult.Text = $"✔  Found — Category: {res.Category}  |  Status: {res.Status}  |  Priority: {res.Priority}  |  Submitted: {res.SubmittedAt:dd MMM yyyy HH:mm}";
                }
                else
                {
                    lblResult.ForeColor = Color.FromArgb(136, 0, 27);
                    lblResult.Text = "✖  No record found for this Reference ID.";
                }
            };

            btnRefresh.Click += (s, e) => RefreshGrid();

            btnPriority.Click += (s, e) =>
            {
                lstPriority.Items.Clear();
                var all = AppData.PriorityQueue.GetAll();
                if (all.Count == 0) { lstPriority.Items.Add("No requests submitted yet."); return; }
                lstPriority.Items.Add("── Priority (highest first) ──");
                foreach (var i in all)
                    lstPriority.Items.Add($"  P{i.Priority}  |  {i.ReferenceId}  |  {i.Category}  |  {i.Status}");
            };

            btnGraph.Click += (s, e) =>
            {
                lstGraph.Items.Clear();
                if (AppData.Issues.Count == 0)
                { lstGraph.Items.Add("Submit an issue first to see the workflow."); return; }

                var first = AppData.Issues.First();
                var flow = AppData.Graph.BFS(first.ReferenceId);
                lstGraph.Items.Add($"── BFS from {first.ReferenceId} ──");
                int step = 1;
                foreach (var node in flow)
                    lstGraph.Items.Add($"  Step {step++}: {node}");
            };

            btnBack.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                header,
                lblSearch, txtSearch, btnSearch, btnRefresh, lblResult,
                lblGrid, grid,
                lblPriority, lstPriority, btnPriority,
                lblGraph, lstGraph, btnGraph,
                btnBack
            });
        }

        private void SetupGrid()
        {
            grid.Columns.Clear();
            grid.AutoGenerateColumns = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ReferenceId", HeaderText = "Reference ID",  FillWeight = 20 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Category",    HeaderText = "Category",      FillWeight = 18 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Location",    HeaderText = "Location",      FillWeight = 25 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status",      HeaderText = "Status",        FillWeight = 15 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Priority",    HeaderText = "Priority",      FillWeight = 10 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SubmittedAt", HeaderText = "Submitted At",  FillWeight = 22,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd MMM yyyy HH:mm" } });

            grid.DataSource = AppData.Issues;

            // Colour alternating rows
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 252);
            grid.ColumnHeadersDefaultCellStyle.BackColor   = Color.FromArgb(136, 0, 27);
            grid.ColumnHeadersDefaultCellStyle.ForeColor   = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font        = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;
        }

        private void RefreshGrid()
        {
            grid.DataSource = null;
            grid.DataSource = AppData.Issues;
        }

        private Label MakeLabel(string text, int left, int top)
        {
            return new Label
            {
                Text = text, Top = top, Left = left, AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 60, 80)
            };
        }

        private Button MakeButton(string text, int left, int top, int width, Color colour)
        {
            var btn = new Button
            {
                Text = text, Top = top, Left = left,
                Width = width, Height = 34,
                BackColor = colour, ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
