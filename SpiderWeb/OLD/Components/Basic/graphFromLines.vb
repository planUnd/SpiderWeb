Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class graphFromLines
    Inherits GH_Component
    Public Sub New()
        MyBase.New("graphFromLines", "graphFromLines", "create a graph from a set of connected lines", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("6a465f3b-e675-4a7e-b86d-44b496ebbb98")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "directed", AddressOf setDirected, True, False = GetValue("undirected", False))
        Menu_AppendItem(menu, "undirected", AddressOf setUndirected, True, True = GetValue("undirected", False))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "VertexList", AddressOf setVL, True, False = GetValue("representation", False))
        Menu_AppendItem(menu, "EdgeList", AddressOf setEL, True, True = GetValue("representation", False))
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

    Private Sub setVL()
        SetValue("representation", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setEL()
        SetValue("representation", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("representation", False)
            Case False
                Message = "VL/"
            Case True
                Message = "EL/"
        End Select

        Select Case GetValue("undirected", False)
            Case False
                Message = Message & "directed"
            Case True
                Message = Message & "undirected"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddLineParameter("ConnectionLines", "L", "Lines to build a graph from", GH_ParamAccess.list)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph representation of lines")
        pManager.Register_PointParam("graph Points", "GP", "3d points of the graph")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.graphFromLines
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim L_List As New List(Of Rhino.Geometry.Line)

        If (Not DA.GetDataList("ConnectionLines", L_List)) Then Return
        If (L_List.Count = 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input L(ConnectionLines)")
            Return
        End If

        Dim undirected As Boolean = GetValue("undirected", False)
        Dim rep As Boolean = GetValue("representation", False)
        Dim Vertex As New List(Of Rhino.Geometry.Point3d)

        If rep Then
            Dim GH_gEL As New GH_graphEdgeList()
            Vertex = GH_gEL.GraphFromLines(L_List, undirected, GH_Component.DocumentTolerance())
            DA.SetData(0, GH_gEL)
        Else

            Dim GH_gVL As New GH_graphVertexList()
            Vertex = GH_gVL.GraphFromLines(L_List, undirected, GH_Component.DocumentTolerance())
            DA.SetData(0, GH_gVL)
        End If

        DA.SetDataList(2, Vertex)
    End Sub
    
End Class
