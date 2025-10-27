# VR Visual Attention Research

A Unity-based virtual reality application for studying visual attention patterns and gaze behavior in immersive environments. This project implements a pseudo-eye tracking system with custom data collection and analysis tools.

## Project Overview

This bachelor project explores visual attention mechanisms in VR through four distinct experimental tasks, measuring fixation patterns, convergence time, and user attention distribution across various scenarios.

## Tech Stack

### Core Technologies
- **Unity Engine** - Primary development platform
- **Unity XR Toolkit** (`UnityEngine.XR`) - VR integration and device communication
- **C#** - Primary programming language
- **Oculus Quest 2** - VR hardware platform
- **Meta Link Cable** - Development and debugging connectivity

### Data Analysis
- **Python** - Data processing and analysis
- **Jupyter Notebook** - Interactive data visualization
- **Pandas** - Data manipulation and CSV file handling
- **NumPy** - Numerical computations (implied for data analysis)

### Development Tools
- **Unity Editor** - Scene design and development
- **Quest Link** - Real-time debugging without recompilation
- **CSV Export System** - Custom data logging implementation

## System Architecture

### 1. Pseudo-Eye Tracking System

**Key Components:**
- **Eye Data Acquisition**: Leverages `InputDevices.GetDevicesWithCharacteristics` to capture position and rotation data from both eyes
- **Virtual Eye Computation**: Averages binocular data to create a unified gaze representation
- **Aim Assist Algorithm**: Dynamically adjusts gaze towards nearest targets using `Vector3.Distance` calculations
- **Adaptive Gaze Distance**: Implements raycast collision detection to prevent gaze penetration through solid objects
- **Intelligent Logging**: Threshold-based system that logs gaze data only when significant movement occurs or after time intervals

**Implementation Highlights:**
```
- CheckGaze(): Main tracking loop
- AdjustGazeDistance(): Dynamic distance adjustment with raycasting
- Aim assist with configurable aimAssistStrength
- Collision detection via boundsLayerMask
```

### 2. Data Collection Pipeline

**CSV Logging System:**
- Automated file naming with counter-based uniqueness
- Captures comprehensive gaze metrics:
  - Fixation duration
  - Scene region identification
  - Target object tracking
  - 3D gaze point coordinates (X, Y, Z)
  - User position in virtual space (X, Y, Z)
- Attached to XRRig GameObject for seamless integration
- Header row generation for structured data export

### 3. Experimental Task Implementations

#### Task 1: Dynamic Movement & Saliency
- **Player Movement System**: Speed and rotation control with destination-based pathfinding
- **Randomized Object Placement**: Dynamic repositioning to prevent pattern recognition
- **Variables**: `speed`, `IsRotating`, `Direction`
- **Methods**: `Rotate()` for orientation management

#### Task 2: Visual Search (Food vs. Non-Food)
- **Fixed Viewpoint Environment**: Single-region constrained area
- **RandomObjectPlacer Script**: Spawns objects within predefined cube boundaries
- **Timed Generations**: 5 generations × 3 seconds each
- **Target Identification**: Sphere (non-food) among food items
- **Metrics**: Convergence time, PercFixInside, TimeToSR

#### Task 3: High-Contrast Light Orbs
- **Environment Design**: Pitch-black skybox for maximum contrast
- **Light Fading System**: Dynamic point light intensity interpolation
- **Random Delay Implementation**: Unique timing per orb for natural effect
- **Reused Spawning Logic**: RandomObjectPlacer for spatial distribution

#### Task 4: Motion-Based Attention
- **Dark Environment**: Room with stationary XRRig
- **MoveCylinders Script**: Controls object motion with MinSpeed parameter
- **Object Design**: Red cylinders positioned outside central vision
- **Focus**: Testing peripheral motion detection

### 4. Data Visualization

**Jupyter Notebook Analysis Suite:**
- Pandas dataframe structures for efficient data handling
- Comparative analysis across experimental conditions
- Statistical calculations for research metrics
- Visual representations of gaze patterns

## Key Metrics Tracked

- **Fixation Duration**: Time spent looking at specific targets
- **Convergence Time**: Speed of attention focusing
- **PercFixInside**: Percentage of fixations within regions of interest
- **TimeToSR**: Time to first fixation on target objects
- **Fixation Count**: Frequency of gaze events per object type

## Implementation Details

### XR Integration
- XRRig serves as the player controller and data collection anchor
- Seamless VR headset compatibility through Unity XR framework
- Real-time gaze tracking without external eye-tracking hardware

### Performance Optimizations
- Conditional logging to avoid unnecessary computations during stable gaze
- Efficient raycast-based distance adjustment
- Optimized object pooling for repeated spawning scenarios

### Development Workflow
- Quest Link for rapid iteration and debugging
- CSV-based data export for post-experiment analysis
- Modular script architecture for task-specific implementations

## Hardware Requirements

- Oculus Quest 2 VR headset
- 2 Oculus Touch controllers
- Meta Link Cable (for development)
- PC capable of running Unity and VR applications

## Research Applications

This system enables research in:
- Visual attention mechanisms in VR
- Saliency detection and object recognition
- Motion perception and peripheral awareness
- Comparative analysis of fixation patterns across different stimuli

## Future Enhancements

- Integration with dedicated eye-tracking hardware
- Expanded metric calculations
- Real-time visualization dashboard
- Machine learning-based gaze prediction

## Notes

This project demonstrates:
- Advanced Unity development skills
- VR application architecture
- Data collection and analysis pipeline design
- User experience research methodology
- Python data science integration

---

**Developed as part of a Bachelor's degree project exploring visual attention in virtual reality environments.**
