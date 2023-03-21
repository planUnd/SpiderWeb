Imports Grasshopper.Kernel
Public Class ConvexSubdivision
    Inherits GH_Component

    Public Sub New()
        MyBase.New("ConvexSubdivision", "ConvexSubdivision", "Subdivides a Surface into Convex Cells", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("19f5e3d1-0134-4cd5-a93f-5b75e2821e41")
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.Register_SurfaceParam("Surface", "S", "Surface to Subdivide", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_CurveParam("Cells", "C", "Cells")
        pManager.Register_MeshParam("M", "M", "M")
    End Sub

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        'collect data
        Dim B As Rhino.Geometry.Brep
        If (Not DA.GetData("Surface", B)) Then Return

        Dim M As New Rhino.Geometry.Mesh()
        Dim C As New List(Of Rhino.Geometry.Curve)

        M = Rhino.Geometry.Mesh.CreateFromBrep(B, Rhino.Geometry.MeshingParameters.Default).First()

        ' run trough Mesh and create Original Cells
        Dim f As Rhino.Geometry.MeshFace

        For Each f In M.Faces


            If f.IsQuad Then

            End If
        Next



        DA.SetDataList(0, C)
        DA.SetData(1, M)

    End Sub
End Class
