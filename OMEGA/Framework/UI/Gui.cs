using System;
using System.Collections.Generic;

namespace OMEGA.Framework.UI
{
    public class Gui
    {
        public int Width => _surface.Width;

        public int Height => _surface.Height;

        public Widget HoveredWidget { get; private set; }

        public Widget ActiveWidget { get; private set; }

        public Widget InputFocusedWidget { get; private set; }

        public IGuiDrawer Drawer { get; set; }

        public bool DebugMode { get; set; } = false;

        private static int _zindex = 1;

        private readonly Dictionary<string, int> _map;

        private readonly Dictionary<string, List<Widget>> _toggleGroups;

        private readonly List<Widget> _widgets;

        private List<Widget> _updatableWidgets;

        private readonly Container _root;

        private int _lastMouseX;
        private int _lastMouseY;

        private int _mouseX;
        private int _mouseY;

        private readonly Texture2D _surface;
        private readonly Quad _quad;

        private bool _refresh = true;

        public Gui()
        {
            Input.EnableMouseButtonPosEventPooling = true;

            Input.OnMouseDown += ProcessMouseDown;
            Input.OnMouseUp += ProcessMouseUp;
            Input.OnMouseMove += ProcessMouseMove;
            Input.OnKeyDown += ProcessKeyDown;
            Input.OnKeyUp += ProcessKeyUp;

            Drawer = new DefaultTheme(this);

            _root = new Container("root", Engine.Canvas.Width, Engine.Canvas.Height);

            _map = new Dictionary<string, int>();

            _toggleGroups = new Dictionary<string, List<Widget>>();

            _widgets = new List<Widget>();

            _surface = Texture2D.Create(Engine.Canvas.Width, Engine.Canvas.Height, Color.Transparent);

            _quad = new Quad(_surface);

            Widget.Gui = this;
        }

        public Widget this[string id] => _map.TryGetValue(id, out var index) ? _widgets[index] : null;

        public void Add(Widget widget)
        {
            if (Register(widget))
            {
                _root.Add(widget);
            }

            RecalculateZIndices(_root);
            ReorderWidgets();
        }

        public void SetProcess(Widget widget, bool process)
        {
            _updatableWidgets ??= new List<Widget>();

            if (process)
            {
                if (!_updatableWidgets.Contains(widget))
                {
                    _updatableWidgets.Add(widget);
                }
                
            }
            else
            {
                _updatableWidgets.Remove(widget);
            }
        }

        public void SetVisible(string widgetName, bool visible)
        {
            if (_map.TryGetValue(widgetName, out int widget_index))
            {
                SetVisible(_widgets[widget_index], visible);
            }
        }

        public void SetVisible(Widget widget, bool visible)
        {
            _refresh = true;

            widget.Visible = visible;

            if (visible)
            {
                if (widget is Container container)
                {
                    foreach (var container_child in container.Children)
                    {
                        SetVisible(container_child, true);
                    }
                }
            }
            else
            {
                if (HoveredWidget != null && HoveredWidget == widget)
                {
                    SetHovered(widget, false);
                }
                else
                {
                    widget.Hovered = false;
                }

                if (InputFocusedWidget == widget)
                {
                    SetInputFocus(widget, false);
                }
                else
                {
                    widget.HasInputFocus = false;
                }

                if (widget is Container container)
                {
                    foreach (var container_child in container.Children)
                    {
                        SetVisible(container_child, false);
                    }
                }
            }

            ProcessMouseMove(Input.MousePos.X, Input.MousePos.Y);
        }

        public void Update()
        {
            if (_updatableWidgets == null)
            {
                return;
            }

            foreach (var updatable_widget in _updatableWidgets)
            {
                updatable_widget.Update();
            }
        }

        public void Draw(Canvas2D canvas)
        {
            if (_refresh)
            {
                Console.WriteLine("Redraw Gui");

                Blitter.Begin(_surface);

                Blitter.Clear();

                _root.Draw(Drawer);

                if (DebugMode)
                {
                    Blitter.DrawText(10, 10, $"Hovered: {HoveredWidget?.Id ?? "None"}");
                    Blitter.DrawText(10, 35, $"Active: {ActiveWidget?.Id ?? "None"}");
                    Blitter.DrawText(10, 65, $"Input Focused: {InputFocusedWidget?.Id ?? "None"}");

                    //for (int i = 0; i < _widgets.Count; ++i)
                    //{
                    //    var w = _widgets[i];
                    //    blitter.Text(10, 50 + (i) * 10, $"{w.Id} [ZIndex: {w.ZIndex}]", 1);
                    //}
                }

                Blitter.End();

                _refresh = false;
            }

            

            canvas.Begin();

            canvas.DrawQuad(in _quad, _surface);

            canvas.End();
            
        }

        internal void AssignToggleGroup(Widget widget, string group)
        {
            if (group != null)
            {
                if (!_toggleGroups.TryGetValue(group, out _))
                {
                    _toggleGroups.Add(group, new List<Widget>());
                }

                _toggleGroups[group].Add(widget);
            }
            else
            {
                var empty_groups = new List<string>();

                foreach (var (groupId, groups) in _toggleGroups)
                {
                    if (!groups.Contains(widget)) continue;

                    groups.Remove(widget);

                    if (groups.Count == 0)
                    {
                        empty_groups.Add(groupId);
                    }
                }

                foreach (var empty_group in empty_groups)
                {
                    _toggleGroups.Remove(empty_group);
                }

                empty_groups.Clear();
            }
        }

        internal bool Register(Widget widget)
        {
            if (_widgets.Contains(widget)) return false;

            _widgets.Add(widget);
            RecalculateZIndices(_root);
            ReorderWidgets();
            return true;

        }

        internal void SetInputFocus(Widget widget, bool focus)
        {
            widget.HasInputFocus = focus;

            if (!focus)
            {
                if (widget == InputFocusedWidget)
                {
                    InputFocusedWidget = null;
                }
            }
            else
            {
                InputFocusedWidget = widget;
            }
        }

        internal void SetHovered(Widget widget, bool hovered)
        {
            if (hovered)
            {
                HoveredWidget = widget;
                HoveredWidget.Hovered = true;
            }
            else
            {
                widget.Hovered = false;
                HoveredWidget = null;
            }
        }

        private void RecalculateZIndices(Widget widget)
        {
            _zindex = -1;
            LoopRecursiveZIndex(widget);
        }

        private void LoopRecursiveZIndex(Widget widget)
        {
            widget.ZIndex = _zindex++;

            if (widget is Container container)
            {
                foreach (var container_child in container.Children)
                {
                    LoopRecursiveZIndex(container_child);
                }
            }
        }

        private void ReorderWidgets()
        {
            _widgets.Sort((widget1, widget2) => widget2.ZIndex.CompareTo(widget1.ZIndex));

            for (int i = 0; i < _widgets.Count; ++i)
            {
                _map[_widgets[i].Id] = i;
            }
        }

        private void ProcessKeyUp(Keys key)
        {
            InputFocusedWidget?.ProcessKeyUp(key);
        }

        private void ProcessKeyDown(Keys key)
        {
            InputFocusedWidget?.ProcessKeyDown(key);
        }

        private void ProcessMouseMove(int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return;
            }

            _mouseX = x;
            _mouseY = y;

            foreach (var widget in _widgets)
            {
                if (widget.IgnoreInput || !widget.Visible)
                {
                    continue;
                }

                if (widget.GlobalGeometry.Contains(x, y))
                {
                    if (HoveredWidget != null && HoveredWidget != widget && HoveredWidget.Hovered)
                    {
                        HoveredWidget.Hovered = false;
                        HoveredWidget.ProcessMouseLeave();
                        _refresh = true;
                    }

                    var to_be_hovered = widget;

                    if (widget.BubbleEventsToParent && widget.Parent != null)
                    {
                        to_be_hovered = widget.Parent;
                    }

                    if (!to_be_hovered.Hovered)
                    {
                        _refresh = true;
                        widget.ProcessMouseEnter();
                    }

                    SetHovered(to_be_hovered, true);

                    break;
                }
                if (widget.Hovered && widget == HoveredWidget)
                {
                    widget.ProcessMouseLeave();

                    SetHovered(widget, false);
                }
            }

            if (ActiveWidget != null)
            {
                if (!ActiveWidget.Draggable)
                {
                    ActiveWidget.ProcessMouseMove(x - ActiveWidget.DrawX, y - ActiveWidget.DrawY);
                }
                else if (ActiveWidget.Dragging)
                {
                    var dx = _mouseX - _lastMouseX;
                    var dy = _mouseY - _lastMouseY;

                    ActiveWidget.X += dx;
                    ActiveWidget.Y += dy;
                }
            }
            else
            {
                HoveredWidget?.ProcessMouseMove(x - HoveredWidget.DrawX, y - HoveredWidget.DrawY);
            }

            _lastMouseX = _mouseX;
            _lastMouseY = _mouseY;
        }

        private void ProcessMouseUp(MouseButton button)
        {
            if (ActiveWidget == null)
            {
                return;
            }

            _refresh = true;

            if (ActiveWidget.Draggable)
            {
                ActiveWidget.Dragging = false;
            }

            ActiveWidget.ProcessMouseUp(button, _mouseX - ActiveWidget.DrawX, _mouseY - ActiveWidget.DrawY);

            ActiveWidget.Active = false;
            ActiveWidget = null;
        }

        private void ProcessMouseDown(MouseButton button)
        {
            if (HoveredWidget == null)
            {
                InputFocusedWidget = null;
                return;
            }

            _refresh = true;

            HoveredWidget.Active = true;
            ActiveWidget = HoveredWidget;
            ActiveWidget.ProcessMouseDown(button, _mouseX - ActiveWidget.DrawX, _mouseY - ActiveWidget.DrawY);

            if (button == MouseButton.Left)
            {
                if (ActiveWidget.Toggable && ActiveWidget.ToggleGroup == null)
                {
                    ActiveWidget.On = !ActiveWidget.On;
                }
                else if (ActiveWidget.Toggable && ActiveWidget.ToggleGroup != null)
                {
                    ActiveWidget.On = true;
                    UpdateToggleGroup(ActiveWidget);
                }

                if (InputFocusedWidget != null)
                {
                    InputFocusedWidget.HasInputFocus = false;
                }

                if (ActiveWidget.CanHaveInputFocus)
                {
                    InputFocusedWidget = ActiveWidget;
                    InputFocusedWidget.HasInputFocus = true;
                }

                if (ActiveWidget.Draggable)
                {
                    ActiveWidget.Dragging = true;
                }
            }

            _lastMouseX = _mouseX;
            _lastMouseY = _mouseY;
        }

        internal void UpdateToggleGroup(Widget widget)
        {
            if (widget.ToggleGroup == null)
            {
                return;
            }

            var toggle_group = _toggleGroups[widget.ToggleGroup];

            foreach (var widget_in_group in toggle_group)
            {
                if (widget_in_group != widget)
                {
                    widget_in_group.On = false;
                }
            }
        }
    }
}