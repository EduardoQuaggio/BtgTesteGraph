using BtgTesteGraph.Base.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace BtgTesteGraph.ViewModel
{
    public partial class GraphViewModel : ObservableObject
    {
        #region Properties
        [ObservableProperty]
        string _initialPrice;

        [ObservableProperty]
        double? _volatility = 0;

        [ObservableProperty]
        double? _media = 0;

        [ObservableProperty]
        string _time;

        [ObservableProperty]
        string _numberSimulation;

        [ObservableProperty]
        ObservableCollection<Graph> _graphs = new();

        [ObservableProperty]
        string _errors;

        [ObservableProperty]
        bool _simulationInvalid;
        #endregion

        #region Command
        [RelayCommand]
        public async Task SimulationGraphic()
        {
            try
            {
                if (ValidateSimulation()) { 
                    Graphs = new();

                    for (int i = 0; i < int.Parse(NumberSimulation); i++)
                    {
                        var graph = new Graph
                        {
                            InitialPrice = double.Parse(InitialPrice),
                            Media = Media.Value,
                            Volatility = Volatility.Value,
                            Time = int.Parse(Time),
                            Curve = GenerateBrownianMotion(sigma: Volatility.Value / 100,
                                                        initialPrice: double.Parse(InitialPrice),
                                                        numDays: int.Parse(Time),
                                                        mean: Media.Value / 100),
                            CurveColor = Color(i)
                        };

                        Graphs.Add(graph);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public GraphViewModel() {}

        public double[] GenerateBrownianMotion(double sigma, double mean, double initialPrice, int numDays)
        {
            Random rand = new();
            double[] prices = new double[numDays];
            prices[0] = initialPrice;

            for (int i = 1; i < numDays; i++)
            {
                double u1 = 1.0 - rand.NextDouble();
                double u2 = 1.0 - rand.NextDouble();
                double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

                double retonoDiario = mean + sigma * z;

                prices[i] = prices[i - 1] * Math.Exp(retonoDiario);
            }

            return prices;
        }

        private SKColor Color(int number)
        {
            return number switch
            {
                1 => SKColors.Green,
                2 => SKColors.Red,
                3 => SKColors.Blue,
                4 => SKColors.Azure,
                5 => SKColors.Salmon,
                6 => SKColors.Brown,
                7 => SKColors.Aqua,
                8 => SKColors.DarkKhaki,
                _ => SKColors.Violet
            };
        }

        public bool ValidateSimulation()
        {
            try
            {
                Errors = null;
                SimulationInvalid = false;

                if (InitialPrice == string.Empty || InitialPrice == null)
                    Errors += "- Initial price is empty";
                else if (InitialPrice.Length == 1 && !Regex.IsMatch(Time, @"^[1-9]+$"))
                    Errors += "- Initial price is equal to zero";
                else if (!Regex.IsMatch(InitialPrice, @"^[0-9]+$"))
                    Errors += "- Initial price contains a letter";

                if (Time == string.Empty || Time == null)
                    Errors += "\n- Time is empty";
                else if (Time.Length == 1 && !Regex.IsMatch(Time, @"^[1-9]+$"))
                    Errors += "\n- Time is equal to zero";
                else if (!Regex.IsMatch(Time, @"^[0-9]+$"))
                    Errors += "\n- Time contains a letter";

                if (NumberSimulation == string.Empty || NumberSimulation == null)
                    Errors += "\n- The number of simulations is empty";
                else if (NumberSimulation.Length == 1 && !Regex.IsMatch(NumberSimulation, @"^[1-9]+$"))
                    Errors += "\n- The number of simulations is zero";
                else if (!Regex.IsMatch(NumberSimulation, @"^[0-9]+$"))
                    Errors += "\n- The number of simulations contains letters";

                if (Volatility == 0)
                    Errors += "\n- Average volatility equal to 0 ";

                if (Media == 0)
                    Errors += "\n- Average return equal to 0 ";

                if (Errors != null)
                {
                    SimulationInvalid = true;
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
