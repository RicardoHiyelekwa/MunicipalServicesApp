# Municipal Services Application — South Africa
### AAPD7112/w | PROG7312 | IIE Rosebank College | 2026

---

## Overview

A C# .NET Framework 4.8 Windows Forms application that provides South African
citizens with a unified platform to:

- **Report municipal issues** (potholes, sanitation, electricity failures, etc.)
- **Browse local events and announcements** with search and smart recommendations
- **Track service request status** using advanced data structures

---

## Prerequisites

| Requirement          | Version         |
|----------------------|-----------------|
| Operating System     | Windows 10/11   |
| .NET Framework       | 4.8             |
| Visual Studio        | 2019 or later   |
| SDK                  | .NET Desktop SDK (included with VS) |

---

## How to Compile

### Option A — Visual Studio (recommended)

1. Open **Visual Studio 2019** or later.
2. Click **File → Open → Project/Solution**.
3. Navigate to the project folder and open `MunicipalServicesApp.sln`.
4. In the **Solution Explorer**, right-click the project → **Build**.
5. Confirm there are no build errors in the **Output** window.

### Option B — Command Line (MSBuild)

```bash
# Open Developer Command Prompt for VS
cd path\to\MunicipalServicesApp
dotnet build MunicipalServicesApp.csproj -c Debug
```

---

## How to Run

### From Visual Studio
Press **F5** (Debug) or **Ctrl+F5** (Run without debugging).

### From the compiled executable
```
MunicipalServicesApp\bin\Debug\net48\MunicipalServicesApp.exe
```

---

## How to Use

### 1. Main Menu
On startup the dashboard presents three options:

| Button                        | Feature                      | Status    |
|-------------------------------|------------------------------|-----------|
| Report Issues                 | Submit a new issue report    | ✔ Active  |
| Local Events & Announcements  | Browse and search events     | ✔ Active  |
| Track Service Requests        | View and search requests     | ✔ Active  |

---

### 2. Report Issues

1. Enter the **location** of the issue (e.g. "12 Main Street, Soweto").
2. Select a **category** from the dropdown (Sanitation, Roads, Utilities, etc.).
3. Write a **description** of the problem in the text box.
4. Optionally click **Attach File** to upload an image or document (max 5 MB).
5. Watch the **progress bar** advance as you complete each field.
6. Click **Submit Report** — a reference ID (e.g. `MSA-7841029384`) is generated.
7. Click **Back** to return to the main menu.

> **Engagement Feature:** The progress bar advances 25% per completed field and
> displays encouraging messages, motivating users to submit complete reports.

---

### 3. Local Events & Announcements

- **Search** — type a category or keyword and click Search to filter events.
- **Clear** — resets to the full sorted event list.
- **Queue view** — shows events in FIFO processing order.
- **Search History** — shows your recent searches (Stack/LIFO).
- **Categories** — lists all unique categories (HashSet).
- **Sorted View** — displays all events sorted chronologically (SortedDictionary).
- **Add New Event** — enter a title, category, and date to add a custom event.
- **Recommendations panel** (right side) — suggests related events based on your search.

---

### 4. Track Service Requests

1. Submit at least one issue via **Report Issues** first.
2. Copy the Reference ID shown in the confirmation message.
3. Open **Track Service Requests**.
4. Paste the Reference ID in the search box and click **Search** (BST lookup).
5. The result shows Category, Status, Priority, and submission date.
6. Click **Load Priority Queue** to see all requests ordered by urgency.
7. Click **Show Workflow Flow** to see the BFS graph traversal of the service pipeline.
8. Click **↻ Refresh List** to update the data grid.

---

## Data Structures Used

| Structure              | Location                  | Purpose                                      |
|------------------------|---------------------------|----------------------------------------------|
| `List<IssueReport>`    | AppData.cs                | Primary storage of all submitted issues      |
| `BinarySearchTree`     | BinarySearchTree.cs       | Fast O(log n) lookup by Reference ID         |
| `PriorityQueueManager` | PriorityQueueManager.cs   | Max-heap ordering issues by priority         |
| `GraphManager`         | GraphManager.cs           | Directed graph + BFS for workflow traversal  |
| `Queue<EventItem>`     | EventsForm.cs             | FIFO event processing order                  |
| `Stack<string>`        | EventsForm.cs             | LIFO search history                          |
| `SortedDictionary`     | EventsForm.cs             | Automatic chronological event sorting        |
| `HashSet<string>`      | EventsForm.cs             | Unique category deduplication                |

---

## Project Structure

```
MunicipalServicesApp/
├── MainForm.cs              — Main menu dashboard
├── IssueForm.cs             — Report Issues form
├── EventsForm.cs            — Local Events & Announcements form
├── EventsForm.Designer.cs   — Designer stub
├── StatusForm.cs            — Service Request Status form
├── StatusForm.Designer.cs   — Designer stub
├── IssueReport.cs           — Issue data model
├── EventItem.cs             — Event data model
├── AppData.cs               — Shared static data store
├── BinarySearchTree.cs      — BST implementation
├── PriorityQueueManager.cs  — Priority queue (max-heap)
├── GraphManager.cs          — Adjacency list graph + BFS
├── Program.cs               — Application entry point
├── MunicipalServicesApp.csproj
├── MunicipalServicesApp.sln
└── README.md                — This file
```

---

## Troubleshooting

| Problem                          | Solution                                              |
|----------------------------------|-------------------------------------------------------|
| App does not compile             | Ensure .NET 8 is installed                            |
| "File too large" on attachment   | Select a file under 5 MB                             |
| Grid shows no data               | Submit an issue first via Report Issues               |
| BFS shows "Submit an issue first"| Submit at least one issue via Report Issues           |
| Build error: missing reference   | Right-click Solution → Restore NuGet Packages         |

---

## References

- Hart, T.G.B. et al., 2020. Innovation for development in South Africa. *Forum for Development Studies*, 47(1).
- Microsoft, 2024. Windows Forms documentation. https://learn.microsoft.com/en-us/dotnet/desktop/winforms
- Hamari, J. et al., 2014. Does gamification work? *HICSS*, pp.3025–3034.
