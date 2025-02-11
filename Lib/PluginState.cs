using AC.API;
using AC.Lib.Screens;
using AC.UIModels;

namespace AC.Lib {
    internal class PluginState {
        public GameScreen CurrentScreen { get; set; } = GameScreen.None;
        public TooltipModel TooltipModel { get; set; }
        public Game Game { get; set; }
    }
}