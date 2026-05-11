using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class IssueForm : Form
    {
        private TextBox txtLocation;
        private ComboBox cmbCategory;
        private RichTextBox rtbDescription;
        private Label lblAttachment;
        private ProgressBar progressBar;
        private Label lblProgress;
        private string attachedFilePath = "";

        public IssueForm()
        {
            Text = "Report a Municipal Issue";
            // 620 wide x 620 tall — all controls fit with 20px margin
            Size = new Size(620, 560);
            MinimumSize = new Size(620, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ── Header ──────────────────────────────────────────
            Panel header = new Panel
            {
                Dock = DockStyle.Top, Height = 65,
                BackColor = Color.FromArgb(136, 0, 27)
            };
            Label lblHeader = new Label
            {
                Text = "📋  Report an Issue",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true, Top = 16, Left = 20
            };
            header.Controls.Add(lblHeader);

            int y = 80;  // current vertical position

            // ── Location ─────────────────────────────────────────
            AddLabel("Location:", 20, y); y += 22;
            txtLocation = new TextBox
            {
                Left = 20, Top = y, Width = 560,
                Font = new Font("Segoe UI", 10), Height = 28
            };
            txtLocation.TextChanged += (s, e) => UpdateProgress();
            y += 38;

            // ── Category ─────────────────────────────────────────
            AddLabel("Category:", 20, y); y += 22;
            cmbCategory = new ComboBox
            {
                Left = 20, Top = y, Width = 560,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCategory.Items.AddRange(new object[]
            {
                "Sanitation", "Roads & Potholes", "Electricity / Utilities",
                "Water & Sewage", "Waste Collection", "Public Safety",
                "Parks & Recreation", "Other"
            });
            cmbCategory.SelectedIndexChanged += (s, e) => UpdateProgress();
            y += 38;

            // ── Description ───────────────────────────────────────
            AddLabel("Description:", 20, y); y += 22;
            rtbDescription = new RichTextBox
            {
                Left = 20, Top = y, Width = 560, Height = 110,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            rtbDescription.TextChanged += (s, e) => UpdateProgress();
            y += 120;

            // ── Media Attachment ──────────────────────────────────
            AddLabel("Attachment:", 20, y); y += 22;
            Button btnAttach = MakeButton("📎  Attach File", 20, y, 130, Color.FromArgb(0, 84, 166));
            lblAttachment = new Label
            {
                Left = 165, Top = y + 5, Width = 415, Height = 24,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Text = "No file attached"
            };
            btnAttach.Click += BtnAttach_Click;
            y += 42;

            // ── Progress bar ──────────────────────────────────────
            lblProgress = new Label
            {
                Left = 20, Top = y, Width = 560, Height = 20,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(136, 0, 27),
                Text = "Fill in the form to submit your report."
            };
            y += 22;
            progressBar = new ProgressBar
            {
                Left = 20, Top = y, Width = 560, Height = 22,
                Minimum = 0, Maximum = 100, Value = 0,
                Style = ProgressBarStyle.Continuous
            };
            y += 36;

            // ── Buttons ───────────────────────────────────────────
            Button btnSubmit = MakeButton("✔  Submit Report", 20,   y, 160, Color.FromArgb(16, 137, 62));
            Button btnBack   = MakeButton("← Back",          195,  y, 100, Color.FromArgb(100, 100, 120));

            btnSubmit.Click += BtnSubmit_Click;
            btnBack.Click   += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                header,
                txtLocation, cmbCategory, rtbDescription,
                btnAttach, lblAttachment,
                lblProgress, progressBar,
                btnSubmit, btnBack
            });
        }

        private Label AddLabel(string text, int left, int top)
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
                Width = width, Height = 36,
                BackColor = colour, ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void UpdateProgress()
        {
            int score = 0;
            string msg;

            if (!string.IsNullOrWhiteSpace(txtLocation.Text))    score += 34;
            if (cmbCategory.SelectedIndex >= 0)                   score += 33;
            if (rtbDescription.Text.Trim().Length >= 10)          score += 33;

            progressBar.Value = score;

            if (score == 0)       msg = "Fill in the form to submit your report.";
            else if (score <= 33) msg = "Good start — keep going! 💪";
            else if (score <= 66) msg = "Almost there — add a description! ✏️";
            else if (score < 100) msg = "Looking good — ready to submit! 🎯";
            else                  msg = "Report complete — click Submit! ✅";

            lblProgress.Text = msg;
            lblProgress.ForeColor = score == 100
                ? Color.FromArgb(16, 137, 62)
                : Color.FromArgb(136, 0, 27);
        }

        private void BtnAttach_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Attach Image or Document";
                ofd.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Documents|*.pdf;*.doc;*.docx;*.txt|All Files|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var info = new FileInfo(ofd.FileName);
                    if (info.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show("File exceeds 5 MB limit. Please choose a smaller file.",
                            "File Too Large", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    attachedFilePath = ofd.FileName;
                    lblAttachment.Text = info.Name;
                    lblAttachment.ForeColor = Color.FromArgb(16, 137, 62);
                }
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            { MessageBox.Show("Please enter a location.", "Missing Field", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cmbCategory.SelectedIndex < 0)
            { MessageBox.Show("Please select a category.", "Missing Field", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (rtbDescription.Text.Trim().Length < 10)
            { MessageBox.Show("Please provide a description (at least 10 characters).", "Missing Field", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string refId = "MSA-" + new Random().Next(100000000, 999999999).ToString();
            int priority  = new Random().Next(1, 6);

            var issue = new IssueReport
            {
                ReferenceId    = refId,
                Location       = txtLocation.Text.Trim(),
                Category       = cmbCategory.SelectedItem.ToString(),
                Description    = rtbDescription.Text.Trim(),
                AttachmentPath = attachedFilePath,
                SubmittedAt    = DateTime.Now,
                Status         = "Pending",
                Priority       = priority
            };

            // Add to shared data
            AppData.Issues.Add(issue);
            AppData.Tree.Insert(issue);
            AppData.PriorityQueue.Enqueue(issue);

            // Add workflow graph edges for this issue
            AppData.Graph.AddEdge(refId, "Assigned");
            AppData.Graph.AddEdge("Assigned", "In Progress");
            AppData.Graph.AddEdge("In Progress", "Resolved");

            MessageBox.Show(
                $"✅  Issue submitted successfully!\n\nReference ID: {refId}\nPriority Level: {priority}/5\n\nUse this ID to track your request.",
                "Report Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }
    }
}
