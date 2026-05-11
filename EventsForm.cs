using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public partial class EventsForm : Form
    {
        // ── Data structures ─────────────────────────────────────────
        private List<EventItem> events                            = new List<EventItem>();
        private Queue<EventItem> eventQueue                       = new Queue<EventItem>();
        private Stack<string> searchHistory                       = new Stack<string>();
        private HashSet<string> uniqueCategories                  = new HashSet<string>();
        private SortedDictionary<DateTime, EventItem> sortedEvents = new SortedDictionary<DateTime, EventItem>();

        // ── UI controls ─────────────────────────────────────────────
        private ListBox lstEvents;
        private ListBox lstRecommendations;
        private TextBox txtSearch;
        private Label lblEventCount;

        public EventsForm()
        {
            Text = "Local Events & Announcements";
            Size = new Size(950, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ── Header ──────────────────────────────────────────────
            Panel header = new Panel
            {
                Dock = DockStyle.Top, Height = 70,
                BackColor = Color.FromArgb(16, 137, 62)
            };
            Label lblHead = new Label
            {
                Text = "📅  Local Events & Announcements",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true, Top = 18, Left = 20
            };
            header.Controls.Add(lblHead);

            // ── Search bar ──────────────────────────────────────────
            Label lblSearch = MakeLabel("Search by category or title:", 20, 90);
            txtSearch = new TextBox
            {
                Top = 113, Left = 20, Width = 350, Height = 30,
                Font = new Font("Segoe UI", 10)
            };
            SetPlaceholder(txtSearch, "e.g. Roads, Cleanup...");
            Button btnSearch = MakeButton("🔍  Search", 385, 110, 120, Color.FromArgb(16, 137, 62));
            Button btnClear  = MakeButton("✖  Clear",   520, 110, 100, Color.FromArgb(180, 180, 190));

            lblEventCount = new Label
            {
                Top = 118, Left = 640, Width = 280, AutoSize = false,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 100)
            };

            // ── Main list ───────────────────────────────────────────
            Label lblEvents = MakeLabel("Events (sorted by date):", 20, 152);
            lstEvents = new ListBox
            {
                Top = 175, Left = 20, Width = 540, Height = 270,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                HorizontalScrollbar = true
            };

            // ── Recommendations ─────────────────────────────────────
            Label lblRec = MakeLabel("Recommended Events:", 580, 152);
            lstRecommendations = new ListBox
            {
                Top = 175, Left = 580, Width = 340, Height = 270,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.FromArgb(0, 84, 166),
                HorizontalScrollbar = true
            };

            // ── Action buttons row ──────────────────────────────────
            Label lblActions = MakeLabel("Data Structure Views:", 20, 462);

            Button btnQueue      = MakeButton("📋 Queue",          20,  487, 130, Color.FromArgb(0, 84, 166));
            Button btnHistory    = MakeButton("🕐 Search History", 165, 487, 150, Color.FromArgb(90, 60, 150));
            Button btnCategories = MakeButton("🏷 Categories",     330, 487, 130, Color.FromArgb(180, 100, 0));
            Button btnSorted     = MakeButton("📆 Sorted View",    475, 487, 140, Color.FromArgb(136, 0, 27));

            // ── Add Event section ───────────────────────────────────
            Panel addPanel = new Panel
            {
                Top = 540, Left = 20, Width = 900, Height = 85,
                BackColor = Color.FromArgb(240, 248, 240),
                BorderStyle = BorderStyle.FixedSingle
            };
            Label lblAdd = MakeLabel("Add New Event:", 10, 8);
            lblAdd.ForeColor = Color.FromArgb(16, 137, 62);

            TextBox txtTitle    = new TextBox { Top = 30, Left = 10,  Width = 200 };
            TextBox txtCat      = new TextBox { Top = 30, Left = 225, Width = 180 };
            SetPlaceholder(txtTitle, "Event title");
            SetPlaceholder(txtCat,   "Category");
            DateTimePicker dtp  = new DateTimePicker { Top = 30, Left = 420, Width = 160, Format = DateTimePickerFormat.Short };
            Button btnAdd       = MakeButton("+ Add", 598, 28, 80, Color.FromArgb(16, 137, 62));
            btnAdd.Height = 26;

            addPanel.Controls.AddRange(new Control[] { lblAdd, txtTitle, txtCat, dtp, btnAdd });

            Button btnBack = MakeButton("← Back to Menu", 20, 637, 160, Color.FromArgb(60, 60, 80));

            // ── Wire events ─────────────────────────────────────────
            btnSearch.Click += (s, e) => Search(txtSearch.Text);
            btnClear.Click  += (s, e) => { txtSearch.Clear(); LoadAllEvents(); };

            btnQueue.Click += (s, e) =>
            {
                lstEvents.Items.Clear();
                lstEvents.Items.Add("── Event Queue (FIFO order) ──");
                foreach (var ev in eventQueue)
                    lstEvents.Items.Add($"  ▶ {ev.Title}  [{ev.Category}]  {ev.Date:dd MMM yyyy}");
            };

            btnHistory.Click += (s, e) =>
            {
                lstEvents.Items.Clear();
                lstEvents.Items.Add("── Search History (most recent first) ──");
                if (searchHistory.Count == 0) { lstEvents.Items.Add("  No searches yet."); return; }
                foreach (var item in searchHistory)
                    lstEvents.Items.Add($"  🔍 \"{item}\"");
            };

            btnCategories.Click += (s, e) =>
            {
                lstEvents.Items.Clear();
                lstEvents.Items.Add("── Unique Categories (HashSet) ──");
                foreach (var cat in uniqueCategories.OrderBy(c => c))
                    lstEvents.Items.Add($"  🏷 {cat}");
            };

            btnSorted.Click += (s, e) =>
            {
                lstEvents.Items.Clear();
                lstEvents.Items.Add("── Sorted by Date (SortedDictionary) ──");
                foreach (var kvp in sortedEvents)
                    lstEvents.Items.Add($"  {kvp.Key:dd MMM yyyy}  —  {kvp.Value.Title}  [{kvp.Value.Category}]");
            };

            btnAdd.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtCat.Text))
                { MessageBox.Show("Please enter a title and category.", "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                AddEvent(txtTitle.Text.Trim(), txtCat.Text.Trim(), dtp.Value.Date);
                txtTitle.Clear(); txtCat.Clear();
                LoadAllEvents();
                MessageBox.Show("Event added successfully.", "Event Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnBack.Click += (s, e) => Close();

            // ── Load sample data ────────────────────────────────────
            LoadSampleData();
            LoadAllEvents();

            Controls.AddRange(new Control[]
            {
                header,
                lblSearch, txtSearch, btnSearch, btnClear, lblEventCount,
                lblEvents, lstEvents,
                lblRec, lstRecommendations,
                lblActions, btnQueue, btnHistory, btnCategories, btnSorted,
                addPanel,
                btnBack
            });
        }

        // ── Helpers ─────────────────────────────────────────────────
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

        // ── Win32 cue banner (PlaceholderText for .NET Framework 4.8) ──
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wp, string lp);
        private const uint EM_SETCUEBANNER = 0x1501;
        private static void SetPlaceholder(TextBox tb, string text)
        {
            tb.HandleCreated += (s, e) => SendMessage(tb.Handle, EM_SETCUEBANNER, (IntPtr)1, text);
        }

        // ── Data operations ─────────────────────────────────────────
        private void AddEvent(string title, string category, DateTime date)
        {
            // Avoid duplicate dates in SortedDictionary by adding seconds offset
            while (sortedEvents.ContainsKey(date))
                date = date.AddSeconds(1);

            var ev = new EventItem { Title = title, Category = category, Date = date };
            events.Add(ev);
            eventQueue.Enqueue(ev);
            uniqueCategories.Add(category);
            sortedEvents[date] = ev;
        }

        private void LoadSampleData()
        {
            AddEvent("Community Cleanup Drive",        "Sanitation",          DateTime.Now.AddDays(2));
            AddEvent("Pothole Repair — Main Road",     "Roads",               DateTime.Now.AddDays(3));
            AddEvent("Water Supply Maintenance",       "Water",               DateTime.Now.AddDays(5));
            AddEvent("Electricity Grid Upgrade",       "Electricity",         DateTime.Now.AddDays(7));
            AddEvent("Public Park Renovation",         "Parks & Recreation",  DateTime.Now.AddDays(10));
            AddEvent("Waste Removal Schedule Update",  "Waste Removal",       DateTime.Now.AddDays(1));
            AddEvent("Street Lighting Installation",   "Public Lighting",     DateTime.Now.AddDays(4));
            AddEvent("Town Hall Meeting",              "Community",           DateTime.Now.AddDays(6));
        }

        private void LoadAllEvents()
        {
            lstEvents.Items.Clear();
            lstEvents.Items.Add("── All Events (sorted by date) ──");
            foreach (var kvp in sortedEvents)
                lstEvents.Items.Add($"  {kvp.Key:dd MMM yyyy}  —  {kvp.Value.Title}  [{kvp.Value.Category}]");
            lblEventCount.Text = $"Total events: {events.Count}  |  Categories: {uniqueCategories.Count}";
        }

        private void Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) { LoadAllEvents(); return; }

            string q = query.Trim().ToLower();
            searchHistory.Push(q);

            lstEvents.Items.Clear();
            lstEvents.Items.Add($"── Results for \"{q}\" ──");

            var results = events.Where(ev =>
                ev.Category.ToLower().Contains(q) ||
                ev.Title.ToLower().Contains(q)).ToList();

            if (results.Count == 0)
                lstEvents.Items.Add("  No matching events found.");
            else
                foreach (var ev in results.OrderBy(ev => ev.Date))
                    lstEvents.Items.Add($"  {ev.Date:dd MMM yyyy}  —  {ev.Title}  [{ev.Category}]");

            lblEventCount.Text = $"{results.Count} result(s) found  |  Search history: {searchHistory.Count} item(s)";

            Recommend(q, results);
        }

        // ── Recommendation algorithm ────────────────────────────────
        // Content-based filtering: matches by category AND title;
        // weighted so category matches score higher than title-only matches.
        private void Recommend(string query, List<EventItem> searchResults)
        {
            lstRecommendations.Items.Clear();

            var categoryMatches = events
                .Where(ev => ev.Category.ToLower().Contains(query) &&
                             !searchResults.Any(r => r.Title == ev.Title))
                .ToList();

            var titleMatches = events
                .Where(ev => ev.Title.ToLower().Contains(query) &&
                             !searchResults.Any(r => r.Title == ev.Title) &&
                             !categoryMatches.Any(c => c.Title == ev.Title))
                .ToList();

            // Also recommend events in the same categories as results
            var sameCategory = events
                .Where(ev => searchResults.Any(r => r.Category == ev.Category) &&
                             !searchResults.Any(r => r.Title == ev.Title) &&
                             !categoryMatches.Any(c => c.Title == ev.Title))
                .ToList();

            if (!categoryMatches.Any() && !titleMatches.Any() && !sameCategory.Any())
            {
                lstRecommendations.Items.Add("No recommendations found.");
                return;
            }

            if (categoryMatches.Any())
            {
                lstRecommendations.Items.Add("── Category matches ──");
                foreach (var ev in categoryMatches)
                    lstRecommendations.Items.Add($"★ {ev.Title} [{ev.Category}]");
            }
            if (sameCategory.Any())
            {
                lstRecommendations.Items.Add("── Related events ──");
                foreach (var ev in sameCategory)
                    lstRecommendations.Items.Add($"◆ {ev.Title} [{ev.Category}]");
            }
            if (titleMatches.Any())
            {
                lstRecommendations.Items.Add("── Title matches ──");
                foreach (var ev in titleMatches)
                    lstRecommendations.Items.Add($"◇ {ev.Title} [{ev.Category}]");
            }
        }
    }
}
