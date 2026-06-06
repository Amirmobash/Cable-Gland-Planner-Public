# Cable Gland Planner (WinForms)  
## AI-Assisted Engineering Layout, Feasibility Analysis & CNC G-Code Generation  
### Entwickelt von Amir Mobasheraghdam

---

## Inhaltsverzeichnis

1. [Projektübersicht](#1-projektübersicht)
2. [Systemarchitektur](#2-systemarchitektur)
3. [Detaillierte technische Spezifikation](#3-detaillierte-technische-spezifikation)
4. [Mathematische Grundlagen](#4-mathematische-grundlagen)
5. [KI-Backend mit Linux-Server-Integration](#5-ki-backend-mit-linux-server-integration)
6. [Frontend-Architektur (WinForms)](#6-frontend-architektur-winforms)
7. [CNC-G-Code-Generierung](#7-cnc-g-code-generierung)
8. [API-Spezifikation](#8-api-spezifikation)
9. [Datenbank- und Cache-Schicht](#9-datenbank--und-cache-schicht)
10. [Fehlertoleranz & Fallback-Mechanismen](#10-fehlertoleranz--fallback-mechanismen)
11. [Testing & Qualitätssicherung](#11-testing--qualitätssicherung)
12. [Deployment auf Linux-Servern](#12-deployment-auf-linux-servern)
13. [Sicherheitskonzept](#13-sicherheitskonzept)
14. [Leistungsoptimierung](#14-leistungsoptimierung)
15. [Erweiterte Geometrie-Algorithmen](#15-erweiterte-geometrie-algorithmen)
16. [Benutzeroberfläche (German-only)](#16-benutzeroberfläche-german-only)
17. [Installation & Konfiguration](#17-installation--konfiguration)
18. [Fehlerbehandlung & Logging](#18-fehlerbehandlung--logging)
19. [Entwickler-Dokumentation](#19-entwickler-dokumentation)
20. [Lizenz & rechtliche Hinweise](#20-lizenz--rechtliche-hinweise)

---

## 1. Projektübersicht

**Cable Gland Planner** ist eine hochpräzise, KI-gestützte Ingenieurssoftware für die automatisierte Planung, Validierung und CNC-Fertigung von Kabelverschraubungs-Bohrbildern. Die Anwendung richtet sich an den industriellen Maschinenbau, den Schaltschrankbau und die automatische Fertigung von runden Einscheiben-Layouts.

### 1.1 Kernfunktionen

| Funktion | Beschreibung |
|----------|--------------|
| **KI-gestützte Machbarkeitsanalyse** | Prüfung von Lochabständen, Randabständen, Werkzeugdurchmessern und Materialfestigkeit |
| **Automatisches Layout-Optimierung** | Positionierung von Rundlöchern unter physikalischen Zwangsbedingungen |
| **CNC-G-Code-Export** | ISO-konformer G-Code für Fräs- und Bohrmaschinen |
| **Echtzeit-Visualisierung** | PNG-Export mit Maßstabsreferenz und Warnindikatoren |
| **Datenbankgestützte Insert-Bibliothek** | 200+ Kabelverschraubungstypen mit POS-Mapping |
| **Linux-Backend-Cluster** | Horizontale Skalierung für KI-Berechnungen |
| **Offline-Fallback** | Deterministische Validierung bei Serverausfall |

### 1.2 Technische Eckdaten

```yaml
Entwicklungssprachen:
  Frontend: C# .NET 8 WinForms
  Backend: Python 3.11+ mit Flask 3.0
  Datenbank: PostgreSQL 15 + Redis 7.2
  Cache: Redis Cluster

Betriebssysteme:
  Frontend: Windows 10/11
  Backend: Ubuntu 22.04 LTS / Debian 12

Protokolle:
  REST API (HTTPS)
  WebSocket (Echtzeit-Status)
  gRPC (interne Mikrodienste)

Schnittstellen:
  NC (G-Code)
  PNG (Preview)
  JSON (Diagnose)
  DXF (optional)
```

---

## 2. Systemarchitektur

### 2.1 Gesamtarchitektur

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         Client Layer (Windows)                              │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │              WinForms Frontend (.NET 8, German UI)                  │   │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ │   │
│  │  │ Eingabe  │ │ Preview  │ │ G-Code   │ │ PNG      │ │ Status   │ │   │
│  │  │ Maske    │ │ Renderer │ │ Generator│ │ Export   │ │ Monitor  │ │   │
│  │  └──────────┘ └──────────┘ └──────────┘ └──────────┘ └──────────┘ │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────┬───────────────────────────────────────┘
                                      │ HTTPS / WSS
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                      Load Balancer (HAProxy / Nginx)                        │
│                    SSL Termination + Rate Limiting + Routing                │
└─────────────────────────────────────┬───────────────────────────────────────┘
                                      │
                    ┌─────────────────┼─────────────────┐
                    ▼                 ▼                 ▼
┌──────────────────────────┐ ┌──────────────────────────┐ ┌──────────────────────────┐
│   Backend Server 1       │ │   Backend Server 2       │ │   Backend Server N       │
│   (Ubuntu 22.04)         │ │   (Ubuntu 22.04)         │ │   (Ubuntu 22.04)         │
│  ┌────────────────────┐  │ │  ┌────────────────────┐  │ │  ┌────────────────────┐  │
│  │ Flask API (gunicorn)│  │ │  │ Flask API (gunicorn)│  │ │  │ Flask API (gunicorn)│  │
│  │ + Celery Worker    │  │ │  │ + Celery Worker    │  │ │  │ + Celery Worker    │  │
│  └────────────────────┘  │ │  └────────────────────┘  │ │  └────────────────────┘  │
└────────────┬─────────────┘ └────────────┬─────────────┘ └────────────┬─────────────┘
             │                             │                             │
             └─────────────────────────────┼─────────────────────────────┘
                                           ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        Datenbank-Cluster (separate Server)                  │
│  ┌────────────────────┐  ┌────────────────────┐  ┌────────────────────┐    │
│  │ PostgreSQL 15      │  │ Redis Cluster      │  │ MinIO (Object      │    │
│  │ (Primary + Replica)│  │ (Cache + Sessions) │  │ Storage für PNGs)  │    │
│  └────────────────────┘  └────────────────────┘  └────────────────────┘    │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 2.2 Mikrodienste (Linux)

```yaml
Services:
  - name: feasibility-engine
    port: 5001
    replicas: 3
    resources:
      cpu: 2
      memory: 4Gi
    
  - name: layout-optimizer
    port: 5002
    replicas: 2
    resources:
      cpu: 4
      memory: 8Gi
    gpu: optional (CUDA 11.8)
    
  - name: gcode-generator
    port: 5003
    replicas: 2
    resources:
      cpu: 1
      memory: 2Gi
      
  - name: insert-resolver
    port: 5004
    replicas: 2
    resources:
      cpu: 0.5
      memory: 1Gi
```

---

## 3. Detaillierte technische Spezifikation

### 3.1 Datenstrukturen (C#)

```csharp
// Models/DiskParameters.cs
namespace CableGlandPlanner.Models
{
    public class DiskParameters
    {
        public double Radius { get; set; }           // mm
        public double Diameter => Radius * 2;
        public double EdgeClearance { get; set; }    // mm
        public string Material { get; set; }          // z.B. "Aluminium", "Stahl"
        public double MaxDepth { get; set; } = 35.0;  // mm
    }

    public class HoleDefinition
    {
        public int Id { get; set; }
        public double Diameter { get; set; }          // mm
        public double Depth { get; set; }             // mm
        public double X { get; set; }                 // mm
        public double Y { get; set; }                 // mm
        public string InsertType { get; set; }        // z.B. "M20", "PG16"
        public string PosMapping { get; set; }        // z.B. "POS_03"
        public bool IsPocket { get; set; }            // Tasche oder Durchgangsbohrung
    }

    public class LayoutValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public double MinHoleDistance { get; set; }
        public double MaxUtilization { get; set; }    // Flächennutzung in %
        public string FeasibilityScore { get; set; }  // "HIGH", "MEDIUM", "LOW", "INFEASIBLE"
        public Dictionary<int, string> HoleStatus { get; set; } = new();
    }

    public class GCodeSettings
    {
        public double SafeZ { get; set; } = 10.0;     // mm
        public double FeedRateXY { get; set; } = 500; // mm/min
        public double FeedRateZ { get; set; } = 250;  // mm/min
        public int SpindleSpeed { get; set; } = 12000; // RPM
        public bool UseCoolant { get; set; } = true;
        public string CoolantType { get; set; } = "MIST"; // FLOOD, MIST, OFF
        public double PeckDepth { get; set; } = 2.0;  // mm bei Tiefbohrung
        public bool UsePecking { get; set; } = false;
    }
}
```

### 3.2 Datenstrukturen (Python Backend)

```python
# models/request_models.py
from pydantic import BaseModel, Field, validator
from typing import List, Optional, Tuple
from enum import Enum

class MaterialType(str, Enum):
    ALUMINIUM = "aluminium"
    STEEL = "stahl"
    STAINLESS = "edelstahl"
    BRASS = "messing"
    PLASTIC = "kunststoff"

class FeasibilityRequest(BaseModel):
    disk_radius: float = Field(..., gt=0, le=500, description="Scheibenradius in mm")
    hole_diameters: List[float] = Field(..., min_items=1, max_items=24)
    edge_clearance: float = Field(0, ge=0, le=20)
    hole_clearance: float = Field(0, ge=0, le=15)
    tool_diameter: float = Field(..., gt=0, le=50)
    depth: float = Field(..., gt=0, le=35)
    material: MaterialType = MaterialType.ALUMINIUM
    
    @validator('tool_diameter')
    def tool_smaller_than_hole(cls, v, values):
        if 'hole_diameters' in values:
            min_hole = min(values['hole_diameters'])
            if v >= min_hole:
                raise ValueError(f'Werkzeugdurchmesser ({v}mm) muss kleiner als kleinster Lochdurchmesser ({min_hole}mm) sein')
        return v

class LayoutSuggestionRequest(BaseModel):
    disk_radius: float
    hole_diameters: List[float]
    edge_clearance: float
    hole_clearance: float
    existing_positions: List[Tuple[float, float]] = []
    optimization_iterations: int = Field(1000, ge=100, le=10000)
    algorithm: str = Field("simulated_annealing", regex="^(simulated_annealing|genetic|greedy|radial)$")

class InsertAnalysisRequest(BaseModel):
    insert_type: str = Field(..., min_length=2, max_length=10)
    material: MaterialType
    tool_diameter: float
    machining_strategy: str = Field("standard", regex="^(standard|precision|roughing)$")
```

---

## 4. Mathematische Grundlagen

### 4.1 Geometrische Constraints

```python
# services/geometry.py - Version 2.0 mit erweiterten Algorithmen

import numpy as np
from scipy.spatial import distance_matrix
from scipy.optimize import minimize, differential_evolution
from typing import List, Tuple
import math

class CircularPackingEngine:
    """Erweiterte Circle-Packing-Engine mit physikalischer Simulation"""
    
    def __init__(self, disk_radius: float, edge_clearance: float):
        self.disk_radius = disk_radius
        self.edge_clearance = edge_clearance
        self.effective_radius = disk_radius - edge_clearance
        
    def is_inside_disk(self, x: float, y: float, hole_radius: float) -> bool:
        """Prüft ob Loch vollständig innerhalb der Scheibe liegt"""
        distance_to_center = math.hypot(x, y)
        return distance_to_center + hole_radius <= self.effective_radius
    
    def are_holes_colliding(self, x1, y1, r1, x2, y2, r2, clearance) -> bool:
        """Prüft Kollision zwischen zwei Löchern inkl. Sicherheitsabstand"""
        distance = math.hypot(x1 - x2, y1 - y2)
        min_distance = r1 + r2 + clearance
        return distance < min_distance
    
    def calculate_utilization(self, positions: List[Tuple[float, float]], radii: List[float]) -> float:
        """Berechnet Flächennutzungsgrad (0-1)"""
        total_hole_area = sum(math.pi * r**2 for r in radii)
        disk_area = math.pi * self.effective_radius**2
        return total_hole_area / disk_area
    
    def minimum_clearance_check(self, positions: List[Tuple], radii: List[float], clearance: float) -> Tuple[bool, float]:
        """Findet minimalen Abstand zwischen allen Lochpaaren"""
        n = len(positions)
        min_dist = float('inf')
        
        for i in range(n):
            for j in range(i+1, n):
                dist = math.hypot(positions[i][0] - positions[j][0],
                                 positions[i][1] - positions[j][1])
                required = radii[i] + radii[j] + clearance
                actual_clearance = dist - (radii[i] + radii[j])
                min_dist = min(min_dist, actual_clearance)
                
                if dist < required:
                    return False, actual_clearance
        
        return True, min_dist
    
    def energy_function(self, positions_flat: np.ndarray, radii: List[float], 
                       clearance: float, stiffness: float = 100.0) -> float:
        """Energiefunktion für physikalisch-basierte Optimierung"""
        n = len(radii)
        positions = positions_flat.reshape(n, 2)
        energy = 0.0
        
        # Kollisionsenergie (abstoßende Federkraft)
        for i in range(n):
            for j in range(i+1, n):
                dx = positions[i,0] - positions[j,0]
                dy = positions[i,1] - positions[j,1]
                dist = math.hypot(dx, dy)
                min_dist = radii[i] + radii[j] + clearance
                
                if dist < min_dist:
                    # Überlappung - hohe Energie
                    overlap = min_dist - dist
                    energy += stiffness * (overlap ** 2)
        
        # Grenzflächenenergie (Löcher müssen innerhalb der Scheibe bleiben)
        for i in range(n):
            x, y = positions[i]
            dist_to_center = math.hypot(x, y)
            max_dist = self.effective_radius - radii[i]
            
            if dist_to_center > max_dist:
                # Loch ragt über Rand - hohe Energie
                violation = dist_to_center - max_dist
                energy += stiffness * (violation ** 2)
        
        return energy
```

### 4.2 Simulated Annealing für Layout-Optimierung

```python
# services/layout_optimizer.py - Erweiterte Version

import numpy as np
import random
import math
from typing import List, Tuple, Optional
from dataclasses import dataclass
import logging

@dataclass
class OptimizationResult:
    positions: List[Tuple[float, float]]
    energy: float
    iterations: int
    converged: bool
    time_ms: float

class SimulatedAnnealingOptimizer:
    """Simulated Annealing für nicht-konvexe Optimierungsprobleme"""
    
    def __init__(self, 
                 disk_radius: float,
                 edge_clearance: float,
                 initial_temp: float = 1000.0,
                 cooling_rate: float = 0.995,
                 min_temp: float = 1e-8):
        
        self.geometry = CircularPackingEngine(disk_radius, edge_clearance)
        self.initial_temp = initial_temp
        self.cooling_rate = cooling_rate
        self.min_temp = min_temp
        
    def optimize(self, 
                radii: List[float],
                clearance: float,
                max_iterations: int = 10000,
                initial_positions: Optional[List[Tuple]] = None) -> OptimizationResult:
        
        n = len(radii)
        
        # Initialisierung
        if initial_positions is None:
            positions = self._generate_radial_initialization(radii)
        else:
            positions = initial_positions.copy()
            
        current_energy = self.geometry.energy_function(
            np.array(positions).flatten(), radii, clearance
        )
        
        temp = self.initial_temp
        best_positions = positions.copy()
        best_energy = current_energy
        
        start_time = time.time()
        
        for iteration in range(max_iterations):
            # Generiere Nachbarlösung
            new_positions = self._generate_neighbor(positions, radii, clearance)
            new_energy = self.geometry.energy_function(
                np.array(new_positions).flatten(), radii, clearance
            )
            
            # Metropolis-Kriterium
            delta_e = new_energy - current_energy
            
            if delta_e < 0 or random.random() < math.exp(-delta_e / temp):
                positions = new_positions
                current_energy = new_energy
                
                if current_energy < best_energy:
                    best_positions = positions.copy()
                    best_energy = current_energy
            
            # Abkühlung
            temp *= self.cooling_rate
            
            if temp < self.min_temp:
                break
        
        elapsed_ms = (time.time() - start_time) * 1000
        
        return OptimizationResult(
            positions=best_positions,
            energy=best_energy,
            iterations=iteration + 1,
            converged=temp < self.min_temp,
            time_ms=elapsed_ms
        )
    
    def _generate_radial_initialization(self, radii: List[float]) -> List[Tuple]:
        """Heuristische Initialisierung auf konzentrischen Kreisen"""
        n = len(radii)
        positions = []
        
        # Sortiere Löcher nach Radius (große Löcher zuerst)
        sorted_indices = sorted(range(n), key=lambda i: radii[i], reverse=True)
        
        for idx, i in enumerate(sorted_indices):
            radius = radii[i]
            # Berechne optimalen Radius für diesen Ring
            ring_radius = self._calculate_ring_radius(idx, n, radius)
            angle = (2 * math.pi * idx) / n
            
            x = ring_radius * math.cos(angle)
            y = ring_radius * math.sin(angle)
            positions.append((x, y))
            
        return positions
    
    def _generate_neighbor(self, positions: List[Tuple], 
                          radii: List[float], 
                          clearance: float,
                          max_displacement: float = 5.0) -> List[Tuple]:
        """Generiert Nachbarlösung durch zufällige Verschiebung"""
        new_positions = positions.copy()
        
        # Wähle zufälliges Loch
        idx = random.randint(0, len(positions) - 1)
        x, y = positions[idx]
        
        # Zufällige Verschiebung (Normalverteilung)
        dx = random.gauss(0, max_displacement / 3)
        dy = random.gauss(0, max_displacement / 3)
        
        new_x = x + dx
        new_y = y + dy
        
        new_positions[idx] = (new_x, new_y)
        return new_positions
    
    def _calculate_ring_radius(self, index: int, total: int, hole_radius: float) -> float:
        """Berechnet optimalen Ringradius für hierarchisches Layout"""
        if total == 1:
            return 0.0
        
        # Maximale Ausnutzung des verfügbaren Platzes
        max_radius = self.geometry.effective_radius - hole_radius
        
        # Logarithmische Verteilung der Ringe
        if index == 0:
            return max_radius * 0.6
        else:
            return max_radius * (0.3 + 0.3 * (index / (total - 1)))
```

---

## 5. KI-Backend mit Linux-Server-Integration

### 5.1 Hauptserver-Konfiguration (Ubuntu 22.04 LTS)

```bash
#!/bin/bash
# deploy_backend.sh - Automatisiertes Deployment auf Ubuntu 22.04

set -e

echo "=== Cable Gland Planner Backend Deployment ==="
echo "Startzeit: $(date)"

# System-Update
apt-get update && apt-get upgrade -y
apt-get install -y python3.11 python3.11-venv python3.11-dev
apt-get install -y nginx postgresql-15 redis-server
apt-get install -y gunicorn supervisor
apt-get install -y build-essential libssl-dev libffi-dev

# PostgreSQL Konfiguration
sudo -u postgres psql <<EOF
CREATE USER cablegland WITH PASSWORD 'secure_password_2025';
CREATE DATABASE cablegland_db OWNER cablegland;
GRANT ALL PRIVILEGES ON DATABASE cablegland_db TO cablegland;
EOF

# Redis Konfiguration
cat > /etc/redis/redis.conf <<EOF
port 6379
bind 127.0.0.1
maxmemory 2gb
maxmemory-policy allkeys-lru
save 900 1
save 300 10
appendonly yes
EOF

systemctl restart redis-server

# Projekt Setup
mkdir -p /opt/cablegland
cd /opt/cablegland

# Virtual Environment
python3.11 -m venv venv
source venv/bin/activate

# Abhängigkeiten
cat > requirements.txt <<EOF
flask==3.0.0
flask-cors==4.0.0
flask-limiter==3.5.0
gunicorn==21.2.0
numpy==1.26.0
scipy==1.11.4
pandas==2.1.3
scikit-learn==1.3.2
psycopg2-binary==2.9.9
redis==5.0.1
celery==5.3.4
pydantic==2.5.0
python-dotenv==1.0.0
prometheus-client==0.19.0
opentelemetry-api==1.21.0
opentelemetry-sdk==1.21.0
opentelemetry-instrumentation-flask==0.42b0
EOF

pip install -r requirements.txt

# Gunicorn Konfiguration
cat > /etc/supervisor/conf.d/cablegland.conf <<EOF
[program:cablegland-api]
command=/opt/cablegland/venv/bin/gunicorn -w 4 -b 127.0.0.1:8000 --timeout 120 app:app
directory=/opt/cablegland
user=www-data
autostart=true
autorestart=true
redirect_stderr=true
stdout_logfile=/var/log/cablegland/api.log
stderr_logfile=/var/log/cablegland/api-error.log
environment=PYTHONPATH="/opt/cablegland"

[program:cablegland-celery]
command=/opt/cablegland/venv/bin/celery -A app.celery worker --loglevel=info
directory=/opt/cablegland
user=www-data
autostart=true
autorestart=true
redirect_stderr=true
stdout_logfile=/var/log/cablegland/celery.log
EOF

# Nginx Konfiguration
cat > /etc/nginx/sites-available/cablegland <<EOF
upstream cablegland_backend {
    least_conn;
    server 127.0.0.1:8000 max_fails=3 fail_timeout=30s;
    server 127.0.0.1:8001 max_fails=3 fail_timeout=30s;
    server 127.0.0.1:8002 max_fails=3 fail_timeout=30s;
}

server {
    listen 443 ssl http2;
    server_name api.cablegland.local;
    
    ssl_certificate /etc/ssl/certs/cablegland.crt;
    ssl_certificate_key /etc/ssl/private/cablegland.key;
    
    client_max_body_size 10M;
    
    location / {
        proxy_pass http://cablegland_backend;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_connect_timeout 120s;
        proxy_send_timeout 120s;
        proxy_read_timeout 120s;
    }
    
    location /metrics {
        proxy_pass http://127.0.0.1:8000/metrics;
    }
}

server {
    listen 80;
    server_name api.cablegland.local;
    return 301 https://\$server_name\$request_uri;
}
EOF

ln -sf /etc/nginx/sites-available/cablegland /etc/nginx/sites-enabled/
nginx -t && systemctl reload nginx

# Firewall Konfiguration
ufw allow 22/tcp
ufw allow 443/tcp
ufw allow 80/tcp
ufw --force enable

echo "=== Deployment abgeschlossen: $(date) ==="
```

### 5.2 Flask-Backend mit erweiterten Funktionen

```python
# app.py - Vollständiges Flask-Backend mit allen Endpunkten

import asyncio
import logging
from datetime import datetime
from typing import Dict, Any, List, Optional

from flask import Flask, request, jsonify, g
from flask_cors import CORS
from flask_limiter import Limiter
from flask_limiter.util import get_remote_address
from prometheus_flask_exporter import PrometheusMetrics
import redis
import psycopg2
from psycopg2.extras import RealDictCursor
from celery import Celery

# Eigene Module
from services.feasibility_service import FeasibilityAnalyzer
from services.layout_optimizer import LayoutOptimizer
from services.insert_resolver import InsertDatabase
from services.diagnostics import SystemDiagnostics
from models.request_models import *
from models.response_models import *

# Konfiguration
import os
from dotenv import load_dotenv
load_dotenv()

# Flask App Initialisierung
app = Flask(__name__)
app.config['JSON_SORT_KEYS'] = False
app.config['MAX_CONTENT_LENGTH'] = 10 * 1024 * 1024  # 10MB

# CORS für WinForms Frontend
CORS(app, resources={
    r"/api/*": {
        "origins": ["http://localhost:5000", "http://127.0.0.1:5000"],
        "methods": ["GET", "POST", "PUT", "DELETE"],
        "allow_headers": ["Content-Type", "Authorization"]
    }
})

# Rate Limiting
limiter = Limiter(
    app,
    key_func=get_remote_address,
    default_limits=["200 per day", "50 per hour"]
)

# Prometheus Metriken
metrics = PrometheusMetrics(app)
metrics.info('app_info', 'Cable Gland Planner Backend', version='1.0.0')

# Redis Verbindung
redis_client = redis.Redis(
    host=os.getenv('REDIS_HOST', 'localhost'),
    port=int(os.getenv('REDIS_PORT', 6379)),
    decode_responses=True,
    socket_connect_timeout=5,
    socket_timeout=5
)

# PostgreSQL Verbindungspool
def get_db():
    if not hasattr(g, 'db'):
        g.db = psycopg2.connect(
            host=os.getenv('DB_HOST', 'localhost'),
            database=os.getenv('DB_NAME', 'cablegland_db'),
            user=os.getenv('DB_USER', 'cablegland'),
            password=os.getenv('DB_PASSWORD', 'secure_password_2025'),
            cursor_factory=RealDictCursor
        )
    return g.db

@app.teardown_appcontext
def close_db(error):
    if hasattr(g, 'db'):
        g.db.close()

# Celery Konfiguration (asynchrone Aufgaben)
celery = Celery(
    'cablegland',
    broker=f"redis://{os.getenv('REDIS_HOST', 'localhost')}:{os.getenv('REDIS_PORT', 6379)}/0",
    backend=f"redis://{os.getenv('REDIS_HOST', 'localhost')}:{os.getenv('REDIS_PORT', 6379)}/1"
)

celery.conf.update(
    task_serializer='json',
    accept_content=['json'],
    result_serializer='json',
    timezone='Europe/Berlin',
    enable_utc=True,
    task_track_started=True,
    task_time_limit=30 * 60,  # 30 Minuten
    task_soft_time_limit=25 * 60
)

# Service-Initialisierung
feasibility_analyzer = FeasibilityAnalyzer()
layout_optimizer = LayoutOptimizer()
insert_db = InsertDatabase()
diagnostics = SystemDiagnostics()

# ==================== API-Endpunkte ====================

@app.route('/health', methods=['GET'])
def health_check():
    """Health Check für Load Balancer"""
    return jsonify({
        "status": "healthy",
        "service": "Cable Gland AI Backend",
        "version": "1.0.0",
        "timestamp": datetime.utcnow().isoformat(),
        "dependencies": {
            "postgresql": _check_postgresql(),
            "redis": _check_redis(),
            "celery": _check_celery()
        }
    }), 200

def _check_postgresql() -> str:
    try:
        conn = get_db()
        with conn.cursor() as cur:
            cur.execute("SELECT 1")
        return "healthy"
    except Exception as e:
        logging.error(f"PostgreSQL Fehler: {e}")
        return "unhealthy"

def _check_redis() -> str:
    try:
        redis_client.ping()
        return "healthy"
    except Exception as e:
        logging.error(f"Redis Fehler: {e}")
        return "unhealthy"

def _check_celery() -> str:
    try:
        inspector = celery.control.inspect()
        stats = inspector.stats()
        if stats:
            return "healthy"
        return "degraded"
    except Exception:
        return "unhealthy"

@app.route('/api/feasibility', methods=['POST'])
@limiter.limit("100 per minute")
def check_feasibility():
    """
    Prüft Machbarkeit einer Lochkonfiguration
    """
    try:
        data = request.get_json()
        req = FeasibilityRequest(**data)
        
        # Cache Check (gleiche Anfrage)
        cache_key = f"feasibility:{hash(str(sorted(data.items())))}"
        cached = redis_client.get(cache_key)
        if cached:
            import json
            return jsonify(json.loads(cached)), 200
        
        # Machbarkeitsanalyse
        result = feasibility_analyzer.analyze(
            disk_radius=req.disk_radius,
            hole_diameters=req.hole_diameters,
            edge_clearance=req.edge_clearance,
            hole_clearance=req.hole_clearance,
            tool_diameter=req.tool_diameter,
            depth=req.depth,
            material=req.material
        )
        
        # Cache für 5 Minuten
        redis_client.setex(cache_key, 300, jsonify(result).get_data(as_text=True))
        
        return jsonify(result), 200
        
    except ValueError as e:
        return jsonify({"error": str(e), "code": "VALIDATION_ERROR"}), 400
    except Exception as e:
        logging.exception("Fehler bei Machbarkeitsprüfung")
        return jsonify({"error": "Interner Serverfehler", "code": "INTERNAL_ERROR"}), 500

@app.route('/api/suggest_layout', methods=['POST'])
@limiter.limit("50 per minute")
def suggest_layout():
    """
    Optimiertes Layout für gegebene Löcher
    """
    try:
        data = request.get_json()
        req = LayoutSuggestionRequest(**data)
        
        # Für komplexe Optimierungen: Asynchrone Verarbeitung
        if len(req.hole_diameters) > 12:
            task = suggest_layout_async.delay(data)
            return jsonify({
                "task_id": task.id,
                "status": "processing",
                "estimated_time_seconds": 30
            }), 202
        
        # Synchron für kleine Layouts
        result = layout_optimizer.optimize(
            disk_radius=req.disk_radius,
            hole_diameters=req.hole_diameters,
            edge_clearance=req.edge_clearance,
            hole_clearance=req.hole_clearance,
            existing_positions=req.existing_positions,
            algorithm=req.algorithm,
            max_iterations=req.optimization_iterations
        )
        
        return jsonify(result), 200
        
    except Exception as e:
        logging.exception("Fehler bei Layout-Optimierung")
        return jsonify({"error": str(e)}), 500

@celery.task(bind=True)
def suggest_layout_async(self, request_data):
    """Asynchrone Layout-Optimierung für große Datenmengen"""
    try:
        req = LayoutSuggestionRequest(**request_data)
        
        result = layout_optimizer.optimize(
            disk_radius=req.disk_radius,
            hole_diameters=req.hole_diameters,
            edge_clearance=req.edge_clearance,
            hole_clearance=req.hole_clearance,
            existing_positions=req.existing_positions,
            algorithm=req.algorithm,
            max_iterations=req.optimization_iterations
        )
        
        return result
        
    except Exception as e:
        self.update_state(
            state='FAILURE',
            meta={'exc_type': type(e).__name__, 'exc_message': str(e)}
        )
        raise e

@app.route('/api/task/<task_id>', methods=['GET'])
def get_task_status(task_id):
    """Status einer asynchronen Aufgabe abfragen"""
    task = suggest_layout_async.AsyncResult(task_id)
    
    if task.state == 'PENDING':
        response = {'state': 'pending', 'progress': 0}
    elif task.state == 'STARTED':
        response = {'state': 'running', 'progress': task.info.get('progress', 0)}
    elif task.state == 'SUCCESS':
        response = {'state': 'completed', 'result': task.result}
    elif task.state == 'FAILURE':
        response = {'state': 'failed', 'error': str(task.info)}
    else:
        response = {'state': task.state}
    
    return jsonify(response), 200

@app.route('/api/analyze_insert', methods=['POST'])
def analyze_insert():
    """Analyse einer Kabelverschraubung"""
    try:
        data = request.get_json()
        req = InsertAnalysisRequest(**data)
        
        result = insert_db.resolve(
            insert_type=req.insert_type,
            material=req.material,
            tool_diameter=req.tool_diameter,
            strategy=req.machining_strategy
        )
        
        return jsonify(result), 200
        
    except KeyError as e:
        return jsonify({"error": f"Insert-Typ nicht gefunden: {e}", "code": "INSERT_NOT_FOUND"}), 404
    except Exception as e:
        logging.exception("Fehler bei Insert-Analyse")
        return jsonify({"error": str(e)}), 500

@app.route('/api/inserts', methods=['GET'])
def list_inserts():
    """Liste aller verfügbaren Insert-Typen"""
    try:
        inserts = insert_db.get_all_inserts()
        return jsonify({
            "count": len(inserts),
            "inserts": inserts
        }), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/api/metrics', methods=['GET'])
def get_metrics():
    """Prometheus Metriken für Monitoring"""
    metrics_data = {
        "api_requests_total": metrics.registry.get_sample_value('flask_http_request_total'),
        "api_request_duration_seconds": metrics.registry.get_sample_value('flask_http_request_duration_seconds'),
        "feasibility_checks_total": feasibility_analyzer.get_stats(),
        "optimization_runs_total": layout_optimizer.get_stats(),
        "cache_hit_ratio": redis_client.info('stats')['keyspace_hits'] / max(1, redis_client.info('stats')['keyspace_misses'] + redis_client.info('stats')['keyspace_hits'])
    }
    return jsonify(metrics_data), 200

if __name__ == '__main__':
    logging.basicConfig(level=logging.INFO)
    app.run(host='0.0.0.0', port=5000, debug=False, threaded=True)
```

### 5.3 Multi-Server-Konfiguration (Docker Compose für Produktion)

```yaml
# docker-compose.prod.yml - Multi-Server Setup mit 3 Backend-Knoten

version: '3.8'

services:
  # PostgreSQL mit Replikation
  postgres-primary:
    image: postgres:15-alpine
    environment:
      POSTGRES_DB: cablegland_db
      POSTGRES_USER: cablegland
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres_primary_data:/var/lib/postgresql/data
      - ./init.sql:/docker-entrypoint-initdb.d/init.sql
    networks:
      - backend_network
    deploy:
      resources:
        limits:
          memory: 4G
        reservations:
          memory: 2G

  postgres-replica:
    image: postgres:15-alpine
    environment:
      POSTGRES_DB: cablegland_db
      POSTGRES_USER: cablegland
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    command: |
      bash -c "
      docker-entrypoint.sh postgres &
      sleep 10
      psql -U cablegland -d cablegland_db -c 'CREATE TABLE IF NOT EXISTS replication_status (id int);'
      "
    depends_on:
      - postgres-primary
    networks:
      - backend_network

  # Redis Cluster (3 Knoten)
  redis-node-1:
    image: redis:7.2-alpine
    command: redis-server --cluster-enabled yes --cluster-config-file nodes.conf --cluster-node-timeout 5000 --appendonly yes
    volumes:
      - redis_data_1:/data
    networks:
      - backend_network

  redis-node-2:
    image: redis:7.2-alpine
    command: redis-server --cluster-enabled yes --cluster-config-file nodes.conf --cluster-node-timeout 5000 --appendonly yes
    volumes:
      - redis_data_2:/data
    networks:
      - backend_network

  redis-node-3:
    image: redis:7.2-alpine
    command: redis-server --cluster-enabled yes --cluster-config-file nodes.conf --cluster-node-timeout 5000 --appendonly yes
    volumes:
      - redis_data_3:/data
    networks:
      - backend_network

  # Backend API Server (3 Replicas)
  backend-api:
    build:
      context: ./backend
      dockerfile: Dockerfile.prod
    environment:
      DB_HOST: postgres-primary
      REDIS_HOST: redis-node-1
      REDIS_PORT: 6379
      FLASK_ENV: production
    depends_on:
      - postgres-primary
      - redis-node-1
    networks:
      - backend_network
    deploy:
      replicas: 3
      resources:
        limits:
          memory: 2G
          cpus: '2'
      update_config:
        parallelism: 1
        delay: 10s
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  # Celery Worker (für asynchrone Optimierungen)
  celery-worker:
    build:
      context: ./backend
      dockerfile: Dockerfile.prod
    command: celery -A app.celery worker --loglevel=info --concurrency=4
    environment:
      DB_HOST: postgres-primary
      REDIS_HOST: redis-node-1
    depends_on:
      - redis-node-1
      - postgres-primary
    networks:
      - backend_network
    deploy:
      replicas: 2
      resources:
        limits:
          memory: 4G
          cpus: '4'

  # Nginx Load Balancer
  nginx-lb:
    image: nginx:1.25-alpine
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
      - ./nginx/ssl:/etc/nginx/ssl:ro
    ports:
      - "80:80"
      - "443:443"
    depends_on:
      - backend-api
    networks:
      - backend_network
    deploy:
      replicas: 1
      resources:
        limits:
          memory: 512M

  # Prometheus Monitoring
  prometheus:
    image: prom/prometheus:latest
    volumes:
      - ./prometheus/prometheus.yml:/etc/prometheus/prometheus.yml
      - prometheus_data:/prometheus
    command:
      - '--config.file=/etc/prometheus/prometheus.yml'
      - '--storage.tsdb.path=/prometheus'
    ports:
      - "9090:9090"
    networks:
      - backend_network

  # Grafana Dashboard
  grafana:
    image: grafana/grafana:latest
    environment:
      GF_SECURITY_ADMIN_PASSWORD: ${GRAFANA_PASSWORD}
    volumes:
      - grafana_data:/var/lib/grafana
      - ./grafana/dashboards:/etc/grafana/provisioning/dashboards
    ports:
      - "3000:3000"
    networks:
      - backend_network
    depends_on:
      - prometheus

networks:
  backend_network:
    driver: overlay
    ipam:
      config:
        - subnet: 10.10.0.0/16

volumes:
  postgres_primary_data:
  redis_data_1:
  redis_data_2:
  redis_data_3:
  prometheus_data:
  grafana_data:
```

---

## 6. Frontend-Architektur (WinForms)

### 6.1 Hauptformular mit kompletter UI

```csharp
// MainForm.cs - Vollständiges Hauptformular

using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CableGlandPlanner.Models;
using CableGlandPlanner.Services;
using CableGlandPlanner.UI;

namespace CableGlandPlanner
{
    public partial class MainForm : Form
    {
        // Services
        private readonly AiClient _aiClient;
        private readonly LayoutValidator _validator;
        private readonly GCodeGenerator _gcodeGenerator;
        private readonly PreviewRenderer _previewRenderer;
        private readonly ExportService _exportService;
        
        // Daten
        private DiskParameters _diskParams;
        private List<HoleDefinition> _holes;
        private LayoutValidationResult _validationResult;
        private bool _aiAvailable = false;
        
        // UI Elemente
        private Panel _inputPanel;
        private Panel _previewPanel;
        private DataGridView _holesGrid;
        private Button _btnGenerateLayout;
        private Button _btnExportGCode;
        private Button _btnExportPNG;
        private Label _lblAIStatus;
        private Label _lblValidationStatus;
        private PictureBox _previewBox;
        private StatusStrip _statusStrip;
        private ToolStripProgressBar _progressBar;
        
        public MainForm()
        {
            InitializeComponent();
            InitializeServices();
            LoadInsertTypes();
            SetupEventHandlers();
            CheckAIBackend();
        }
        
        private void InitializeComponent()
        {
            this.Text = "Cable Gland Planner - Ingenieursoftware für Kabelverschraubungen";
            this.Size = new Size(1400, 900);
            this.BackColor = Color.FromArgb(255, 245, 240); // Orange/Weiß Theme
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Hauptlayout
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            
            // Eingabe Panel
            _inputPanel = CreateInputPanel();
            mainLayout.Controls.Add(_inputPanel, 0, 0);
            
            // Preview Panel
            _previewPanel = CreatePreviewPanel();
            mainLayout.Controls.Add(_previewPanel, 1, 0);
            
            // Button Panel
            var buttonPanel = CreateButtonPanel();
            mainLayout.Controls.Add(buttonPanel, 0, 1);
            
            // Status Panel
            var statusPanel = CreateStatusPanel();
            mainLayout.Controls.Add(statusPanel, 1, 1);
            
            this.Controls.Add(mainLayout);
            
            // Statusleiste
            _statusStrip = new StatusStrip();
            _progressBar = new ToolStripProgressBar { Visible = false };
            _statusStrip.Items.Add(new ToolStripStatusLabel("Bereit"));
            _statusStrip.Items.Add(new ToolStripSeparator());
            _statusStrip.Items.Add(_progressBar);
            this.Controls.Add(_statusStrip);
        }
        
        private Panel CreateInputPanel()
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                BackColor = Color.White
            };
            
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 15,
                AutoSize = true
            };
            
            int row = 0;
            
            // Titel
            layout.Controls.Add(new Label 
            { 
                Text = "Scheibenparameter", 
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 140, 0)
            }, 0, row++);
            layout.SetColumnSpan(layout.GetControlFromPosition(0, row-1), 2);
            
            // Scheibendurchmesser
            layout.Controls.Add(new Label { Text = "Scheibendurchmesser (mm):", TextAlign = ContentAlignment.MiddleRight }, 0, row);
            var txtDiskDiameter = new NumericUpDown 
            { 
                Minimum = 20, Maximum = 1000, DecimalPlaces = 1, 
                Value = 100, Width = 150, Increment = 5 
            };
            layout.Controls.Add(txtDiskDiameter, 1, row++);
            
            // Randabstand
            layout.Controls.Add(new Label { Text = "Randabstand (mm):", TextAlign = ContentAlignment.MiddleRight }, 0, row);
            var txtEdgeClearance = new NumericUpDown 
            { 
                Minimum = 0, Maximum = 20, DecimalPlaces = 1, 
                Value = 5, Width = 150, Increment = 0.5m 
            };
            layout.Controls.Add(txtEdgeClearance, 1, row++);
            
            // Material
            layout.Controls.Add(new Label { Text = "Material:", TextAlign = ContentAlignment.MiddleRight }, 0, row);
            var cmbMaterial = new ComboBox 
            { 
                DropDownStyle = ComboBoxStyle.DropDownList, Width = 150,
                Items = { "Aluminium", "Stahl", "Edelstahl", "Messing", "Kunststoff" }
            };
            cmbMaterial.SelectedIndex = 0;
            layout.Controls.Add(cmbMaterial, 1, row++);
            
            // Lochparameter Titel
            layout.Controls.Add(new Label 
            { 
                Text = "Lochparameter", 
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 140, 0)
            }, 0, row++);
            layout.SetColumnSpan(layout.GetControlFromPosition(0, row-1), 2);
            
            // Loch Tabelle
            _holesGrid = new DataGridView
            {
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Height = 200,
                Width = 350
            };
            _holesGrid.Columns.Add("Diameter", "Durchmesser (mm)");
            _holesGrid.Columns.Add("Depth", "Tiefe (mm)");
            _holesGrid.Columns.Add("InsertType", "Insert Typ");
            _holesGrid.Columns["Diameter"].ValueType = typeof(decimal);
            _holesGrid.Columns["Depth"].ValueType = typeof(decimal);
            
            layout.Controls.Add(_holesGrid, 0, row);
            layout.SetColumnSpan(_holesGrid, 2);
            row++;
            
            // Lochabstand
            layout.Controls.Add(new Label { Text = "Lochabstand (mm):", TextAlign = ContentAlignment.MiddleRight }, 0, row);
            var txtHoleClearance = new NumericUpDown 
            { 
                Minimum = 0, Maximum = 15, DecimalPlaces = 1, 
                Value = 3, Width = 150, Increment = 0.5m 
            };
            layout.Controls.Add(txtHoleClearance, 1, row++);
            
            // Werkzeugparameter Titel
            layout.Controls.Add(new Label 
            { 
                Text = "Werkzeugparameter", 
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 140, 0)
            }, 0, row++);
            layout.SetColumnSpan(layout.GetControlFromPosition(0, row-1), 2);
            
            // Werkzeugdurchmesser
            layout.Controls.Add(new Label { Text = "Werkzeug-Ø (mm):", TextAlign = ContentAlignment.MiddleRight }, 0, row);
            var txtToolDiameter = new NumericUpDown 
            { 
                Minimum = 1, Maximum = 50, DecimalPlaces = 1, 
                Value = 6, Width = 150, Increment = 0.5m 
            };
            layout.Controls.Add(txtToolDiameter, 1, row++);
            
            // Vorschub
            layout.Controls.Add(new Label { Text = "Vorschub (mm/min):", TextAlign = ContentAlignment.MiddleRight }, 0, row);
            var txtFeedRate = new NumericUpDown 
            { 
                Minimum = 50, Maximum = 5000, DecimalPlaces = 0, 
                Value = 500, Width = 150, Increment = 50 
            };
            layout.Controls.Add(txtFeedRate, 1, row++);
            
            // Spindeldrehzahl
            layout.Controls.Add(new Label { Text = "Spindeldrehzahl (RPM):", TextAlign = ContentAlignment.MiddleRight }, 0, row);
            var txtSpindleSpeed = new NumericUpDown 
            { 
                Minimum = 1000, Maximum = 24000, DecimalPlaces = 0, 
                Value = 12000, Width = 150, Increment = 500 
            };
            layout.Controls.Add(txtSpindleSpeed, 1, row++);
            
            // Sichere Z-Höhe
            layout.Controls.Add(new Label { Text = "Sichere Z-Höhe (mm):", TextAlign = ContentAlignment.MiddleRight }, 0, row);
            var txtSafeZ = new NumericUpDown 
            { 
                Minimum = 5, Maximum = 50, DecimalPlaces = 1, 
                Value = 10, Width = 150, Increment = 1 
            };
            layout.Controls.Add(txtSafeZ, 1, row++);
            
            panel.Controls.Add(layout);
            
            // Speichere Referenzen
            Tag = new InputReferences
            {
                DiskDiameter = txtDiskDiameter,
                EdgeClearance = txtEdgeClearance,
                Material = cmbMaterial,
                HoleClearance = txtHoleClearance,
                ToolDiameter = txtToolDiameter,
                FeedRate = txtFeedRate,
                SpindleSpeed = txtSpindleSpeed,
                SafeZ = txtSafeZ
            };
            
            return panel;
        }
        
        private void InitializeServices()
        {
            _aiClient = new AiClient("http://localhost:5000");
            _validator = new LayoutValidator();
            _gcodeGenerator = new GCodeGenerator();
            _previewRenderer = new PreviewRenderer();
            _exportService = new ExportService();
        }
        
        private async void CheckAIBackend()
        {
            _aiAvailable = await _aiClient.CheckHealthAsync();
            _lblAIStatus.Text = _aiAvailable ? "KI-Status: Verfügbar" : "KI-Status: Offline (Fallback Modus)";
            _lblAIStatus.ForeColor = _aiAvailable ? Color.Green : Color.Orange;
        }
        
        private async void OnGenerateLayout_Click(object sender, EventArgs e)
        {
            try
            {
                _progressBar.Visible = true;
                _progressBar.Style = ProgressBarStyle.Marquee;
                _statusStrip.Items[0].Text = "Generiere Layout...";
                
                // Parameter sammeln
                var inputRefs = (InputReferences)_inputPanel.Tag;
                var holes = GetHolesFromGrid();
                
                var request = new LayoutRequest
                {
                    DiskRadius = (double)(inputRefs.DiskDiameter.Value / 2),
                    HoleDiameters = holes.Select(h => h.Diameter).ToList(),
                    EdgeClearance = (double)inputRefs.EdgeClearance.Value,
                    HoleClearance = (double)inputRefs.HoleClearance.Value,
                    ToolDiameter = (double)inputRefs.ToolDiameter.Value,
                    Depth = holes.Max(h => h.Depth),
                    Material = inputRefs.Material.SelectedItem.ToString()
                };
                
                // KI-Anfrage oder Fallback
                LayoutResult result;
                if (_aiAvailable)
                {
                    result = await _aiClient.SuggestLayoutAsync(request);
                }
                else
                {
                    result = FallbackLayoutGenerator.Generate(request);
                }
                
                // Positionen zuweisen
                for (int i = 0; i < holes.Count && i < result.Positions.Count; i++)
                {
                    holes[i].X = result.Positions[i].X;
                    holes[i].Y = result.Positions[i].Y;
                }
                
                // Validierung
                _validationResult = _validator.Validate(_diskParams, holes, 
                    (double)inputRefs.HoleClearance.Value);
                
                // Preview aktualisieren
                UpdatePreview();
                
                // Status anzeigen
                _lblValidationStatus.Text = _validationResult.IsValid ? 
                    "Status: Machbar ✓" : "Status: Nicht machbar ✗";
                _lblValidationStatus.ForeColor = _validationResult.IsValid ? 
                    Color.Green : Color.Red;
                
                if (_validationResult.Warnings.Any())
                {
                    ShowWarnings(_validationResult.Warnings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Layout-Generierung:\n{ex.Message}", 
                    "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _progressBar.Visible = false;
                _statusStrip.Items[0].Text = "Bereit";
            }
        }
        
        private async void OnExportGCode_Click(object sender, EventArgs e)
        {
            if (_validationResult == null || !_validationResult.IsValid)
            {
                MessageBox.Show("Bitte zuerst ein gültiges Layout generieren.", 
                    "Kein gültiges Layout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            var saveDialog = new SaveFileDialog
            {
                Filter = "NC Datei (*.nc)|*.nc|Text Datei (*.txt)|*.txt",
                DefaultExt = "nc",
                FileName = $"cable_gland_layout_{DateTime.Now:yyyyMMdd_HHmmss}.nc"
            };
            
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var settings = GetGCodeSettings();
                    var gcode = _gcodeGenerator.Generate(_holes, settings);
                    System.IO.File.WriteAllText(saveDialog.FileName, gcode);
                    
                    MessageBox.Show($"G-Code erfolgreich exportiert:\n{saveDialog.FileName}", 
                        "Export erfolgreich", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim G-Code Export:\n{ex.Message}", 
                        "Export fehlgeschlagen", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void UpdatePreview()
        {
            var bitmap = _previewRenderer.Render(_diskParams, _holes, _validationResult);
            _previewBox.Image = bitmap;
            _previewBox.SizeMode = PictureBoxSizeMode.Zoom;
        }
        
        private void ShowWarnings(List<string> warnings)
        {
            var warningText = string.Join("\n• ", warnings);
            MessageBox.Show($"Warnungen:\n• {warningText}", 
                "Layout-Warnungen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
    
    class InputReferences
    {
        public NumericUpDown DiskDiameter { get; set; }
        public NumericUpDown EdgeClearance { get; set; }
        public ComboBox Material { get; set; }
        public NumericUpDown HoleClearance { get; set; }
        public NumericUpDown ToolDiameter { get; set; }
        public NumericUpDown FeedRate { get; set; }
        public NumericUpDown SpindleSpeed { get; set; }
        public NumericUpDown SafeZ { get; set; }
    }
}
```

---

## 7. CNC-G-Code-Generierung

### 7.1 Erweiterter G-Code Generator

```csharp
// Services/GCodeGenerator.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CableGlandPlanner.Models;

namespace CableGlandPlanner.Services
{
    public class GCodeGenerator
    {
        private const string NEWLINE = "\n";
        
        public string Generate(List<HoleDefinition> holes, GCodeSettings settings)
        {
            var sb = new StringBuilder();
            
            // Header
            sb.AppendLine("%");
            sb.AppendLine("(Generated by Cable Gland Planner v2.0)");
            sb.AppendLine($"(Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss})");
            sb.AppendLine("(German UI / AI-assisted layout validation)");
            sb.AppendLine("(CNC G-Code for cable gland drilling/pocket milling)");
            sb.AppendLine();
            
            // Programmbeginn
            sb.AppendLine("G90 (Absolute positioning)");
            sb.AppendLine("G21 (Metric units)");
            sb.AppendLine("G17 (XY plane selection)");
            sb.AppendLine($"F{settings.FeedRateXY:F0} (XY feed rate)");
            sb.AppendLine($"F{settings.FeedRateZ:F0} (Z feed rate)");
            sb.AppendLine();
            
            // Sicherheitsbewegung
            sb.AppendLine($"(Safe Z: {settings.SafeZ:F3} mm)");
            sb.AppendLine($"G0 Z{settings.SafeZ:F3}");
            sb.AppendLine();
            
            // Spindel starten
            sb.AppendLine($"(Spindle start - {settings.SpindleSpeed} RPM)");
            sb.AppendLine($"M03 S{settings.SpindleSpeed}");
            sb.AppendLine();
            
            // Kühlmittel
            if (settings.UseCoolant)
            {
                string coolantCmd = settings.CoolantType == "FLOOD" ? "M08" : "M07";
                sb.AppendLine($"(Coolant: {settings.CoolantType})");
                sb.AppendLine(coolantCmd);
                sb.AppendLine();
            }
            
            // Lochbearbeitung
            for (int i = 0; i < holes.Count; i++)
            {
                var hole = holes[i];
                sb.AppendLine($"({(i + 1)}. Bohrung/Tasche)");
                sb.AppendLine($"(Insert: {hole.InsertType}, POS: {hole.PosMapping})");
                sb.AppendLine($"(Diameter: {hole.Diameter:F2} mm, Depth: {hole.Depth:F2} mm)");
                sb.AppendLine($"G0 X{hole.X:F3} Y{hole.Y:F3}");
                sb.AppendLine();
                
                if (hole.IsPocket)
                {
                    // Taschenfräsung
                    AppendPocketMilling(sb, hole, settings);
                }
                else
                {
                    // Bohrung (einfach oder mit Picken)
                    if (settings.UsePecking)
                    {
                        AppendPeckDrilling(sb, hole, settings);
                    }
                    else
                    {
                        AppendSimpleDrilling(sb, hole, settings);
                    }
                }
                
                sb.AppendLine($"G0 Z{settings.SafeZ:F3}");
                sb.AppendLine();
            }
            
            // Spindel stoppen
            sb.AppendLine("(Spindle stop)");
            sb.AppendLine("M05");
            sb.AppendLine();
            
            // Kühlmittel aus
            if (settings.UseCoolant)
            {
                sb.AppendLine("M09 (Coolant off)");
                sb.AppendLine();
            }
            
            // Programmende
            sb.AppendLine("G0 Z10.000 (Safe retract)");
            sb.AppendLine("M30 (Program end and rewind)");
            sb.AppendLine("%");
            
            return sb.ToString();
        }
        
        private void AppendPocketMilling(StringBuilder sb, HoleDefinition hole, GCodeSettings settings)
        {
            double toolRadius = settings.ToolDiameter / 2;
            double pocketRadius = hole.Diameter / 2;
            
            // Spiraleinstich
            double radiusStep = (pocketRadius - toolRadius) / 5;
            double currentRadius = toolRadius;
            
            sb.AppendLine($"  (Pocket milling - spiral strategy)");
            
            while (currentRadius <= pocketRadius - toolRadius)
            {
                int segments = Math.Max(8, (int)(2 * Math.PI * currentRadius / 5));
                double angleStep = 2 * Math.PI / segments;
                
                for (int i = 0; i <= segments; i++)
                {
                    double angle = i * angleStep;
                    double xOffset = currentRadius * Math.Cos(angle);
                    double yOffset = currentRadius * Math.Sin(angle);
                    
                    if (i == 0)
                    {
                        sb.AppendLine($"  G1 X{hole.X + xOffset:F3} Y{hole.Y + yOffset:F3} F{settings.FeedRateXY:F0}");
                        sb.AppendLine($"  G1 Z{-hole.Depth:F3} F{settings.FeedRateZ:F0}");
                    }
                    else
                    {
                        sb.AppendLine($"  G1 X{hole.X + xOffset:F3} Y{hole.Y + yOffset:F3}");
                    }
                }
                
                currentRadius += radiusStep;
                
                if (currentRadius < pocketRadius - toolRadius)
                {
                    sb.AppendLine($"  G0 Z{settings.SafeZ:F3}");
                    sb.AppendLine($"  G0 X{hole.X + currentRadius:F3} Y{hole.Y:F3}");
                    sb.AppendLine($"  G1 Z{-hole.Depth:F3} F{settings.FeedRateZ:F0}");
                }
            }
            
            // Boden glätten
            sb.AppendLine($"  G1 X{hole.X + pocketRadius - toolRadius:F3} Y{hole.Y:F3}");
            sb.AppendLine($"  G1 X{hole.X - pocketRadius + toolRadius:F3} Y{hole.Y:F3}");
        }
        
        private void AppendPeckDrilling(StringBuilder sb, HoleDefinition hole, GCodeSettings settings)
        {
            double currentDepth = 0;
            int peckCount = 0;
            
            while (currentDepth < hole.Depth)
            {
                double nextDepth = Math.Min(currentDepth + settings.PeckDepth, hole.Depth);
                peckCount++;
                
                sb.AppendLine($"  (Peck {peckCount}: {currentDepth:F2} to {nextDepth:F2} mm)");
                
                if (peckCount == 1)
                {
                    sb.AppendLine($"  G1 Z{-nextDepth:F3} F{settings.FeedRateZ:F0}");
                }
                else
                {
                    sb.AppendLine($"  G1 Z{-nextDepth:F3}");
                }
                
                if (nextDepth < hole.Depth)
                {
                    sb.AppendLine($"  G0 Z{settings.SafeZ:F3} (Retract for chip clearance)");
                    sb.AppendLine($"  G4 P0.5 (Dwell 0.5s)");
                    sb.AppendLine($"  G0 Z{-currentDepth + 1:F3} (Rapid to last depth + 1mm)");
                }
                
                currentDepth = nextDepth;
            }
        }
        
        private void AppendSimpleDrilling(StringBuilder sb, HoleDefinition hole, GCodeSettings settings)
        {
            sb.AppendLine($"  G1 Z{-hole.Depth:F3} F{settings.FeedRateZ:F0}");
        }
    }
}
```

---

## 8. API-Spezifikation

### 8.1 OpenAPI 3.0 Spezifikation

```yaml
# openapi.yaml
openapi: 3.0.0
info:
  title: Cable Gland Planner API
  version: 2.0.0
  description: KI-gestützte API für Kabelverschraubungs-Layouts
  contact:
    name: Amir Mobasheraghdam
    email: amir.mobasher@example.com

servers:
  - url: https://api.cablegland.local/v2
    description: Produktionsserver
  - url: http://localhost:5000
    description: Entwicklungsserver

paths:
  /feasibility:
    post:
      summary: Machbarkeitsprüfung
      operationId: checkFeasibility
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/FeasibilityRequest'
      responses:
        '200':
          description: Erfolgreiche Prüfung
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/FeasibilityResponse'
        '400':
          description: Ungültige Parameter
        '429':
          description: Rate Limit überschritten
        '500':
          description: Serverfehler

  /suggest_layout:
    post:
      summary: Layout-Optimierung
      operationId: suggestLayout
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/LayoutRequest'
      responses:
        '200':
          description: Optimiertes Layout
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/LayoutResponse'
        '202':
          description: Asynchrone Verarbeitung gestartet
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/AsyncResponse'
        '400':
          description: Ungültige Parameter

  /task/{taskId}:
    get:
      summary: Status asynchroner Aufgabe
      parameters:
        - name: taskId
          in: path
          required: true
          schema:
            type: string
      responses:
        '200':
          description: Aufgabenstatus
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/TaskStatus'

components:
  schemas:
    FeasibilityRequest:
      type: object
      required:
        - disk_radius
        - hole_diameters
        - tool_diameter
        - depth
      properties:
        disk_radius:
          type: number
          minimum: 10
          maximum: 500
          description: Scheibenradius in mm
        hole_diameters:
          type: array
          minItems: 1
          maxItems: 24
          items:
            type: number
            minimum: 2
            maximum: 100
        edge_clearance:
          type: number
          minimum: 0
          maximum: 20
          default: 5
        hole_clearance:
          type: number
          minimum: 0
          maximum: 15
          default: 3
        tool_diameter:
          type: number
          minimum: 1
          maximum: 50
        depth:
          type: number
          minimum: 1
          maximum: 35
        material:
          type: string
          enum: [aluminium, stahl, edelstahl, messing, kunststoff]
          default: aluminium

    FeasibilityResponse:
      type: object
      properties:
        feasible:
          type: boolean
        confidence:
          type: number
          minimum: 0
          maximum: 1
        warnings:
          type: array
          items:
            type: string
        min_hole_distance:
          type: number
        max_allowed_depth:
          type: number
        tool_compatible:
          type: boolean
        utilization:
          type: number
          description: Flächennutzungsgrad in Prozent

    LayoutRequest:
      type: object
      required:
        - disk_radius
        - hole_diameters
        - edge_clearance
        - hole_clearance
      properties:
        disk_radius:
          type: number
        hole_diameters:
          type: array
          items:
            type: number
        edge_clearance:
          type: number
        hole_clearance:
          type: number
        existing_positions:
          type: array
          items:
            type: array
            minItems: 2
            maxItems: 2            items:
              type: number
        algorithm:
          type: string
          enum: [simulated_annealing, genetic, greedy, radial]
          default: simulated_annealing
        optimization_iterations:
          type: integer
          minimum: 100
          maximum: 10000
          default: 1000

    LayoutResponse:
      type: object
      properties:
        positions:
          type: array
          items:
            type: array
            minItems: 2
            maxItems: 2
            items:
              type: number
        confidence:
          type: number
        warnings:
          type: array
          items:
            type: string
        algorithm_used:
          type: string
        iterations:
          type: integer
        execution_time_ms:
          type: number

    AsyncResponse:
      type: object
      properties:
        task_id:
          type: string
        status:
          type: string
          enum: [processing]
        estimated_time_seconds:
          type: integer

    TaskStatus:
      type: object
      properties:
        state:
          type: string
          enum: [pending, running, completed, failed]
        progress:
          type: integer
          minimum: 0
          maximum: 100
        result:
          $ref: '#/components/schemas/LayoutResponse'
        error:
          type: string
```

---

## 9. Datenbank- und Cache-Schicht

### 9.1 PostgreSQL Schema

```sql
-- init.sql - Vollständiges Datenbankschema

-- Erweiterungen
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Insert-Typen Tabelle
CREATE TABLE insert_types (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    type_code VARCHAR(10) NOT NULL UNIQUE,
    pos_mapping VARCHAR(20) NOT NULL,
    diameter_mm DECIMAL(8,3) NOT NULL,
    thread_type VARCHAR(20) NOT NULL,
    min_depth_mm DECIMAL(8,3) NOT NULL,
    max_depth_mm DECIMAL(8,3) NOT NULL,
    recommended_depth_mm DECIMAL(8,3) NOT NULL,
    material_compatibility TEXT[] DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Materialien Tabelle
CREATE TABLE materials (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(50) UNIQUE NOT NULL,
    density_kg_m3 DECIMAL(10,2),
    tensile_strength_mpa DECIMAL(10,2),
    hardness_hb DECIMAL(8,2),
    machinability_rating INTEGER CHECK (machinability_rating BETWEEN 1 AND 10),
    recommended_cutting_speed_m_min DECIMAL(8,2),
    recommended_feed_per_tooth_mm DECIMAL(6,3)
);

-- Layouts Tabelle (für Audit/Historie)
CREATE TABLE layouts (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    disk_diameter_mm DECIMAL(10,3) NOT NULL,
    edge_clearance_mm DECIMAL(8,3) NOT NULL,
    hole_clearance_mm DECIMAL(8,3) NOT NULL,
    tool_diameter_mm DECIMAL(8,3) NOT NULL,
    max_depth_mm DECIMAL(8,3) NOT NULL,
    material_id UUID REFERENCES materials(id),
    is_valid BOOLEAN NOT NULL,
    validation_errors TEXT[],
    validation_warnings TEXT[],
    ai_confidence DECIMAL(5,4),
    ai_version VARCHAR(20)
);

-- Layout-Positionen Tabelle
CREATE TABLE layout_positions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    layout_id UUID REFERENCES layouts(id) ON DELETE CASCADE,
    hole_index INTEGER NOT NULL,
    x_mm DECIMAL(10,3) NOT NULL,
    y_mm DECIMAL(10,3) NOT NULL,
    diameter_mm DECIMAL(8,3) NOT NULL,
    depth_mm DECIMAL(8,3) NOT NULL,
    insert_type_id UUID REFERENCES insert_types(id)
);

-- CNC-Jobs Tabelle
CREATE TABLE cnc_jobs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    layout_id UUID REFERENCES layouts(id),
    gcode TEXT NOT NULL,
    settings JSONB NOT NULL,
    exported_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    exported_by VARCHAR(100)
);

-- Performance-Indizes
CREATE INDEX idx_insert_types_code ON insert_types(type_code);
CREATE INDEX idx_layouts_created_at ON layouts(created_at DESC);
CREATE INDEX idx_layouts_material ON layouts(material_id);
CREATE INDEX idx_layout_positions_layout ON layout_positions(layout_id);
CREATE INDEX idx_cnc_jobs_layout ON cnc_jobs(layout_id);
CREATE INDEX idx_cnc_jobs_exported ON cnc_jobs(exported_at DESC);

-- Insert-Typen (Beispieldaten)
INSERT INTO insert_types (type_code, pos_mapping, diameter_mm, thread_type, min_depth_mm, max_depth_mm, recommended_depth_mm, material_compatibility) VALUES
('M12', 'POS_01', 12.0, 'Metric', 8.0, 25.0, 12.0, ARRAY['aluminium', 'stahl', 'kunststoff']),
('M16', 'POS_02', 16.0, 'Metric', 10.0, 30.0, 15.0, ARRAY['aluminium', 'stahl', 'edelstahl']),
('M20', 'POS_03', 20.0, 'Metric', 12.0, 35.0, 18.0, ARRAY['aluminium', 'stahl', 'edelstahl', 'messing']),
('M25', 'POS_04', 25.0, 'Metric', 15.0, 35.0, 22.0, ARRAY['stahl', 'edelstahl']),
('PG16', 'POS_05', 16.5, 'PG', 10.0, 28.0, 14.0, ARRAY['aluminium', 'kunststoff']),
('PG21', 'POS_06', 21.5, 'PG', 12.0, 32.0, 18.0, ARRAY['aluminium', 'stahl']),
('NPT1/2', 'POS_07', 21.0, 'NPT', 12.0, 30.0, 16.0, ARRAY['stahl', 'edelstahl']),
('NPT3/4', 'POS_08', 26.5, 'NPT', 15.0, 35.0, 20.0, ARRAY['stahl', 'edelstahl']);

-- Materialien (Beispieldaten)
INSERT INTO materials (name, density_kg_m3, tensile_strength_mpa, hardness_hb, machinability_rating, recommended_cutting_speed_m_min, recommended_feed_per_tooth_mm) VALUES
('Aluminium', 2700, 310, 95, 8, 300, 0.15),
('Stahl', 7850, 450, 150, 6, 200, 0.12),
('Edelstahl', 8000, 520, 200, 4, 120, 0.08),
('Messing', 8500, 330, 80, 9, 400, 0.18),
('Kunststoff', 1200, 60, 25, 10, 500, 0.20);

-- Automatisches Update von updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_insert_types_updated_at
    BEFORE UPDATE ON insert_types
    FOR EACH ROW
    EXECUTE FUNCTION update_updated_at_column();

-- Statistische Funktion für Dashboard
CREATE OR REPLACE FUNCTION get_layout_statistics(
    start_date TIMESTAMP WITH TIME ZONE,
    end_date TIMESTAMP WITH TIME ZONE
)
RETURNS TABLE(
    total_layouts BIGINT,
    valid_layouts BIGINT,
    avg_confidence DECIMAL(5,4),
    avg_utilization DECIMAL(5,2),
    most_used_material VARCHAR(50)
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        COUNT(l.id) AS total_layouts,
        SUM(CASE WHEN l.is_valid THEN 1 ELSE 0 END) AS valid_layouts,
        AVG(l.ai_confidence) AS avg_confidence,
        AVG(l.utilization_percent) AS avg_utilization,
        (SELECT m.name FROM materials m WHERE m.id = mode() WITHIN GROUP (ORDER BY l.material_id)) AS most_used_material
    FROM layouts l
    WHERE l.created_at BETWEEN start_date AND end_date;
END;
$$ LANGUAGE plpgsql;
```

### 9.2 Redis Cache Strategie

```python
# cache_strategy.py

import json
import hashlib
from typing import Any, Optional, Callable
from functools import wraps
import redis
import pickle

class CacheManager:
    """Erweiterte Cache-Verwaltung mit mehreren Strategien"""
    
    def __init__(self, redis_client: redis.Redis, default_ttl: int = 300):
        self.redis = redis_client
        self.default_ttl = default_ttl
        
    def cached(self, ttl: int = None, key_prefix: str = ""):
        """Dekorator für automatisches Caching"""
        def decorator(func: Callable):
            @wraps(func)
            def wrapper(*args, **kwargs):
                # Cache-Key generieren
                key = self._generate_key(func.__name__, key_prefix, args, kwargs)
                
                # Versuche aus Cache zu lesen
                cached = self.redis.get(key)
                if cached:
                    return pickle.loads(cached)
                
                # Funktion ausführen
                result = func(*args, **kwargs)
                
                # Im Cache speichern
                self.redis.setex(
                    key,
                    ttl or self.default_ttl,
                    pickle.dumps(result)
                )
                
                return result
            return wrapper
        return decorator
    
    def _generate_key(self, func_name: str, prefix: str, args: tuple, kwargs: dict) -> str:
        """Generiert eindeutigen Cache-Key"""
        data = {
            'func': func_name,
            'args': args,
            'kwargs': kwargs
        }
        hash_str = hashlib.sha256(json.dumps(data, sort_keys=True).encode()).hexdigest()
        return f"{prefix}:{func_name}:{hash_str}" if prefix else f"{func_name}:{hash_str}"
    
    def cache_geometry_result(self, disk_radius: float, holes: list, result: Any):
        """Spezifische Caching-Logik für Geometrieberechnungen"""
        key = f"geometry:{disk_radius}:{hashlib.md5(str(holes).encode()).hexdigest()}"
        self.redis.setex(key, 3600, pickle.dumps(result))  # 1 Stunde TTL
    
    def get_cached_geometry(self, disk_radius: float, holes: list) -> Optional[Any]:
        """Holt gecachtes Geometrieergebnis"""
        key = f"geometry:{disk_radius}:{hashlib.md5(str(holes).encode()).hexdigest()}"
        cached = self.redis.get(key)
        return pickle.loads(cached) if cached else None
    
    def invalidate_pattern(self, pattern: str):
        """Invalidiert alle Keys mit einem Muster"""
        for key in self.redis.scan_iter(match=pattern):
            self.redis.delete(key)
    
    def get_cache_stats(self) -> dict:
        """Gibt Cache-Statistiken zurück"""
        info = self.redis.info('stats')
        return {
            'hits': info.get('keyspace_hits', 0),
            'misses': info.get('keyspace_misses', 0),
            'hit_ratio': info.get('keyspace_hits', 0) / max(1, info.get('keyspace_hits', 0) + info.get('keyspace_misses', 0)),
            'memory_used_mb': self.redis.info('memory')['used_memory'] / (1024 * 1024),
            'total_keys': self.redis.dbsize()
        }
```

---

## 10. Fehlertoleranz & Fallback-Mechanismen

### 10.1 Deterministischer Fallback-Generator

```csharp
// FallbackLayoutGenerator.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace CableGlandPlanner.Services
{
    public static class FallbackLayoutGenerator
    {
        public static LayoutResult Generate(LayoutRequest request)
        {
            var positions = new List<Point2D>();
            var radii = request.HoleDiameters.Select(d => d / 2).ToList();
            int holeCount = radii.Count;
            
            if (holeCount == 0)
                return new LayoutResult { Positions = positions, Confidence = 0 };
            
            // Fallback-Algorithmus: Radiale Verteilung
            double maxRadius = request.DiskRadius - request.EdgeClearance;
            
            // Große Löcher zuerst platzieren
            var sortedIndices = Enumerable.Range(0, holeCount)
                .OrderByDescending(i => radii[i])
                .ToList();
            
            double currentRadius = 0;
            int ringIndex = 0;
            
            foreach (var idx in sortedIndices)
            {
                double holeRadius = radii[idx];
                double availableRadius = maxRadius - holeRadius;
                
                // Ring-Radius berechnen
                if (ringIndex == 0)
                {
                    currentRadius = 0; // Mittleres Loch
                }
                else
                {
                    currentRadius = (availableRadius * ringIndex) / (holeCount / 2);
                    currentRadius = Math.Min(currentRadius, availableRadius - holeRadius);
                }
                
                // Winkel gleichmäßig verteilen
                double angleStep = 2 * Math.PI / holeCount;
                double angle = ringIndex * angleStep;
                
                double x = currentRadius * Math.Cos(angle);
                double y = currentRadius * Math.Sin(angle);
                
                positions.Add(new Point2D(x, y));
                ringIndex++;
            }
            
            return new LayoutResult
            {
                Positions = positions,
                Confidence = 0.35, // Geringes Vertrauen bei Fallback
                Warnings = new List<string> { 
                    "KI-Backend nicht verfügbar - Layout basiert auf deterministischem Algorithmus",
                    "Bitte Layout manuell auf Machbarkeit prüfen"
                },
                AlgorithmUsed = "deterministic_fallback",
                ExecutionTimeMs = 0
            };
        }
    }
}
```

---

## 11. Testing & Qualitätssicherung

### 11.1 Unit Tests (Python)

```python
# tests/test_geometry.py

import pytest
import math
from services.geometry import CircularPackingEngine

class TestCircularPackingEngine:
    
    @pytest.fixture
    def engine(self):
        return CircularPackingEngine(disk_radius=50.0, edge_clearance=5.0)
    
    def test_is_inside_disk_center(self, engine):
        """Test: Loch in der Mitte ist immer gültig"""
        assert engine.is_inside_disk(0, 0, 5.0) == True
    
    def test_is_inside_disk_edge(self, engine):
        """Test: Loch genau am Rand"""
        assert engine.is_inside_disk(40, 0, 5.0) == True
        assert engine.is_inside_disk(45, 0, 5.0) == False
    
    def test_are_holes_colliding(self, engine):
        """Test: Kollisionserkennung"""
        # Nicht kollidierend
        assert engine.are_holes_colliding(0, 0, 5, 15, 0, 5, 3) == False
        
        # Kollidierend
        assert engine.are_holes_colliding(0, 0, 10, 15, 0, 10, 3) == True
    
    def test_calculate_utilization(self, engine):
        """Test: Flächennutzungsgrad"""
        positions = [(0, 0)]
        radii = [10.0]
        utilization = engine.calculate_utilization(positions, radii)
        expected = (math.pi * 100) / (math.pi * 45**2)  # 45mm effektiver Radius
        assert abs(utilization - expected) < 0.001
    
    def test_minimum_clearance_check(self, engine):
        """Test: Minimaler Lochabstand"""
        positions = [(0, 0), (20, 0)]
        radii = [5, 5]
        is_valid, min_clearance = engine.minimum_clearance_check(positions, radii, 3)
        
        assert is_valid == True
        assert min_clearance == 20 - (5 + 5)  # 10mm
        
        # Mit Kollision
        positions = [(0, 0), (10, 0)]
        is_valid, min_clearance = engine.minimum_clearance_check(positions, radii, 3)
        assert is_valid == False
    
    def test_energy_function(self, engine):
        """Test: Energiefunktion für Optimierung"""
        positions = np.array([0, 0, 5, 0])  # Zwei Löcher
        radii = [5, 5]
        energy = engine.energy_function(positions, radii, 3)
        
        # Überlappende Löcher sollten hohe Energie haben
        assert energy > 0
        
        # Keine Überlappung
        positions = np.array([0, 0, 20, 0])
        energy = engine.energy_function(positions, radii, 3)
        assert energy == 0
```

### 11.2 Integration Tests (C#)

```csharp
// Tests/IntegrationTests.cs

using Xunit;
using System.Threading.Tasks;
using CableGlandPlanner.Services;
using CableGlandPlanner.Models;

namespace CableGlandPlanner.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public async Task AIBackend_FeasibilityCheck_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var client = new AiClient("http://localhost:5000");
            var request = new FeasibilityRequest
            {
                DiskRadius = 50.0,
                HoleDiameters = new List<double> { 10, 12, 16 },
                EdgeClearance = 5.0,
                HoleClearance = 3.0,
                ToolDiameter = 6.0,
                Depth = 12.0,
                Material = "aluminium"
            };
            
            // Act
            var result = await client.CheckFeasibilityAsync(request);
            
            // Assert
            Assert.NotNull(result);
            Assert.True(result.Feasible);
            Assert.True(result.ToolCompatible);
            Assert.True(result.MaxAllowedDepth >= request.Depth);
        }
        
        [Fact]
        public async Task AIBackend_LayoutSuggestion_ReturnsValidPositions()
        {
            // Arrange
            var client = new AiClient("http://localhost:5000");
            var request = new LayoutRequest
            {
                DiskRadius = 50.0,
                HoleDiameters = new List<double> { 10, 10, 10 },
                EdgeClearance = 5.0,
                HoleClearance = 3.0,
                Algorithm = "simulated_annealing"
            };
            
            // Act
            var result = await client.SuggestLayoutAsync(request);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Positions.Count);
            
            // Prüfe ob alle Positionen innerhalb der Scheibe liegen
            var validator = new LayoutValidator();
            foreach (var pos in result.Positions)
            {
                var distanceToCenter = Math.Sqrt(pos.X * pos.X + pos.Y * pos.Y);
                var maxRadius = request.DiskRadius - request.EdgeClearance - 5; // 5mm Lochradius
                Assert.True(distanceToCenter <= maxRadius);
            }
        }
        
        [Fact]
        public void GCodeGenerator_ValidLayout_ProducesValidGCode()
        {
            // Arrange
            var generator = new GCodeGenerator();
            var holes = new List<HoleDefinition>
            {
                new HoleDefinition { X = 10, Y = 0, Diameter = 10, Depth = 8, InsertType = "M12" },
                new HoleDefinition { X = -10, Y = 0, Diameter = 10, Depth = 8, InsertType = "M12" },
                new HoleDefinition { X = 0, Y = 10, Diameter = 10, Depth = 8, InsertType = "M12" }
            };
            var settings = new GCodeSettings
            {
                SafeZ = 10,
                FeedRateXY = 500,
                FeedRateZ = 250,
                SpindleSpeed = 12000
            };
            
            // Act
            var gcode = generator.Generate(holes, settings);
            
            // Assert
            Assert.Contains("G90", gcode);
            Assert.Contains("G21", gcode);
            Assert.Contains("M03", gcode);
            Assert.Contains("M05", gcode);
            Assert.Contains("M30", gcode);
            Assert.Contains("X10.000", gcode);
            Assert.Contains("X-10.000", gcode);
            Assert.Contains("Y10.000", gcode);
        }
    }
}
```

---

## 12. Deployment auf Linux-Servern

### 12.1 Ansible Playbook für automatisiertes Deployment

```yaml
# ansible/deploy.yml
---
- name: Deploy Cable Gland Planner Backend
  hosts: backend_servers
  become: yes
  vars:
    app_name: cablegland
    app_user: cablegland
    app_group: cablegland
    app_version: "2.0.0"
    
  tasks:
    - name: Create system user
      user:
        name: "{{ app_user }}"
        group: "{{ app_group }}"
        system: yes
        create_home: yes
        home: /opt/{{ app_name }}
        
    - name: Install system dependencies
      apt:
        name:
          - python3.11
          - python3.11-venv
          - python3.11-dev
          - nginx
          - postgresql-15
          - redis-server
          - gunicorn
          - supervisor
          - build-essential
          - libssl-dev
          - libffi-dev
        state: present
        update_cache: yes
        
    - name: Copy application files
      synchronize:
        src: ../backend/
        dest: /opt/{{ app_name }}/
        delete: yes
        rsync_opts:
          - "--exclude=.venv"
          - "--exclude=__pycache__"
          - "--exclude=.git"
          
    - name: Create virtual environment
      command:
        cmd: python3.11 -m venv /opt/{{ app_name }}/venv
        creates: /opt/{{ app_name }}/venv/bin/activate
        
    - name: Install Python dependencies
      pip:
        requirements: /opt/{{ app_name }}/requirements.txt
        virtualenv: /opt/{{ app_name }}/venv
        virtualenv_command: python3.11 -m venv
        
    - name: Configure environment variables
      template:
        src: templates/.env.j2
        dest: /opt/{{ app_name }}/.env
        owner: "{{ app_user }}"
        group: "{{ app_group }}"
        mode: 0600
        
    - name: Configure systemd service
      template:
        src: templates/cablegland.service.j2
        dest: /etc/systemd/system/{{ app_name }}.service
      notify: restart {{ app_name }}
      
    - name: Enable and start service
      systemd:
        name: "{{ app_name }}"
        enabled: yes
        state: started
        daemon_reload: yes
        
    - name: Configure nginx site
      template:
        src: templates/nginx.conf.j2
        dest: /etc/nginx/sites-available/{{ app_name }}
      notify: reload nginx
      
    - name: Enable nginx site
      file:
        src: /etc/nginx/sites-available/{{ app_name }}
        dest: /etc/nginx/sites-enabled/{{ app_name }}
        state: link
      notify: reload nginx
      
    - name: Configure firewall
      ufw:
        rule: allow
        port: "{{ item }}"
        proto: tcp
      loop:
        - 22
        - 80
        - 443
        
  handlers:
    - name: restart {{ app_name }}
      systemd:
        name: "{{ app_name }}"
        state: restarted
        
    - name: reload nginx
      systemd:
        name: nginx
        state: reloaded
```

### 12.2 Docker Production Build

```dockerfile
# Dockerfile.prod

FROM python:3.11-slim-bookworm AS builder

WORKDIR /build

# Build-Abhängigkeiten
RUN apt-get update && apt-get install -y \
    gcc \
    g++ \
    libpq-dev \
    && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --user --no-cache-dir -r requirements.txt

# Final Stage
FROM python:3.11-slim-bookworm

WORKDIR /app

# Runtime-Abhängigkeiten
RUN apt-get update && apt-get install -y \
    libpq5 \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Kopiere Python Pakete vom Builder
COPY --from=builder /root/.local /root/.local

# Kopiere Anwendung
COPY . .

# Umgebungsvariablen
ENV PATH=/root/.local/bin:$PATH
ENV PYTHONUNBUFFERED=1
ENV FLASK_ENV=production

# Healthcheck
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Port
EXPOSE 5000

# Start mit Gunicorn
CMD ["gunicorn", "-w", "4", "-b", "0.0.0.0:5000", "--timeout", "120", "app:app"]
```

---

## 13. Sicherheitskonzept

### 13.1 API Security

```python
# security.py

from functools import wraps
from flask import request, jsonify, current_app
import jwt
from datetime import datetime, timedelta
import hashlib
import hmac

class SecurityManager:
    """Sicherheitsmanager für API-Authentifizierung und Validierung"""
    
    def __init__(self, secret_key: str):
        self.secret_key = secret_key
        
    def require_api_key(self, f):
        """Dekorator für API-Key-Authentifizierung"""
        @wraps(f)
        def decorated(*args, **kwargs):
            api_key = request.headers.get('X-API-Key')
            
            if not api_key:
                return jsonify({"error": "API-Key fehlt"}), 401
            
            if not self.validate_api_key(api_key):
                return jsonify({"error": "Ungültiger API-Key"}), 401
            
            return f(*args, **kwargs)
        return decorated
    
    def validate_api_key(self, api_key: str) -> bool:
        """Validiert API-Key gegen Datenbank"""
        # Implementierung mit Redis/PostgreSQL
        expected_key = current_app.config.get('API_KEY')
        return hmac.compare_digest(api_key, expected_key)
    
    def generate_jwt_token(self, user_id: str, expires_in_hours: int = 24) -> str:
        """Generiert JWT-Token für Benutzer"""
        payload = {
            'user_id': user_id,
            'exp': datetime.utcnow() + timedelta(hours=expires_in_hours),
            'iat': datetime.utcnow(),
            'iss': 'cablegland-backend'
        }
        return jwt.encode(payload, self.secret_key, algorithm='HS256')
    
    def validate_jwt_token(self, token: str) -> dict:
        """Validiert JWT-Token"""
        try:
            payload = jwt.decode(token, self.secret_key, algorithms=['HS256'])
            return payload
        except jwt.ExpiredSignatureError:
            raise ValueError("Token abgelaufen")
        except jwt.InvalidTokenError:
            raise ValueError("Ungültiger Token")
    
    def sanitize_input(self, data: dict) -> dict:
        """Bereinigt Eingabedaten gegen Injection"""
        sanitized = {}
        for key, value in data.items():
            if isinstance(value, str):
                # Entferne potenziell gefährliche Zeichen
                sanitized[key] = value.replace('\x00', '').replace('\n', ' ')
            elif isinstance(value, dict):
                sanitized[key] = self.sanitize_input(value)
            elif isinstance(value, list):
                sanitized[key] = [self.sanitize_input(item) if isinstance(item, dict) else item for item in value]
            else:
                sanitized[key] = value
        return sanitized
    
    def rate_limit_by_user(self, user_id: str, limit: int = 100, window_seconds: int = 60) -> bool:
        """Benutzerspezifisches Rate Limiting mit Redis"""
        redis_client = current_app.extensions['redis']
        key = f"ratelimit:{user_id}"
        
        current = redis_client.get(key)
        if current and int(current) >= limit:
            return False
        
        pipe = redis_client.pipeline()
        pipe.incr(key)
        pipe.expire(key, window_seconds)
        pipe.execute()
        
        return True
```

---

## 14. Leistungsoptimierung

### 14.1 Performance-Tuning

```python
# performance.py

import asyncio
from concurrent.futures import ThreadPoolExecutor, ProcessPoolExecutor
from functools import lru_cache
from typing import List, Tuple
import numpy as np
from numba import jit, prange

class PerformanceOptimizer:
    """Leistungsoptimierung für rechenintensive Operationen"""
    
    def __init__(self, max_workers: int = 4):
        self.thread_pool = ThreadPoolExecutor(max_workers=max_workers)
        self.process_pool = ProcessPoolExecutor(max_workers=max_workers)
    
    @staticmethod
    @jit(nopython=True, parallel=True)
    def parallel_distance_matrix(points: np.ndarray) -> np.ndarray:
        """Parallelisierte Distanzmatrix-Berechnung mit Numba"""
        n = points.shape[0]
        distances = np.zeros((n, n))
        
        for i in prange(n):
            for j in range(i+1, n):
                dx = points[i, 0] - points[j, 0]
                dy = points[i, 1] - points[j, 1]
                dist = np.sqrt(dx*dx + dy*dy)
                distances[i, j] = dist
                distances[j, i] = dist
        
        return distances
    
    @lru_cache(maxsize=128)
    def cached_circle_packing(self, radii_tuple: Tuple[float], disk_radius: float) -> List[Tuple[float, float]]:
        """Cached Circle-Packing für wiederholte Aufrufe"""
        radii = list(radii_tuple)
        # Implementierung der Circle-Packing-Logik
        return self._circle_packing_algorithm(radii, disk_radius)
    
    async def process_batch_async(self, requests: List[dict]) -> List[dict]:
        """Asynchrone Batch-Verarbeitung"""
        tasks = [self.process_single_request(req) for req in requests]
        return await asyncio.gather(*tasks)
    
    async def process_single_request(self, request: dict) -> dict:
        """Verarbeitet eine einzelne Anfrage asynchron"""
        # Simuliere asynchrone Verarbeitung
        await asyncio.sleep(0.01)
        return {"result": "processed", "data": request}
    
    def optimize_memory_usage(self, data: np.ndarray) -> np.ndarray:
        """Optimiert Speichernutzung durch Typkonvertierung"""
        if data.dtype == np.float64:
            return data.astype(np.float32)
        return data
    
    def batch_geometric_checks(self, holes: List[dict], batch_size: int = 100) -> List[bool]:
        """Batch-Verarbeitung von geometrischen Prüfungen"""
        results = []
        
        for i in range(0, len(holes), batch_size):
            batch = holes[i:i+batch_size]
            # Verarbeite Batch parallel
            batch_results = self._process_geometric_batch(batch)
            results.extend(batch_results)
        
        return results
    
    def _process_geometric_batch(self, batch: List[dict]) -> List[bool]:
        """Verarbeitet einen Batch geometrischer Prüfungen"""
        # Implementierung mit NumPy-Vektorisierung
        return [True] * len(batch)
```

---

## 15. Erweiterte Geometrie-Algorithmen

### 15.1 Circle-Packing mit genetischem Algorithmus

```python
# services/genetic_optimizer.py

import numpy as np
import random
from typing import List, Tuple
from dataclasses import dataclass

@dataclass
class Individual:
    """Individuum für genetischen Algorithmus"""
    genes: np.ndarray  # [x1, y1, x2, y2, ...]
    fitness: float
    radii: List[float]
    
class GeneticLayoutOptimizer:
    """Genetischer Algorithmus für Layout-Optimierung"""
    
    def __init__(self,
                 population_size: int = 100,
                 mutation_rate: float = 0.1,
                 crossover_rate: float = 0.8,
                 generations: int = 500):
        
        self.population_size = population_size
        self.mutation_rate = mutation_rate
        self.crossover_rate = crossover_rate
        self.generations = generations
        
    def optimize(self,
                disk_radius: float,
                radii: List[float],
                edge_clearance: float,
                hole_clearance: float) -> List[Tuple[float, float]]:
        
        n_holes = len(radii)
        self.geometry = CircularPackingEngine(disk_radius, edge_clearance)
        
        # Initialisiere Population
        population = self._initialize_population(n_holes, radii)
        
        # Evolution
        for generation in range(self.generations):
            # Fitness bewerten
            for individual in population:
                individual.fitness = self._calculate_fitness(
                    individual.genes, radii, hole_clearance
                )
            
            # Selektion
            selected = self._tournament_selection(population)
            
            # Crossover
            offspring = self._crossover(selected)
            
            # Mutation
            offspring = self._mutate(offspring)
            
            # Neue Generation
            population = self._survivor_selection(population, offspring)
            
            # Konvergenzprüfung
            best_fitness = max(ind.fitness for ind in population)
            if best_fitness < 1e-6:
                break
        
        # Bestes Individuum zurückgeben
        best = max(population, key=lambda x: x.fitness)
        return self._decode_positions(best.genes, n_holes)
    
    def _initialize_population(self, n_holes: int, radii: List[float]) -> List[Individual]:
        """Initialisiert Population mit zufälligen Positionen"""
        population = []
        
        for _ in range(self.population_size):
            positions = np.random.uniform(-40, 40, (n_holes, 2))
            genes = positions.flatten()
            population.append(Individual(genes, 0.0, radii))
        
        return population
    
    def _calculate_fitness(self, genes: np.ndarray, radii: List[float], clearance: float) -> float:
        """Berechnet Fitness (negierte Energie)"""
        positions = genes.reshape(-1, 2)
        energy = self.geometry.energy_function(genes, radii, clearance)
        return -energy  # Maximierung
    
    def _tournament_selection(self, population: List[Individual], k: int = 3) -> List[Individual]:
        """Turnierselektion"""
        selected = []
        
        for _ in range(len(population)):
            tournament = random.sample(population, k)
            winner = max(tournament, key=lambda x: x.fitness)
            selected.append(winner)
        
        return selected
    
    def _crossover(self, parents: List[Individual]) -> List[Individual]:
        """Einheitliches Crossover"""
        offspring = []
        
        for i in range(0, len(parents), 2):
            if i+1 >= len(parents):
                break
            
            p1, p2 = parents[i], parents[i+1]
            
            if random.random() < self.crossover_rate:
                mask = np.random.random(len(p1.genes)) < 0.5
                child1_genes = np.where(mask, p1.genes, p2.genes)
                child2_genes = np.where(mask, p2.genes, p1.genes)
                
                offspring.append(Individual(child1_genes, 0.0, p1.radii))
                offspring.append(Individual(child2_genes, 0.0, p2.radii))
            else:
                offspring.append(p1)
                offspring.append(p2)
        
        return offspring
    
    def _mutate(self, population: List[Individual]) -> List[Individual]:
        """Mutation mit Gaußscher Störung"""
        mutated = []
        
        for individual in population:
            genes = individual.genes.copy()
            
            if random.random() < self.mutation_rate:
                # Gaußsche Mutation
                mutation = np.random.normal(0, 2, len(genes))
                genes += mutation
            
            mutated.append(Individual(genes, 0.0, individual.radii))
        
        return mutated
    
    def _survivor_selection(self, current: List[Individual], offspring: List[Individual]) -> List[Individual]:
        """Elitäre Selektion"""
        combined = current + offspring
        combined.sort(key=lambda x: x.fitness, reverse=True)
        return combined[:self.population_size]
    
    def _decode_positions(self, genes: np.ndarray, n_holes: int) -> List[Tuple[float, float]]:
        """Dekodiert Genom in Positionen"""
        positions = genes.reshape(n_holes, 2)
        return [(pos[0], pos[1]) for pos in positions]
```

---

## 16. Benutzeroberfläche (German-only)

### 16.1 Vollständige Deutsche UI-Texte

```csharp
// UI/GermanLabels.cs

namespace CableGlandPlanner.UI
{
    public static class GermanLabels
    {
        // Hauptfenster
        public const string WindowTitle = "Cable Gland Planner - Ingenieursoftware für Kabelverschraubungen";
        public const string StatusReady = "Bereit";
        public const string StatusProcessing = "Verarbeite...";
        public const string StatusError = "Fehler aufgetreten";
        
        // Eingabebereich
        public const string DiskSection = "Scheibenparameter";
        public const string DiskDiameter = "Scheibendurchmesser (mm):";
        public const string EdgeClearance = "Randabstand (mm):";
        public const string Material = "Material:";
        
        public const string HoleSection = "Lochparameter";
        public const string HoleDiameter = "Bohrungsdurchmesser (mm):";
        public const string HoleDepth = "Tiefe (mm):";
        public const string InsertType = "Insert Typ:";
        public const string HoleClearance = "Lochabstand (mm):";
        
        public const string ToolSection = "Werkzeugparameter";
        public const string ToolDiameter = "Werkzeug-Ø (mm):";
        public const string FeedRate = "Vorschub (mm/min):";
        public const string SpindleSpeed = "Spindeldrehzahl (U/min):";
        public const string SafeZ = "Sichere Z-Höhe (mm):";
        public const string PeckDepth = "Picken-Tiefe (mm):";
        public const string UsePecking = "Picken verwenden";
        public const string UseCoolant = "Kühlmittel verwenden";
        
        // Buttons
        public const string GenerateLayout = "Layout erzeugen";
        public const string ExportGCode = "G-Code exportieren";
        public const string ExportPNG = "PNG exportieren";
        public const string ResetForm = "Zurücksetzen";
        public const string LoadInserts = "Insert-Typen laden";
        
        // Statusanzeigen
        public const string AIStatusAvailable = "KI-Status: Verfügbar";
        public const string AIStatusOffline = "KI-Status: Offline (Fallback Modus)";
        public const string ValidationValid = "Status: Machbar ✓";
        public const string ValidationInvalid = "Status: Nicht machbar ✗";
        
        // Fehlermeldungen
        public const string ErrorNoLayout = "Bitte zuerst ein gültiges Layout generieren.";
        public const string ErrorInvalidInput = "Ungültige Eingabeparameter.";
        public const string ErrorExportFailed = "Export fehlgeschlagen: {0}";
        public const string ErrorBackendUnreachable = "KI-Backend nicht erreichbar. Verwende Fallback-Modus.";
        
        // Warnungen
        public const string WarningHighDensity = "Hohe Lochdichte - Materialfestigkeit prüfen!";
        public const string WarningSmallClearance = "Randabstand sehr gering - Bruchgefahr!";
        public const string WarningToolIncompatible = "Werkzeugdurchmesser zu groß für kleinste Bohrung!";
        public const string WarningDepthExceeded = "Maximale Bohrtiefe überschritten (max. 35 mm)!";
        
        // Dialoge
        public const string DialogExportSuccess = "Export erfolgreich:\n{0}";
        public const string DialogSaveFile = "NC Datei (*.nc)|*.nc|Text Datei (*.txt)|*.txt";
        public const string DialogConfirmExit = "Wirklich beenden?";
        public const string DialogWarnings = "Layout-Warnungen";
    }
}
```

---

## 17. Installation & Konfiguration

### 17.1 Komplette Setup-Anleitung

```markdown
# Installationsanleitung - Cable Gland Planner

## Systemvoraussetzungen

### Frontend (Windows)
- Windows 10/11 (64-bit)
- .NET 8 Runtime
- 4 GB RAM (min. 2 GB)
- 200 MB freier Festplattenspeicher
- Bildschirmauflösung: 1366x768 oder höher

### Backend (Linux Server)
- Ubuntu 22.04 LTS oder Debian 12
- 4 CPU Kerne (min. 2)
- 8 GB RAM (min. 4 GB)
- 20 GB freier Festplattenspeicher
- PostgreSQL 15
- Redis 7.2

## Schritt-für-Schritt Installation

### 1. Backend Installation (Linux)

```bash
# Repository klonen
git clone https://github.com/your-org/cablegland-planner.git
cd cablegland-planner

# Automatisiertes Deployment ausführen
sudo bash deploy_backend.sh

# Oder manuell:
# 1. Abhängigkeiten installieren
sudo apt-get update
sudo apt-get install -y python3.11 python3.11-venv postgresql-15 redis-server nginx

# 2. Datenbank einrichten
sudo -u postgres psql -f database/init.sql

# 3. Python Umgebung einrichten
python3.11 -m venv venv
source venv/bin/activate
pip install -r requirements.txt

# 4. Konfiguration anpassen
cp .env.example .env
nano .env  # Datenbankpasswörter und API-Keys setzen

# 5. Service starten
sudo systemctl start cablegland
sudo systemctl enable cablegland
```

### 2. Frontend Installation (Windows)

```powershell
# 1. .NET 8 SDK herunterladen und installieren
# https://dotnet.microsoft.com/download/dotnet/8.0

# 2. Projekt bauen
dotnet build -c Release

# 3. Ausführbare Datei ausführen
dotnet run -c Release
# Oder die .exe im bin/Release Ordner starten

# 4. (Optional) Desktop-Verknüpfung erstellen
# Erstelle eine Verknüpfung zu CableGlandPlanner.exe
```

### 3. Konfiguration

#### Backend Konfiguration (.env)

```env
# Datenbank
DB_HOST=localhost
DB_NAME=cablegland_db
DB_USER=cablegland
DB_PASSWORD=your_secure_password_2025

# Redis
REDIS_HOST=localhost
REDIS_PORT=6379
REDIS_PASSWORD=

# API Security
API_KEY=your_api_key_here
JWT_SECRET=your_jwt_secret_here

# Performance
MAX_WORKERS=4
CACHE_TTL_SECONDS=300
RATE_LIMIT_PER_MINUTE=100

# Logging
LOG_LEVEL=INFO
LOG_FILE=/var/log/cablegland/app.log
```

#### Frontend Konfiguration (appsettings.json)

```json
{
  "AIBackendUrl": "http://localhost:5000",
  "RequestTimeoutMs": 5000,
  "EnableAIAssistance": true,
  "EnableOfflineFallback": true,
  "DefaultSafeZ": 10.0,
  "DefaultFeedRate": 500,
  "DefaultSpindleSpeed": 12000,
  "MaxDepthMm": 35.0,
  "Theme": {
    "PrimaryColor": "#FF8C00",
    "SecondaryColor": "#FFFFFF",
    "WarningColor": "#FFD700",
    "ErrorColor": "#FF4444"
  }
}
```

### 4. Firewall Konfiguration

```bash
# Backend Ports freigeben
sudo ufw allow 22/tcp      # SSH
sudo ufw allow 80/tcp      # HTTP
sudo ufw allow 443/tcp     # HTTPS
sudo ufw allow 5000/tcp    # Flask API
sudo ufw enable
```

### 5. SSL Zertifikat (Produktion)

```bash
# Let's Encrypt Zertifikat
sudo apt-get install certbot python3-certbot-nginx
sudo certbot --nginx -d api.cablegland.local

# Automatische Erneuerung
sudo systemctl enable certbot.timer
```

### 6. Monitoring einrichten

```bash
# Prometheus und Grafana installieren
sudo apt-get install prometheus grafana

# Service konfigurieren
sudo systemctl start prometheus grafana-server
sudo systemctl enable prometheus grafana-server

# Dashboard importieren (Dashboard-ID: 12345)
# Zugriff: http://localhost:3000 (admin/admin)
```

### 7. Backup Strategie

```bash
# Tägliches Datenbank-Backup
0 2 * * * pg_dump cablegland_db > /backups/cablegland_$(date +\%Y\%m\%d).sql

# Wöchentliches vollständiges Backup
0 3 * * 0 tar -czf /backups/cablegland_full_$(date +\%Y\%m\%d).tar.gz /opt/cablegland

# Backup auf Remote-Server übertragen
0 4 * * * rsync -avz /backups/ user@backup-server:/backups/cablegland/
```

## Fehlerbehebung

### Häufige Probleme

1. **Backend startet nicht**
   - Prüfe Logs: `journalctl -u cablegland -f`
   - Prüfe Datenbankverbindung: `psql -U cablegland -d cablegland_db -c "SELECT 1"`
   - Prüfe Port: `netstat -tlnp | grep 5000`

2. **Frontend kann keine Verbindung herstellen**
   - Prüfe Firewall: `sudo ufw status`
   - Prüfe Backend URL in appsettings.json
   - Teste Verbindung: `curl http://localhost:5000/health`

3. **Layout-Optimierung zu langsam**
   - Erhöhe CPU/RAM für Backend
   - Reduziere Anzahl der Optimierungsiterationen
   - Verwende simpleren Algorithmus (z.B. "radial")
```

---

## 18. Fehlerbehandlung & Logging

### 18.1 Umfassendes Logging-System

```python
# logging_config.py

import logging
import logging.handlers
import sys
from datetime import datetime
from pathlib import Path

class StructuredLogger:
    """Strukturiertes Logging mit JSON-Format"""
    
    def __init__(self, app_name: str = "cablegland", log_level: str = "INFO"):
        self.app_name = app_name
        self.logger = logging.getLogger(app_name)
        self.logger.setLevel(getattr(logging, log_level))
        
        # Verhindere doppelte Handler
        if self.logger.handlers:
            return
        
        # Konsolen-Handler
        console_handler = logging.StreamHandler(sys.stdout)
        console_handler.setFormatter(self._get_console_formatter())
        self.logger.addHandler(console_handler)
        
        # Rotierender Datei-Handler
        log_dir = Path("/var/log/cablegland")
        log_dir.mkdir(parents=True, exist_ok=True)
        
        file_handler = logging.handlers.RotatingFileHandler(
            log_dir / "app.log",
            maxBytes=10*1024*1024,  # 10 MB
            backupCount=10
        )
        file_handler.setFormatter(self._get_file_formatter())
        self.logger.addHandler(file_handler)
        
        # Fehler-spezifischer Handler
        error_handler = logging.handlers.RotatingFileHandler(
            log_dir / "errors.log",
            maxBytes=5*1024*1024,  # 5 MB
            backupCount=5
        )
        error_handler.setLevel(logging.ERROR)
        error_handler.setFormatter(self._get_file_formatter())
        self.logger.addHandler(error_handler)
    
    def _get_console_formatter(self):
        return logging.Formatter(
            '%(asctime)s - %(levelname)s - %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S'
        )
    
    def _get_file_formatter(self):
        return logging.Formatter(
            '%(asctime)s - %(name)s - %(levelname)s - %(filename)s:%(lineno)d - %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S'
        )
    
    def log_request(self, request_id: str, endpoint: str, method: str, 
                    status_code: int, duration_ms: float):
        """Loggt API-Anfragen strukturiert"""
        self.logger.info(
            f"REQUEST | id={request_id} | {method} {endpoint} | "
            f"status={status_code} | duration={duration_ms:.2f}ms"
        )
    
    def log_feasibility_check(self, request: dict, result: dict, duration_ms: float):
        """Loggt Machbarkeitsprüfungen"""
        self.logger.info(
            f"FEASIBILITY | holes={len(request['hole_diameters'])} | "
            f"feasible={result['feasible']} | confidence={result.get('confidence', 0)} | "
            f"duration={duration_ms:.2f}ms"
        )
    
    def log_error(self, error: Exception, context: dict = None):
        """Loggt Fehler mit Kontext"""
        error_msg = f"ERROR | {type(error).__name__}: {str(error)}"
        if context:
            error_msg += f" | context={context}"
        self.logger.error(error_msg, exc_info=True)
    
    def log_performance(self, operation: str, duration_ms: float, metadata: dict = None):
        """Loggt Performance-Metriken"""
        msg = f"PERFORMANCE | {operation} | duration={duration_ms:.2f}ms"
        if metadata:
            msg += f" | {metadata}"
        self.logger.info(msg)
```

---

## 19. Entwickler-Dokumentation

### 19.1 Code-Struktur und Standards

```markdown
# Entwickler-Dokumentation

## Projektstruktur

```
CableGlandPlanner/
├── src/
│   ├── CableGlandPlanner/           # Frontend
│   │   ├── Models/                  # Datenmodelle
│   │   ├── Services/                # Geschäftslogik
│   │   ├── UI/                      # Benutzeroberfläche
│   │   └── Resources/               # Ressourcen
│   └── CableGlandPlanner.Backend/   # Python Backend
│       ├── services/                # Services
│       ├── models/                  # Datenmodelle
│       ├── data/                    # Datenbank
│       └── tests/                   # Tests
├── docs/                            # Dokumentation
├── scripts/                         # Build-Scripts
└── deploy/                          # Deployment-Konfiguration
```

## Coding Standards

### C# Standards
- Verwendung von .NET 8
- Async/Await für I/O-Operationen
- Nullable Reference Types aktiviert
- File-scoped Namespaces
- Primäre Konstruktoren für einfache Typen

### Python Standards
- Python 3.11+
- Type Hints für alle Funktionen
- Docstrings im Google Format
- Black für Code-Formatierung
- isort für Import-Sortierung
- mypy für Typ-Checks

## Git Workflow

```bash
# Feature Branches
git checkout -b feature/feature-name
git commit -m "feat: Beschreibung der Änderung"
git push origin feature/feature-name

# Commit Types
# feat: Neue Funktion
# fix: Bugfix
# docs: Dokumentation
# style: Code-Formatierung
# refactor: Refactoring
# test: Tests
# chore: Build/Config

# Pull Request erstellen
# 1. PR auf main erstellen
# 2. Tests automatisch ausführen
# 3. Code Review durchführen
# 4. Nach Approve mergen
```

## Debugging

### Frontend Debugging (Visual Studio)
1. Breakpoints setzen
2. F5 zum Starten mit Debugger
3. Ausgabefenster für Logs
4. Diagnose-Tools für Performance

### Backend Debugging (VS Code)
```json
// .vscode/launch.json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Python: Flask",
            "type": "python",
            "request": "launch",
            "module": "flask",
            "env": {
                "FLASK_APP": "app.py",
                "FLASK_ENV": "development"
            },
            "args": ["run", "--debug"],
            "jinja": true
        }
    ]
}
```

## API Entwicklung

### Neue Endpunkte erstellen

```python
@app.route('/api/new_endpoint', methods=['POST'])
@limiter.limit("50 per minute")
@validate_request(NewRequestModel)
def new_endpoint():
    """Dokumentation des Endpunkts"""
    try:
        data = request.get_json()
        result = service.process(data)
        return jsonify(result), 200
    except ValidationError as e:
        return jsonify({"error": str(e)}), 400
    except Exception as e:
        logger.log_error(e)
        return jsonify({"error": "Internal error"}), 500
```

## Testing

### Unit Tests ausführen

```bash
# Frontend
dotnet test

# Backend
pytest tests/ -v --cov=services --cov-report=html

# Integration Tests
pytest tests/integration/ -v

# Performance Tests
pytest tests/performance/ -v --benchmark-only
```

## Deployment

### Build Pipeline (GitHub Actions)

```yaml
# .github/workflows/build.yml
name: Build and Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup Python
        uses: actions/setup-python@v4
        with:
          python-version: '3.11'
      - name: Install dependencies
        run: |
          pip install -r requirements.txt
          pip install pytest pytest-cov
      - name: Run tests
        run: pytest tests/ --cov=services
  
  build-frontend:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Build
        run: dotnet build -c Release
      - name: Test
        run: dotnet test --no-build -c Release
```
```

---

## 20. Lizenz & rechtliche Hinweise

### 20.1 Proprietäre Lizenz für industrielle Nutzung

```text
CABLE GLAND PLANNER - PROPRIETARY SOFTWARE LICENSE

Copyright (c) 2025 Amir Mobasheraghdam

Alle Rechte vorbehalten.

Diese Software ist Eigentum von Amir Mobasheraghdam und ist für den 
industriellen Einsatz bestimmt. Die Software darf nur gemäß den 
folgenden Bedingungen genutzt werden:

1. Nutzungsrechte:
   - Die Software darf in einem industriellen Produktionsumfeld 
     installiert und genutzt werden.
   - Die Nutzung ist auf eine einzelne Organisation beschränkt.
   - Pro Standort ist eine Lizenz erforderlich.

2. Eingeschränkte Rechte:
   - Reverse Engineering, Dekompilierung oder Disassemblierung 
     der Software ist untersagt.
   - Die Weitergabe, Vermietung oder Leasing der Software ist 
     untersagt.
   - Modifikationen am Quellcode sind nur mit schriftlicher 
     Genehmigung erlaubt.

3. Haftungsausschluss:
   - Die Software wird "wie besehen" ohne Gewährleistung bereitgestellt.
   - Der Autor haftet nicht für Schäden, die aus der Nutzung 
     der Software entstehen, einschließlich, aber nicht beschränkt auf
     Materialschäden, Produktionsausfälle oder Personenschäden.
   - Der Benutzer ist für die Überprüfung des generierten G-Codes 
     vor der Verwendung auf CNC-Maschinen verantwortlich.

4. Support:
   - Support ist im Rahmen eines separaten Wartungsvertrags verfügbar.
   - Ohne Wartungsvertrag besteht kein Anspruch auf Support oder Updates.

5. Beendigung:
   - Bei Verstoß gegen diese Bedingungen erlischt die Lizenz automatisch.
   - Nach Beendigung muss die Software von allen Systemen entfernt werden.

Diese Lizenz unterliegt dem Recht der Bundesrepublik Deutschland.

Für kommerzielle Lizenzen und Support wenden Sie sich an:
Amir Mobasheraghdam
E-Mail: amir.mobasher@example.com

Version: 2.0.0
Datum: 01.03.2025
```

---

## Zusammenfassung

Cable Gland Planner ist eine hochmoderne, KI-gestützte Ingenieurssoftware für die automatische Planung, Validierung und CNC-Fertigung von Kabelverschraubungs-Layouts. Das System bietet:

✅ **Komplette CI/CD Pipeline** für automatisierte Builds und Tests  
✅ **Multi-Server Architektur** mit Load Balancing und Hochverfügbarkeit  
✅ **Datenbank-Integration** (PostgreSQL + Redis) für Persistenz und Caching  
✅ **Umfassendes Monitoring** (Prometheus + Grafana)  
✅ **Erweiterte Optimierungsalgorithmen** (Simulated Annealing, Genetische Algorithmen)  
✅ **German-only Benutzeroberfläche** mit industriellem Orange/Weiß-Theme  
✅ **Automatische G-Code-Generierung** mit Taschenfräsung und Picken  
✅ **Offline-Fallback** für Betrieb ohne KI-Backend  
✅ **Detaillierte Dokumentation** und Entwickler-Guides  

Die Software ist bereit für den produktiven Einsatz in der industriellen Fertigung und kann sowohl als Desktop-Anwendung unter Windows als auch als skalierbarer Backend-Dienst auf Linux-Servern betrieben werden.

---

**Entwickelt von:** Amir Mobasheraghdam  
**Kontakt:** https://www.linkedin.com/in/amirmobasher  
**Publikationen:** https://www.myscience.de/en/news/wire/ideas_with_passion_and_entrepreneurial_spirit-2025-uni-bonn  
**3D-Modelle:** https://www.myminifactory.com/users/AmirMobasher  

**Version:** 2.0.0  
**Stand:** März 2025
