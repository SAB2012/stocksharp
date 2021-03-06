namespace StockSharp.Studio.Ribbon
{
	using System.Collections.Generic;
	using System.Windows;
	using System.Windows.Input;

	using Ecng.Collections;
	using Ecng.Common;
	using Ecng.Xaml;

	using Ookii.Dialogs.Wpf;

	using StockSharp.Algo.Strategies.Reporting;
	using StockSharp.Studio.Core;
	using StockSharp.Xaml;
	using StockSharp.Localization;

	public partial class StrategyAdditionalGroup
	{
		public readonly static RoutedCommand RiskManagerCommand = new RoutedCommand();
		public readonly static RoutedCommand ComissionManagerCommand = new RoutedCommand();
		public readonly static RoutedCommand ReportCommand = new RoutedCommand();
		public readonly static RoutedCommand OpenReportCommand = new RoutedCommand();

		public static readonly DependencyProperty SelectedStrategyProperty = DependencyProperty.Register("SelectedStrategy", typeof(StrategyContainer), typeof(StrategyAdditionalGroup));

		public StrategyContainer SelectedStrategy
		{
			get { return (StrategyContainer)GetValue(SelectedStrategyProperty); }
			set { SetValue(SelectedStrategyProperty, value); }
		}

		public static readonly DependencyProperty SelectedStrategiesProperty = DependencyProperty.Register("SelectedStrategies", typeof(IEnumerable<StrategyContainer>), typeof(StrategyAdditionalGroup));

		public IEnumerable<StrategyContainer> SelectedStrategies
		{
			get { return (IEnumerable<StrategyContainer>)GetValue(SelectedStrategiesProperty); }
			set { SetValue(SelectedStrategiesProperty, value); }
		}

		public static readonly DependencyProperty IsTerminalTypeProperty = DependencyProperty.Register("IsTerminal", typeof(bool), typeof(StrategyAdditionalGroup));

		public bool IsTerminal
		{
			get { return (bool)GetValue(IsTerminalTypeProperty); }
			set { SetValue(IsTerminalTypeProperty, value); }
		}

		public static readonly DependencyProperty IsStrategyTypeProperty = DependencyProperty.Register("IsStrategy", typeof(bool), typeof(StrategyAdditionalGroup));

		public bool IsStrategy
		{
			get { return (bool)GetValue(IsStrategyTypeProperty); }
			set { SetValue(IsStrategyTypeProperty, value); }
		}

		public StrategyAdditionalGroup()
		{
			InitializeComponent();
		}

		private void ExecutedRiskManagerCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var wnd = new RiskWindow();
			wnd.Rules.AddRange(SelectedStrategy.RiskManager.Rules);
			wnd.ShowModal(this);
		}

		private void CanExecuteRiskManagerCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = CheckStrategyType();
		}

		private void ExecutedComissionManagerCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var wnd = new CommissionWindow();
			wnd.ShowModal(this);
		}

		private void CanExecuteComissionManagerCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = CheckStrategyType();
		}

		private void ExecutedOpenReportCommand(object sender, ExecutedRoutedEventArgs e)
		{
			var type = (string)e.Parameter;

			var dlg = new VistaSaveFileDialog
			{
				Filter = LocalizedStrings.Str3591Params.Put(type),
				DefaultExt = type,
				RestoreDirectory = true,
				//AddExtension = false,
			};

			if (dlg.ShowDialog(Application.Current.GetActiveOrMainWindow()) != true)
				return;

			var path = dlg.FileName;
			var strategies = SelectedStrategies ?? new[] { SelectedStrategy };

			switch (type)
			{
				case "csv":
					new CsvStrategyReport(strategies, path).Generate();
					break;

				case "xlsx":
					new ExcelStrategyReport(strategies, path).Generate();
					break;

				case "xml":
					new XmlStrategyReport(strategies, path).Generate();
					break;
			}

			path.OpenFile();
		}

		private void CanExecuteReportCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (CheckStrategyType() && SelectedStrategies == null) || (SelectedStrategies != null && !SelectedStrategies.IsEmpty());
		}

		private bool CheckStrategyType()
		{
			return SelectedStrategy != null && ((IsTerminal && SelectedStrategy.IsTerminal()) || (IsStrategy && SelectedStrategy.IsStrategy()));
		}
	}
}