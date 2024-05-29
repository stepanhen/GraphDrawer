# GraphDrawer Development Documentation

## Table of Contents

1. [Introduction](#introduction)
2. [Project Structure](#project-structure)
3. [Main Components](#main-components)
    - [MainWindow.xaml](#mainwindowxaml)
    - [MainWindow.xaml.cs](#mainwindowxamlcs)
    - [WeightInput.xaml and WeightInput.xaml.cs](#weightinputxaml-and-weightinputxamlcs)
    - [Graph.cs](#graphcs)
    - [Edge.cs](#edgecs)
    - [Node.cs](#nodecs)
    - [ShortestPathEngine.cs](#shortestpathenginecs)
4. [Event Handling](#event-handling)
5. [Undo Functionality](#undo-functionality)
6. [Images and Icons](#images-and-icons)

## Introduction

GraphDrawer is a Windows WPF application running on .NET7.

## Project Structure

- **MainWindow.xaml**: Main window layout including the canvas and toolbox.
- **MainWindow.xaml.cs**: Main logic file handling drawing, user input, and interactions with the graph representation.
- **WeightInput.xaml & WeightInput.xaml.cs**: Handles user input for edge weights.
- **Graph.cs**: Defines the graph structure and related methods.
- **Edge.cs**: Defines the edge structure.
- **Node.cs**: Defines the node structure.
- **ShortestPathEngine.cs**: Contains the logic for computing the shortest path between nodes.
- **img folder**: Contains icon images used in the application.

## Main Components

### MainWindow.xaml

The XAML file defines the layout of the main application window, including the canvas where graph elements are drawn and the toolbox containing the various tools and options.

### MainWindow.xaml.cs

This file contains the core logic for handling user interactions and drawing operations on the canvas. It interfaces with the graph representation defined in `Graph.cs`, `Edge.cs`, and `Node.cs`.

- `GraphCanvas_MouseLeftButtonDown`: Determines actions when the left mouse button is pressed, such as selecting nodes, starting to draw nodes, lines, or curves.

- `GraphCanvas_MouseMove`: Updates the position of the object currently being drawn.

- `GraphCanvas_MouseLeftButtonUp`: Finalizes the drawing action, creating nodes, lines, or curves as necessary.


### WeightInput.xaml and WeightInput.xaml.cs

These files define the user interface and logic for inputting weights for weighted edges.

### Graph.cs

Defines the graph structure, managing nodes and edges, and providing methods to manipulate the graph, such as adding or removing nodes and edges.

### Edge.cs

Defines the structure and properties of edges, including weight information if applicable.

### Node.cs

Defines the structure and properties of nodes.

### ShortestPathEngine.cs

Contains the logic for computing the shortest path between two nodes using algorithms such as Dijkstra's or BFS. Chooses between them depending on if the graph is weighted.

## Undo Functionality

The application uses a stack-based event handling mechanism to manage actions on the canvas. Each action (e.g., drawing a node, drawing a line) is encapsulated as an `Event` object and pushed onto a stack. Each event can be undone by reverting the changes associated with that event.

## Images and Icons

The `img` folder contains various icons used in the application, such as toolbar icons. These images are referenced in the XAML files to provide a visual interface for the tools and options available in the application.
