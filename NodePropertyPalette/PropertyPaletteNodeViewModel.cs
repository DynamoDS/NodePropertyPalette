using Dynamo.Core;
using Dynamo.Graph.Nodes;
namespace NodePropertyPalette
{
    public class PropertyPaletteNodeViewModel : NotificationObject
    {
        #region Properties
        /// <summary>
        /// The name of this PropertyPalette Node.
        /// This value should reflect node name.
        /// </summary>
        public string Name
        {
            get
            {
                return NodeModel.Name;
            }
        }

        /// <summary>
        /// The most recent execution time of this node
        /// </summary>
        public string NodeType
        {
            get
            {
                return NodeModel.NodeType;
            }
        }

        internal NodeModel NodeModel { get; set; }

        #endregion

        /// <summary>
        /// Create a Profiled Node View Model from a NodeModel
        /// </summary>
        /// <param name="node"></param>
        public PropertyPaletteNodeViewModel(NodeModel node)
        {
            NodeModel = node;
        }
    }
}
