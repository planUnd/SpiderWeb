Option Explicit On

Imports Grasshopper
Imports Rhino.Geometry
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphTools

Public Class GH_Component_BFS
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Breadth-first Search", "BFS", "Breadth-first Search", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("72fbdf5f-a5dc-424f-9451-3e1e1e36fe11")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Find All (A)", AddressOf setFindAll, True, 0 = GetValue("methode", 1))
        Menu_AppendItem(menu, "Find First (F)", AddressOf setFindFirst, True, 1 = GetValue("methode", 1))
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("methode", 1)
            Case 0
                Message = "A"
            Case 1
                Message = "F"
        End Select
    End Sub

    Private Sub setFindAll(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setFindFirst(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("methode", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_GraphVertexListParam(), "Graph", "G", "Graph (grpahVertexList / graphEdgeList)", GH_ParamAccess.item)
        pManager.AddIntegerParameter("StartPoints", "SP", "Index of Starting Points", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("previouseGraph", "pG", "Directed Graph Pointing to the Previouse Item")
        pManager.Register_DoubleParam("Cost", "C", "Cost from the Starting Points (step)")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.BFS
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gVL As New GH_GraphVertexList()
        Dim Start As Integer

        If (Not DA.GetData("Graph", gVL)) Then Return
        If (Not DA.GetData("StartPoints", Start)) Then Return

        If (gVL.vertexCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        ElseIf (gVL.vertexCount < Start) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid starting point for BFS")
            Return
        End If

        Dim BFS As New BFS(gVL)

        Select Case GetValue("methode", 1)
            Case 0
                BFS.findALL(Start, GH_Component.DocumentTolerance())
            Case 1
                BFS.find(Start, GH_Component.DocumentTolerance())
        End Select

        DA.SetData(0, BFS)
        DA.SetDataList(1, BFS.dist)
    End Sub
End Class
