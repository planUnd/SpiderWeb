Option Explicit On

Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_GraphFromMesh
    Inherits GH_Component

    Public Sub New()
        MyBase.New("graphFromMesh", "gFM", "Create a directed Graph From a Mesh", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("a3b7ba01-8c2f-4427-9bde-c79f5df20e41")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Edge Graph (EG)", AddressOf setEdgeGraph, True, 0 = GetValue("graphType", 0))
        Menu_AppendItem(menu, "Face Graph (FG)", AddressOf setFaceGraph, True, 1 = GetValue("graphType", 0))
    End Sub

    Private Sub setEdgeGraph()
        SetValue("graphType", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setFaceGraph()
        SetValue("graphType", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Private Sub SetMessage()
        Select Case GetValue("graphType", 0)
            Case 0
                Message = "EG"
            Case 1
                Message = "FG"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddMeshParameter("Mesh", "M", "mesh to Build the Graph From", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph representation of Mesh")
        pManager.Register_PointParam("graphPoints", "GP", "3d points of the Graph")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.GraphFromMesh
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim M As New Rhino.Geometry.Mesh()
        Dim gT As Integer = GetValue("graphType", 0)
        Dim undirected As Boolean = GetValue("undirected", False)

        If (Not DA.GetData("Mesh", M)) Then Return

        Dim GP_L As New List(Of Rhino.Geometry.Point3d)
        Dim gVL As New GH_GraphVertexList()

        If gT = 1 Then
            GP_L = gVL.FaceGraphFromMesh(M)
        Else
            gVL.EdgeGraphFromMesh(M)
            GP_L.AddRange(M.Vertices.ToPoint3dArray)
        End If

        DA.SetData(0, gVL)
        DA.SetDataList(1, GP_L)
    End Sub
End Class
