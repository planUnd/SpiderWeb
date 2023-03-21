Option Explicit On

Imports Rhino.Geometry
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_visualGraphGrid
    Inherits GH_Component

    Public Sub New()
        MyBase.New("visualGraphGrid", "vGG", "Create a visualGraphGrid from obstacles and a boundary area", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("78A9496D-1BA1-4416-8DDB-E3DD6C9D2EF0")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddCurveParameter("Area", "A", "Outer boundary of the area within to create the viusalGraph", GH_ParamAccess.item)
        pManager.AddCurveParameter("Obstacles", "O", "Obstacles within the area", GH_ParamAccess.list)
        pManager(1).Optional = True
        pManager.AddNumberParameter("gridSize", "gS", "Size of the gridCells of the visualGraph", GH_ParamAccess.item)
        pManager.AddNumberParameter("overlapDistance", "oD", "Distance of overlap", GH_ParamAccess.item, 0)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("visualGraph", "vG", "visualGraph representation")
        pManager.Register_RectangleParam("gridCells", "gC", "Grid Cells")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.VGAGrid
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim A As Curve
        Dim O_List As New List(Of Curve)
        Dim SP As New List(Of Integer)
        Dim sXY As Double
        Dim oD As Double

        If (Not DA.GetData("Area", A)) Then Return
        If (Not DA.GetData("gridSize", sXY)) Then Return
        DA.GetDataList("Obstacles", O_List)
        DA.GetData("overlapDistance", oD)

        Dim gV As New GH_visualGraph()
        Dim RList As New List(Of Rectangle3d)

        RList = gV.visualGraphGrid(A, O_List, sXY, oD)

        DA.SetData(0, gV)
        DA.SetDataList(1, RList)
    End Sub

End Class
