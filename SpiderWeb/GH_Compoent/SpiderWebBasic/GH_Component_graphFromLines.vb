Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_graphFromLines
    Inherits GH_Component
    Public Sub New()
        MyBase.New("graphFromLines", "gFL", "create a graph from a set of connected lines", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("6a465f3b-e675-4a7e-b86d-44b496ebbb98")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Directed (d)", AddressOf setDirected, True, False = GetValue("undirected", False))
        Menu_AppendItem(menu, "Undirected (u)", AddressOf setUndirected, True, True = GetValue("undirected", False))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "Endpoints (E)", AddressOf setEndpoints, True, 0 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Intersection (I)", AddressOf setIntersection, True, 1 = GetValue("methode", 0))
    End Sub

    Private Sub setEndpoints()
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setIntersection()
        SetValue("methode", 1)
        SetValue("undirected", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setDirected()
        SetValue("undirected", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setUndirected()
        SetValue("undirected", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub SetMessage()

        Select Case GetValue("undirected", False)
            Case False
                Message = "d"
            Case True
                Message = "u"
        End Select
        Select Case GetValue("methode", 0)
            Case 0
                Message = Message & "/ E"
            Case 1
                Message = Message & "/ I"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddLineParameter("ConnectionLines", "L", "Lines to build a graph from", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph representation of lines")
        pManager.Register_PointParam("graphPoints", "GP", "3d points of the graph")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.graphFromLines
        End Get
    End Property
    ' Add intersecting ...
    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim L_List As New List(Of Rhino.Geometry.Line)

        If (Not DA.GetDataList("ConnectionLines", L_List)) Then Return
        If (L_List.Count = 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input L(ConnectionLines)")
            Return
        End If

        Dim GP_L As New List(Of Rhino.Geometry.Point3d)
        Dim gVL As New GH_GraphVertexList

        Dim m As Integer = GetValue("methode", 0)
        If m = 0 Then
            GP_L = gVL.GraphFromLines(L_List, GetValue("undirected", False), GH_Component.DocumentTolerance())
        ElseIf m = 1 Then
            GP_L = gVL.GraphFromLineIntersection(L_List, GH_Component.DocumentTolerance())
        End If

        DA.SetData(0, gVL)
        DA.SetDataList(1, GP_L)
    End Sub

End Class
