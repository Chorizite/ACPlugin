namespace AC.API {
    public class Actions {
        /// <summary>
        /// Selects the specified object
        /// </summary>
        /// <param name="obj"></param>
        public void SelectObject(WorldObject obj) {
            SelectObjectId(obj?.Id ?? 0);
        }

        /// <summary>
        /// Selects the specified object by id
        /// </summary>
        /// <param name="id"></param>
        public void SelectObjectId(uint id) {
            if (ACPlugin.Instance.Game.State != ClientState.InGame) {
                return;
            }
            ACPlugin.Instance.ClientBackend.SelectedObjectId = id;
        }
    }
}