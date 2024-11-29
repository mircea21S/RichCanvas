using RichCanvas.UITests.Tests;
using System.Collections.Generic;
using System.Drawing;

namespace RichCanvas.UITests.Helpers
{
    internal class GeneratorData
    {
        public int PointsCount { get; }
        public Direction Direction { get; }
        public Point FirstDraggingPoint { get; }
        public int OffsetBetweenPoints { get; }

        public GeneratorData(int pointsCount,
            Direction direction,
            Point firstDraggingPoint,
            int offsetBetweenPoints)
        {
            PointsCount = pointsCount;
            Direction = direction;
            FirstDraggingPoint = firstDraggingPoint;
            OffsetBetweenPoints = offsetBetweenPoints;
        }
    }

    internal class PointsGenerator
    {
        internal static List<Point> GenerateFrom(GeneratorData data)
        {
            var generatedPoints = new List<Point>(data.PointsCount)
            {
                data.FirstDraggingPoint
            };
            for (int i = 1; i < data.PointsCount; i++)
            {
                Point nextPoint = GetNextPoint(generatedPoints[i - 1], data.Direction, data.OffsetBetweenPoints);
                generatedPoints.Add(nextPoint);
            }

            return generatedPoints;
        }

        private static Point GetNextPoint(Point previousPoint, Direction direction, int offset)
        {
            return direction switch
            {
                Direction.Left => previousPoint.OffsetNew(new Point(-offset, 0)),
                Direction.Right => previousPoint.OffsetNew(new Point(offset, 0)),
                Direction.Down => previousPoint.OffsetNew(new Point(0, offset)),
                Direction.Up => previousPoint.OffsetNew(new Point(0, -offset)),
                _ => previousPoint
            };
        }
    }
}
