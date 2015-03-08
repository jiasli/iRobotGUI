using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;

namespace iRobotGUI.Controls
{
	/// <summary>
	/// Interaction logic for ProgramList.xaml
	/// </summary>
	public partial class ProgramList : UserControl
	{
		#region data

		private ListViewDragDropManager<Instruction> dragMgr;
		private HLProgram program;

		#endregion

		#region constructor

		public ProgramList()
		{
			InitializeComponent();
			this.Loaded += ListView1_Loaded;

			program = new HLProgram();
		}

		#endregion

		#region HLProgram

		public HLProgram Program
		{
			get
			{
				return program;
			}
			set
			{
				program = value;
				UpdateContent();
			}
		}

		#endregion

		#region ListView1_Loaded

		void ListView1_Loaded(object sender, RoutedEventArgs e)
		{
			this.dragMgr = new ListViewDragDropManager<Instruction>(ListviewProgram);
			ListviewProgram.PreviewMouseLeftButtonDown += NewPreviewMouseLeftButtonDown;
            ListviewProgram.PreviewMouseRightButtonDown += listView_PreviewMouseRightButtonDown;
			ListviewProgram.Drop -= dragMgr.listView_Drop;
			ListviewProgram.Drop += NewDrop;
		}

		#endregion

		#region event handler

        /// <summary>
        /// open the dialog when an item is double clicked
        /// </summary>
		void NewPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{

			if (e.ClickCount == 2)
			{
				Instruction selectedIns = this.ListviewProgram.SelectedItem as Instruction;
				DialogInvoker.ShowDialog(selectedIns, Window.GetWindow(this));
				UpdateContent();
			}

		}

        /// <summary>
        ///  delete item when right button is clicked
        /// </summary>
        void listView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Instruction selectedIns = this.ListviewProgram.SelectedItem as Instruction;
            if (selectedIns == null)
                return;

            this.ListviewProgram.Items.Remove(selectedIns);
            program.Remove(selectedIns);
        }

        /// <summary>
        /// handler for drop event
        /// </summary>
		void NewDrop(object sender, DragEventArgs e)
		{
			// drag inside program list
			if (this.dragMgr.IsDragInProgress)
			{
				Instruction data = e.Data.GetData(typeof(Instruction)) as Instruction;
				if (data == null)
					return;

				int oldIndex = this.ListviewProgram.Items.IndexOf(data);
				int newIndex = this.dragMgr.IndexUnderDragCursor;

				if (newIndex < 0)
				{
					if (this.ListviewProgram.Items.Count == 0)
						newIndex = 0;
					else if (oldIndex < 0)
						newIndex = this.ListviewProgram.Items.Count;
					else
						return;
				}

				if (oldIndex == newIndex)
					return;

				this.ListviewProgram.Items.Remove(data);
				this.ListviewProgram.Items.Insert(newIndex, data);

				e.Effects = DragDropEffects.Move;

				program.Rearrange(data, newIndex);
			}
			// drag from instruction panel to program list
			else
			{
				string op = (string)e.Data.GetData(DataFormats.StringFormat);
				Instruction newIns = Instruction.CreatFromOpcode(op);

				if (newIns != null)
				{
					program.Add(newIns);
				}

				UpdateContent();
				ListviewProgram.SelectedItem = newIns;
			}
		}

		#endregion

		#region updateContent

		public void UpdateContent()
		{
			ListviewProgram.Items.Clear();

			foreach (Instruction i in program.GetInstructionList())
			{
				ListviewProgram.Items.Add(i);
			}

		}

		#endregion

	}
}
