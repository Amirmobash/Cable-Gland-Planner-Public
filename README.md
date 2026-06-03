# Cable Gland Planner (WinForms)  
## AI-Assisted Engineering Layout, Feasibility Analysis & CNC G-Code Generation Amir Mobasheraghdam

Cable Gland Planner is a Windows desktop engineering application for designing, validating, visualising, and manufacturing cable gland drilling and pocket-milling layouts.  
The system is built around a **WinForms frontend** and an **AI-assisted Python Flask backend**, enabling automated feasibility checks, layout optimisation, insert handling, and CNC-ready **G-Code** generation.

The application is intended for engineering workflows where circular cable gland inserts must be positioned inside a circular disk, while respecting machining constraints such as hole diameter, tool diameter, edge clearance, inter-hole clearance, maximum depth, and non-overlap rules.

The UI is designed as a **German-only one-page workflow** with a consistent **orange/white industrial interface**, allowing operators to configure, validate, preview, and export manufacturing data from a single screen.

---

## 1. Project Purpose

The purpose of Cable Gland Planner is to convert real manufacturing requirements into a reliable and machine-usable output.

The application takes input such as:

- disk radius or disk diameter
- cable gland insert type
- number of holes
- hole diameters
- required edge clearance
- required clearance between holes
- tool diameter
- machining depth
- safe Z height
- feed rate and spindle options

and produces:

- a validated geometric hole layout
- AI-assisted feasibility diagnostics
- manufacturing warnings
- optimised or suggested hole positions
- CNC-ready `.nc` / G-Code output
- PNG layout preview for documentation
- deterministic fallback results when the AI backend is unavailable

---

## 2. System Overview

Cable Gland Planner consists of two main components:

```text
+-------------------------------------------------------------+
|                    WinForms Frontend (.NET 8)               |
|-------------------------------------------------------------|
| - German-only user interface                                |
| - One-page workflow                                         |
| - Parameter input                                           |
| - Layout preview                                            |
| - G-Code generation                                         |
| - PNG export                                                |
| - Offline fallback logic                                    |
+-----------------------------+-------------------------------+
                              |
                              | HTTP / JSON
                              v
+-------------------------------------------------------------+
|                  Python Flask AI Backend                    |
|-------------------------------------------------------------|
| - Feasibility analysis                                      |
| - Insert mapping                                            |
| - Constraint validation                                     |
| - Layout optimisation suggestions                           |
| - Diagnostic warnings                                       |
| - Confidence scoring                                        |
| - Engineering recommendations                               |
+-------------------------------------------------------------+
````

The frontend is responsible for user interaction, visualisation, exporting, and local deterministic safety checks.

The backend acts as an engineering intelligence layer. It analyses input constraints, detects problematic configurations, proposes layout improvements, and returns structured JSON responses to the frontend.

---

## 3. Main Engineering Capabilities

### 3.1 Circular Layout Planning

The core geometric problem is the placement of multiple circular holes inside a circular disk boundary.

Each hole must satisfy the following constraints:

```text
distance(hole_center, disk_center) + hole_radius <= disk_radius - edge_clearance
```

For every pair of holes:

```text
distance(center_i, center_j) >= radius_i + radius_j + hole_clearance
```

This ensures:

* no hole exceeds the disk boundary
* no two holes overlap
* minimum manufacturing clearance is respected
* unsafe high-density layouts are detected before CNC export

---

### 3.2 AI-Assisted Feasibility Analysis

The Flask AI backend evaluates whether the requested configuration is manufacturable.

The feasibility engine may use:

* deterministic geometric checks
* circle packing heuristics
* density estimation
* boundary clearance validation
* pairwise collision analysis
* rule-based engineering constraints
* optional optimisation algorithms

Typical feasibility output includes:

```json
{
  "feasible": true,
  "confidence": 0.92,
  "warnings": [],
  "min_hole_distance": 13.0,
  "recommended_edge_clearance": 5.0,
  "recommended_hole_clearance": 3.0
}
```

If the layout is close to the feasibility limit, the backend can return warnings such as:

* hole density too high
* edge clearance too small
* selected tool diameter unsuitable
* requested depth exceeds allowed limit
* hole spacing may cause weak material bridges
* layout optimisation recommended

---

### 3.3 AI-Driven Layout Suggestion

The AI backend can generate or improve initial hole positions.

The suggested layout may be based on:

* radial distribution
* angular balancing
* equal spacing heuristics
* greedy placement
* iterative collision relaxation
* simulated annealing
* genetic optimisation
* constraint solver output

Example response:

```json
{
  "positions": [
    [12.5, 20.0],
    [-14.0, 18.5],
    [0.0, -22.0]
  ],
  "confidence": 0.89,
  "warnings": [
    "Layout is feasible but close to minimum clearance threshold."
  ]
}
```

The frontend visualises these positions and allows the user to review the result before exporting CNC data.

---

## 4. AI-First Insert Handling

Cable gland insert definitions are managed by the backend instead of exposing raw JSON or configuration files to the user.

The operator selects an insert type in the frontend.
The AI backend resolves the required manufacturing parameters internally, such as:

* insert identifier
* POS mapping
* pocket depth
* diameter class
* tool compatibility
* allowed machining depth
* safety limits
* recommended machining strategy

This design keeps the frontend simple and prevents operators from manually editing sensitive technical configuration files.

---

## 5. CNC G-Code Generation

The application generates CNC-ready G-Code for drilling or pocket milling.

The output can include:

* absolute positioning mode
* safe Z movement
* XY positioning
* plunge movement
* pocket milling strategy
* retract movement
* spindle start and stop commands
* feed rate configuration
* machining depth limits
* structured comments for readability

Typical G-Code structure:

```gcode
%
(Generated by Cable Gland Planner)
(German UI / AI-assisted layout validation)

G90
G21
G17

(Safe move)
G0 Z10.000

(Spindle start)
M03 S12000

(Hole 1)
G0 X12.500 Y20.000
G0 Z5.000
G1 Z-8.000 F250
( Pocket milling operation )
G0 Z10.000

(Hole 2)
G0 X-14.000 Y18.500
G0 Z5.000
G1 Z-8.000 F250
( Pocket milling operation )
G0 Z10.000

(Spindle stop)
M05

G0 Z10.000
M30
%
```

---

## 6. Machining Safety Rules

The system applies engineering safety constraints before export.

Typical rules include:

| Rule                  | Description                                          |
| --------------------- | ---------------------------------------------------- |
| Maximum depth         | Machining depth is capped at 35 mm                   |
| Minimum hole diameter | Holes below the machine threshold are rejected       |
| Tool compatibility    | Tool diameter must be smaller than the hole diameter |
| Edge clearance        | Holes must remain inside the disk boundary           |
| Inter-hole clearance  | Holes must not overlap or violate spacing rules      |
| Safe Z height         | All rapid movements use a safe retract height        |
| Offline fallback      | Frontend safety checks remain active even without AI |

The AI backend may recommend adjusted values if the requested parameters are unsafe or inefficient.

---

## 7. Frontend Architecture

The frontend is implemented using **.NET 8 WinForms**.

Responsibilities:

* German-only UI rendering
* one-page workflow
* input validation
* communication with Flask backend
* local deterministic fallback
* layout drawing
* PNG export
* G-Code export
* operator warnings
* status indicators
* AI availability indicator

Recommended frontend structure:

```text
CableGlandPlanner/
├── Program.cs
├── MainForm.cs
├── Models/
│   ├── DiskParameters.cs
│   ├── HoleDefinition.cs
│   ├── LayoutResult.cs
│   ├── AiResponse.cs
│   └── GCodeSettings.cs
├── Services/
│   ├── AiClient.cs
│   ├── LayoutValidator.cs
│   ├── GCodeGenerator.cs
│   ├── PreviewRenderer.cs
│   └── ExportService.cs
├── UI/
│   ├── Theme.cs
│   ├── GermanLabels.cs
│   └── ValidationMessages.cs
└── Resources/
```

---

## 8. Backend Architecture

The backend is implemented using **Python 3.10+** and **Flask**.

Recommended backend structure:

```text
ai_server/
├── app.py
├── requirements.txt
├── config.py
├── models/
│   ├── request_models.py
│   └── response_models.py
├── services/
│   ├── feasibility_service.py
│   ├── layout_optimizer.py
│   ├── insert_resolver.py
│   ├── diagnostics.py
│   └── geometry.py
├── data/
│   └── internal_insert_definitions.py
└── tests/
    ├── test_feasibility.py
    ├── test_geometry.py
    └── test_layout_optimizer.py
```

---

## 9. API Endpoints

### 9.1 Health Check

```http
GET /health
```

Example response:

```json
{
  "status": "ok",
  "service": "Cable Gland AI Backend",
  "version": "1.0.0"
}
```

---

### 9.2 Feasibility Check

```http
POST /feasibility
```

Example request:

```json
{
  "disk_radius": 50.0,
  "hole_diameters": [10.0, 12.0, 16.0],
  "edge_clearance": 5.0,
  "hole_clearance": 3.0,
  "tool_diameter": 6.0,
  "depth": 12.0
}
```

Example response:

```json
{
  "feasible": true,
  "confidence": 0.91,
  "warnings": [],
  "min_hole_distance": 13.0,
  "max_allowed_depth": 35.0,
  "tool_compatible": true
}
```

---

### 9.3 Layout Suggestion

```http
POST /suggest_layout
```

Example request:

```json
{
  "disk_radius": 50.0,
  "hole_diameters": [10.0, 12.0, 16.0],
  "edge_clearance": 5.0,
  "hole_clearance": 3.0,
  "existing_positions": []
}
```

Example response:

```json
{
  "positions": [
    [18.0, 0.0],
    [-12.0, 20.0],
    [-12.0, -20.0]
  ],
  "confidence": 0.88,
  "warnings": []
}
```

---

### 9.4 Insert Analysis

```http
POST /analyze_insert
```

Example request:

```json
{
  "insert_type": "M20",
  "material": "aluminium",
  "tool_diameter": 6.0
}
```

Example response:

```json
{
  "insert_type": "M20",
  "pos_mapping": "POS_03",
  "recommended_depth": 14.0,
  "max_depth": 35.0,
  "tool_compatible": true,
  "warnings": []
}
```

---

## 10. Configuration

The frontend can connect to a local or remote AI backend.

Example configuration:

```json
{
  "ai_backend_url": "http://localhost:5000",
  "request_timeout_ms": 5000,
  "enable_ai_assistance": true,
  "enable_offline_fallback": true,
  "default_safe_z": 10.0,
  "default_feed_rate": 250,
  "default_spindle_speed": 12000,
  "max_depth_mm": 35.0
}
```

---

## 11. Offline Fallback Mode

If the Flask backend is unavailable, the frontend switches into offline mode.

In offline mode:

* user input remains available
* local validation is performed
* basic geometry checks are executed
* G-Code generation remains possible only if constraints are valid
* AI recommendations are disabled
* warning messages inform the user that AI assistance is unavailable

This ensures that the application is not fully dependent on network or backend availability.

---

## 12. Visual Preview Export

The application can export a `.png` preview of the current layout.

The preview should include:

* circular disk boundary
* hole outlines
* hole center points
* hole numbers
* clearance visualization
* warning indicators
* scale reference
* export timestamp
* selected insert type

The preview is intended for documentation, internal approval, and manufacturing review.

---

## 13. German-Only UI Requirements

The user interface must be entirely German.

Examples:

| English Meaning | German UI Text      |
| --------------- | ------------------- |
| Disk diameter   | Scheibendurchmesser |
| Hole diameter   | Bohrungsdurchmesser |
| Edge clearance  | Randabstand         |
| Hole clearance  | Lochabstand         |
| Generate layout | Layout erzeugen     |
| Export G-Code   | G-Code exportieren  |
| AI status       | KI-Status           |
| Feasible        | Machbar             |
| Warning         | Warnung             |
| Error           | Fehler              |

No English labels, tooltips, validation messages, or status messages should appear in the production UI.

---

## 14. Engineering Validation Logic

Before any export, the system should validate:

```text
1. Disk radius > 0
2. Every hole diameter > 0
3. Tool diameter > 0
4. Tool diameter < hole diameter
5. Edge clearance >= 0
6. Hole clearance >= 0
7. Depth > 0
8. Depth <= 35 mm
9. Every hole lies inside the disk
10. No two holes overlap
11. Safe Z height is greater than machining Z
12. Feed rate is positive
```

If any validation fails, export must be blocked.

---

## 15. Suggested Technology Stack

### Frontend

* C#
* .NET 8
* Windows Forms
* System.Text.Json
* System.Drawing or SkiaSharp for preview rendering
* HttpClient for backend communication

### Backend

* Python 3.10+
* Flask
* NumPy
* SciPy
* optional scikit-learn
* optional OR-Tools
* optional pytest

### Output Formats

* `.nc` for CNC G-Code
* `.png` for visual preview
* optional `.json` for internal diagnostic export

---

## 16. Development Setup

### 16.1 Backend Setup

```bash
cd ai_server
python -m venv .venv
```

Windows:

```bash
.venv\Scripts\activate
```

Linux/macOS:

```bash
source .venv/bin/activate
```

Install dependencies:

```bash
pip install flask numpy scipy
```

Run server:

```bash
python app.py
```

Default backend URL:

```text
http://localhost:5000
```

---

### 16.2 Frontend Setup

```bash
dotnet restore
dotnet build -c Release
dotnet run -c Release
```

Recommended environment:

* Windows 10 or Windows 11
* .NET 8 SDK
* Visual Studio 2022 or newer
* local Flask backend running on port 5000

---

## 17. Example Workflow

```text
1. Start Flask AI backend.
2. Start WinForms application.
3. Select cable gland insert type.
4. Enter disk diameter.
5. Enter hole diameters and clearances.
6. Click "Layout erzeugen".
7. Frontend sends parameters to AI backend.
8. Backend validates feasibility and suggests positions.
9. Frontend renders the preview.
10. Operator reviews warnings.
11. Export PNG preview.
12. Export CNC G-Code.
13. Use generated .nc file in CNC workflow after machine-side verification.
```

---

## 18. Testing Strategy

Recommended tests:

### Backend Tests

* geometry boundary validation
* pairwise hole collision detection
* infeasible density detection
* insert mapping
* depth limit enforcement
* API response schema validation

### Frontend Tests

* input validation
* German label consistency
* backend timeout handling
* fallback mode
* G-Code output formatting
* PNG export
* warning display

### Integration Tests

* frontend to backend communication
* valid layout generation
* invalid layout rejection
* AI unavailable scenario
* export blocking on unsafe parameters

---

## 19. Safety Notice

Generated G-Code must always be reviewed before use on a real CNC machine.

The application provides engineering assistance, validation, and structured output, but final responsibility remains with the operator, CAM engineer, or machine technician.

Before machining, verify:

* machine coordinate system
* work offset
* tool length offset
* tool diameter
* spindle speed
* feed rate
* material clamping
* safe Z height
* depth values
* coolant strategy
* CNC controller compatibility

---

## 20. Licence

Specify the project licence before distribution.

Examples:

```text
MIT
Apache-2.0
GPL-3.0
Proprietary / Internal Use Only
```

---

## 21. Status

Current project status:

```text
Architecture: AI-assisted WinForms + Flask
Frontend: .NET 8 WinForms
Backend: Python Flask
UI language: German only
Theme: Orange / White
Primary output: CNC G-Code
Secondary output: PNG preview
AI mode: Local or remote backend
Fallback mode: Deterministic local validation
```

---

## 22. Summary

Cable Gland Planner is an AI-assisted engineering application for transforming cable gland layout requirements into validated geometry, visual documentation, and CNC-ready manufacturing output.

It combines:

* deterministic engineering validation
* AI-assisted feasibility analysis
* layout optimisation
* insert mapping
* German-only operator workflow
* CNC G-Code generation
* PNG preview export
* offline fallback safety
https://www.linkedin.com/in/amirmobasher
https://www.myscience.de/en/news/wire/ideas_with_passion_and_entrepreneurial_spirit-2025-uni-bonn
https://www.myminifactory.com/users/AmirMobasher
https://www.lafeltrinelli.it/ergebnis-automation-fur-die-bundesliga-libro-inglese-amir-mobasheraghdam-ladan-seddighi/e/9783695724925?srsltid=AfmBOoqNAdVPwowgd7P6kX-sQqW3NZf80f4DjqeHGT9CAfTtLHbYGR8v


