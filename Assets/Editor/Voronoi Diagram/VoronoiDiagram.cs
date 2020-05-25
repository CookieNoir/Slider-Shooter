using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class VoronoiDiagram : Vector2Mathf
{
    [Serializable]
    public class VoronoiEdge
    {
        public Vector2 point1; // Координата первой точки 
        public Vector2 point2; // Координата второй точки
        public int leftCell; // индекс "левой" ячейки
        public int rightCell; // индекс "правой" ячейки
        public List<VoronoiEdge> point1connections; // Ребра, концами которых является 1 точка
        public List<VoronoiEdge> point2connections; // Ребра, концами которых является 2 точка

        public VoronoiEdge(Vector2 p1, Vector2 p2, int lc, int rc)
        {
            point1 = p1;
            point2 = p2;
            leftCell = lc;
            rightCell = rc;
            point1connections = new List<VoronoiEdge>();
            point2connections = new List<VoronoiEdge>();
        }
    }
    [Serializable]
    public class VoronoiCell
    {
        public Vector2 center; // Координата центра ячейки
        public List<VoronoiEdge> edges; // Ребра, формирующие ячейку

        public VoronoiCell(Vector2 point)
        {
            center = point;
            edges = new List<VoronoiEdge>();
        }
    }

    [Serializable]
    public class BoundIntersection
    {
        public Vector2 point;
        public VoronoiEdge edge;
        public bool isPoint1; // Является ли точка первой в ребре или нет

        public BoundIntersection(Vector2 newPoint, VoronoiEdge newEdge)
        {
            point = newPoint;
            edge = newEdge;
            if (point == edge.point1) isPoint1 = true;
            else isPoint1 = false;
        }
    }

    public List<VoronoiCell> cells;
    public List<VoronoiEdge> edges;
    public List<VoronoiEdge> shortEdges; // Вспомогательный список, хранящий ребра с малой длиной

    // Границы диаграммы (значения не могут быть за пределами рамки)
    public Vector2 lowerLeftCorner;
    public Vector2 lowerRightCorner;
    public Vector2 upperLeftCorner;
    public Vector2 upperRightCorner;

    // Пересечения с границами. Могут использоваться в алгоритмах, использующих диаграмму
    public List<BoundIntersection> intersectionsUp; // Пересечения с верхней границей диаграммы
    public List<BoundIntersection> intersectionsRight; // Пересечения с правой границей диаграммы
    public List<BoundIntersection> intersectionsDown; // Пересечения с нижней границей диаграммы
    public List<BoundIntersection> intersectionsLeft; // Пересечения с левой границей диаграммы

    public VoronoiDiagram(Vector2[] points, Vector2 point1, Vector2 point2)
    {
        // 1.Сортировка массива вершин
        IComparer<Vector2> comparer = new Vector2Comparer();
        Array.Sort(points, comparer);

        // 2.Образование элементарных подмножеств
        cells = new List<VoronoiCell>();
        edges = new List<VoronoiEdge>();
        shortEdges = new List<VoronoiEdge>();

        intersectionsUp = new List<BoundIntersection>();
        intersectionsRight = new List<BoundIntersection>();
        intersectionsDown = new List<BoundIntersection>();
        intersectionsLeft = new List<BoundIntersection>();

        if (points.Length < 1) return;

        for (int i = 0; i < points.Length; ++i)
            cells.Add(new VoronoiCell(points[i]));

        // 3. Установка границ
        SetBounds(point1, point2);

        // 4. Построение диаграммы методом "Разделяй и властвуй"
        int c = cells.Count / 2;
        int top = -1;
        Voronoi_DivideAndConquer_Recursive(0, c, cells.Count, ref top, true);
        // 5. Вписывание диаграммы в установленные границы
        FitInBounds();
        Debug.Log("Diagram for " + cells.Count + " points completed.");
    }

    private void SetBounds(Vector2 point1, Vector2 point2)
    {
        if (point1.x <= point2.x)
        {
            lowerLeftCorner.x = point1.x;
            lowerRightCorner.x = point2.x;
            upperLeftCorner.x = point1.x;
            upperRightCorner.x = point2.x;
        }
        else
        {
            lowerLeftCorner.x = point2.x;
            lowerRightCorner.x = point1.x;
            upperLeftCorner.x = point2.x;
            upperRightCorner.x = point1.x;
        }

        if (point1.y <= point2.y)
        {
            lowerLeftCorner.y = point1.y;
            lowerRightCorner.y = point1.y;
            upperLeftCorner.y = point2.y;
            upperRightCorner.y = point2.y;
        }
        else
        {
            lowerLeftCorner.y = point2.y;
            lowerRightCorner.y = point2.y;
            upperLeftCorner.y = point1.y;
            upperRightCorner.y = point1.y;
        }
    }

    private void Voronoi_DivideAndConquer_case3(int startIndex, ref int middleIndex, int endIndex)
    {
        int top2, bot2;
        if (IsFirstHigher(cells[middleIndex].center, cells[middleIndex + 1].center, false))
        {
            top2 = middleIndex;
            bot2 = middleIndex + 1;
        }
        else
        {
            top2 = middleIndex + 1;
            bot2 = middleIndex;
        }
        Vector2
            point1 = (cells[startIndex].center + cells[top2].center) / 2f,
            point2 = cells[top2].center - cells[startIndex].center,
            point3 = (cells[bot2].center + cells[top2].center) / 2f,
            point4 = cells[top2].center - cells[bot2].center,
            intersection = Vector2.zero;
        point2.y *= -1; Swap(ref point2.x, ref point2.y);
        point2 += point1;
        point4.y *= -1; Swap(ref point4.x, ref point4.y);
        point4 += point3;
        if (StraightLineIntersect(point1, point2, point3, point4, ref intersection))
        {
            if (intersection.x > cells[middleIndex].center.x) middleIndex++;
        }
    }

    private void Voronoi_DivideAndConquer_Recursive(int startIndex, int middleIndex, int endIndex, ref int top, bool isLeft)
    // Диапазоны: [startIndex, middleIndex)
    //            [middleIndex, endIndex)
    {
        if (endIndex - startIndex <= 1)
        {
            top = startIndex;
            return;
        }
        if (endIndex - startIndex == 3)
        {
            if ((cells[middleIndex].center.y > cells[startIndex].center.y) != (cells[middleIndex].center.y > cells[middleIndex + 1].center.y))
            {
                Voronoi_DivideAndConquer_case3(startIndex, ref middleIndex, endIndex);
            }
        }
        // Построение диаграмм для двух подмножеств
        int c1 = startIndex + (middleIndex - startIndex) / 2,
            c2 = middleIndex + (endIndex - middleIndex) / 2,
        top1 = -1, top2 = -1;
        Voronoi_DivideAndConquer_Recursive(startIndex, c1, middleIndex, ref top1, true);
        Voronoi_DivideAndConquer_Recursive(middleIndex, c2, endIndex, ref top2, false);
        // Объединение подмножеств
        int leftIndex = top1, rightIndex = top2;

        Vector2 direction = cells[rightIndex].center - cells[leftIndex].center;
        Vector2 midPoint = (cells[rightIndex].center + cells[leftIndex].center) / 2f;
        // Получение нормали к прямой
        direction.y *= -1;
        Swap(ref direction.x, ref direction.y);
        if (direction.y < 0) direction *= -1;
        float cos = direction.x / direction.magnitude;
        Vector2 dirPoint = direction + midPoint;

        Vector2 intersection = midPoint;
        Vector2 upperPoint = midPoint;
        Vector2 lowerPoint = midPoint;

        // Поиск верхней и нижей точек луча
        if (cos <= -1 + EPS || cos >= 1 - EPS)
        {
            StraightLineIntersect(midPoint, dirPoint, lowerLeftCorner, upperLeftCorner, ref upperPoint);
            StraightLineIntersect(upperPoint, dirPoint, lowerRightCorner, upperRightCorner, ref lowerPoint);
        }
        else
        {
            // Поиск верхней точки
            StraightLineIntersect(midPoint, dirPoint, new Vector2(upperLeftCorner.x, UPPER_Y), new Vector2(upperRightCorner.x, UPPER_Y), ref upperPoint);

            // Поиск нижней точки
            StraightLineIntersect(upperPoint, dirPoint, lowerLeftCorner, lowerRightCorner, ref lowerPoint);
        }

        // Список индексов ребер - кандидатов на удаление
        List<VoronoiEdge> quarantine = new List<VoronoiEdge>();

        bool fromLeft = false;
        midPoint = lowerPoint;

        VoronoiEdge newEdge = null;
        VoronoiEdge prevEdge = null;
        VoronoiEdge intersectedEdge = null;
        VoronoiEdge prevIntersectedEdge = null;
        VoronoiEdge quarantineEdge = null;
        List<VoronoiEdge> stuckSolver = new List<VoronoiEdge>();

        while (true)
        {
            // Просмотр всех отрезков, принадлежащих ячейкам, поиск ближайшей к верхней точки пересечения
            foreach (VoronoiEdge edge in cells[leftIndex].edges)
            {
                if (edge != prevEdge && edge != prevIntersectedEdge && !stuckSolver.Contains(edge))
                {
                    if (UniversalIntersect(upperPoint, lowerPoint, MainEdgeType(prevEdge), edge.point1, edge.point2, EdgeType(edge), ref intersection))
                    {
                        if (SqrMagnitude(upperPoint - intersection) < SqrMagnitude(upperPoint - midPoint))
                        {
                            midPoint = intersection;
                            intersectedEdge = edge;
                            fromLeft = true;
                        }
                    }
                }
            }
            foreach (VoronoiEdge edge in cells[rightIndex].edges)
            {
                if (edge != prevEdge && edge != prevIntersectedEdge && !stuckSolver.Contains(edge))
                {
                    if (UniversalIntersect(upperPoint, lowerPoint, MainEdgeType(prevEdge), edge.point1, edge.point2, EdgeType(edge), ref intersection))
                    {
                        if (SqrMagnitude(upperPoint - intersection) < SqrMagnitude(upperPoint - midPoint))
                        {
                            midPoint = intersection;
                            intersectedEdge = edge;
                            fromLeft = false;

                        }
                    }
                }
            }

            if (intersectedEdge == null) break;
            // Создание ребра
            newEdge = new VoronoiEdge(upperPoint, midPoint, leftIndex, rightIndex);
            edges.Add(newEdge);
            cells[leftIndex].edges.Add(newEdge);
            cells[rightIndex].edges.Add(newEdge);

            // Создание связи с предыдущим ребром
            if (prevEdge != null && prevEdge != newEdge) ConnectEdges(prevEdge, newEdge);
            else
            {
                if (newEdge.point2.y > upperRightCorner.y)
                    newEdge.point1 = midPoint + direction;
            }


            // Разбиение пересеченного отрезка
            switch (EdgeType(intersectedEdge))
            {
                case 0:
                    {
                        if (((intersectedEdge.point1.x < midPoint.x) == (intersectedEdge.point2.x < midPoint.x)) &&
                           ((intersectedEdge.point1.y < midPoint.y) == (intersectedEdge.point2.y < midPoint.y)))
                        {
                            if (SqrMagnitude(midPoint - intersectedEdge.point1) < SqrMagnitude(midPoint - intersectedEdge.point2))
                            {
                                intersectedEdge.point1 = midPoint;
                                intersectedEdge.point1connections.Add(newEdge);
                            }
                            else
                            {
                                intersectedEdge.point2 = midPoint;
                                intersectedEdge.point2connections.Add(newEdge);
                            }
                        }
                        else
                        {
                            if (fromLeft)
                            {
                                if (SqrMagnitude((midPoint + intersectedEdge.point1) / 2f - cells[leftIndex].center) <
                                    SqrMagnitude((midPoint + intersectedEdge.point1) / 2f - cells[rightIndex].center))
                                {
                                    intersectedEdge.point2 = midPoint;
                                    intersectedEdge.point2connections.Add(newEdge);
                                }
                                else
                                {
                                    intersectedEdge.point1 = midPoint;
                                    intersectedEdge.point1connections.Add(newEdge);
                                }
                            }
                            else
                            {
                                if (SqrMagnitude((midPoint + intersectedEdge.point2) / 2f - cells[rightIndex].center) <
                                    SqrMagnitude((midPoint + intersectedEdge.point2) / 2f - cells[leftIndex].center))
                                {
                                    intersectedEdge.point1 = midPoint;
                                    intersectedEdge.point1connections.Add(newEdge);
                                }
                                else
                                {
                                    intersectedEdge.point2 = midPoint;
                                    intersectedEdge.point2connections.Add(newEdge);
                                }
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        if (((intersectedEdge.point1.x < midPoint.x) == (intersectedEdge.point2.x < midPoint.x)) &&
                           ((intersectedEdge.point1.y < midPoint.y) == (intersectedEdge.point2.y < midPoint.y)))
                        {
                            intersectedEdge.point2 = midPoint;
                            intersectedEdge.point2connections.Add(newEdge);
                        }
                        else
                        {
                            if (fromLeft)
                            {
                                if (SqrMagnitude((midPoint + intersectedEdge.point1) / 2f - cells[leftIndex].center) <
                                    SqrMagnitude((midPoint + intersectedEdge.point1) / 2f - cells[rightIndex].center))
                                {
                                    intersectedEdge.point2 = midPoint;
                                    intersectedEdge.point2connections.Add(newEdge);
                                }
                                else
                                {
                                    // Добавление лишней части отрезка в список на удаление
                                    quarantineEdge = new VoronoiEdge(intersectedEdge.point1, midPoint, -1, -1);
                                    foreach (VoronoiEdge index in intersectedEdge.point1connections)
                                        quarantineEdge.point1connections.Add(index);
                                    ChangeConnections(intersectedEdge, quarantineEdge, true);

                                    intersectedEdge.point1 = midPoint;
                                    intersectedEdge.point1connections.Clear();
                                    intersectedEdge.point1connections.Add(newEdge);
                                }
                            }
                            else
                            {
                                if (SqrMagnitude((midPoint + intersectedEdge.point2) / 2f - cells[rightIndex].center) <
                                    SqrMagnitude((midPoint + intersectedEdge.point2) / 2f - cells[leftIndex].center))
                                {
                                    // Добавление лишней части отрезка в список на удаление
                                    quarantineEdge = new VoronoiEdge(intersectedEdge.point1, midPoint, -1, -1);
                                    foreach (VoronoiEdge index in intersectedEdge.point1connections)
                                        quarantineEdge.point1connections.Add(index);
                                    ChangeConnections(intersectedEdge, quarantineEdge, true);

                                    intersectedEdge.point1 = midPoint;
                                    intersectedEdge.point1connections.Clear();
                                    intersectedEdge.point1connections.Add(newEdge);
                                }
                                else
                                {
                                    intersectedEdge.point2 = midPoint;
                                    intersectedEdge.point2connections.Add(newEdge);
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        if (((intersectedEdge.point1.x < midPoint.x) == (intersectedEdge.point2.x < midPoint.x)) &&
                           ((intersectedEdge.point1.y < midPoint.y) == (intersectedEdge.point2.y < midPoint.y)))
                        {
                            intersectedEdge.point1 = midPoint;
                            intersectedEdge.point1connections.Add(newEdge);
                        }
                        else
                        {
                            if (fromLeft)
                            {
                                if (SqrMagnitude((midPoint + intersectedEdge.point1) / 2f - cells[leftIndex].center) <
                                    SqrMagnitude((midPoint + intersectedEdge.point1) / 2f - cells[rightIndex].center))
                                {
                                    // Добавление лишней части отрезка в список на удаление
                                    quarantineEdge = new VoronoiEdge(midPoint, intersectedEdge.point2, -1, -1);
                                    foreach (VoronoiEdge index in intersectedEdge.point2connections)
                                        quarantineEdge.point2connections.Add(index);
                                    ChangeConnections(intersectedEdge, quarantineEdge, false);

                                    intersectedEdge.point2 = midPoint;
                                    intersectedEdge.point2connections.Clear();
                                    intersectedEdge.point2connections.Add(newEdge);
                                }
                                else
                                {
                                    intersectedEdge.point1 = midPoint;
                                    intersectedEdge.point1connections.Add(newEdge);
                                }
                            }
                            else
                            {
                                if (SqrMagnitude((midPoint + intersectedEdge.point2) / 2f - cells[rightIndex].center) <
                                    SqrMagnitude((midPoint + intersectedEdge.point2) / 2f - cells[leftIndex].center))
                                {
                                    intersectedEdge.point1 = midPoint;
                                    intersectedEdge.point1connections.Add(newEdge);
                                }
                                else
                                {
                                    // Добавление лишней части отрезка в список на удаление
                                    quarantineEdge = new VoronoiEdge(midPoint, intersectedEdge.point2, -1, -1);
                                    foreach (VoronoiEdge index in intersectedEdge.point2connections)
                                        quarantineEdge.point2connections.Add(index);
                                    ChangeConnections(intersectedEdge, quarantineEdge, false);

                                    intersectedEdge.point2 = midPoint;
                                    intersectedEdge.point2connections.Clear();
                                    intersectedEdge.point2connections.Add(newEdge);
                                }
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (fromLeft)
                        {
                            if (SqrMagnitude((midPoint + intersectedEdge.point1) / 2f - cells[leftIndex].center) <
                                SqrMagnitude((midPoint + intersectedEdge.point1) / 2f - cells[rightIndex].center))
                            {
                                // Добавление лишней части отрезка в список на удаление
                                quarantineEdge = new VoronoiEdge(midPoint, intersectedEdge.point2, -1, -1);
                                foreach (VoronoiEdge index in intersectedEdge.point2connections)
                                    quarantineEdge.point2connections.Add(index);
                                ChangeConnections(intersectedEdge, quarantineEdge, false);

                                intersectedEdge.point2 = midPoint;
                                intersectedEdge.point2connections.Clear();
                                intersectedEdge.point2connections.Add(newEdge);
                            }
                            else
                            {
                                // Добавление лишней части отрезка в список на удаление
                                quarantineEdge = new VoronoiEdge(intersectedEdge.point1, midPoint, -1, -1);
                                foreach (VoronoiEdge index in intersectedEdge.point1connections)
                                    quarantineEdge.point1connections.Add(index);
                                ChangeConnections(intersectedEdge, quarantineEdge, true);

                                intersectedEdge.point1 = midPoint;
                                intersectedEdge.point1connections.Clear();
                                intersectedEdge.point1connections.Add(newEdge);
                            }
                        }
                        else
                        {
                            if (SqrMagnitude((midPoint + intersectedEdge.point2) / 2f - cells[rightIndex].center) <
                                SqrMagnitude((midPoint + intersectedEdge.point2) / 2f - cells[leftIndex].center))
                            {
                                // Добавление лишней части отрезка в список на удаление
                                quarantineEdge = new VoronoiEdge(intersectedEdge.point1, midPoint, -1, -1);
                                foreach (VoronoiEdge index in intersectedEdge.point1connections)
                                    quarantineEdge.point1connections.Add(index);
                                ChangeConnections(intersectedEdge, quarantineEdge, true);

                                intersectedEdge.point1 = midPoint;
                                intersectedEdge.point1connections.Clear();
                                intersectedEdge.point1connections.Add(newEdge);
                            }
                            else
                            {
                                // Добавление лишней части отрезка в список на удаление
                                quarantineEdge = new VoronoiEdge(midPoint, intersectedEdge.point2, -1, -1);
                                foreach (VoronoiEdge index in intersectedEdge.point2connections)
                                    quarantineEdge.point2connections.Add(index);
                                ChangeConnections(intersectedEdge, quarantineEdge, false);

                                intersectedEdge.point2 = midPoint;
                                intersectedEdge.point2connections.Clear();
                                intersectedEdge.point2connections.Add(newEdge);
                            }
                        }
                        break;
                    }
            }

            if (quarantineEdge != null) quarantine.Add(quarantineEdge);

            newEdge.point2connections.Add(intersectedEdge);
            if (prevEdge != null && SqrMagnitude(upperPoint - midPoint) < EPSEXT)
            {
                stuckSolver.Add(prevEdge);
                stuckSolver.Add(prevIntersectedEdge);
                shortEdges.Add(newEdge);
            }
            else
            {
                if (stuckSolver.Count > 0) stuckSolver.Clear();
            }
            prevEdge = newEdge;
            prevIntersectedEdge = intersectedEdge;
            // перейти на смежную ячейку
            if (fromLeft)
            {
                if (intersectedEdge.leftCell == leftIndex)
                    leftIndex = intersectedEdge.rightCell;
                else
                    leftIndex = intersectedEdge.leftCell;
            }
            else
            {
                if (intersectedEdge.leftCell == rightIndex)
                    rightIndex = intersectedEdge.rightCell;
                else
                    rightIndex = intersectedEdge.leftCell;
            }

            // Поиск перпендикуляра к новой прямой
            upperPoint = midPoint;
            direction = cells[rightIndex].center - cells[leftIndex].center;
            direction.y *= -1;
            Swap(ref direction.x, ref direction.y);
            if (direction.y < 0) direction *= -1;
            cos = direction.x / direction.magnitude;
            direction += upperPoint;

            if (cos <= -1 + EPS || cos >= 1 - EPS)
            {
                StraightLineIntersect(upperPoint, direction, lowerRightCorner, upperRightCorner, ref lowerPoint);
            }
            else
            {
                // Поиск нижней точки
                StraightLineIntersect(upperPoint, direction, lowerLeftCorner, lowerRightCorner, ref lowerPoint);
            }
            intersectedEdge = null;
            quarantineEdge = null;
            midPoint = lowerPoint;
        } // Конец While

        // Завершение ломаной
        newEdge = new VoronoiEdge(upperPoint, lowerPoint, leftIndex, rightIndex);
        edges.Add(newEdge);
        cells[leftIndex].edges.Add(newEdge);
        cells[rightIndex].edges.Add(newEdge);

        if (prevEdge != null) ConnectEdges(prevEdge, newEdge);

        // Удаление лишних ребер

        RemoveEdges(ref quarantine);

        // Замена верхнего и нижнего индексов
        if (IsFirstHigher(cells[top1].center, cells[top2].center, isLeft)) top = top1;
        else top = top2;
    }

    protected void ConnectEdges(VoronoiEdge edgeIndex, VoronoiEdge indexToConnect)
    {
        foreach (VoronoiEdge connection in edgeIndex.point2connections)
        {
            if (connection.point1 == edgeIndex.point2)
                connection.point1connections.Add(indexToConnect);
            else
                connection.point2connections.Add(indexToConnect);
            indexToConnect.point1connections.Add(connection);
        }
        edgeIndex.point2connections.Add(indexToConnect);
        indexToConnect.point1connections.Add(edgeIndex);
    }

    protected void ChangeConnections(VoronoiEdge edgeIndex, VoronoiEdge indexToConnect, bool point1)
    {
        if (point1)
        {
            foreach (VoronoiEdge connection in edgeIndex.point1connections)
            {
                if (connection.point1 == edgeIndex.point1)
                {
                    connection.point1connections.Remove(edgeIndex);
                    connection.point1connections.Add(indexToConnect);
                }
                else
                {
                    connection.point2connections.Remove(edgeIndex);
                    connection.point2connections.Add(indexToConnect);
                }
            }
        }
        else
        {
            foreach (VoronoiEdge connection in edgeIndex.point2connections)
            {
                if (connection.point1 == edgeIndex.point2)
                {
                    connection.point1connections.Remove(edgeIndex);
                    connection.point1connections.Add(indexToConnect);
                }
                else
                {
                    connection.point2connections.Remove(edgeIndex);
                    connection.point2connections.Add(indexToConnect);
                }
            }
        }
    }

    protected void RemoveEdges(ref List<VoronoiEdge> list)
    {
        int count = list.Count;
        for (int i = 0; i < count; ++i)
        {
            WatchEdge(list[i], ref list);
        }

        foreach (VoronoiEdge edge in list)
        {
            if (edge.leftCell != -1)
            {
                cells[edge.leftCell].edges.Remove(edge);
                cells[edge.rightCell].edges.Remove(edge);
            }
            //Debug.Log("Removed: " + edges.IndexOf(edge));
            edges.Remove(edge);
        }
    }

    protected void WatchEdge(VoronoiEdge watchIndex, ref List<VoronoiEdge> list)
    {
        foreach (VoronoiEdge j in watchIndex.point1connections)
        {
            if (!list.Contains(j))
            {
                list.Add(j);
                WatchEdge(j, ref list);
            }
        }
        foreach (VoronoiEdge j in watchIndex.point2connections)
        {
            if (!list.Contains(j))
            {
                list.Add(j);
                WatchEdge(j, ref list);
            }
        }
    }

    protected int EdgeType(VoronoiEdge edge)
    {
        int result = 0;
        if (edge.point1connections.Count > 0) result += 1;
        if (edge.point2connections.Count > 0) result += 2;
        return result;
    }

    private int MainEdgeType(VoronoiEdge prev)
    {
        if (prev != null) return 1;
        else return 0;
    }

    protected bool InsideBounds(Vector2 point)
    {
        return point.x >= lowerLeftCorner.x - EPS && point.x <= upperRightCorner.x + EPS &&
               point.y >= lowerLeftCorner.y - EPS && point.y <= upperRightCorner.y + EPS;
    }

    private void FitInBounds()
    {
        List<VoronoiEdge> toRemove = new List<VoronoiEdge>();
        Vector2
            intersectionUp = Vector2.negativeInfinity,
            intersectionRight = Vector2.negativeInfinity,
            intersectionDown = Vector2.negativeInfinity,
            intersectionLeft = Vector2.negativeInfinity,
            nearestPoint;
        int crossResult;
        float sqrMag;
        foreach (VoronoiEdge edge in edges)
        {
            crossResult = CrossBounds(edge, ref intersectionUp, ref intersectionRight, ref intersectionDown, ref intersectionLeft);
            if (!InsideBounds(edge.point1) && !InsideBounds(edge.point2) && crossResult == 0)
            {
                RemoveConnections(edge);
                toRemove.Add(edge);
            }
            else
            {
                if (!InsideBounds(edge.point1))
                {
                    RemoveConnectionsParticular(edge, true);
                    edge.point1connections.Clear();
                    nearestPoint = Vector2.negativeInfinity;
                    sqrMag = float.PositiveInfinity;
                    if (crossResult % 2 == 1 && SqrMagnitude(edge.point1 - intersectionUp) < sqrMag)
                    {
                        nearestPoint = intersectionUp;
                        sqrMag = SqrMagnitude(edge.point1 - nearestPoint);
                    }
                    if ((crossResult % 4) / 2 == 1 && SqrMagnitude(edge.point1 - intersectionRight) < sqrMag)
                    {
                        nearestPoint = intersectionRight;
                        sqrMag = SqrMagnitude(edge.point1 - nearestPoint);
                    }
                    if ((crossResult % 8) / 4 == 1 && SqrMagnitude(edge.point1 - intersectionDown) < sqrMag)
                    {
                        nearestPoint = intersectionDown;
                        sqrMag = SqrMagnitude(edge.point1 - nearestPoint);
                    }
                    if (crossResult / 8 == 1 && SqrMagnitude(edge.point1 - intersectionLeft) < sqrMag)
                    {
                        nearestPoint = intersectionLeft;
                        sqrMag = SqrMagnitude(edge.point1 - nearestPoint);
                    }
                    edge.point1 = nearestPoint;
                }
                if (!InsideBounds(edge.point2))
                {
                    RemoveConnectionsParticular(edge, false);
                    edge.point2connections.Clear();
                    nearestPoint = Vector2.negativeInfinity;
                    sqrMag = float.PositiveInfinity;
                    if (crossResult % 2 == 1 && SqrMagnitude(edge.point2 - intersectionUp) < sqrMag)
                    {
                        nearestPoint = intersectionUp;
                        sqrMag = SqrMagnitude(edge.point2 - nearestPoint);
                    }
                    if ((crossResult % 4) / 2 == 1 && SqrMagnitude(edge.point2 - intersectionRight) < sqrMag)
                    {
                        nearestPoint = intersectionRight;
                        sqrMag = SqrMagnitude(edge.point2 - nearestPoint);
                    }
                    if ((crossResult % 8) / 4 == 1 && SqrMagnitude(edge.point2 - intersectionDown) < sqrMag)
                    {
                        nearestPoint = intersectionDown;
                        sqrMag = SqrMagnitude(edge.point2 - nearestPoint);
                    }
                    if (crossResult / 8 == 1 && SqrMagnitude(edge.point2 - intersectionLeft) < sqrMag)
                    {
                        nearestPoint = intersectionLeft;
                        sqrMag = SqrMagnitude(edge.point2 - nearestPoint);
                    }
                    edge.point2 = nearestPoint;
                }
            }
            if (crossResult > 0)
            {
                if (crossResult % 2 == 1)
                {
                    if (Between(upperLeftCorner.x, upperRightCorner.x, edge.point1.x) && Between(upperLeftCorner.y, upperRightCorner.y, edge.point1.y))
                    {
                        intersectionsUp.Add(new BoundIntersection(edge.point1, edge));
                    }
                    if (Between(upperLeftCorner.x, upperRightCorner.x, edge.point2.x) && Between(upperLeftCorner.y, upperRightCorner.y, edge.point2.y))
                    {
                        intersectionsUp.Add(new BoundIntersection(edge.point2, edge));
                    }
                }
                if ((crossResult % 4) / 2 == 1)
                {
                    if (Between(upperRightCorner.x, lowerRightCorner.x, edge.point1.x) && Between(upperRightCorner.y, lowerRightCorner.y, edge.point1.y))
                    {
                        intersectionsRight.Add(new BoundIntersection(edge.point1, edge));
                    }
                    if (Between(upperRightCorner.x, lowerRightCorner.x, edge.point2.x) && Between(upperRightCorner.y, lowerRightCorner.y, edge.point2.y))
                    {
                        intersectionsRight.Add(new BoundIntersection(edge.point2, edge));
                    }
                }
                if ((crossResult % 8) / 4 == 1)
                {
                    if (Between(lowerRightCorner.x, lowerLeftCorner.x, edge.point1.x) && Between(lowerRightCorner.y, lowerLeftCorner.y, edge.point1.y))
                    {
                        intersectionsDown.Add(new BoundIntersection(edge.point1, edge));
                    }
                    if (Between(lowerRightCorner.x, lowerLeftCorner.x, edge.point2.x) && Between(lowerRightCorner.y, lowerLeftCorner.y, edge.point2.y))
                    {
                        intersectionsDown.Add(new BoundIntersection(edge.point2, edge));
                    }
                }
                if (crossResult / 8 == 1)
                {
                    if (Between(lowerLeftCorner.x, upperLeftCorner.x, edge.point1.x) && Between(lowerLeftCorner.y, upperLeftCorner.y, edge.point1.y))
                    {
                        intersectionsLeft.Add(new BoundIntersection(edge.point1, edge));
                    }
                    if (Between(lowerLeftCorner.x, upperLeftCorner.x, edge.point2.x) && Between(lowerLeftCorner.y, upperLeftCorner.y, edge.point2.y))
                    {
                        intersectionsLeft.Add(new BoundIntersection(edge.point2, edge));
                    }
                }
            }
        }

        foreach (VoronoiEdge edge in toRemove)
        {
            cells[edge.leftCell].edges.Remove(edge);
            cells[edge.rightCell].edges.Remove(edge);
            edges.Remove(edge);
        }
    }

    protected int CrossBounds(VoronoiEdge edge, ref Vector2 intersectionUp, ref Vector2 intersectionRight, ref Vector2 intersectionDown, ref Vector2 intersectionLeft)
    {
        bool a = UniversalIntersect(edge.point1, edge.point2, EdgeType(edge), upperLeftCorner, upperRightCorner, 3, ref intersectionUp);
        bool b = UniversalIntersect(edge.point1, edge.point2, EdgeType(edge), upperRightCorner, lowerRightCorner, 3, ref intersectionRight);
        bool c = UniversalIntersect(edge.point1, edge.point2, EdgeType(edge), lowerRightCorner, lowerLeftCorner, 3, ref intersectionDown);
        bool d = UniversalIntersect(edge.point1, edge.point2, EdgeType(edge), lowerLeftCorner, upperLeftCorner, 3, ref intersectionLeft);
        int result = 0;
        if (a) result += 1;
        if (b) result += 2;
        if (c) result += 4;
        if (d) result += 8;
        return result;
    }

    protected void RemoveConnections(VoronoiEdge edge)
    {
        foreach (VoronoiEdge connection in edge.point1connections)
        {
            if (connection.point1 == edge.point1)
            {
                connection.point1connections.Remove(edge);
            }
            else
            {
                connection.point2connections.Remove(edge);
            }
        }
        foreach (VoronoiEdge connection in edge.point2connections)
        {
            if (connection.point1 == edge.point2)
            {
                connection.point1connections.Remove(edge);
            }
            else
            {
                connection.point2connections.Remove(edge);
            }
        }
    }

    protected void RemoveConnectionsParticular(VoronoiEdge edge, bool point1)
    {
        if (point1)
        {
            foreach (VoronoiEdge connection in edge.point1connections)
            {
                if (connection.point1 == edge.point1)
                {
                    connection.point1connections.Remove(edge);
                }
                else
                {
                    connection.point2connections.Remove(edge);
                }
            }
        }
        else
        {
            foreach (VoronoiEdge connection in edge.point2connections)
            {
                if (connection.point1 == edge.point2)
                {
                    connection.point1connections.Remove(edge);
                }
                else
                {
                    connection.point2connections.Remove(edge);
                }
            }
        }
    }
}