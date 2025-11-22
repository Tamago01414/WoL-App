using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace IconTool
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var outputDir = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.Combine(Environment.CurrentDirectory, "assets");
            Directory.CreateDirectory(outputDir);

            string svgPath = args.Length > 1 ? Path.GetFullPath(args[1]) : Path.Combine(outputDir, "Downloaded.svg");
            if (!File.Exists(svgPath))
            {
                svgPath = Path.Combine(outputDir, "AppIcon.svg");
            }

            var pngPath = Path.Combine(outputDir, "AppIconPreview.png");
            var icoPath = Path.Combine(outputDir, "AppIcon.ico");
            IconConverter.Convert(svgPath, pngPath, icoPath);

            var appIconSvg = Path.Combine(outputDir, "AppIcon.svg");
            if (!string.Equals(svgPath, appIconSvg, StringComparison.OrdinalIgnoreCase) && File.Exists(svgPath))
            {
                File.Copy(svgPath, appIconSvg, overwrite: true);
            }
        }
    }

    internal static class IconConverter
    {
        private static readonly int[] IconSizes = { 256, 192, 128, 96, 64, 48, 32, 24, 16 };

        public static void Convert(string svgPath, string pngPath, string icoPath)
        {
            var images = new List<IconImage>();
            var renderer = new SimpleSvgRenderer(svgPath);
            using (var baseBitmap = renderer.Render(256, 256))
            {
                baseBitmap.Save(pngPath, ImageFormat.Png);

                foreach (var size in IconSizes)
                {
                    using (var resized = ResizeBitmap(baseBitmap, size))
                    {
                        images.Add(new IconImage(size, BitmapToPng(resized)));
                    }
                }
            }

            WriteIconFile(icoPath, images);
        }

        private static Bitmap ResizeBitmap(Bitmap source, int size)
        {
            var target = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            target.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            using (var g = Graphics.FromImage(target))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(source, new Rectangle(0, 0, size, size));
            }

            return target;
        }

        private static byte[] BitmapToPng(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private static void WriteIconFile(string path, List<IconImage> images)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write((short)0);
                bw.Write((short)1);
                bw.Write((short)images.Count);

                int offset = 6 + 16 * images.Count;
                foreach (var image in images)
                {
                    byte width = (byte)(image.Size >= 256 ? 0 : image.Size);
                    byte height = (byte)(image.Size >= 256 ? 0 : image.Size);
                    bw.Write(width);
                    bw.Write(height);
                    bw.Write((byte)0);
                    bw.Write((byte)0);
                    bw.Write((short)1);
                    bw.Write((short)32);
                    bw.Write(image.Data.Length);
                    bw.Write(offset);
                    offset += image.Data.Length;
                }

                foreach (var image in images)
                {
                    bw.Write(image.Data);
                }
            }
        }

        private struct IconImage
        {
            public IconImage(int size, byte[] data)
            {
                Size = size;
                Data = data;
            }

            public int Size;
            public byte[] Data;
        }
    }

    internal sealed class SimpleSvgRenderer
    {
        private readonly float _svgWidth;
        private readonly float _svgHeight;
        private readonly List<ISvgShape> _shapes = new List<ISvgShape>();

        public SimpleSvgRenderer(string svgPath)
        {
            var doc = XDocument.Load(svgPath);
            if (doc.Root == null || doc.Root.Name.LocalName != "svg")
            {
                throw new InvalidDataException("Invalid SVG file");
            }

            var widthAttr = doc.Root.Attribute("width");
            var heightAttr = doc.Root.Attribute("height");
            _svgWidth = ParseLength(widthAttr != null ? widthAttr.Value : null) ?? 256f;
            _svgHeight = ParseLength(heightAttr != null ? heightAttr.Value : null) ?? 256f;

            foreach (var element in doc.Root.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "rect":
                        _shapes.Add(RectShape.Parse(element));
                        break;
                    case "circle":
                        _shapes.Add(CircleShape.Parse(element));
                        break;
                    case "line":
                        _shapes.Add(LineShape.Parse(element));
                        break;
                }
            }
        }

        public Bitmap Render(int width, int height)
        {
            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmp.SetResolution(256, 256);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                float scaleX = width / _svgWidth;
                float scaleY = height / _svgHeight;

                foreach (var shape in _shapes)
                {
                    shape.Draw(g, scaleX, scaleY);
                }
            }

            return bmp;
        }

        internal static float? ParseLength(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            value = value.Trim().ToLowerInvariant().Replace("px", string.Empty);
            float result;
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }

            return null;
        }
    }

    internal interface ISvgShape
    {
        void Draw(Graphics g, float scaleX, float scaleY);
    }

    internal sealed class RectShape : ISvgShape
    {
        private readonly float _x;
        private readonly float _y;
        private readonly float _width;
        private readonly float _height;
        private readonly float _radius;
        private readonly Color? _fill;
        private readonly Color? _stroke;
        private readonly float _strokeWidth;

        private RectShape(float x, float y, float width, float height, float radius, Color? fill, Color? stroke, float strokeWidth)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _radius = radius;
            _fill = fill;
            _stroke = stroke;
            _strokeWidth = strokeWidth;
        }

        public static RectShape Parse(XElement element)
        {
            var attrX = element.Attribute("x");
            var attrY = element.Attribute("y");
            var attrWidth = element.Attribute("width");
            var attrHeight = element.Attribute("height");
            var attrRadius = element.Attribute("rx");
            var attrFill = element.Attribute("fill");
            var attrStroke = element.Attribute("stroke");
            var attrStrokeWidth = element.Attribute("stroke-width");

            float x = SimpleSvgRenderer.ParseLength(attrX != null ? attrX.Value : null) ?? 0f;
            float y = SimpleSvgRenderer.ParseLength(attrY != null ? attrY.Value : null) ?? 0f;
            float width = SimpleSvgRenderer.ParseLength(attrWidth != null ? attrWidth.Value : null) ?? 0f;
            float height = SimpleSvgRenderer.ParseLength(attrHeight != null ? attrHeight.Value : null) ?? 0f;
            float radius = SimpleSvgRenderer.ParseLength(attrRadius != null ? attrRadius.Value : null) ?? 0f;
            var fill = SvgColorParser.Parse(attrFill != null ? attrFill.Value : null);
            var stroke = SvgColorParser.Parse(attrStroke != null ? attrStroke.Value : null);
            float strokeWidth = SimpleSvgRenderer.ParseLength(attrStrokeWidth != null ? attrStrokeWidth.Value : null) ?? 0f;
            return new RectShape(x, y, width, height, radius, fill, stroke, strokeWidth);
        }

        public void Draw(Graphics g, float scaleX, float scaleY)
        {
            var rect = new RectangleF(_x * scaleX, _y * scaleY, _width * scaleX, _height * scaleY);
            float radius = _radius * Math.Min(scaleX, scaleY);
            GraphicsPath path = null;
            if (radius > 0)
            {
                path = CreateRoundedRect(rect, radius);
            }

            try
            {
                if (_fill.HasValue)
                {
                    using (var brush = new SolidBrush(_fill.Value))
                    {
                        if (path != null)
                        {
                            g.FillPath(brush, path);
                        }
                        else
                        {
                            g.FillRectangle(brush, rect);
                        }
                    }
                }

                if (_stroke.HasValue && _strokeWidth > 0)
                {
                    using (var pen = new Pen(_stroke.Value, _strokeWidth * Math.Min(scaleX, scaleY)))
                    {
                        pen.LineJoin = LineJoin.Round;
                        if (path != null)
                        {
                            g.DrawPath(pen, path);
                        }
                        else
                        {
                            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                        }
                    }
                }
            }
            finally
            {
                if (path != null)
                {
                    path.Dispose();
                }
            }
        }

        private static GraphicsPath CreateRoundedRect(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            float diameter = radius * 2f;
            var arc = new RectangleF(rect.Location, new SizeF(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    internal sealed class CircleShape : ISvgShape
    {
        private readonly float _cx;
        private readonly float _cy;
        private readonly float _radius;
        private readonly Color? _fill;
        private readonly Color? _stroke;
        private readonly float _strokeWidth;

        private CircleShape(float cx, float cy, float radius, Color? fill, Color? stroke, float strokeWidth)
        {
            _cx = cx;
            _cy = cy;
            _radius = radius;
            _fill = fill;
            _stroke = stroke;
            _strokeWidth = strokeWidth;
        }

        public static CircleShape Parse(XElement element)
        {
            var attrCx = element.Attribute("cx");
            var attrCy = element.Attribute("cy");
            var attrR = element.Attribute("r");
            var attrFill = element.Attribute("fill");
            var attrStroke = element.Attribute("stroke");
            var attrStrokeWidth = element.Attribute("stroke-width");

            float cx = SimpleSvgRenderer.ParseLength(attrCx != null ? attrCx.Value : null) ?? 0f;
            float cy = SimpleSvgRenderer.ParseLength(attrCy != null ? attrCy.Value : null) ?? 0f;
            float radius = SimpleSvgRenderer.ParseLength(attrR != null ? attrR.Value : null) ?? 0f;
            var fill = SvgColorParser.Parse(attrFill != null ? attrFill.Value : null);
            var stroke = SvgColorParser.Parse(attrStroke != null ? attrStroke.Value : null);
            float strokeWidth = SimpleSvgRenderer.ParseLength(attrStrokeWidth != null ? attrStrokeWidth.Value : null) ?? 0f;
            return new CircleShape(cx, cy, radius, fill, stroke, strokeWidth);
        }

        public void Draw(Graphics g, float scaleX, float scaleY)
        {
            float scale = Math.Min(scaleX, scaleY);
            var rect = new RectangleF((_cx - _radius) * scaleX, (_cy - _radius) * scaleY, _radius * 2 * scaleX, _radius * 2 * scaleY);

            if (_fill.HasValue)
            {
                using (var brush = new SolidBrush(_fill.Value))
                {
                    g.FillEllipse(brush, rect);
                }
            }

            if (_stroke.HasValue && _strokeWidth > 0)
            {
                using (var pen = new Pen(_stroke.Value, _strokeWidth * scale))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawEllipse(pen, rect);
                }
            }
        }
    }

    internal sealed class LineShape : ISvgShape
    {
        private readonly float _x1;
        private readonly float _y1;
        private readonly float _x2;
        private readonly float _y2;
        private readonly Color? _stroke;
        private readonly float _strokeWidth;
        private readonly LineCap _lineCap;

        private LineShape(float x1, float y1, float x2, float y2, Color? stroke, float strokeWidth, LineCap lineCap)
        {
            _x1 = x1;
            _y1 = y1;
            _x2 = x2;
            _y2 = y2;
            _stroke = stroke;
            _strokeWidth = strokeWidth;
            _lineCap = lineCap;
        }

        public static LineShape Parse(XElement element)
        {
            var attrX1 = element.Attribute("x1");
            var attrY1 = element.Attribute("y1");
            var attrX2 = element.Attribute("x2");
            var attrY2 = element.Attribute("y2");
            var attrStroke = element.Attribute("stroke");
            var attrStrokeWidth = element.Attribute("stroke-width");
            var attrLineCap = element.Attribute("stroke-linecap");

            float x1 = SimpleSvgRenderer.ParseLength(attrX1 != null ? attrX1.Value : null) ?? 0f;
            float y1 = SimpleSvgRenderer.ParseLength(attrY1 != null ? attrY1.Value : null) ?? 0f;
            float x2 = SimpleSvgRenderer.ParseLength(attrX2 != null ? attrX2.Value : null) ?? 0f;
            float y2 = SimpleSvgRenderer.ParseLength(attrY2 != null ? attrY2.Value : null) ?? 0f;
            var stroke = SvgColorParser.Parse(attrStroke != null ? attrStroke.Value : null);
            float strokeWidth = SimpleSvgRenderer.ParseLength(attrStrokeWidth != null ? attrStrokeWidth.Value : null) ?? 0f;
            var lineCap = SvgColorParser.ParseLineCap(attrLineCap != null ? attrLineCap.Value : null);
            return new LineShape(x1, y1, x2, y2, stroke, strokeWidth, lineCap);
        }

        public void Draw(Graphics g, float scaleX, float scaleY)
        {
            if (!_stroke.HasValue || _strokeWidth <= 0)
            {
                return;
            }

            float scale = Math.Min(scaleX, scaleY);

            using (var pen = new Pen(_stroke.Value, _strokeWidth * scale))
            {
                pen.LineJoin = LineJoin.Round;
                pen.StartCap = _lineCap;
                pen.EndCap = _lineCap;

                var start = new PointF(_x1 * scaleX, _y1 * scaleY);
                var end = new PointF(_x2 * scaleX, _y2 * scaleY);
                g.DrawLine(pen, start, end);
            }
        }
    }

    internal static class SvgColorParser
    {
        public static Color? Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (value.StartsWith("#", StringComparison.Ordinal))
            {
                var hex = value.Substring(1);
                int intColor;
                if (hex.Length == 6 && int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out intColor))
                {
                    int r = (intColor >> 16) & 0xFF;
                    int g = (intColor >> 8) & 0xFF;
                    int b = intColor & 0xFF;
                    return Color.FromArgb(r, g, b);
                }
            }

            return Color.FromName(value);
        }

        public static LineCap ParseLineCap(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return LineCap.Flat;
            }

            switch (value.ToLowerInvariant())
            {
                case "round":
                    return LineCap.Round;
                case "square":
                    return LineCap.Square;
                default:
                    return LineCap.Flat;
            }
        }
    }
}
