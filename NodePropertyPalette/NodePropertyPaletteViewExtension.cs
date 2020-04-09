using System;
using System.Windows.Controls;
using Dynamo.Wpf.Extensions;

namespace NodePropertyPalette
{
    /// <summary>
    /// This sample view extension demonstrates a sample IViewExtension 
    /// which allows Dynamo users to analyze the performance of graphs
    /// and diagnose bottlenecks and problem areas.
    /// </summary>
    public class NodePropertyPaletteViewExtension : IViewExtension
    {
        internal MenuItem NodePropertyPaletteMenuItem;
        private NodePropertyPaletteWindow NodePropertyPaletteView;
        internal NodePropertyPaletteWindowViewModel ViewModel;

        public void Dispose()
        {
            NodePropertyPaletteView.Dispose();
        }

        public void Startup(ViewStartupParams p)
        {
        }

        public void Loaded(ViewLoadedParams p)
        {
            ViewModel = new NodePropertyPaletteWindowViewModel(p);
            NodePropertyPaletteView = new NodePropertyPaletteWindow(p, ViewModel)
            {
                // Set the data context for the main grid in the window.
                NodesTable = { DataContext = ViewModel },
                MainGrid = { DataContext = ViewModel },
                Owner = p.DynamoWindow
            };

            NodePropertyPaletteMenuItem = new MenuItem { Header = "Show NodePropertyPalette", IsCheckable = true, IsChecked = false };
            NodePropertyPaletteMenuItem.Click += (sender, args) =>
            {
                if (NodePropertyPaletteMenuItem.IsChecked)
                {
                    p.AddToExtensionsSideBar(this, NodePropertyPaletteView);
                }
                else
                {
                    p.CloseExtensioninInSideBar(this);
                }

            };
            p.AddMenuItem(MenuBarType.View, NodePropertyPaletteMenuItem);
        }

        /// <summary>
        /// Tear down function.
        /// </summary>
        public void Shutdown()
        {
            this.Dispose();
        }

        /// <summary>
        /// ID for the NodePropertyPalette extension
        /// </summary>
        public string UniqueId
        {
            get
            {
                return Constants.ExtensionUniqueId;
            }
        }

        /// <summary>
        /// Name of this extension
        /// </summary>
        public string Name
        {
            get
            {
                return Constants.ExtensionName;
            }
        }
    }
}