using BtgTesteGraph.ViewModel;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace BtgTesteGraph.View;

public partial class GraphPage : ContentPage
{
    GraphViewModel viewModel;
	public GraphPage()
	{
		InitializeComponent();
        viewModel = this.BindingContext as GraphViewModel;
	}

    private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
        if (!viewModel.Graphs.Any())
            return;

        var canvas = e.Surface.Canvas;
        var info = e.Info;

        canvas.Clear(SKColors.White);

        // Padding (margem) do gráfico
        float padding = 40;

        // Área do gráfico
        float contentWidth = info.Width - 2 * padding;
        float contentHeight = info.Height - 2 * padding;

        float max = 0;
        float min = 0;
        float range = 0;

        //Limitar max e min entre todos os registros
        foreach (var graph in viewModel.Graphs)
        {
            float[] data = graph.Curve.Select(d => (float)d).ToArray();

            if (min == 0 || min > data.Min()) min = data.Min();

            if (max == 0 || max < data.Max()) max = data.Max();
        }

        //Desenhar os graficos
        foreach (var graph in viewModel.Graphs)
        {
            float[] data = graph.Curve.Select(d => (float)d).ToArray();

            range = max - min;

            // Escala por pixel
            float xStep = contentWidth / (data.Length - 1);
            float yScale = contentHeight / range;

            // Desenha o gráfico
            using var paint = new SKPaint
            {
                Color = graph.CurveColor,
                StrokeWidth = 1,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            using var path = new SKPath();

            for (int i = 0; i < data.Length; i++)
            {
                float x = padding + i * xStep;
                float y = padding + (max - data[i]) * yScale;

                if (i == 0)
                    path.MoveTo(x, y);
                else
                    path.LineTo(x, y);
            }

            canvas.DrawPath(path, paint);
        }
    }

    private void UpdateGraph()
    {
        var sk = new SKCanvasView();
        sk.PaintSurface += SKCanvasView_PaintSurface;
        graphView.Content = sk;
    }

    private void SliderMedia_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        viewModel.Media = Math.Round(e.NewValue, 0);
    }

    private void SliderVolatility_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        viewModel.Volatility = Math.Round(e.NewValue, 0);
    }

    private void NewSumulationClicked(object sender, EventArgs e)
    {
        viewModel.SimulationGraphic();

        if (!viewModel.SimulationInvalid)
            UpdateGraph();
    }
}
