using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using Dynamo.Core;
using Dynamo.Engine.Profiling;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.ViewModels;
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
        public ObservableCollection<PropertyPaletteNodeViewModel> ProfiledNodes { get; set; } = new ObservableCollection<PropertyPaletteNodeViewModel>();

        /// <summary>
        /// Collection of profiling data for nodes in the current workspace.
        /// Profiling data in this collection is grouped by the profiled nodes' states.
        /// </summary>
        public CollectionViewSource ProfiledNodesCollection { get; set; }
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
        }

        #endregion

        #region ProfilingMethods

        #endregion

        #region ExecutionEvents

        private void CurrentWorkspaceModel_EvaluationStarted(object sender, EventArgs e)
        {

        }

        private void CurrentWorkspaceModel_EvaluationCompleted(object sender, Dynamo.Models.EvaluationCompletedEventArgs e)
        {
            RaisePropertyChanged(nameof(ProfiledNodesCollection));
            RaisePropertyChanged(nameof(ProfiledNodes));

            ProfiledNodesCollection.Dispatcher.Invoke(() =>
            {
                ProfiledNodesCollection.SortDescriptions.Clear();
                // Sort nodes into execution group
                ProfiledNodesCollection.SortDescriptions.Add(new SortDescription(nameof(PropertyPaletteNodeViewModel.Name), ListSortDirection.Ascending));

                // Sort nodes into execution order and make sure Total execution time is always bottom
                ProfiledNodesCollection.SortDescriptions.Add(new SortDescription(nameof(PropertyPaletteNodeViewModel.NodeType), ListSortDirection.Descending));
                if (ProfiledNodesCollection.View != null)
                    ProfiledNodesCollection.View.Refresh();
            });
        }


        internal void OnNodeExecutionBegin(NodeModel nm)
        {
            RaisePropertyChanged(nameof(ProfiledNodesCollection));
        }

        internal void OnNodeExecutionEnd(NodeModel nm)
        {
            RaisePropertyChanged(nameof(ProfiledNodesCollection));
        }

        #endregion

        #region Workspace Events

        private void CurrentWorkspaceModel_NodeAdded(NodeModel node)
        {
            var profiledNode = new PropertyPaletteNodeViewModel(node);
            node.NodeExecutionBegin += OnNodeExecutionBegin;
            node.NodeExecutionEnd += OnNodeExecutionEnd;
            ProfiledNodes.Add(profiledNode);
            RaisePropertyChanged(nameof(ProfiledNodesCollection));
        }

        private void CurrentWorkspaceModel_NodeRemoved(NodeModel node)
        {
            node.NodeExecutionBegin -= OnNodeExecutionBegin;
            node.NodeExecutionEnd -= OnNodeExecutionEnd;
            RaisePropertyChanged(nameof(ProfiledNodesCollection));
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
                node.NodeExecutionBegin += OnNodeExecutionBegin;
                node.NodeExecutionEnd += OnNodeExecutionEnd;
            }
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
