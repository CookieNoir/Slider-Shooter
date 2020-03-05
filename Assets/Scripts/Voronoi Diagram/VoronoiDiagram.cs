using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class VoronoiDiagram
{
    [Serializable]
    public class VoronoiEdge
    {
        public Vector2 point1;
        public Vector2 point2;
        public int leftCell;
        public int rightCell;
        public List<int> point1connections;
        public List<int> point2connections;

        public VoronoiEdge(Vector2 p1, Vector2 p2, int lc, int rc)
        {
            point1 = p1;
            point2 = p2;
            leftCell = lc;
            rightCell = rc;
            point1connections = new List<int>();
            point2connections = new List<int>();
        }
    }
    [Serializable]
    public class VoronoiCell
    {
        public Vector2 center;
        public List<int> edges;

        public VoronoiCell(Vector2 point)
        {
            center = point;
            edges = new List<int>();
        }
    }

    public List<VoronoiCell> cells;
    public List<VoronoiEdge> edges;

    // Границы диаграммы (значения не могут быть за пределами рамки)
    public Vector2 lowerLeftCorner;
    public Vector2 lowerRightCorner;
    public Vector2 upperLeftCorner;
    public Vector2 upperRightCorner;

    // Индексы верхней и нижней точек диаграммы
    public int topIndex;
    public int botIndex;

    private int loopLimit;

    public VoronoiDiagram(Vector2[] points, Vector2 point1, Vector2 point2, int LoopLimit)
    {
        // 1.Сортировка массива вершин
        IComparer comparer = new Vector2Comparer();
        Array.Sort(points, comparer);

        // 2.Образование элементарных подмножеств
        cells = new List<VoronoiCell>();
        edges = new List<VoronoiEdge>();

        for (int i = 0; i < points.Length; ++i)
            cells.Add(new VoronoiCell(points[i]));

        // 3. Установка границ
        SetBounds(point1, point2);

        // 4.Построение диаграммы методом "Разделяй и властвуй"
        int c = cells.Count / 2;
        int top = -1, bot = -1;
        loopLimit = LoopLimit;
        Voronoi_DivideAndConquer_Recursive(0, c, cells.Count, ref top, ref bot);
        topIndex = top;
        botIndex = bot;
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

    private void Voronoi_DivideAndConquer_Recursive(int startIndex, int middleIndex, int endIndex, ref int top, ref int bot)
    // Диапазоны: [startIndex, middleIndex)
    //            [middleIndex, endIndex)
    {
        if (endIndex - startIndex <= 1)
        {
            top = startIndex;
            bot = startIndex;
            return;
        }

        // Построение диаграмм для двух подмножеств
        int c1 = startIndex + (middleIndex - startIndex) / 2,
            c2 = middleIndex + (endIndex - middleIndex) / 2,
        top1 = -1, bot1 = -1, // Индексы верхней и нижней ячеек для первого подмножества
        top2 = -1, bot2 = -1; // Индексы верхней и нижней ячеек для второго подмножества
        Voronoi_DivideAndConquer_Recursive(startIndex, c1, middleIndex, ref top1, ref bot1);
        Voronoi_DivideAndConquer_Recursive(middleIndex, c2, endIndex, ref top2, ref bot2);

        // Объединение подмножеств
        int leftIndex = top1, rightIndex = top2;

        Vector2 lineDir = cells[rightIndex].center - cells[leftIndex].center;
        Vector2 midPoint = (cells[rightIndex].center + cells[leftIndex].center) / 2f;
        // Получение нормали к прямой
        lineDir.y *= -1;
        Vector2Mathf.Swap(ref lineDir.x, ref lineDir.y);
        if (lineDir.y < 0) lineDir *= -1;
        float cos = lineDir.x / lineDir.magnitude;
        lineDir += midPoint;

        Vector2 intersection = midPoint;
        Vector2 upperPoint = midPoint;
        Vector2 lowerPoint = midPoint;

        // Поиск верхней и нижей точек луча
        if (cos <= -1 + Vector2Mathf.EPS || cos >= 1 - Vector2Mathf.EPS)
        {
            Vector2Mathf.straightLineIntersect(midPoint, lineDir, lowerLeftCorner, upperLeftCorner, ref upperPoint);
            Vector2Mathf.straightLineIntersect(upperPoint, lineDir, lowerRightCorner, upperRightCorner, ref lowerPoint);
        }
        else
        {
            // Поиск верхней точки
            Vector2Mathf.straightLineIntersect(midPoint, lineDir, upperLeftCorner, upperRightCorner, ref upperPoint);
            if (cos >= 0)
                Vector2Mathf.straightLineIntersect(midPoint, lineDir, lowerRightCorner, upperRightCorner, ref intersection);
            else
                Vector2Mathf.straightLineIntersect(midPoint, lineDir, lowerLeftCorner, upperLeftCorner, ref intersection);

            if (Vector2Mathf.SqrMagnitude(intersection - midPoint) < Vector2Mathf.SqrMagnitude(upperPoint - midPoint))
            {
                upperPoint = intersection;
            }

            // Поиск нижней точки
            Vector2Mathf.straightLineIntersect(upperPoint, lineDir, lowerLeftCorner, lowerRightCorner, ref lowerPoint);
            if (cos >= 0)
                Vector2Mathf.straightLineIntersect(upperPoint, lineDir, lowerLeftCorner, upperLeftCorner, ref intersection);
            else
                Vector2Mathf.straightLineIntersect(upperPoint, lineDir, lowerRightCorner, upperRightCorner, ref intersection);

            if (Vector2Mathf.SqrMagnitude(intersection - upperPoint) < Vector2Mathf.SqrMagnitude(lowerPoint - upperPoint))
            {
                lowerPoint = intersection;
            }
        }

        // Список индексов ребер - кандидатов на удаление
        List<int> quarantine = new List<int>();

        // Индекс предыдущего ребра
        int prev = -1;
        int intersect1 = -1, intersect2 = -1;
        bool fromLeft = false;
        int intersectionIndex = -1;
        midPoint = lowerPoint;
        
        VoronoiEdge newEdge = null;
        int newEdgeIndex = -1;
        VoronoiEdge quarantineEdge = null;
        int quarantineEdgeIndex = -1;


        int loopCount = 0;
        while (loopCount < loopLimit && (leftIndex != bot1 || rightIndex != bot2))
        {
            // Просмотр всех отрезков, принадлежащих ячейкам, поиск ближайшей к верхней точки пересечения
            for (int i = 0; i < cells[leftIndex].edges.Count; ++i)
            {
                if (cells[leftIndex].edges[i] != intersect1 && cells[leftIndex].edges[i] != intersect2)
                    if (Vector2Mathf.intersect(upperPoint, lowerPoint, edges[cells[leftIndex].edges[i]].point1, edges[cells[leftIndex].edges[i]].point2, ref intersection))
                    {
                        if (Vector2Mathf.SqrMagnitude(intersection - upperPoint) < Vector2Mathf.SqrMagnitude(midPoint - upperPoint))
                        {
                            midPoint = intersection;
                            intersectionIndex = cells[leftIndex].edges[i];
                            fromLeft = true;
                        }
                    }
            }
            for (int i = 0; i < cells[rightIndex].edges.Count; ++i)
            {
                if (cells[rightIndex].edges[i] != intersect1 && cells[rightIndex].edges[i] != intersect2)
                    if (Vector2Mathf.intersect(upperPoint, lowerPoint, edges[cells[rightIndex].edges[i]].point1, edges[cells[rightIndex].edges[i]].point2, ref intersection))
                    {
                        if (Vector2Mathf.SqrMagnitude(intersection - upperPoint) < Vector2Mathf.SqrMagnitude(midPoint - upperPoint))
                            midPoint = intersection;
                        intersectionIndex = cells[rightIndex].edges[i];
                        fromLeft = false;
                    }
            }
            // Создание ребра
            newEdge = new VoronoiEdge(upperPoint, midPoint, leftIndex, rightIndex);
            edges.Add(newEdge);
            newEdgeIndex = edges.IndexOf(newEdge);
            cells[leftIndex].edges.Add(newEdgeIndex);
            cells[rightIndex].edges.Add(newEdgeIndex);

            // Создание связи с предыдущим ребром
            if (prev != -1) connectEdges(prev, newEdgeIndex);

            if (intersectionIndex == -1) break;
            // Разбиение пересеченного отрезка

            if (fromLeft)
            {
                if (Vector2Mathf.SqrMagnitude((midPoint + edges[intersectionIndex].point1) / 2f - cells[leftIndex].center) <
                    Vector2Mathf.SqrMagnitude((midPoint + edges[intersectionIndex].point1) / 2f - cells[rightIndex].center))
                {
                    // Добавление лишней части отрезка в список на удаление
                    quarantineEdge = new VoronoiEdge(midPoint, edges[intersectionIndex].point2, -1, -1);
                    foreach (int index in edges[intersectionIndex].point2connections)
                        quarantineEdge.point2connections.Add(index);

                    edges[intersectionIndex].point2 = midPoint;
                    edges[intersectionIndex].point2connections.Clear();
                    edges[intersectionIndex].point2connections.Add(newEdgeIndex);
                }
                else
                {
                    // Добавление лишней части отрезка в список на удаление
                    quarantineEdge = new VoronoiEdge(edges[intersectionIndex].point1, midPoint, -1, -1);
                    foreach (int index in edges[intersectionIndex].point1connections)
                        quarantineEdge.point1connections.Add(index);

                    edges[intersectionIndex].point1 = midPoint;
                    edges[intersectionIndex].point1connections.Clear();
                    edges[intersectionIndex].point1connections.Add(newEdgeIndex);
                }
            }
            else
            {
                if (Vector2Mathf.SqrMagnitude((midPoint + edges[intersectionIndex].point2) / 2f - cells[rightIndex].center) <
                    Vector2Mathf.SqrMagnitude((midPoint + edges[intersectionIndex].point2) / 2f - cells[leftIndex].center))
                {
                    // Добавление лишней части отрезка в список на удаление
                    quarantineEdge = new VoronoiEdge(edges[intersectionIndex].point1, midPoint, -1, -1);
                    foreach (int index in edges[intersectionIndex].point1connections)
                        quarantineEdge.point1connections.Add(index);

                    edges[intersectionIndex].point1 = midPoint;
                    edges[intersectionIndex].point1connections.Clear();
                    edges[intersectionIndex].point1connections.Add(newEdgeIndex);
                }
                else
                {
                    // Добавление лишней части отрезка в список на удаление
                    quarantineEdge = new VoronoiEdge(midPoint, edges[intersectionIndex].point2, -1, -1);
                    foreach (int index in edges[intersectionIndex].point2connections)
                        quarantineEdge.point2connections.Add(index);                  

                    edges[intersectionIndex].point2 = midPoint;
                    edges[intersectionIndex].point2connections.Clear();
                    edges[intersectionIndex].point2connections.Add(newEdgeIndex);
                }
            }
            edges.Add(quarantineEdge);
            quarantineEdgeIndex = edges.IndexOf(quarantineEdge);
            quarantine.Add(quarantineEdgeIndex);

            edges[newEdgeIndex].point2connections.Add(intersectionIndex);

            intersect1 = intersectionIndex; intersect2 = quarantineEdgeIndex;

            // перейти на смежную ячейку
            if (fromLeft)
            {
                if (edges[intersectionIndex].leftCell == leftIndex)
                    leftIndex = edges[intersectionIndex].rightCell;
                else
                    leftIndex = edges[intersectionIndex].leftCell;
            }
            else
            {
                if (edges[intersectionIndex].leftCell == rightIndex)
                    rightIndex = edges[intersectionIndex].rightCell;
                else
                    rightIndex = edges[intersectionIndex].leftCell;
            }

            prev = newEdgeIndex;
            // Поиск перпендикуляра к новой прямой
            upperPoint = midPoint;
            lineDir = cells[rightIndex].center - cells[leftIndex].center;
            lineDir.y *= -1;
            Vector2Mathf.Swap(ref lineDir.x, ref lineDir.y);
            if (lineDir.y < 0) lineDir *= -1;
            cos = lineDir.x / lineDir.magnitude;
            lineDir += upperPoint;

            if (cos <= -1 + Vector2Mathf.EPS || cos >= 1 - Vector2Mathf.EPS)
            {
                Vector2Mathf.straightLineIntersect(upperPoint, lineDir, lowerRightCorner, upperRightCorner, ref lowerPoint);
            }
            else
            {
                // Поиск нижней точки
                Vector2Mathf.straightLineIntersect(upperPoint, lineDir, lowerLeftCorner, lowerRightCorner, ref lowerPoint);
                if (cos >= 0)
                    Vector2Mathf.straightLineIntersect(upperPoint, lineDir, lowerLeftCorner, upperLeftCorner, ref intersection);
                else
                    Vector2Mathf.straightLineIntersect(upperPoint, lineDir, lowerRightCorner, upperRightCorner, ref intersection);

                if (Vector2Mathf.SqrMagnitude(intersection - upperPoint) < Vector2Mathf.SqrMagnitude(lowerPoint - upperPoint))
                {
                    lowerPoint = intersection;
                }
            }
            intersectionIndex = -1;
            midPoint = lowerPoint;
            loopCount++;
            Debug.Log(leftIndex + " " + rightIndex);
        } // Конец While
        if (loopCount >= loopLimit) Debug.Log("ПИЗДЕЦ");
        // Завершение ломаной
        newEdge = new VoronoiEdge(upperPoint, lowerPoint, leftIndex, rightIndex);
        edges.Add(newEdge);
        newEdgeIndex = edges.IndexOf(newEdge);
        cells[leftIndex].edges.Add(newEdgeIndex);
        cells[rightIndex].edges.Add(newEdgeIndex);

        if (prev != -1) connectEdges(prev, newEdgeIndex);

        // Удаление лишних ребер

        //removeEdges(ref quarantine);

        // Замена верхнего и нижнего индексов
        if (Vector2Mathf.isFirstHigher(cells[top1].center, cells[top2].center)) top = top1;
        else top = top2;
        if (Vector2Mathf.isFirstHigher(cells[bot1].center, cells[bot2].center)) bot = bot2;
        else bot = bot1;
    }

    private void connectEdges(int edgeIndex, int indexToConnect)
    {
        foreach (int connection in edges[edgeIndex].point2connections)
        {
            if (edges[connection].point1 == edges[edgeIndex].point2)
                edges[connection].point1connections.Add(indexToConnect);
            else
                edges[connection].point2connections.Add(indexToConnect);
            edges[indexToConnect].point1connections.Add(connection);
        }
        edges[edgeIndex].point2connections.Add(indexToConnect);
        edges[indexToConnect].point1connections.Add(edgeIndex);
    }

    private void removeEdges(ref List<int> list)
    {
        int count = list.Count;
        for (int i = 0; i < count; ++i)
        {
            watchEdge(list[i], ref list);
        }

        foreach (int removeIndex in list)
        {
            edges.RemoveAt(removeIndex);
        }
    }

    private void watchEdge(int watchIndex, ref List<int> list)
    {
        foreach (int j in edges[watchIndex].point1connections)
        {
            if (!list.Contains(j))
            {
                list.Add(j);
                watchEdge(j, ref list);
            }
        }
        foreach (int j in edges[watchIndex].point2connections)
        {
            if (!list.Contains(j))
            {
                list.Add(j);
                watchEdge(j, ref list);
            }
        }
    }
}