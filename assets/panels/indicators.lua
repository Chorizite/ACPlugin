local rx = require('rx')
local RootElementId = CS.Chorizite.Common.Enums.RootElementId
local backend = require('ClientBackend')
local ui = require('Plugins.RmlUi')
local PluginManager = require('PluginManager')
local DamageType = CS.Chorizite.Common.Enums.DamageType
local Game = require('Plugins.AC').Game

local state = rx:CreateState({
  UIIsLocked = backend.UIBackend.IsUILocked,
  loggedIn = Game.State == CS.AC.API.ClientState.InGame or Game.State == CS.AC.API.ClientState.EnteringGame,
  Plugins = {},
  Cheating = 1
})

local function LoadPlugins()
  for i=0,ui.PanelManager.Panels.Count-1 do
    if ui.PanelManager.Panels[i].ShowInBar then
      state.Plugins[ui.PanelManager.Panels[i].Name] = {
        ["isVisible"] = ui.PanelManager.Panels[i].IsVisible,
        ["icon"] = ui.PanelManager.Panels[i].IconUri
      }
    end
  end
  state.Cheating = state.Cheating + 1
end

Game:OnStateChanged('+', function (sender, evt)
  state.loggedIn = evt.NewState == CS.AC.API.ClientState.InGame or evt.NewState == CS.AC.API.ClientState.EnteringGame
end )

PluginManager:OnPluginsLoaded('+', function(s, e)
  LoadPlugins()
end)
LoadPlugins()

backend.UIBackend:OnUILockChanged('+', function(s, e)
  state.UIIsLocked = e.IsLocked
end)

ui.PanelManager:OnPanelAdded('+', function(s, e)
  if e.Panel.ShowInBar then
    state.Plugins[e.Panel.Name] = state.Plugins[e.Panel.Name] or {}
    state.Plugins[e.Panel.Name].isVisible = e.Panel.IsVisible
    state.Plugins[e.Panel.Name].icon = e.Panel.IconUri
    state.Cheating = state.Cheating + 1
  end
end)

ui.PanelManager:OnPanelRemoved('+', function(s, e)
  state.Plugins[e.Panel.Name] = nil
  state.Cheating = state.Cheating + 1
end)

ui.PanelManager:OnPanelVisibilityChanged('+', function(s, e)
  if e.Panel.ShowInBar then
    state.Plugins[e.Panel.Name] = state.Plugins[e.Panel.Name] or {}
    state.Plugins[e.Panel.Name].isVisible = e.Panel.IsVisible
    state.Plugins[e.Panel.Name].icon = e.Panel.IconUri
    state.Cheating = state.Cheating + 1
  end
end)

local IndicatorsView = function(state)
  return rx:Div({ class="panel" }, {
    rx:Div({ class="panel-inner" }, {
      rx:Handle({
        move_target="#document"
      }, {
        rx:Div({
          class= {
            hidden = state.UIIsLocked
          },
          id="handle"
        })
      }),
      rx:Div({ class="game-icons"}, {
        rx:Div({ class="status-icons" }, function()
          if state.loggedIn then
            return {
              rx:Button({
                class="icon",
                style="decorator: image(dat://0x06007498);",
                onclick = function()
                  backend.UIBackend:ToggleRootElement(RootElementId.LinkStatus)
                end
              }),
              rx:Button({
                class="icon",
                style="decorator: image(dat://0x0600749C);",
                onclick = function()
                  backend.UIBackend:ToggleRootElement(RootElementId.PositiveEffects)
                end
              }),
              rx:Button({
                class="icon",
                style="decorator: image(dat://0x0600749F);",
                onclick = function()
                  backend.UIBackend:ToggleRootElement(RootElementId.NegativeEffects)
                end
              }),
              rx:Button({
                class="icon",
                style="decorator: image(dat://0x060074A1);",
                onclick = function()
                  backend.UIBackend:ToggleRootElement(RootElementId.Vitae)
                end
              }),
              rx:Button({
                class="icon",
                style="decorator: image(dat://0x060074A2);",
                onclick = function()
                  backend.UIBackend:ToggleRootElement(RootElementId.CharacterInfo)
                end
              }),
              rx:Button({
                class="icon",
                style="decorator: image(dat://0x060074A6);",
                onclick = function()
                  backend.UIBackend:ToggleRootElement(RootElementId.MiniGame)
                end
              }),
            }
          else
            return {}
          end
        end),
        rx:Div({
          class="plugin-icons",
          count = state.Cheating
        }, function()
          local res = {}
          for panelName,panelInfo in pairs(state.Plugins) do
            res[#res + 1] = rx:Button({
              class={
                icon = true,
                visible = panelInfo.isVisible
              },
              onclick=function()
                if panelInfo.isVisible then
                  ui.PanelManager:GetPanel(panelName):Hide()
                else
                  ui.PanelManager:GetPanel(panelName):Show()
                  ui.PanelManager:GetPanel(panelName):PullToFront()
                end
              end
            }, function()
              if panelInfo.icon ~= nil then
                return {
                  rx:Div({ class="overlay" }, {
                    rx:Img({ class="overlay", src=panelInfo.icon })
                  })
                }
              else
                return {
                  rx:Div({ class="overlay" }, string.sub(panelName, 1, 1))
                }
              end
            end)
          end
          return res
        end),
        rx:Button({
          class="icon",
          style="decorator: image(dat://0x060074B1);",
          onclick = function()
            ui.PanelManager:ShowModalConfirmation("Are you sure you want to log off?", function(res)
              if res == "Yes" then
                backend:LogOff()
              end
            end, "Yes", "No")
          end
        }),
      })
    })
  })
end

document:Mount(function() return IndicatorsView(state) end, "#indicators-wrapper")