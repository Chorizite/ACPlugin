﻿using Chorizite.Core.Backend.Client;
using Chorizite.Core.Input;
using Microsoft.Extensions.Logging;
using RmlUi;
using RmlUi.Lib;
using RmlUiNet;
using RmlUiNet.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC.Lib {

    internal class DragDropManager : IDisposable {
        private string _panelPath;
        private Panel? _panel;
        private bool _showingDragDropOverlay;
        private Dictionary<string, object> _dragDropExternalData;
        private int _mouseStartX = 0;
        private int _mouseStartY = 0;

        public DragDropManager() {
            ACPlugin.Instance.ClientBackend.UIBackend.OnGameObjectDragStart += OnGameObjectDragStart;
            ACPlugin.Instance.ClientBackend.UIBackend.OnGameObjectDragEnd += OnGameObjectDragEnd;
            ACPlugin.Instance.ChoriziteBackend.Input.OnMouseDown += Input_OnMouseDown;

            _panelPath = Path.Combine(ACPlugin.Instance.AssemblyDirectory, "assets", "panels", "DragDropOverlay.rml");
            if (File.Exists(_panelPath)) {
                _panel = ACPlugin.Instance.RmlUi.CreatePanel("DragDropOverlay", _panelPath);
                if (_panel is not null) {
                    _panel.IsGhost = true;
                    _panel.Hide();
                }
            }
        }

        private void Input_OnMouseDown(object? sender, MouseDownEventArgs e) {
            _mouseStartX = ACPlugin.Instance.ChoriziteBackend.Input.MouseX;
            _mouseStartY = ACPlugin.Instance.ChoriziteBackend.Input.MouseY;
        }

        private void OnGameObjectDragStart(object? sender, GameObjectDragDropEventArgs e) {
            if (_panel is null) return;

            _dragDropExternalData = new Dictionary<string, object>() {
                { "DropFlags", (uint)e.Flags },
                { "ObjectId", e.Id },
                { "IsSpell", e.IsSpell },
                { "ObjectName", e.Name },
                { "IconId", e.IconData.Icon },
                { "IconUnderlay", e.IconData.Underlay },
                { "IconOverlay", e.IconData.Overlay },
                { "IconEffects", (uint)e.IconData.Effects },
            };

            RmlUiPlugin.Instance.PanelManager.SetExternalDragDropEventData(_dragDropExternalData);

            _panel.Show();
            if (e.IsSpell) {
                _panel.ScriptableDocument?.GetElementById("icon-inner")?.SetProperty("decorator", $"image(dat://0x{e.IconData.Icon:X8}?underlay=0x{e.IconData.Underlay:X8}&overlay=0x{e.IconData.Overlay:X8}&effects={e.IconData.Effects})");
            }
            else {
                _panel.ScriptableDocument?.GetElementById("icon-inner")?.SetProperty("decorator", $"image(dat://0x{e.IconData.Icon:X8}?overlay=0x{e.IconData.Overlay:X8}&effects={e.IconData.Effects})");
            }

            if (!_showingDragDropOverlay) {
                _showingDragDropOverlay = true;
                CenterDragDropOverlayOnMouse();

                // first update the rmlui context so that this the dragdrop overlay is in the correct position
                // and rendered, then fake a mouse down on it to start the drag operation
                RmlUiPlugin.Instance.Context.Update();
                RmlUiPlugin.Instance.Context.Update();
                RmlUiPlugin.Instance.Context.ProcessMouseButtonDown(0, KeyModifier.None);

                ACPlugin.Instance.ChoriziteBackend.Input.OnMouseUp += Input_OnMouseUp;
                ACPlugin.Instance.ChoriziteBackend.Input.OnMouseMove += Input_OnMouseMove;
                RmlUiPlugin.Instance.Context.Update();
            }
        }

        private void Input_OnMouseMove(object? sender, MouseMoveEventArgs e) {
            if (_showingDragDropOverlay) {
                var el = _panel?.ScriptableDocument?.GetElementById("icon-inner");
                if (el is not null) {
                    CenterDragDropOverlayOnMouse();
                }
            }
        }

        private void Input_OnMouseUp(object? sender, MouseUpEventArgs e) {
            // if there is a panel under the mouse, cancel the game drag
            if (_showingDragDropOverlay && RmlUiPlugin.Instance?.PanelManager.IsAnyPanelUnderMouse() == true) {
                ACPlugin.Instance.ClientBackend.UIBackend.ClearDragandDrop();
                // dont eat (this is true because CoreUI says we are interacting with rmlui, not the game)
                // we dont eat otherwise the game will never see the mouse up, and not properly cancel the drag
                e.Eat = false;
            }
        }

        private void CenterDragDropOverlayOnMouse(Element? element = null) {
            element ??= _panel?.ScriptableDocument;
            if (element is null) return;


            var x = ACPlugin.Instance.ChoriziteBackend.Input.MouseX;
            var y = ACPlugin.Instance.ChoriziteBackend.Input.MouseY;
            x -= ((int)element.GetClientWidth() / 2);
            y -= ((int)element.GetClientHeight() / 2);
            element?.SetAttribute("style", "left: " + x + "px; top: " + y + "px;");
        }

        private void OnGameObjectDragEnd(object? sender, GameObjectDragDropEventArgs e) {
            if (_panel is null) return;

            if (_dragDropExternalData is not null) {
                RmlUiPlugin.Instance.PanelManager.ClearExternalDragDropEventData(_dragDropExternalData);
                _dragDropExternalData = null;
            }
            ACPlugin.Instance.ChoriziteBackend.Input.OnMouseUp -= Input_OnMouseUp;
            ACPlugin.Instance.ChoriziteBackend.Input.OnMouseMove -= Input_OnMouseMove;

            // if there is a panel under the mouse, eat the event so the game doesn't handle it
            if (RmlUiPlugin.Instance?.PanelManager.IsAnyPanelUnderMouse() == true) {
                e.Eat = true;
            }

            _panel.Hide();
            _showingDragDropOverlay = false;
        }

        public void Dispose() {
            ACPlugin.Instance.ClientBackend.UIBackend.OnGameObjectDragStart -= OnGameObjectDragStart;
            ACPlugin.Instance.ClientBackend.UIBackend.OnGameObjectDragEnd -= OnGameObjectDragEnd;
            ACPlugin.Instance.ChoriziteBackend.Input.OnMouseUp -= Input_OnMouseUp;
            ACPlugin.Instance.ChoriziteBackend.Input.OnMouseDown -= Input_OnMouseDown;
            ACPlugin.Instance.ChoriziteBackend.Input.OnMouseMove -= Input_OnMouseMove;

            if (_dragDropExternalData is not null) {
                RmlUiPlugin.Instance.PanelManager.ClearExternalDragDropEventData(_dragDropExternalData);
                _dragDropExternalData = null;
            }
            _panel?.Dispose();
        }
    }
}
