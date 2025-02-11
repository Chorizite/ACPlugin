using AC.API;
using AC.Lib.Screens;

namespace AC.Lib {
    internal class PluginState {
        public GameScreen CurrentScreen { get; set; } = GameScreen.None;
        public Game Game { get; set; }
    }
}