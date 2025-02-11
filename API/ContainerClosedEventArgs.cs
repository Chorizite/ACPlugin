using AC.API.WorldObjects;

namespace AC.API {
    /// <summary>
    /// ContainerClosedEventArgs
    /// </summary>
    public class ContainerClosedEventArgs : System.EventArgs {
        /// <summary>
        /// The container weenie that was closed
        /// </summary>
        public Container Container { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container"></param>
        public ContainerClosedEventArgs(Container container) {
            Container = container;
        }
    }
}