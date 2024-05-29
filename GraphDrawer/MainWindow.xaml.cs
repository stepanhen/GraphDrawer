using GraphDrawer;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphDrawingApp
{
    
    public partial class MainWindow : Window
    {
        // Represents the current object being drawn
        private Ellipse currentNode; 
        private Line currentLine;
        private Polyline currentPolyline;

        // List of all objects of a type
        private List<Ellipse> nodes;
        private List<Shape> lines;

        // Can be Node, Straight line, Weighted straight line, Curve, Weighted curve
        private string selectedDrawStyle;

        // Fields needed for node drawing
        private bool isDrawingNode;
        private int nodeRadius = 15;
        private SolidColorBrush selectedColor;

        // Fields needed for edge drawing
        private Ellipse selectedNode; // If we start drawing an edge, selectedNode is the starting node

        // Stack for undoing
        private Stack<Event> eventHappened = new Stack<Event>();

        // Graph representation
        private Graph graph;

        // Fields for finding the shortest path
        private Ellipse startNode;
        private Ellipse endNode;
        private bool isSelectingShortestPath = false;

        public MainWindow()
        {
            InitializeComponent();
            nodes = new List<Ellipse>();
            lines = new List<Shape>();

            // Default settings
            selectedColor = Brushes.Black;
            selectedDrawStyle = "Node";

            // Set actions
            graphCanvas.MouseLeftButtonDown += GraphCanvas_MouseLeftButtonDown;
            graphCanvas.MouseMove += GraphCanvas_MouseMove;
            graphCanvas.MouseLeftButtonUp += GraphCanvas_MouseLeftButtonUp;

            // Graph representation
            graph = new Graph();
        }

        /// <summary>
        /// Decide what to do when Mouse left button is down. Choices are: Select node and try finding the shortest path, Start drawing a node, Start drawing a line or curve with or without weight.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(graphCanvas);
            if (isSelectingShortestPath)
            {
                Ellipse node = GetNodeAtPosition(position);
                var previousColors = DrawShortestPath(node); 
                eventHappened.Push(new Event(previousColors)); // Store the previousColors to allow undoing the new coloring
                return;
            }
            else if (selectedDrawStyle == "Node")
            {
                // Check if the new node overlaps with any existing nodes
                if (IsPositionValidForNode(position))
                {
                    // Create a new node with the selected color
                    currentNode = new Ellipse
                    {
                        Width = 2 * nodeRadius,
                        Height = 2 * nodeRadius,
                        Fill = selectedColor
                    };
                    Canvas.SetLeft(currentNode, position.X - nodeRadius);
                    Canvas.SetTop(currentNode, position.Y - nodeRadius);
                    Canvas.SetZIndex(currentNode, 1); // Ensure node is in the foreground

                    // Add node to all collections
                    graphCanvas.Children.Add(currentNode);
                    nodes.Add(currentNode);
                    eventHappened.Push(new Event(currentNode, graphCanvas));

                    isDrawingNode = true;

                    // Add node to the graph representation
                    int nodeID = nodes.IndexOf(currentNode);
                    graph.CreateNode(nodeID, currentNode);
                }
            }
            else
            {
                // Start drawing an edge
                selectedNode = GetNodeAtPosition(position);
                if (selectedNode != null)
                {
                    if (selectedDrawStyle == "Straight" || selectedDrawStyle == "Straight_Weighted")
                    {
                        currentLine = new Line
                        {
                            Stroke = selectedColor,
                            X1 = (int)position.X,
                            Y1 = (int)position.Y,
                            X2 = (int)position.X,
                            Y2 = (int)position.Y,
                            StrokeThickness = 2
                        };
                        Canvas.SetZIndex(currentLine, 0); // Ensure line is in the background
                        graphCanvas.Children.Add(currentLine);
                    }
                    else if (selectedDrawStyle == "Follow_Mouse" || selectedDrawStyle == "Follow_Mouse_Weighted")
                    {
                        currentPolyline = new Polyline
                        {
                            Stroke = selectedColor,
                            StrokeThickness = 2
                        };
                        currentPolyline.Points.Add(new Point((int)position.X, (int)position.Y));
                        Canvas.SetZIndex(currentPolyline, 0); // Ensure polyline is in the background
                        graphCanvas.Children.Add(currentPolyline);
                    }
                }
            }
        }
        /// <summary>
        /// Updates position of the currently drawn object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawingNode && currentNode != null)
            {
                Point position = e.GetPosition(graphCanvas);

                // Check if the new node overlaps with any existing nodes, if not update the position
                if (IsPositionValidForNode(position))
                {
                    Canvas.SetLeft(currentNode, position.X - currentNode.Width / 2);
                    Canvas.SetTop(currentNode, position.Y - currentNode.Height / 2);
                }
            }
            else if (currentLine != null && selectedNode != null)
            {
                Point position = e.GetPosition(graphCanvas);

                currentLine.X2 = (int)position.X;
                currentLine.Y2 = (int)position.Y;

            }
            else if (currentPolyline != null && selectedNode != null)
            {
                Point position = e.GetPosition(graphCanvas);
                currentPolyline.Points.Add(new Point((int)position.X, (int)position.Y));
            }
        }
        /// <summary>
        /// Decides what to do when Mouse left button is up. 
        /// Choices are: 
        ///              Create a node,
        ///              Create a line with or without weight,
        ///              Create a curve with or without weight.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingNode) // Creates a node
            {
                isDrawingNode = false;
                currentNode = null;

                // Enable undo button
                Undo.IsEnabled = true;
            }
            else if (currentLine != null && selectedNode != null)
            {   // Creates a line if the end point is a node. Also add the edge to the graph with or without the weight and push the event to the stack.
                Point position = e.GetPosition(graphCanvas);
                Ellipse targetNode = GetNodeAtPosition(position);
                if (targetNode != null && targetNode != selectedNode)
                {
                    int startNodeID = nodes.IndexOf(selectedNode);
                    int targetNodeID = nodes.IndexOf(targetNode);

                    if (graph.HasEdge(startNodeID, targetNodeID))
                    {
                        graphCanvas.Children.Remove(currentLine);
                    }
                    else
                    {
                        TextBlock? textBlock = null;
                        if (selectedDrawStyle == "Straight_Weighted")
                        {
                            int? weight = AskForWeight();
                            if (weight == null)
                            {
                                graphCanvas.Children.Remove(currentLine);
                                return;
                            }
                            textBlock = DrawWeightOnLine(currentLine, (int)weight);
                            graph.AddWeightedEdge(currentLine, startNodeID, targetNodeID, (int)weight);
                        }
                        else
                        {
                            graph.AddEdge(currentLine, startNodeID, targetNodeID);
                        }
                        currentLine.X2 = GetNodeX(targetNode);
                        currentLine.Y2 = GetNodeY(targetNode);

                        lines.Add(currentLine);

                        if (selectedDrawStyle == "Straight")
                        {
                            eventHappened.Push(new Event(currentLine, graphCanvas));
                        }
                        else if(textBlock != null)
                        {
                            eventHappened.Push(new Event(currentLine, textBlock, graphCanvas));
                        }


                    }
                }
                else
                {
                    graphCanvas.Children.Remove(currentLine);
                }
                currentLine = null;
                selectedNode = null;
            }
            else if (currentPolyline != null && selectedNode != null)
            {   // Creates a curve if the end point is a node. Also add the edge to the graph with or without the weight and push the event to the stack.
                Point position = e.GetPosition(graphCanvas);
                Ellipse targetNode = GetNodeAtPosition(position);
                if (targetNode != null && targetNode != selectedNode)
                {
                    // Add edge to the graph
                    int startNodeID = nodes.IndexOf(selectedNode);
                    int targetNodeID = nodes.IndexOf(targetNode);

                    if (graph.HasEdge(startNodeID, targetNodeID))
                    {
                        graphCanvas.Children.Remove(currentPolyline);
                    }
                    else
                    {
                        TextBlock? textBlock = null;
                        if (selectedDrawStyle == "Follow_Mouse_Weighted")
                        {
                            int? weight = AskForWeight();
                            if (weight == null)
                            {
                                graphCanvas.Children.Remove(currentLine);
                                return;
                            }
                            textBlock = DrawWeightOnPolyline(currentPolyline, (int)weight);
                            graph.AddWeightedEdge(currentPolyline, startNodeID, targetNodeID, (int)weight);
                        }
                        else
                        {
                            graph.AddEdge(currentPolyline, startNodeID, targetNodeID);
                        }

                        currentPolyline.Points.Add(new Point(GetNodeX(targetNode), GetNodeY(targetNode)));

                        lines.Add(currentPolyline);

                        if (selectedDrawStyle == "Follow_Mouse")
                        {
                            eventHappened.Push(new Event(currentPolyline, graphCanvas));
                        }
                        else if (textBlock != null)
                        {
                            eventHappened.Push(new Event(currentPolyline, textBlock, graphCanvas));
                        }
                    }
                }
                else
                {
                    graphCanvas.Children.Remove(currentPolyline);
                }
                currentPolyline = null;
                selectedNode = null;
            }
        }
        private int GetDistance(Point p1, Point p2)
        {
            int dx = (int)p1.X - (int)p2.X;
            int dy = (int)p1.Y - (int)p2.Y;
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }

        private int GetNodeX(Ellipse node)
        {
            return (int)Canvas.GetLeft(node) + (int)node.Width / 2;
        }
        private int GetNodeY(Ellipse node)
        {
            return (int)Canvas.GetTop(node) + (int)node.Height / 2;
        }
        /// <summary>
        /// Checks if the position is free and node at this position doesn't overlap with other nodes.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsPositionValidForNode(Point position)
        {
            foreach (var node in nodes)
            {
                if (node == currentNode)
                {
                    continue;
                }
                int nodeCenterX = GetNodeX(node);
                int nodeCenterY = GetNodeY(node);
                int distance = GetDistance(position, new Point(nodeCenterX, nodeCenterY));
                if (distance < 2 * nodeRadius)
                {
                    return false; // Position is too close to an existing node
                }
            }
            return true;
        }

        private Ellipse GetNodeAtPosition(Point position)
        {
            return GetNodeAtPosition((int)position.X, (int)position.Y);
        }

        private Ellipse GetNodeAtPosition(int X, int Y)
        {
            foreach (var node in nodes)
            {
                int left = (int)Canvas.GetLeft(node);
                int top = (int)Canvas.GetTop(node);
                if (X >= left && X <= left + node.Width &&
                    Y >= top && Y <= top + node.Height)
                {
                    return node;
                }
            }
            return null;
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (colorComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Black":
                        selectedColor = Brushes.Black;
                        break;
                    case "Blue":
                        selectedColor = Brushes.Blue;
                        break;
                    case "Red":
                        selectedColor = Brushes.Red;
                        break;
                    case "Green":
                        selectedColor = Brushes.Green;
                        break;
                    case "Yellow":
                        selectedColor = Brushes.Yellow;
                        break;
                    case "Purple":
                        selectedColor = Brushes.Purple;
                        break;
                }
            }
        }
        /// <summary>
        /// Change selected DrawMode. The options are Node, Straight line, Weighted straight line, Curve, Weighted curve.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DrawModeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                selectedDrawStyle = selectedItem.Name;
            }
        }

        /// <summary>
        /// This button is for debugging purposes.
        /// Prints all nodes and lines with their coordinates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowObjectsButton_Click(object sender, RoutedEventArgs e)
        {
            var allLines = lines;
            string message = "Lines:\n";
            foreach (var line in allLines)
            {
                if (line is Line)
                {
                    var l = (Line)line;
                    message += $"Line from ({l.X1},{l.Y1}) to ({l.X2},{l.Y2})\n";
                }
                else if (line is Polyline)
                {
                    var pl = (Polyline)line;
                    message += $"Polyline from ({pl.Points[0].X},{pl.Points[0].Y}) to ({pl.Points[pl.Points.Count - 1].X},{pl.Points[pl.Points.Count - 1].Y})\n";
                }
            }
            message += "Nodes:\n";
            foreach (Ellipse node in nodes)
            {
                message += $"Node at ({GetNodeX(node)}, {GetNodeY(node)})\n";
            }

            MessageBox.Show(message, "All Lines");
        }
        /// <summary>
        /// Pops an item from eventHappend stack and reverts it. If it was a node or an edge, removes it from the graph representation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoButton_Click(Object sender, RoutedEventArgs e)
        {
            Event lastEvent = eventHappened.Pop();
            lastEvent.RevertEvent();
            if (!lastEvent.isColoring)
            {
                Shape lastAddedShape = lastEvent.shape;
                if (lastAddedShape is Ellipse el)
                {
                    // Remove node in graph
                    graph.RemoveNode(nodes.IndexOf(el));

                    nodes.Remove(el);
                }
                else
                {
                    // Remove edge in graph
                    if (lastAddedShape is Line l)
                    {
                        var startNode = GetNodeAtPosition((int)l.X1, (int)l.Y1);
                        var targetNode = GetNodeAtPosition((int)l.X2, (int)l.Y2);

                        graph.RemoveEdge(nodes.IndexOf(startNode), nodes.IndexOf(targetNode));
                    }
                    else if (lastAddedShape is Polyline pl)
                    {
                        var startNode = GetNodeAtPosition(pl.Points[0]);
                        var targetNode = GetNodeAtPosition(pl.Points[pl.Points.Count - 1]);

                        graph.RemoveEdge(nodes.IndexOf(startNode), nodes.IndexOf(targetNode));
                    }

                    lines.Remove(lastAddedShape);
                }
            } 
            else
            {
                lastEvent = eventHappened.Pop();
                lastEvent.RevertEvent();
            }
            if (eventHappened.Count == 0)
            {
                Undo.IsEnabled = false;
            }
        }
        /// <summary>
        /// Pops up a window to get weight for an edge.
        /// </summary>
        /// <returns>The weight.</returns>
        private int? AskForWeight()
        {
            WeightInput weightInput = new WeightInput();
            if (weightInput.ShowDialog() == true)
            {
                if (int.TryParse(weightInput.InputText, out int weight))
                {
                    return weight;
                }
                else
                {
                    MessageBox.Show("Please enter a valid integer.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return null;
        }
        /// <summary>
        /// Writes a weight in the middle of the curve. Tries to calculate offset so that the text doesn't cover the curve.
        /// </summary>
        /// <param name="line">Curve to write weight next to.</param>
        /// <param name="weight">Weight of the edge.</param>
        /// <returns>The TextBlock to allow weight writing undoing.</returns>
        private TextBlock DrawWeightOnPolyline(Polyline polyline, int weight)
        {
            double midX = polyline.Points[polyline.Points.Count / 2].X;
            double midY = polyline.Points[polyline.Points.Count / 2].Y;

            double dx = polyline.Points[polyline.Points.Count / 2 - 10].X + polyline.Points[polyline.Points.Count / 2 + 10].X;
            double dy = polyline.Points[polyline.Points.Count / 2 - 10].Y + polyline.Points[polyline.Points.Count / 2 + 10].Y;

            double length = Math.Sqrt(dx * dx + dy * dy);
            dx /= length;
            dy /= length;

            double offsetX = -dy * 20;
            double offsetY = dx * 10;

            double textX = midX + offsetX;
            double textY = midY + offsetY;

            TextBlock textBlock = new TextBlock
            {
                Text = weight.ToString(),
                Foreground = Brushes.Black,
                Background = Brushes.White,
            };

            Canvas.SetLeft(textBlock, textX);
            Canvas.SetTop(textBlock, textY);
            graphCanvas.Children.Add(textBlock);

            return textBlock;
        }
        /// <summary>
        /// Writes a weight in the middle of the line. Tries to calculate offset so that the text doesn't cover the line.
        /// </summary>
        /// <param name="line">Line to write weight next to.</param>
        /// <param name="weight">Weight of the edge.</param>
        /// <returns>The TextBlock to allow weight writing undoing.</returns>
        private TextBlock DrawWeightOnLine(Line line, int weight)
        {
            // Calculate the midpoint of the line
            double midX = (line.X1 + line.X2) / 2;
            double midY = (line.Y1 + line.Y2) / 2;

            // Calculate the direction of the line
            double dx = line.X2 - line.X1;
            double dy = line.Y2 - line.Y1;

            // Normalize the direction vector
            double length = Math.Sqrt(dx * dx + dy * dy);
            dx /= length;
            dy /= length;

            // Perpendicular direction
            double offsetX = -dy * 20;
            double offsetY = dx * 10;

            // Position the text slightly off the line
            double textX = midX + offsetX;
            double textY = midY + offsetY;

            TextBlock textBlock = new TextBlock
            {
                Text = weight.ToString(),
                Foreground = Brushes.Black,
            };

            Canvas.SetLeft(textBlock, textX);
            Canvas.SetTop(textBlock, textY);
            graphCanvas.Children.Add(textBlock);

            return textBlock;
        }
        /// <summary>
        /// If there is zero selected nodes, select first node. Else select second node, find the shortest path and draw it.
        /// </summary>
        /// <param name="node"> Is a selected node for the shortest path. </param>
        /// <returns>List of (Shape, SolidColorBrush) pairs, which represent original coloring of the recolored path to allow coloring undoing.</returns>
        private List<(Shape, SolidColorBrush)> DrawShortestPath(Ellipse node)
        {
            List<(Shape, SolidColorBrush)> previousColors = new List<(Shape, SolidColorBrush)>(); // Keep the original coloring of nodes and edges
            if (node != null)
            {
                if (startNode == null)
                {
                    startNode = node;
                }
                else
                {
                    endNode = node;
                    isSelectingShortestPath = false;
                    var sp = graph.FindShortestPath(nodes.IndexOf(startNode), nodes.IndexOf(endNode));

                    // Show the path
                    for (int i = 0; i < sp.Count; i++)
                    {
                        if (i == 0 || i == sp.Count - 1)
                        {
                            if (sp[i] is Ellipse el)
                            {
                                previousColors.Add((el, (SolidColorBrush) el.Fill));
                                el.Fill = Brushes.Red;
                            }
                        }
                        else
                        {
                            if (sp[i] is Ellipse el)
                            {
                                previousColors.Add((el, (SolidColorBrush) el.Fill));
                                el.Fill = Brushes.Yellow;
                            }
                            else if (sp[i] is Line l)
                            {
                                previousColors.Add((l, (SolidColorBrush) l.Stroke));
                                l.Stroke = Brushes.Red;
                            }
                            else if (sp[i] is Polyline pl)
                            {
                                previousColors.Add((pl, (SolidColorBrush)pl.Stroke));
                                pl.Stroke = Brushes.Red;
                            }
                        }
                    }
                    ShortestPath.Content = "Shortest Path";
                }
            }
            return previousColors;
        }
        /// <summary>
        /// When pressed, selecting two nodes is needed to start finding the shortest path between them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShortestPathButton_Click(Object sender, RoutedEventArgs e)
        {
            if (isSelectingShortestPath)
            {
                isSelectingShortestPath = false;
                startNode = null;
                endNode = null;
                ShortestPath.Content = "Shortest path";
                return;
            }
            startNode = null;
            endNode = null;
            isSelectingShortestPath = true;
            ShortestPath.Content = "Click to cancel";
            MessageBox.Show("Select two nodes to find shortest path or click button again to cancel.");
        }
        /// <summary>
        /// Event class represents events like drawing a line, drawing a node, recoloring of the shortest path.
        /// </summary>
        private class Event
        {
            public bool isColoring = false;
            private List<(Shape, SolidColorBrush)> colors;
            public Shape shape;
            private TextBlock text;
            private Canvas canvas;

            public Event(List<(Shape, SolidColorBrush)> colors)
            {
                this.colors = colors;
                isColoring = true;
            }
            public Event(Shape shape, TextBlock text, Canvas canvas)
            {
                this.shape = shape;
                this.text = text;
                this.canvas = canvas;
            }
            public Event(Shape shape, Canvas canvas)
            {
                this.shape = shape;
                this.canvas = canvas;
            }
            /// <summary>
            /// Reverts all changes that happened in this event.
            /// </summary>
            public void RevertEvent()
            {
                if (isColoring)
                {
                    foreach ((var shape, var color) in colors)
                    {
                        if (shape is Ellipse s)
                        {
                            s.Fill = color;
                        }
                        else if (shape is Line l)
                        {
                            l.Stroke = color;
                        }
                        else if (shape is Polyline pl)
                        {
                            pl.Stroke = color;
                        }
                    }
                }
                else
                {
                    canvas.Children.Remove(shape);
                    if (text != null)
                    {
                        canvas.Children.Remove(text);
                    }
                }
            }
        }
    }
}
