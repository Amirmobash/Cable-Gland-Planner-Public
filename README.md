# Cable Gland Planner (WinForms) — AI‑Driven Layout & G‑Code Generation

A Windows desktop application for planning cable gland drilling / pocket layouts and generating CNC‑ready **G‑Code**.  
The tool offers a streamlined one‑page workflow with a consistent **orange/white** UI theme and a **German‑only** interface.

The **AI‑First** architecture uses a **Python Flask backend** as the central intelligence: it interprets constraints, validates feasibility, guides layout optimisation, and provides structured recommendations – all without exposing internal heuristics to the frontend.

---

## Overview

Cable Gland Planner transforms real‑world requirements (disk geometry, hole diameters, clearances, machine positions) into:

- a **validated circular layout** (non‑overlapping holes within boundaries)
- a **manufacturing‑ready G‑Code program** (`.nc`) for pocket milling / drilling
- a **visual preview** (`.png`) for documentation

The **AI Assistant** (Flask server) handles:
- parsing and normalising user input (diameters, tolerances, constraints)
- feasibility analysis (boundary conditions, spacing rules, tool compatibility)
- layout optimisation guidance (constraint weighting, initial position suggestions)
- diagnostic outputs (confidence scores, warnings, recommendations)

---

## Architecture & AI Integration

The system consists of two integrated components:

1. **WinForms Frontend (.NET 8)**  
   - German‑only UI, one‑page workflow, orange/white theme.  
   - Collects user parameters (disk geometry, hole diameters, clearances).  
   - Sends requests to the Flask backend for AI assistance.  
   - Performs layout visualisation, G‑Code generation, and PNG export.  
   - Falls back to deterministic local logic if the AI server is unreachable.

2. **Flask Backend (Python 3.10+)** – The AI Engine  
   - Exposes REST endpoints (e.g., `/analyze`, `/feasibility`, `/suggest_layout`).  
   - Can run **locally** (`localhost`) or as a **remote internal service**.  
   - Uses rule‑based reasoning, constraint solvers, or machine learning models (depending on deployment).  
   - Returns JSON responses that the frontend consumes to adjust parameters or warn the user.

**Communication example:**  
The frontend sends a POST request with disk radius, hole diameters, and clearances. The Flask server responds with a feasibility score, recommended minimal hole distances, and optional initial positions.

---

## Core Capabilities (AI‑Centric)

### 1) One‑Page Workflow (German UI)
- No multi‑tab navigation – all controls and results on a single window.  
- Orange/white colour scheme applied to buttons, panels, and status indicators.  
- All labels, messages, and tooltips are in German.

### 2) AI‑Assisted Gland Insert Handling
- Gland insert definitions are managed by the AI backend (no manual JSON editing).  
- The AI automatically determines the correct **POS** mapping per insert and provides depth values.  
- Users only select the insert type; the AI handles the rest.

### 3) AI‑Driven Layout Generation & Validation
- Places multiple circular holes inside a circular disk boundary.  
- Enforces:
  - minimum edge clearance
  - minimum inter‑hole clearance
  - non‑overlap constraints  
- **AI backend** performs:
  - pre‑validation of input combinations (detects impossible densities)
  - suggestion of better spacing rules or hole order
  - real‑time warnings when the layout approaches infeasible regions  
- Visual feedback and warning messages are shown to the user.

### 4) CNC G‑Code Export (AI‑Optimised)
- Generates CNC‑friendly toolpaths:
  - absolute positioning (G90)
  - safe Z and retract moves
  - spindle control (M03/M05) if enabled
  - pocket milling strategy based on tool diameter and hole size  
- Operational limits (capped depth **35 mm**, minimum hole threshold) are enforced by the AI based on tool capabilities.

### 5) AI‑Generated Preview Export
- Exports a PNG image of the current layout for documentation and review.

---

## AI Backend Setup

### Requirements
- **Frontend:** Windows 10/11, .NET 8 SDK, Visual Studio 2022+  
- **Backend (AI Flask server):** Python 3.10+, pip, Flask, and optional libraries (`numpy`, `scipy`, etc.)

### 1) Start the Flask AI Backend
Navigate to the `ai_server/` folder and run:

```bash
pip install flask numpy scipy   # or use requirements.txt
python app.py
```

The server runs on `http://localhost:5000` by default.  
You can change the endpoint URL in the WinForms settings.

### 2) Build & Run the WinForms Frontend
```bash
dotnet restore
dotnet build -c Release
dotnet run -c Release
```

Ensure the backend is running before using AI‑assisted features; otherwise the frontend works in offline mode (with limited functionality).

---

## Configuration

The frontend can be configured to point to any Flask server address (local or remote). Example AI endpoints:

| Endpoint           | Method | Request Body (JSON)                              | Response (JSON)                                      |
|--------------------|--------|--------------------------------------------------|------------------------------------------------------|
| `/feasibility`     | POST   | `{ "disk_radius": 50, "hole_diameters": [10,12], "edge_clearance": 5, "hole_clearance": 3 }` | `{ "feasible": true, "warnings": [], "min_hole_distance": 13 }` |
| `/suggest_layout`  | POST   | same as above + optional existing positions     | `{ "positions": [[x1,y1],...], "confidence": 0.92 }` |

All data files (previously JSON) are now **internal to the AI backend** – users never interact with raw data files.

---

## Notes on the AI Assistant

- The AI component is a **black‑box assistant** – it provides decisions, diagnostics, and structured recommendations without exposing internal heuristics.  
- Depending on deployment, the Flask server may use:
  - **rule‑based solvers** (circle packing heuristics)  
  - **optimisation algorithms** (simulated annealing, genetic algorithms)  
  - **lightweight machine learning models** (feasibility classifiers)  
- The frontend never relies on the AI for core safety; it always has a deterministic fallback.

---

## Licence

[Specify your licence here – e.g., MIT, proprietary]
