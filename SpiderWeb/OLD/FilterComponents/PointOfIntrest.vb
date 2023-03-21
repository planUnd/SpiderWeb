Imports Grasshopper.Kernel

Public Class PointOfIntrest
    Inherits GH_Component

    Public Sub New()
        MyBase.New("Points of Interest", "POI", "Points of Interest", "Extra", "SpiderWebFilter")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("c50c36f3-ccd2-4f49-8778-8d3ea4f243e1")
        End Get
    End Property

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.POI
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as Datatree", GH_ParamAccess.tree)
        pManager.AddNumberParameter("Values", "V", "Values to Filter", GH_ParamAccess.list)
        pManager.AddNumberParameter("Reference Value", "RV", "Reference Value", GH_ParamAccess.item, 0.0)
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Point Index", "PI", "Point Index")
        pManager.Register_DoubleParam("Intrest Value", "IV", "Intrest Value")
    End Sub

    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim G_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim Values As New List(Of Double)
        Dim RV As Double

        If (Not DA.GetDataTree("Graph", G_DATATREE)) Then Return
        If (Not DA.GetDataList("Values", Values)) Then Return
        If (Not DA.GetData("Reference Value", RV)) Then Return

        If (Values.Count <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input V(Values)")
            Return
        End If
        If (G_DATATREE.DataCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If

        If (G_DATATREE.Branches.Count <> Values.Count) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input G doesn't match input V. Number of branches (G) must be equal to number of Values (V) ")
            Return
        End If

        Dim PI_List As New List(Of Integer)
        Dim IV_List As New List(Of Double)

        GH_GraphFilter.IntrestValues(G_DATATREE, Values, RV, IV_List)
        GH_GraphFilter.POI(IV_List, PI_List)

        DA.SetDataList(0, PI_List)
        DA.SetDataList(1, IV_List)

    End Sub
End Class
