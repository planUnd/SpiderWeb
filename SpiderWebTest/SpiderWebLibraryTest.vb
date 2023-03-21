Option Explicit On

Imports SpiderWebLibrary
Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports SpiderWebLibrary.graphTools
Imports SpiderWebLibrary.graphRepresentaions

Module SpiderWebLibraryTest

    Sub Main()
        'System.Console.WriteLine("----------------------------- testElements -----------------------------")
        ' testgraphElement()
        'System.Console.WriteLine("----------------------------- testGraph -----------------------------")
        ' Dim i As Integer = 9
        ' For i = 0 To 10
        ' System.Console.WriteLine("Test Number: " & i)
        ' System.Console.WriteLine(" Passed: " & testGraph(i))
        ' System.Console.WriteLine(" MinimalSpanningTree: " & testMinST(i))
        ' System.Console.WriteLine(" BFS: " & testBFS(i))
        ' System.Console.WriteLine("")
        ' System.Console.WriteLine(" DFS: " & testDFS(i))
        ' System.Console.WriteLine("")
        ' System.Console.WriteLine(" SSSP: " & testSSSP(i))
        ' System.Console.WriteLine(" EulerianPath: " & testEP(i))
        ' System.Console.WriteLine(" rndSGC: " & testrndSGC(i))
        'System.Console.WriteLine("")
        'System.Console.WriteLine("")
        ' Next
        Dim w, h As Integer
        w = 30
        h = 30
        Dim VGA As New visualGraph(w, h)

        VGA.gridCell(1, 1) = visualGraph.State.solid
        'VGA.changeState(2, 1) = recursiveShadowCasting.State.solid
        VGA.gridCell(1, 2) = visualGraph.State.solid
        VGA.gridCell(2, 2) = visualGraph.State.solid

        VGA.gridCell(6, 2) = visualGraph.State.solid
        VGA.gridCell(6, 3) = visualGraph.State.solid
        VGA.gridCell(6, 6) = visualGraph.State.solid
        VGA.gridCell(6, 7) = visualGraph.State.solid

        For iX As Integer = 0 To w - 1
            Dim str As String = ""
            For iY As Integer = 0 To h - 1
                str = str & VGA.gridCell(iX, iY)
            Next
            System.Console.WriteLine(str)
        Next

        Dim sX As Integer = 2
        Dim sY As Integer = 1

        System.Console.WriteLine("Visibility:")

        VGA.recursive(sX, sY)

        For iX As Integer = 0 To w - 1
            Dim str As String = ""
            For iY As Integer = 0 To h - 1
                If iX = sX And iY = sY Then
                    str = str & "X"
                Else
                    str = str & VGA.getInNB(iX * w + iY).Count
                End If
            Next
            System.Console.WriteLine(str)
        Next

        System.Console.WriteLine(VGA.Item(sX * w + sY))



        System.Console.ReadLine()
    End Sub

    Private Function testrndSGC(ByVal s As Integer) As String
        Dim testCase As Integer = 0


        Dim gEL As graphEdgeList
        gEL = getRandomGraphEdgeList(15, 1, s)
        outPutgEL(gEL)
        Dim rndG As New graphColoring(gEL)

        ' testCase = Convert.ToInt32(rndG.rndSGC(6, 1000)) << 0

        System.Console.WriteLine(" colorCount: " & rndG.colorCount)
        ' outPutIL(rndG.color)

        Return Convert.ToString(testCase, 2)
    End Function


    Private Function testEP(ByVal s As Integer) As String
        Dim testCase As Integer = 0


        Dim gEL As graphEdgeList
        gEL = getRandomGraphEdgeList(20, 1, s)

        Dim gEP As New eulerianPath(gEL)

        ' gEP.ensureUndirected()
        outPutgVL(gEP)

        Dim test As Integer = 0

        For i As Integer = 0 To gEP.vertexCount - 1
            System.Console.WriteLine(gEP.getInNB(i).Count & " <> " & gEP.Item(i).outDegree)
            If gEP.getInNB(i).Count <> gEP.Item(i).outDegree Then
                test += 1
                If test > 2 Then
                    Exit For
                End If
            End If

        Next

        testCase += Convert.ToInt32(gEP.computeSPEP() = (test <= 2)) << 0

        System.Console.WriteLine("SP: " & gEP.getSP.Count & " EP: " & gEP.getEP.Count)
        Return Convert.ToString(testCase, 2)
    End Function

    Private Function testSSSP(ByVal s As Integer) As String
        Dim testCase As Integer = 0


        Dim gEL As graphEdgeList
        gEL = getRandomGraphEdgeList(20, 1, s)
        gEL.ensureUndirected()

        Dim gVL As New graphVertexList(gEL)
        'outPutgEL(gEL)
        'outPutgEL(New graphEdgeList(gVL))


        Dim gSSSP As New SSSP(gVL)
        ' outPutgEL(New graphEdgeList(gSSSP))


        gSSSP.find(4)
        'outPutgVL(gSSSP)
        'outPutDL(gSSSP.dist)

        Dim EP As Integer = 0


        outPutgEL(gSSSP.gELPath(EP))

        ' outPutDL(gDFS.dist)

        testCase += Convert.ToInt32(gSSSP.isValidTree) << 0

        Return Convert.ToString(testCase, 2)
    End Function

    Private Function testDFS(ByVal s As Integer) As String
        Dim testCase As Integer = 0


        Dim gEl As graphEdgeList
        gEl = getRandomGraphEdgeList(20, 20, s)
        gEl.ensureUndirected()

        Dim gVL As New graphVertexList(gEl)

        Dim gDFS As New DFS(gVL)
        gDFS.find(4)

        Dim EP As Integer = 0
        'outPutgVL(gDFS.gVLPath(EP))
        outPutgEL(gDFS.gELPath(EP))

        ' outPutDL(gDFS.dist)

        testCase += Convert.ToInt32(gDFS.isValidTree) << 0

        Return Convert.ToString(testCase, 2)
    End Function

    Private Function testBFS(ByVal s As Integer) As String
        Dim testCase As Integer = 0


        Dim gEl As graphEdgeList
        gEl = getRandomGraphEdgeList(20, 20, s)
        gEl.ensureUndirected()

        Dim gVL As New graphVertexList(gEl)

        Dim gBFS As New BFS(gVL)
        gBFS.find(4)

        Dim EP As Integer = 0
        ' outPutgVL(gBFS.gVLPath(EP))
        outPutgEL(gBFS.gELPath(EP))

        'outPutDL(gBFS.dist)

        testCase += Convert.ToInt32(gBFS.isValidTree) << 0

        Return Convert.ToString(testCase, 2)
    End Function

    Private Sub outPutIL(ByVal d As List(Of Integer))
        For Each i As Integer In d
            System.Console.WriteLine(i)
        Next
    End Sub

    Private Sub outPutDL(ByVal d As List(Of Double))
        For Each i As Double In d
            System.Console.WriteLine(i)
        Next
    End Sub

    Private Function getRandomGraphEdgeList(ByVal maxV As Integer, _
                                            ByVal maxC As Integer, _
                                            ByVal s As Integer) As graphEdgeList
        Dim rnd As New Random(s)
        Dim gEl As New graphEdgeList()
        Dim gE As graphEdge

        For i = 0 To maxV
            For ii = 0 To maxV / 2
                gE = New graphEdge(rnd.Next(0, maxV), rnd.Next(0, maxV), Math.Max(rnd.NextDouble() * maxC, 1))
                gEl.insert(gE)
            Next
        Next

        Return gEl
    End Function

    Private Function testMinST(ByVal s As Integer) As String
        Dim testCase As Integer = 0

        Dim gEl As graphEdgeList

        gEl = getRandomGraphEdgeList(10, 20, s)

        gEl.ensureUndirected()

        Dim gMS As New minSpanningTree(gEl)
        '  gMS.computeTree()

        ' System.Console.WriteLine("----------------------------- MinimalSpanningTree -----------------------------")
        ' outPutgEL(gEl)
        ' System.Console.WriteLine("-----------------------------")
        ' outPutgEL(gMS)
        ' System.Console.WriteLine("----------------------------- MinimalSpanningTree -----------------------------")
        testCase += Convert.ToInt32(gMS.isValidTree()) << 0

        Return Convert.ToString(testCase, 2)
    End Function

    Private Function testGraph(ByVal s As Integer) As String

        Dim testCase As Integer = 0

        Dim rndC As New Random(s)
        Dim rndB As New Random(s + 43)

        Dim gEl As New graphEdgeList()

        Dim maxV As Integer = 10
        Dim maxC As Double = 20
        Dim i As Integer

        Dim gE As graphEdge

        For i = 0 To maxV
            gE = New graphEdge(rndB.Next(0, maxV), rndB.Next(0, maxV), rndC.NextDouble() * maxC)
            gEl.insert(gE)
        Next
        Dim gVL As New graphVertexList(gEl.getVertexList)

        gEl.insert(New graphEdge(0, 23))
        gVL.insert(New graphEdge(0, 23))
        gEl.insert(New graphEdge(0, 35))
        gVL.insert(New graphEdge(0, 35))

        Dim gEL2 As New graphEdgeList()
        Dim gVL2 As New graphVertexList()

        Dim gVtmp As New graphVertex(12)
        gVtmp.insert(0)
        gVtmp.insert(8)
        gVtmp.insert(14)

        gVL2.insert(gVtmp)
        gEL2.insert(gVtmp)
        gVL.insert(gVtmp)
        gEl.insert(gVtmp)


        gEL2.union(gEl)
        gVL2.union(gVL)
        ' outPutgVL(gVL)
        ' outPutgEL(gEl)
        ' System.Console.WriteLine(" ")
        gVL2.simplify()
        gVL.ensureUndirected()
        gEl.ensureUndirected()

        gVL2.ensureUndirected()
        gVL.simplify()
        gEl.simplify()

        gVL.delete(New graphEdge(0, 23))
        gEl.delete(New graphEdge(0, 23))
        gVL2.delete(New graphEdge(0, 23))

        gVL.delete(New graphEdge(0, 1))
        gEl.delete(New graphEdge(0, 1))
        gVL2.delete(New graphEdge(0, 1))

        gVL.delete(New graphVertex(0))
        gEl.delete(New graphVertex(0))
        gVL2.delete(New graphVertex(0))

        gVL2.simplify()
        gVL.simplify()
        gEl.simplify()

        'outPutgVL(gVL)
        'outPutgEL(gEl)

        ' gEl.dualNodeGraph()
        ' gVL.dualNodeGraph()
        ' gEL2.dualNodeGraph()
        ' gVL2.dualNodeGraph()

        ' outPutgVL(gVL)
        'outPutgEL(gEl)

        ' outPutgVL(gVL2)
        ' outPutgVL(gVL)
        ' outPutgEL(gEl)

        Dim last As Integer
        'System.Console.WriteLine("Graph: " & gEl.ToString() & "------------------")
        last = gEl.edgeCount - 1
        'System.Console.WriteLine("gEl.last " & gEl.Item(last).ToString())
        'System.Console.WriteLine("")
        'System.Console.WriteLine("Graph: " & gVL.ToString() & "------------------")
        last = gVL.vertexCount - 1
        'System.Console.WriteLine("gVL.last " & gVL.Item(gVL.vertexCount - 1).ToString())

        'system.Console.WriteLine("")
        'System.Console.WriteLine("edgeCount: " & gEl.edgeCount & "/" & gVL.edgeCount)
        testCase += Convert.ToInt32(gEl.edgeCount = gVL.edgeCount And _
                                    gVL2.edgeCount = gEl.edgeCount) << 0

        System.Console.WriteLine("vertexCount: " & gEl.vertexCount & "/" & gVL.vertexCount & "/" & gEl.getVertexList.Count)
        testCase += Convert.ToInt32(gEl.vertexCount = gVL.vertexCount And _
                                    gVL2.vertexCount = gVL.vertexCount) << 1

        'System.Console.WriteLine("negativeCost: " & gEl.negativeCost)
        testCase += Convert.ToInt32(gEl.negativeCost = gVL.negativeCost) << 2

        i = rndB.Next(0, maxV)
        'System.Console.WriteLine("getInDegree(" & i & "): " & gEl.getInEdges(i).Count)
        testCase += Convert.ToInt32(gEl.getInEdges(i).Count = gVL.getInEdges(i).Count And _
                     gEl.getInNB(i).Count = gVL.getInNB(i).Count And _
                      gEl.getInNB(i).Count = gEl.getInEdges(i).Count) << 3

        i = rndB.Next(0, maxV)
        'System.Console.WriteLine("getOutDegree(" & i & "): " & gEl.getOutEdges(i).Count)
        testCase += Convert.ToInt32(gEl.getOutEdges(i).Count = gVL.getOutEdges(i).Count And _
                     gEl.getOutNB(i).Count = gVL.getOutNB(i).Count And _
                     gEl.getOutNB(i).Count = gEl.getOutEdges(i).Count And _
                     gVL.Item(i).outDegree = gEl.getOutEdges(i).Count) << 4

        Dim V As New List(Of Double)
        Dim V2 As List(Of Double)

        ' outPutgVL(gVL)

        For Each gV In gVL.getVertexList
            V.Add(rndC.Next(0, 10))
        Next

        V2 = gVL.Median(V)

        Dim count As Integer = 0
        For j As Integer = 0 To V.Count - 1
            Dim str As String = ""
            Dim gV As graphVertex = gVL.Item(j)
            Dim tmpV As New List(Of Double)
            tmpV.Add(V.Item(j))
            For Each nb In gV.neighbours
                tmpV.Add(V.Item(nb))
            Next
            tmpV.Sort()
            For Each value As Double In tmpV
                str = str & " " & value
            Next
            ' System.Console.WriteLine(V.Item(j) & " : " & str & " / " & V2.Item(j))
            count += Convert.ToInt32(tmpV.Item(Math.Max(tmpV.Count - 1, 0) / 2) = V2.Item(j))
        Next
        testCase += Convert.ToInt32(count = V.Count) << 5

        V2 = gVL.Average(V)

        count = 0
        For j As Integer = 0 To V.Count - 1
            Dim gV As graphVertex = gVL.Item(j)
            Dim sum As Double = V.Item(j)
            For Each nb In gV.neighbours
                sum += V.Item(nb)
            Next

            count += Convert.ToInt32((sum / (gV.outDegree + 1)) = V2.Item(j))
        Next
        testCase += Convert.ToInt32(count = V.Count) << 6

        gVL2.clear()
        gEL2.clear()
        ' System.Console.WriteLine(gEl.ToString())
        ' System.Console.WriteLine(gEL2.ToString())
        ' outPutgEL(gEL2)
        ' System.Console.WriteLine(gVL.ToString())
        ' System.Console.WriteLine(gVL2.ToString())

        Dim old As Integer = gEl.edgeCount
        Dim dL As New List(Of Integer)
        For ii As Integer = 0 To 4
            dL.Add(ii)
        Next
        gVL.delete(dL)
        gEl.delete(dL)
        testCase += Convert.ToInt32(old = gEl.edgeCount + dL.Count) << 7


        Return Convert.ToString(testCase, 2)
    End Function

    Private Sub outPutgVL(ByVal gVL As graphVertexList)
        For Each gV As graphVertex In gVL.getVertexList
            System.Console.WriteLine(gV.ToString())
        Next
    End Sub

    Private Sub outPutgEL(ByVal gEl As graphEdgeList)
        For Each gE In gEl.getEdgeList
            System.Console.WriteLine(gE.ToString())
        Next
    End Sub

    Private Sub testgraphElement()

        Dim gV As New graphVertex(0)

        System.Console.WriteLine("ToString: " & gV.ToString())
        System.Console.WriteLine("isValid: " & gV.isValid())
        System.Console.WriteLine("maxIdnex: " & gV.maxIndex)
        System.Console.WriteLine("negativeCost: " & gV.negativeCost)
        System.Console.WriteLine("-----------------------------")
        System.Console.WriteLine("Insert 9: " & gV.insert(9, -2))
        System.Console.WriteLine("Insert 23: " & gV.insert(23))
        System.Console.WriteLine("Insert -1: " & gV.insert(-1))
        System.Console.WriteLine("Insert 1: " & gV.insert(1))
        System.Console.WriteLine("Insert 3: " & gV.insert(3))
        System.Console.WriteLine("Insert 2: " & gV.insert(2))
        System.Console.WriteLine("-----------------------------")
        System.Console.WriteLine("negativeCost: " & gV.negativeCost)
        System.Console.WriteLine("-----------------------------")
        System.Console.WriteLine("findNB -1: " & gV.findNB(-1))
        System.Console.WriteLine("findNB 3: " & gV.findNB(3))
        System.Console.WriteLine("findNB 9: " & gV.findNB(9))
        System.Console.WriteLine("-----------------------------")
        System.Console.WriteLine("removeNB 22: " & gV.removeNB(22))
        System.Console.WriteLine("removeNB 23: " & gV.removeNB(23))
        System.Console.WriteLine("-----------------------------")

        Dim gV2 As New graphVertex(0)

        Dim gE As New graphEdge(0, 11, 5.34)
        System.Console.WriteLine("gV2 insert [" & gE.ToString() & "]: " & gV2.insert(gE))
        System.Console.WriteLine("gV2 ToString: " & gV2.ToString())

        gE = New graphEdge(11, 0, 5.34)
        System.Console.WriteLine("gV2 insert [" & gE.ToString() & "]: " & gV2.insert(gE))
        System.Console.WriteLine("gV2 ToString: " & gV2.ToString())

        gE = New graphEdge(0, 54, 2.69)
        System.Console.WriteLine("gV2 insert [" & gE.ToString() & "]: " & gV2.insert(gE))
        System.Console.WriteLine("gV2 ToString: " & gV2.ToString())

        gE = New graphEdge(0, 9, 1.23)
        System.Console.WriteLine("gV2 insert [" & gE.ToString() & "]: " & gV2.insert(gE))
        System.Console.WriteLine("gV2 ToString: " & gV2.ToString())
        System.Console.WriteLine("-----------------------------")
        System.Console.WriteLine("merge: " & gV.merge(gV2))
        System.Console.WriteLine("-----------------------------")

        gV2.initialize()
        System.Console.WriteLine("gV2 ToString: " & gV2.ToString())
        System.Console.WriteLine("-----------------------------")

        Dim ListgE As New List(Of graphEdge)

        ListgE = gV.outEdges
        ListgE.Add(New graphEdge(9, 11, 2))
        System.Console.WriteLine("outEdges: " & ListgE.Count)
        For Each gE In ListgE
            gE.addCost = 2
            System.Console.WriteLine("Merge: " & gE.Merge(New graphEdge(9, 11, 2)))
            System.Console.WriteLine("isValid: " & gE.isValid())
            System.Console.WriteLine(gE.ToString())

            gV = New graphVertex(gE)
            System.Console.WriteLine(gV.ToString())
        Next


        Dim gEL As New graphEdgeList(ListgE)
        System.Console.WriteLine(gEL.ToString())
        Dim gVL As New graphVertexList(gEL.getVertexList)
        System.Console.WriteLine(gVL.ToString())

    End Sub

End Module
