using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Graph
{
    public enum VisualizationMode
    {
        Lines,
        StackedLines
    }

    public partial class Graph : UserControl, IDictionary<string, IGraphValueList>
    {
        Dictionary<string, IGraphValueList> graphs;
        Image buffer;

        VisualizationMode visualizationMode = VisualizationMode.Lines;
        Color backgroundColor = Color.White;
        Color labelColor = Color.FromArgb(127, 0, 0, 0);
        bool initialized = false;

        public Graph()
        {

            InitializeComponent();
            try
            {
                pictureBoxGraph.SizeChanged += new EventHandler(pictureBoxGraph_SizeChanged);
                Bitmap image = new Bitmap(pictureBoxGraph.Width, pictureBoxGraph.Height);
                pictureBoxGraph.Image = image;
                buffer = new Bitmap(pictureBoxGraph.Image.Width, pictureBoxGraph.Image.Height);
            }
            catch (Exception)
            {
                buffer = new Bitmap(1, 1);
            }
            graphs = new Dictionary<string, IGraphValueList>();
        }

        void pictureBoxGraph_SizeChanged(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBoxGraph.Width, pictureBoxGraph.Height);
            buffer = new Bitmap(pictureBoxGraph.Width, pictureBoxGraph.Height);
            pictureBoxGraph.Image = image;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = Graphics.FromImage(buffer);
            PaintGraph(graphics);
            base.OnPaint(e);
        }

        public override void Refresh()
        {
            Graphics graphics = Graphics.FromImage(buffer);
            PaintGraph(graphics);
            base.Refresh();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            initialized = true;
        }

        private void PaintGraph(Graphics graphics)
        {
            int width = (int)pictureBoxGraph.Width;
            int height = (int)pictureBoxGraph.Height;
            Point[] pArray = new Point[4];


            PaintBackground(graphics);

            foreach (string Key in graphs.Keys)
            {
                IGraphValueList graphValue = graphs[Key];
                if (graphValue.Count == 0)
                {
                    continue;
                }
                graphValue.Sort();
                Pen pen = new Pen(new SolidBrush(graphValue.Color), graphValue.LineThickness);
                Color planeColor = Color.FromArgb((int)(graphValue.PlaneAlpha * 255), graphValue.Color.R, graphValue.Color.G, graphValue.Color.B);
                Pen PlanePen = new Pen(new SolidBrush(planeColor), graphValue.LineThickness);
                decimal maxValue = GetMaxValue(graphValue);
                decimal minValue = GetMinValue(graphValue);
                decimal maxPosition = GetMaxPosition(graphValue);
                decimal minPosition = GetMinPosition(graphValue);
                IGraphValue startPoint, endPoint;
                startPoint = graphValue[0];
                decimal displayValue = 0;
                decimal displayPosition = 0;

                displayValue = GetRelativePosition(startPoint.DisplayValue, minValue, maxValue);
                displayPosition = GetRelativePosition(startPoint.DisplayPosition, minPosition, maxPosition);
                PointF p1, p2, p1Bottom, p2Bottom;

                p1 = new PointF((float)(displayPosition * width), height - (float)(displayValue * height));
                for (int i = 1; i < graphValue.Count; i++)
                {


                    int numPositions = 0;
                    displayValue = 0;
                    displayPosition = 0;
                    do
                    {
                        endPoint = graphValue[i];
                        displayValue += GetRelativePosition(endPoint.DisplayValue, minValue, maxValue);
                        displayPosition = GetRelativePosition(endPoint.DisplayPosition, minPosition, maxPosition);
                        numPositions++;
                        p2 = new PointF((float)((displayPosition) * width), height - (float)((displayValue / numPositions) * height));
                        if (p2.X - p1.X < 1)
                        {
                            i++;
                        }

                    } while (p2.X - p1.X < 1 && i < graphValue.Count);



                    switch (visualizationMode)
                    {
                        case VisualizationMode.Lines:
                            if (graphValue.DrawGraph)
                            {
                                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                                graphics.DrawLine(pen, p1, p2);
                            }
                            if (graphValue.DrawPlane)
                            {
                                p1Bottom = new PointF(p1.X, height);
                                p2Bottom = new PointF(p2.X, height);
                                pArray[0] = new Point((int)p1.X, (int)p1.Y);
                                pArray[1] = new Point((int)p2.X, (int)p2.Y);
                                pArray[2] = new Point((int)p2Bottom.X, (int)p2Bottom.Y);
                                pArray[3] = new Point((int)p1Bottom.X, (int)p2Bottom.Y);
                                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed; // No Gaps between Polygons
                                graphics.FillPolygon(PlanePen.Brush, pArray);
                            }
                            break;
                        case VisualizationMode.StackedLines:
                            break;
                        default:
                            break;
                    }


                    p1 = p2;

                }
            }
            FlipBuffer();
        }

        private decimal GetMaxValue(IGraphValueList values)
        {
            if (values.Count == 0)
            {
                return 0;
            }
            decimal maxValue = values[0].DisplayValue;
            for (int i = 1; i < values.Count; i++)
            {
                if (maxValue < values[i].DisplayValue)
                {
                    maxValue = values[i].DisplayValue;
                }
            }
            return maxValue;
        }

        private decimal GetMinValue(IGraphValueList values)
        {
            if (values.Count == 0)
            {
                return 0;
            }
            decimal minValue = values[0].DisplayValue;
            for (int i = 1; i < values.Count; i++)
            {
                if (minValue > values[i].DisplayValue)
                {
                    minValue = values[i].DisplayValue;
                }
            }
            return minValue;
        }

        private decimal GetMaxPosition(IGraphValueList values)
        {
            if (values.Count == 0)
            {
                return 0;
            }
            decimal maxPos = values[0].DisplayPosition;
            for (int i = 1; i < values.Count; i++)
            {
                if (maxPos < values[i].DisplayPosition)
                {
                    maxPos = values[i].DisplayPosition;
                }
            }
            return maxPos;
        }

        private decimal GetMinPosition(IGraphValueList values)
        {
            if (values.Count == 0)
            {
                return 0;
            }
            decimal minPos = values[0].DisplayPosition;
            for (int i = 1; i < values.Count; i++)
            {
                if (minPos > values[i].DisplayPosition)
                {
                    minPos = values[i].DisplayPosition;
                }
            }
            return minPos;
        }

        private Decimal GetRelativePosition(decimal value, decimal min, decimal max)
        {
            return max - min == 0 ? 0 : (value - min) / (max - min);
        }

        private void PaintBackground(Graphics graphics)
        {
            int width = (int)pictureBoxGraph.Width;
            int height = (int)pictureBoxGraph.Height;
            Graphics bufferGraphics = Graphics.FromImage(buffer);
            Brush brush = new SolidBrush(backgroundColor);
            Brush labelBrush = new SolidBrush(labelColor);
            Pen labelPen = new Pen(labelBrush, 1);
            bufferGraphics.FillRectangle(brush, new Rectangle(new Point(), new Size(buffer.Width, buffer.Height)));
            if (graphs.Count >= 1)
            {
                IGraphValueList list = graphs[graphs.Keys.First()];
                if (list.Count >= 2)
                {
                    decimal maxValue = GetMaxValue(list);
                    decimal minValue = GetMinValue(list);
                    decimal absMaxValue = maxValue - minValue;
                    int dimension = 1;
                    decimal step;

                    bool dimensionFound = false;

                    for (dimension = 1; !dimensionFound && absMaxValue > 10; dimension *= 10)
                    {
                        absMaxValue /= 10;
                        if (absMaxValue <= 10)
                        {
                            dimensionFound = true;
                        }
                    }
                    step = Math.Round((absMaxValue / 10) / ((decimal)height / 500), 1);

                    if (step > 0)
                    {
                        for (decimal i = -1; i <= 10; i += step)
                        {
                            int pos;

                            decimal roundedPos = Math.Floor(minValue);

                            pos = height - (int)(GetRelativePosition(dimension * i + roundedPos, minValue, maxValue) * height);

                            if (pos > 0 && pos < height)
                            {
                                graphics.DrawLine(labelPen, new Point(0, pos), new Point(width, pos));


                                graphics.DrawString((dimension * i + roundedPos).ToString(), DefaultFont, labelBrush, new PointF(0, pos - 1 - DefaultFont.Height));

                            }

                            pos = height - (int)(GetRelativePosition(dimension * -i + roundedPos, minValue, maxValue) * height);

                            if (pos > 0 && pos < height)
                            {
                                graphics.DrawLine(labelPen, new Point(0, pos), new Point(width, pos));


                                graphics.DrawString((dimension * -i + roundedPos).ToString(), DefaultFont, labelBrush, new PointF(0, pos - 1 - DefaultFont.Height));

                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        private void FlipBuffer()
        {
            if (initialized)
            {
                Graphics picboxGraph = Graphics.FromImage(pictureBoxGraph.Image);
                picboxGraph.DrawImageUnscaled(buffer, 0, 0);
            }
        }

        #region IDictionary<string,IGraphValueList> Member

        public void Add(string key, IGraphValueList value)
        {
            graphs.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return graphs.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return graphs.Keys; }
        }

        public bool Remove(string key)
        {
            return graphs.Remove(key);
        }

        public bool TryGetValue(string key, out IGraphValueList value)
        {
            return graphs.TryGetValue(key, out value);
        }

        public ICollection<IGraphValueList> Values
        {
            get { return graphs.Values; }
        }

        public IGraphValueList this[string key]
        {
            get
            {
                return graphs[key];
            }
            set
            {
                graphs[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<string,IGraphValueList>> Member

        public void Add(KeyValuePair<string, IGraphValueList> item)
        {
            graphs.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            graphs.Clear();
        }

        public bool Contains(KeyValuePair<string, IGraphValueList> item)
        {
            return graphs.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, IGraphValueList>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return graphs.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, IGraphValueList> item)
        {
            return graphs.Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,IGraphValueList>> Member

        public IEnumerator<KeyValuePair<string, IGraphValueList>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Member

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
