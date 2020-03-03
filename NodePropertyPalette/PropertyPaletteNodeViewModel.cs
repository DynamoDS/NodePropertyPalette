using Dynamo.Core;
using Dynamo.Graph.Nodes;
using System;
using System.ComponentModel;
using System.Linq;

namespace NodePropertyPalette
{
    public class PropertyPaletteNodeViewModel : NotificationObject, IDisposable
    {
        private readonly string[] BoundProperties = new string[] { "IsFrozen" };

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
