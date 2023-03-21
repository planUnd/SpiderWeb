Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class GraphFromMesh
    Inherits GH_Component

    Public Sub New()
        MyBase.New("graphFromMesh", "graphFromMesh", "Create a directed Graph From a Mesh", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("a3b7ba01-8c2f-4427-9bde-c79f5df20e41")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "directed", AddressOf setDirected, True, False = GetValue("undirected", False))
        Menu_AppendItem(menu, "undirected", AddressOf setUndirected, True, True = GetValue("undirected", False))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "EdgeGraph", AddressOf setEdgeGraph, True, 0 = GetValue("graphType", 0))
        Menu_AppendItem(menu, "FaceGraph", AddressOf setFaceGraph, True, 1 = GetValue("graphType", 0))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "VertexList", AddressOf setVL, True, False = GetValue("representation", False))
        Menu_AppendItem(menu, "EdgeList", AddressOf setEL, True, True = GetValue("representation", False))
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

    Private Sub setEdgeGraph()
        SetValue("graphType", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setFaceGraph()
        setUndirected()
        SetValue("graphType", 1)
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
        Select Case GetValue("representation", False)
            Case False
                Message = "VL/"
            Case True
                Message = "EL/"
        End Select
        Select Case GetValue("graphType", 0)
            Case 0
                Message = Message & "EdgeGraph/"
            Case 1
                Message = Message & "FaceGraph/"
        End Select
        Select Case GetValue("undirected", False)
            Case False
                Message = "directed"
            Case True
                Message = "undirected"
        End Select

    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddMeshParameter("Mesh", "M", "mesh to Build the Graph From", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph representation of mesh")
        pManager.Register_PointParam("graph Points", "GP", "3d points of the graph")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.GraphFromMesh
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim M As New Rhino.Geometry.Mesh()
        Dim gT As Integer = GetValue("graphType", 0)
        Dim rep As Boolean = GetValue("representation", False)
        Dim undirected As Boolean = GetValue("undirected", False)

        If (Not DA.GetData("Mesh", M)) Then Return

        If (M.Faces.Count > 65534 And gT = 1 And rep Or _
            M.Vertices.Count > 65534 And gT = 0 And rep) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "To many faces. Graph limited to 65534 vertices!")
            Return
        End If

        Dim Vertex As New List(Of Rhino.Geometry.Point3d)

        If rep Then
            Dim GH_gEL As New GH_graphEdgeList()
            If gT = 1 Then
                Vertex = GH_gEL.FaceGraphFromMesh(M)
            Else
                GH_gEL.EdgeGraphFromMesh(M)
                Vertex.AddRange(M.Vertices)
            End If
            DA.SetData(0, GH_gEL)
        Else
            Dim GH_gVL As New GH_graphVertexList()
            If gT = 1 Then
                Vertex = GH_gVL.FaceGraphFromMesh(M)
            Else
                GH_gVL.EdgeGraphFromMesh(M)
                Vertex.AddRange(M.Vertices)
            End If
            DA.SetData(0, GH_gVL)
        End If

        DA.SetDataList(1, Vertex)
    End Sub
End Class
