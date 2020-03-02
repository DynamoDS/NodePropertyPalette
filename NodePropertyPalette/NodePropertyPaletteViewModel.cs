using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Data;
using Dynamo.Core;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Wpf.Extensions;

namespace NodePropertyPalette
{
    /// <summary>
    /// ViewModel for NodePropertyPalette. 
    /// Handles profiling setup, workspace events, execution events, etc.
    /// </summary>
    public class NodePropertyPaletteWindowViewModel : NotificationObject, IDisposable
    {
        #region Internal Properties
        private ViewLoadedParams viewLoadedParams;
        private HomeWorkspaceModel currentWorkspace;
        private SynchronizationContext uiContext;

        private HomeWorkspaceModel CurrentWorkspace
        {
            get
            {
                return currentWorkspace;
            }
            set
            {
                // Unsubscribe from old workspace
                if (currentWorkspace != null)
                {
                    UnsubscribeWorkspaceEvents(currentWorkspace);
                }

                // Subscribe to new workspace
                if (value != null)
                {
                    // Set new workspace
                    currentWorkspace = value;
                    SubscribeWorkspaceEvents(currentWorkspace);
                }
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Collection of profiling data for nodes in the current workspace
        /// </summary>
        public ObservableCollection<PropertyPaletteNodeViewModel> PropertyPaletteNodes { get; set; } = new ObservableCollection<PropertyPaletteNodeViewModel>();

        /// <summary>
        /// Collection of profiling data for nodes in the current workspace.
        /// Profiling data in this collection is grouped by the profiled nodes' states.
        /// </summary>
        public CollectionViewSource PropertyPaletteNodesCollection { get; set; }
        #endregion

        #region Constructor

        public NodePropertyPaletteWindowViewModel(ViewLoadedParams p)
        {
            viewLoadedParams = p;
            // Saving UI context so later when we touch the collection, it is still performed in the same context
            uiContext = SynchronizationContext.Current;
            p.CurrentWorkspaceChanged += OnCurrentWorkspaceChanged;
            p.CurrentWorkspaceCleared += OnCurrentWorkspaceCleared;

            if (p.CurrentWorkspaceModel is HomeWorkspaceModel)
            {
                CurrentWorkspace = p.CurrentWorkspaceModel as HomeWorkspaceModel;
            }
            PropertyPaletteNodesCollection = new CollectionViewSource();
            PropertyPaletteNodesCollection.Source = PropertyPaletteNodes;
            PropertyPaletteNodesCollection.SortDescriptions.Clear();
            PropertyPaletteNodesCollection.SortDescriptions.Add(new SortDescription(nameof(PropertyPaletteNodeViewModel.NodeType), ListSortDirection.Descending));
            if (PropertyPaletteNodesCollection.View != null)
                PropertyPaletteNodesCollection.View.Refresh();
        }

        #endregion

        #region ExecutionEvents

        private void CurrentWorkspaceModel_EvaluationStarted(object sender, EventArgs e)
        {
        }

        private void CurrentWorkspaceModel_EvaluationCompleted(object sender, Dynamo.Models.EvaluationCompletedEventArgs e)
        {
            // TODO: We may need to update node values after graph execution,
            // Depending on if we display node values at all.
        }

        internal void OnNodeExecutionBegin(NodeModel nm)
        {
            RaisePropertyChanged(nameof(PropertyPaletteNodesCollection));
        }

        internal void OnNodeExecutionEnd(NodeModel nm)
        {
            RaisePropertyChanged(nameof(PropertyPaletteNodesCollection));
        }

        #endregion

        #region Workspace Events

        private void CurrentWorkspaceModel_NodeAdded(NodeModel node)
        {
            // When a new node added on canvas, update PropertyPalette
            var profiledNode = new PropertyPaletteNodeViewModel(node);
            PropertyPaletteNodes.Add(profiledNode);
            RaisePropertyChanged(nameof(PropertyPaletteNodesCollection));
        }

        private void CurrentWorkspaceModel_NodeRemoved(NodeModel node)
        {
            // TODO: remove the deleted node as well in PropertyPalette
            RaisePropertyChanged(nameof(PropertyPaletteNodesCollection));
        }

        private void OnCurrentWorkspaceChanged(IWorkspaceModel workspace)
        {
            CurrentWorkspace = workspace as HomeWorkspaceModel;
        }

        private void OnCurrentWorkspaceCleared(IWorkspaceModel workspace)
        {
            CurrentWorkspace = viewLoadedParams.CurrentWorkspaceModel as HomeWorkspaceModel;
        }

        #endregion

        #region Dispose or setup

        /// <summary>
        /// When switching workspaces or closing NodePropertyPalette extension,
        /// unsubscribe workspace events for profiling
        /// </summary>
        /// <param name="workspace">target workspace</param>
        private void UnsubscribeWorkspaceEvents(HomeWorkspaceModel workspace)
        {
            workspace.NodeAdded -= CurrentWorkspaceModel_NodeAdded;
            workspace.NodeRemoved -= CurrentWorkspaceModel_NodeRemoved;
            workspace.EvaluationStarted -= CurrentWorkspaceModel_EvaluationStarted;
            workspace.EvaluationCompleted -= CurrentWorkspaceModel_EvaluationCompleted;

            foreach (var node in workspace.Nodes)
            {
                node.NodeExecutionBegin -= OnNodeExecutionBegin;
                node.NodeExecutionEnd -= OnNodeExecutionEnd;
            }
        }

        /// <summary>
        /// When switching workspaces or closing NodePropertyPalette extension,
        /// subscribe workspace events for profiling
        /// </summary>
        /// <param name="workspace">target workspace</param>
        private void SubscribeWorkspaceEvents(HomeWorkspaceModel workspace)
        {
            workspace.NodeAdded += CurrentWorkspaceModel_NodeAdded;
            workspace.NodeRemoved += CurrentWorkspaceModel_NodeRemoved;
            workspace.EvaluationStarted += CurrentWorkspaceModel_EvaluationStarted;
            workspace.EvaluationCompleted += CurrentWorkspaceModel_EvaluationCompleted;

            foreach (var node in workspace.Nodes)
            {
                var profiledNode = new PropertyPaletteNodeViewModel(node);
                PropertyPaletteNodes.Add(profiledNode);
                node.NodeExecutionBegin += OnNodeExecutionBegin;
                node.NodeExecutionEnd += OnNodeExecutionEnd;
            }
            RaisePropertyChanged(nameof(PropertyPaletteNodesCollection));
        }

        /// <summary>
        /// ViewModel dispose function
        /// </summary>
        public void Dispose()
        {
            UnsubscribeWorkspaceEvents(CurrentWorkspace);
            viewLoadedParams.CurrentWorkspaceChanged -= OnCurrentWorkspaceChanged;
            viewLoadedParams.CurrentWorkspaceCleared -= OnCurrentWorkspaceCleared;
        }

        #endregion
    }
}
