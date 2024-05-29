# GraphDrawer User Documentation

## Table of Contents

1. [Introduction](#introduction)
2. [Getting Started](#getting-started)
    - [Downloading the Repository](#downloading-the-repository)
    - [Running the Program](#running-the-program)
3. [Application Overview](#application-overview)
    - [Canvas](#canvas)
    - [Toolbox](#toolbox)
        - [Undo Button](#undo-button)
        - [Color ComboBox](#color-combobox)
        - [DrawMode ComboBox](#drawmode-combobox)
        - [ShowObjects Button](#showobjects-button)
        - [Shortest Path Button](#shortest-path-button)
4. [Drawing Modes](#drawing-modes)
    - [Drawing Node](#drawing-node)
    - [Line](#line)
    - [Weighted Line](#weighted-line)
    - [Curve](#curve)
    - [Weighted Curve](#weighted-curve)

## Introduction

Welcome to **GraphDrawer**, a tool for creating and manipulating graphs. GraphDrawer allows you to draw nodes, edges, and curves with customizable weights and colors on a canvas.

## Getting Started

### Downloading the Repository

To try GraphDrawer, follow these steps:

1. **Download the Repository**

2. **Open the Repository**
    - Open the repository in Visual Studio

2. **Run the program in Visual Studio**
    - Build the program for Release and then navigate to bin/Release/net7.0-windows/ and run GraphDrawer.exe
    - Or run the code directly.

## Application Overview

### Canvas

The **Canvas** is the main area where you will draw and manipulate graph elements such as nodes, lines, and curves.

### Toolbox

The **Toolbox** provides essential tools to interact with the canvas:

#### Undo Button

- **Function**: Reverts the most recent change made on the canvas.
- **Usage**: Click the "Undo" button to undo the last action.

#### Color ComboBox

- **Function**: Allows selection of different colors for drawing.
- **Usage**: Select a color from the dropdown menu to change the drawing color.

#### DrawMode ComboBox

- **Function**: Switches between different drawing modes.
- **Options**:
    - **Drawing Node**: Draw nodes on the canvas.
    - **Line**: Draw straight lines between nodes.
    - **Weighted Line**: Draw lines with weights between nodes.
    - **Curve**: Draw curved lines between nodes.
    - **Weighted Curve**: Draw curved lines with weights between nodes.
- **Usage**: Select a drawing mode from the dropdown menu.

#### ShowObjects Button

- **Function**: Lists all objects currently on the canvas for debugging purposes.
- **Usage**: Click the "ShowObjects" button to display a list of objects.

#### Shortest Path Button

- **Function**: Finds the shortest path between two selected nodes.
- **Usage**:
    - Click the "Shortest Path" button.
    - Select two nodes on the canvas.
    - To cancel, click the "Shortest Path" button again.

## Drawing Modes

### Drawing Node

- **Function**: Adds nodes to the canvas.
- **Usage**: Select "Drawing Node" from the DrawMode ComboBox and click on the canvas to add a node.

### Line

- **Function**: Draws straight lines between nodes.
- **Usage**: Select "Line" from the DrawMode ComboBox and draw between two nodes on the canvas.

### Weighted Line

- **Function**: Draws lines with weights between nodes.
- **Usage**:
    - Select "Weighted Line" from the DrawMode ComboBox.
    - Draw between two nodes.
    - Enter an integer weight in the pop-up window. If an invalid value is entered, the line will not be drawn.

### Curve

- **Function**: Draws curved lines between nodes.
- **Usage**: Select "Curve" from the DrawMode ComboBox and draw between two nodes on the canvas.

### Weighted Curve

- **Function**: Draws curved lines with weights between nodes.
- **Usage**:
    - Select "Weighted Curve" from the DrawMode ComboBox.
    - Draw between two nodes.
    - Enter an integer weight in the pop-up window. If an invalid value is entered, the curve will not be drawn.

---
