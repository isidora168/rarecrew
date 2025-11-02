using Employees.Services;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace Employees.Controllers;

public class ChartController : Controller
{
    private readonly IEntryDataService _svc;
    public ChartController(IEntryDataService svc) => _svc = svc;

    [HttpGet("/chart/pie")]
    public async Task<IActionResult> Pie()
    {
        var totals = (await _svc.GetTotalsAsync()).ToList();
        var sum = totals.Sum(t => t.TotalHours);
        if (sum <= 0) sum = 1;

        int W = 800;
        float pad = 24f, pieHeight = 440f, box = 18f, gapBoxText = 8f, gapItems = 24f, lineH = 26f;

        using var fLegend = new SKFont(SKTypeface.Default, 18);
        using var fPct = new SKFont(SKTypeface.Default, 18);
        using var pLegend = new SKPaint { IsAntialias = true, Color = SKColors.Black };
        using var pPctFill = new SKPaint { IsAntialias = true, Color = SKColors.White };

        var colors = MakePalette(Math.Max(totals.Count, 12));

        float maxW = W - 2 * pad, used = 0f; int lines = 1;
        foreach (var t in totals)
        {
            string label = t.Employee;
            float itemW = box + gapBoxText + fLegend.MeasureText(label) + gapItems;
            if (used + itemW > maxW) { lines++; used = 0f; }
            used += itemW;
        }
        int H = (int)(pad + pieHeight + lines * lineH + pad * 2);

        using var bmp = new SKBitmap(W, H);
        using var canvas = new SKCanvas(bmp);
        canvas.Clear(SKColors.White);

        var pieRect = new SKRect(pad, pad, W - pad, pad + pieHeight);
        var center = new SKPoint((pieRect.Left + pieRect.Right) / 2f, (pieRect.Top + pieRect.Bottom) / 2f);
        float radius = Math.Min(pieRect.Width, pieRect.Height) * 0.45f;

        static SKPoint Polar(SKPoint c, float r, float deg)
        {
            float rad = deg * (float)Math.PI / 180f;
            return new SKPoint(c.X + r * (float)Math.Cos(rad), c.Y + r * (float)Math.Sin(rad));
        }

        float start = -90f;
        for (int i = 0; i < totals.Count; i++)
        {
            var t = totals[i];
            float frac = (float)(t.TotalHours / sum);
            float sweep = frac * 360f;

            using var fill = new SKPaint { IsAntialias = true, Color = colors[i % colors.Length], Style = SKPaintStyle.Fill };
            using var path = new SKPath();
            path.MoveTo(center);
            path.ArcTo(new SKRect(center.X - radius, center.Y - radius, center.X + radius, center.Y + radius), start, sweep, false);
            path.Close();
            canvas.DrawPath(path, fill);

            if (frac >= 0.05f)
            {
                var p = Polar(center, radius * 0.65f, start + sweep / 2f);
                string pct = (t.TotalHours / sum).ToString("P0", System.Globalization.CultureInfo.InvariantCulture);
                using var blob = SKTextBlob.Create(pct, fPct);
                float f = p.Y + fPct.Size / 3f; 
                canvas.DrawText(blob, p.X, f, pPctFill);
            }
            start += sweep;
        }

        float x = pad, y = pieRect.Bottom + pad;
        for (int i = 0; i < totals.Count; i++)
        {
            var t = totals[i];
            string label = t.Employee;
            float textW = fLegend.MeasureText(label);
            float itemW = box + gapBoxText + textW + gapItems;

            if (x + itemW > W - pad) { x = pad; y += lineH; }

            using var boxPaint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill, Color = colors[i % colors.Length] };
            canvas.DrawRect(new SKRect(x, y, x + box, y + box), boxPaint);

            using var blob = SKTextBlob.Create(label, fLegend);
            canvas.DrawText(blob, x + box + gapBoxText, y + box - 3f, pLegend);

            x += itemW;
        }

        using var img = SKImage.FromBitmap(bmp);
        using var data = img.Encode(SKEncodedImageFormat.Png, 100);
        return File(data.ToArray(), "image/png");
    }

    static SKColor[] MakePalette(int n)
    {
        if (n <= 0) return Array.Empty<SKColor>();
        var list = new List<SKColor>(n);
        for (int i = 0; i < n; i++)
        {
            float hue = (360f * i) / n;   
            float sat = 70f;              
            float lig = 55f;             
            list.Add(SKColor.FromHsl(hue, sat, lig));
        }
        return list.ToArray();
    }
}
