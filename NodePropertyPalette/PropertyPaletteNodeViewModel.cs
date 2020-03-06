using Dynamo.Core;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.ZeroTouch;
using System;
using System.ComponentModel;
using System.Linq;

namespace NodePropertyPalette
{
    public class PropertyPaletteNodeViewModel : NotificationObject, IDisposable
    {
        private readonly string[] BoundProperties = new string[] { "IsFrozen" };
        private readonly string[] KnownDynamoNodeNamespaces = new string[] { "DSCore", "DSOffice", "Autodesk.DesignScript", "DesignScript", "Tessellation" };

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
        /// The node type of this node
        /// </summary>
        public string NodeType
        {
            get
            {
                return NodeModel.NodeType;
            }
        }

        /// <summary>
        /// Whether the node is built-into Dynamo or not (either hosted or imported).
        /// </summary>
        public NodeBuiltInStatus IsBuiltIn
        {
            get
            {
                // We are assuming non built-in nodes are always zero-touch
                if (NodeModel is DSFunctionBase)
                {
                    // And that they don't use any of our node namespaces
                    var mangledName = (NodeModel as DSFunctionBase).Controller.MangledName;
                    return KnownDynamoNodeNamespaces.Any(ns => mangledName.StartsWith(ns)) ? NodeBuiltInStatus.Yes : NodeBuiltInStatus.No;
                }
                return NodeBuiltInStatus.Yes;
            }
        }

        public bool Selected { get; set; }

        /// <summary>
        /// Reflect if node is set as input
        /// </summary>
        public bool IsSetAsInput
        {
            get 
            {
                return NodeModel.IsSetAsInput;
            }
            set
            {
                NodeModel.IsSetAsInput = value;
            }
        }

        /// <summary>
        /// Reflect if node is set as output
        /// </summary>
        public bool IsSetAsOutput
        {
            get
            {
                return NodeModel.IsSetAsOutput;
            }
            set
            {
                NodeModel.IsSetAsOutput = value;
            }
        }

        /// <summary>
        /// Reflect node's GUID
        /// </summary>
        public string GUID
        {
            get
            {
                return NodeModel.GUID.ToString();
            }
        }

        /// <summary>
        /// Reflect if node is frozen
        /// </summary>
        public bool IsFrozen
        {
            get
            {
                return NodeModel.IsFrozen;
            }
            set
            {
                NodeModel.IsFrozen = value;
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
            SubscribeToUpdateEvents();
        }

        public void Dispose()
        {
            UnsubscribeFromUpdateEvents();
        }

        private void UnsubscribeFromUpdateEvents()
        {
            NodeModel.PropertyChanged -= Node_PropertyChanged;
        }

        private void SubscribeToUpdateEvents()
        {
            NodeModel.PropertyChanged += Node_PropertyChanged;
        }

        private void Node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (BoundProperties.Contains(e.PropertyName))
            {
                RaisePropertyChanged(e.PropertyName);
            }
        }
    }
}
