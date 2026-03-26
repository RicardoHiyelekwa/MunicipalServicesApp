using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class IssueForm : Form
    {
        // List to store all submitted issues
        private static List<IssueReport> issues = new List<IssueReport>();

        TextBox txtLocation;
        ComboBox cmbCategory;
        RichTextBox txtDescription;
        Label lblAttachment;
        Label lblEncourage;
        ProgressBar progress;

        string attachment = "";

        public IssueForm()
        {
            Text = "Report Municipal Issue";
            Width = 850;
            Height = 650;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // Header
            Label header = new Label
            {
                Text = "Submit Municipal Service Issue",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Top = 20,
                Left = 220,
                AutoSize = true
            };

            // Location
            Label l1 = new Label { Text = "Location", Top = 80, Left = 50 };
            txtLocation = new TextBox { Top = 105, Left = 50, Width = 300 };

            // Category
            Label l2 = new Label { Text = "Category", Top = 150, Left = 50 };
            cmbCategory = new ComboBox
            {
                Top = 175,
                Left = 50,
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbCategory.Items.AddRange(new string[]
            {
                "Sanitation",
                "Roads",
                "Utilities",
                "Electricity",
                "Water Leakage"
            });

            // Description
            Label l3 = new Label { Text = "Description", Top = 220, Left = 50 };
            txtDescription = new RichTextBox
            {
                Top = 245,
                Left = 50,
                Width = 500,
                Height = 120
            };

            // Attach button
            Button btnAttach = new Button
            {
                Text = "Attach Image / Document",
                Top = 390,
                Left = 50,
                Width = 180,
                Height = 35,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };

            lblAttachment = new Label
            {
                Text = "No file selected",
                Top = 398,
                Left = 250,
                Width = 500
            };

            // Progress bar (Engagement Feature)
            progress = new ProgressBar
            {
                Top = 450,
                Left = 50,
                Width = 500,
                Height = 25,
                Maximum = 100,
                Style = ProgressBarStyle.Continuous
            };

            // Encouragement label
            lblEncourage = new Label
            {
                Text = "Start filling the form to help your municipality.",
                Top = 485,
                Left = 50,
                Width = 500,
                ForeColor = Color.DarkGreen
            };

            // Submit button
            Button btnSubmit = new Button
            {
                Text = "Submit Issue",
                Top = 530,
                Left = 50,
                Width = 150,
                Height = 40,
                BackColor = Color.Green,
                ForeColor = Color.White
            };

            // View Issues button
            Button btnView = new Button
            {
                Text = "View Submitted Issues",
                Top = 530,
                Left = 220,
                Width = 170,
                Height = 40
            };

            // Back button
            Button btnBack = new Button
            {
                Text = "Back",
                Top = 530,
                Left = 410,
                Width = 100,
                Height = 40
            };

            // Events
            txtLocation.TextChanged += UpdateProgress;
            cmbCategory.SelectedIndexChanged += UpdateProgress;
            txtDescription.TextChanged += UpdateProgress;

            btnAttach.Click += AttachFile;
            btnSubmit.Click += SubmitIssue;
            btnView.Click += ViewIssues;
            btnBack.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                header,l1,txtLocation,l2,cmbCategory,l3,txtDescription,
                btnAttach,lblAttachment,progress,lblEncourage,
                btnSubmit,btnView,btnBack
            });
        }

        // Updates progress bar based on user input
        private void UpdateProgress(object sender, EventArgs e)
        {
            int value = 0;

            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) value += 25;
            if (cmbCategory.SelectedIndex >= 0) value += 25;
            if (!string.IsNullOrWhiteSpace(txtDescription.Text)) value += 25;
            if (!string.IsNullOrWhiteSpace(attachment)) value += 25;

            progress.Value = value;

            if (value == 100)
                lblEncourage.Text = "Excellent! Your report is ready for submission.";
            else if (value >= 75)
                lblEncourage.Text = "Almost done — complete remaining fields.";
            else
                lblEncourage.Text = $"Progress: {value}%";
        }

        // Attach file using OpenFileDialog
        private void AttachFile(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog
            {
                Filter = "All Files|*.*"
            };

            if (open.ShowDialog() == DialogResult.OK)
            {
                // Optional file size validation (max 5MB)
                if (new FileInfo(open.FileName).Length > 5000000)
                {
                    MessageBox.Show("File too large (max 5MB).");
                    return;
                }

                attachment = open.FileName;
                lblAttachment.Text = Path.GetFileName(attachment);
                UpdateProgress(null, null);
            }
        }

        // Submit issue with validation
        private void SubmitIssue(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text) ||
                cmbCategory.SelectedIndex < 0 ||
                string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please complete all required fields.", "Validation Error");
                return;
            }

            var issue = new IssueReport
            {
                ReferenceId = "MSA-" + DateTime.Now.Ticks.ToString().Substring(10),
                Location = txtLocation.Text,
                Category = cmbCategory.Text,
                Description = txtDescription.Text,
                AttachmentPath = attachment,
                SubmittedAt = DateTime.Now
            };

            issues.Add(issue);

            MessageBox.Show($"Issue submitted successfully!\nReference: {issue.ReferenceId}");

            // Reset form
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            txtDescription.Clear();
            attachment = "";
            lblAttachment.Text = "No file selected";
            progress.Value = 0;
            lblEncourage.Text = "Ready for a new report!";
        }

        // Display submitted issues
        private void ViewIssues(object sender, EventArgs e)
        {
            Form listForm = new Form
            {
                Text = "Submitted Issues",
                Width = 900,
                Height = 400
            };

            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                DataSource = issues
            };

            listForm.Controls.Add(grid);
            listForm.ShowDialog();
        }
    }
}
