using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public partial class EventsForm : Form
    {
        // ── Data structures ────────────────────────────────────────
        private SortedDictionary<DateTime, List<EventItem>> sortedEvents
            = new SortedDictionary<DateTime, List<EventItem>>();
        private Queue<EventItem>      eventQueue    = new Queue<EventItem>();
        private Stack<string>         searchHistory = new Stack<string>();
        private HashSet<string>       categories    = new HashSet<string>();
        private Dictionary<string, int> searchCount = new Dictionary<string, int>();

        // ── Controls ───────────────────────────────────────────────
        private ListBox  lstEvents;
        private ListBox  lstRecommendations;
        private TextBox  txtSearch;
        private ListBox  lstInfo;

        public EventsForm()
        {
            Text = "Local Events & Announcements";
            // 950 wide x 680 tall — all controls fit
            Size = new Size(980, 695);
            MinimumSize = new Size(980, 695);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            SeedSampleEvents();

            // ── Header ────────────────────────────────────────────
            Panel header = new Panel
            {
                Dock = DockStyle.Top, Height = 65,
                BackColor = Color.FromArgb(0, 84, 166)
            };
            Label lblHeader = new Label
            {
                Text = "📅  Local Events & Announcements",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true, Top = 16, Left = 20
            };
            header.Controls.Add(lblHeader);

            // ── Search bar ────────────────────────────────────────
            Label lblSearch = MakeLabel("Search (category / keyword):", 20, 80);
            txtSearch = new TextBox
            {
                Left = 20, Top = 100, Width = 330,
                Font = new Font("Segoe UI", 10)
            };
            Button btnSearch = MakeButton("🔍 Search", 360, 97, 90, Color.FromArgb(0, 84, 166));
            Button btnClear  = MakeButton("✖ Clear",   460, 97, 80, Color.FromArgb(100, 100, 120));

            // ── Events list ───────────────────────────────────────
            Label lblEvents = MakeLabel("Events (sorted by date):", 20, 135);
            lstEvents = new ListBox
            {
                Left = 20, Top = 155, Width = 540, Height = 330,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            // ── Recommendations panel ─────────────────────────────
            Label lblRec = MakeLabel("Recommendations:", 580, 135);
            lstRecommendations = new ListBox
            {
                Left = 580, Top = 155, Width = 340, Height = 330,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 248, 255)
            };

            // ── Info / data-structure buttons ─────────────────────
            Button btnQueue   = MakeButton("Queue View",    20,  500, 130, Color.FromArgb(136, 0, 27));
            Button btnHistory = MakeButton("Search History",160, 500, 140, Color.FromArgb(136, 0, 27));
            Button btnCats    = MakeButton("Categories",    310, 500, 120, Color.FromArgb(136, 0, 27));
            Button btnSorted  = MakeButton("Sorted View",   440, 500, 120, Color.FromArgb(136, 0, 27));

            // ── Add event panel ───────────────────────────────────
            Label lblAdd = MakeLabel("Add Custom Event:", 20, 555);

            TextBox txtTitle = new TextBox
            {
                Left = 20,
                Top = 578,
                Width = 200,
                Height = 28,
                Font = new Font("Segoe UI", 9)
            };

            TextBox txtCat = new TextBox
            {
                Left = 230,
                Top = 578,
                Width = 140,
                Font = new Font("Segoe UI", 9)
            };

            DateTimePicker dtp = new DateTimePicker
            {
                Left = 380,
                Top = 578,
                Width = 135,
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short
            };

            Button btnAdd = MakeButton("➕ Add", 545, 573, 95, Color.FromArgb(16, 137, 62));
            // ── Info listbox ──────────────────────────────────────
            lstInfo = new ListBox
            {
                Left = 580,
                Top = 500,
                Width = 340,
                Height = 90, // antes 110
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 250, 255)
            };
            // ── Back button ───────────────────────────────────────
            Button btnBack = MakeButton("← Back to Menu", 20, 615, 155, Color.FromArgb(60, 60, 80));
            // ── Wire events ───────────────────────────────────────
            btnSearch.Click += (s, e) => DoSearch(txtSearch.Text.Trim());
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) DoSearch(txtSearch.Text.Trim()); };
            btnClear.Click  += (s, e) => { txtSearch.Clear(); LoadAllEvents(); lstRecommendations.Items.Clear(); };

            btnQueue.Click   += (s, e) => ShowQueue();
            btnHistory.Click += (s, e) => ShowHistory();
            btnCats.Click    += (s, e) => ShowCategories();
            btnSorted.Click  += (s, e) => LoadAllEvents();

            btnAdd.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtCat.Text))
                { MessageBox.Show("Please enter a title and category.", "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                var ev = new EventItem { Title = txtTitle.Text.Trim(), Category = txtCat.Text.Trim(), Date = dtp.Value.Date };
                AddEvent(ev);
                txtTitle.Clear(); txtCat.Clear();
                LoadAllEvents();
                MessageBox.Show($"Event '{ev.Title}' added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnBack.Click += (s, e) => Close();

            // ── Placeholder hints ─────────────────────────────────
            txtTitle.Text = "Event title…"; txtTitle.ForeColor = Color.Gray;
            txtTitle.GotFocus += (s, e) => { if (txtTitle.Text == "Event title…") { txtTitle.Clear(); txtTitle.ForeColor = Color.Black; } };
            txtTitle.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtTitle.Text)) { txtTitle.Text = "Event title…"; txtTitle.ForeColor = Color.Gray; } };
            txtCat.Text = "Category…"; txtCat.ForeColor = Color.Gray;
            txtCat.GotFocus += (s, e) => { if (txtCat.Text == "Category…") { txtCat.Clear(); txtCat.ForeColor = Color.Black; } };
            txtCat.LostFocus += (s, e) => { if (string.IsNullOrWhiteSpace(txtCat.Text)) { txtCat.Text = "Category…"; txtCat.ForeColor = Color.Gray; } };

            Controls.AddRange(new Control[]
            {
                header,
                lblSearch, txtSearch, btnSearch, btnClear,
                lblEvents, lstEvents,
                lblRec, lstRecommendations,
                btnQueue, btnHistory, btnCats, btnSorted,
                lblAdd, txtTitle, txtCat, dtp, btnAdd,
                lstInfo,
                btnBack
            });

            LoadAllEvents();
        }

        // ── Seed sample events ────────────────────────────────────
        private void SeedSampleEvents()
        {
            var samples = new List<EventItem>
            {
                new EventItem { Title="Community Clean-Up Day",        Category="Environment",  Date=DateTime.Today.AddDays(3)  },
                new EventItem { Title="Town Hall Meeting",              Category="Governance",   Date=DateTime.Today.AddDays(7)  },
                new EventItem { Title="Youth Sports Day",               Category="Sports",       Date=DateTime.Today.AddDays(10) },
                new EventItem { Title="Road Repair Notice — Main St",   Category="Roads",        Date=DateTime.Today.AddDays(1)  },
                new EventItem { Title="Water Shutdown Notice",          Category="Utilities",    Date=DateTime.Today.AddDays(2)  },
                new EventItem { Title="Health Screening Drive",         Category="Health",       Date=DateTime.Today.AddDays(5)  },
                new EventItem { Title="Ward Committee Meeting",         Category="Governance",   Date=DateTime.Today.AddDays(14) },
                new EventItem { Title="Pothole Repair — Church Road",   Category="Roads",        Date=DateTime.Today.AddDays(4)  },
                new EventItem { Title="Electricity Maintenance Outage", Category="Utilities",    Date=DateTime.Today.AddDays(6)  },
                new EventItem { Title="Library Reading Programme",      Category="Education",    Date=DateTime.Today.AddDays(8)  },
            };
            foreach (var ev in samples) AddEvent(ev);
        }

        private void AddEvent(EventItem ev)
        {
            if (!sortedEvents.ContainsKey(ev.Date))
                sortedEvents[ev.Date] = new List<EventItem>();
            sortedEvents[ev.Date].Add(ev);
            eventQueue.Enqueue(ev);
            categories.Add(ev.Category);
        }

        // ── Load all events sorted by date ────────────────────────
        private void LoadAllEvents()
        {
            lstEvents.Items.Clear();
            lstEvents.Items.Add("  Date              Category          Title");
            lstEvents.Items.Add("  ─────────────────────────────────────────────────────");
            foreach (var kvp in sortedEvents)
                foreach (var ev in kvp.Value)
                    lstEvents.Items.Add($"  {ev.Date:dd MMM yyyy}   {ev.Category,-18} {ev.Title}");
        }

        // ── Search & recommend ────────────────────────────────────
        private void DoSearch(string query)
        {
            if (string.IsNullOrEmpty(query)) return;

            // Track search pattern
            searchHistory.Push(query);
            string key = query.ToLower();
            if (!searchCount.ContainsKey(key)) searchCount[key] = 0;
            searchCount[key]++;

            // Filter
            lstEvents.Items.Clear();
            lstEvents.Items.Add($"  Results for: \"{query}\"");
            lstEvents.Items.Add("  ─────────────────────────────────────────────────────");
            bool found = false;
            foreach (var kvp in sortedEvents)
                foreach (var ev in kvp.Value)
                    if (ev.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                     || ev.Category.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        lstEvents.Items.Add($"  {ev.Date:dd MMM yyyy}   {ev.Category,-18} {ev.Title}");
                        found = true;
                    }
            if (!found) lstEvents.Items.Add("  No matching events found.");

            // Recommendations based on frequency of past searches
            BuildRecommendations(key);
        }

        private void BuildRecommendations(string lastKey)
        {
            lstRecommendations.Items.Clear();
            lstRecommendations.Items.Add("── Based on your searches ──");

            // Find all events whose category contains any frequently searched keyword
            var topTerms = searchCount.OrderByDescending(kv => kv.Value).Take(3).Select(kv => kv.Key).ToList();

            var recommended = new HashSet<string>();
            foreach (var term in topTerms)
                foreach (var kvp in sortedEvents)
                    foreach (var ev in kvp.Value)
                        if ((ev.Category.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0
                          || ev.Title.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                          && !recommended.Contains(ev.Title))
                        {
                            lstRecommendations.Items.Add($"• {ev.Title}");
                            recommended.Add(ev.Title);
                        }

            if (lstRecommendations.Items.Count == 1)
                lstRecommendations.Items.Add("No recommendations yet.");
        }

        private void ShowQueue()
        {
            lstInfo.Items.Clear();
            lstInfo.Items.Add("── Event Queue (FIFO) ──");
            foreach (var ev in eventQueue)
                lstInfo.Items.Add($"  {ev.Date:dd MMM}  {ev.Category}  {ev.Title}");
        }

        private void ShowHistory()
        {
            lstInfo.Items.Clear();
            lstInfo.Items.Add("── Search History (LIFO) ──");
            if (searchHistory.Count == 0) { lstInfo.Items.Add("  No searches yet."); return; }
            foreach (var s in searchHistory)
                lstInfo.Items.Add($"  > {s}");
        }

        private void ShowCategories()
        {
            lstInfo.Items.Clear();
            lstInfo.Items.Add("── Unique Categories (HashSet) ──");
            foreach (var c in categories.OrderBy(x => x))
                lstInfo.Items.Add($"  • {c}");
        }

        private Label MakeLabel(string text, int left, int top)
        {
            var lbl = new Label
            {
                Text = text, Left = left, Top = top, AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 60, 80)
            };
            Controls.Add(lbl);
            return lbl;
        }

        private Button MakeButton(string text, int left, int top, int width, Color colour)
        {
            var btn = new Button
            {
                Text = text, Left = left, Top = top,
                Width = width, Height = 32,
                BackColor = colour, ForeColor = Color.White,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
