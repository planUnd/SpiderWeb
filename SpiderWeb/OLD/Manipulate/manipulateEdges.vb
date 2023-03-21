Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class manipulateEdges
    Inherits GH_Component

    Public Sub New()
        MyBase.New("manipulateEdges", "mE", "Merge, Delete, Insert Edges", "Extra", "SpiderWebManipulation")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("27caf46c-7c76-4b04-ba8c-a2c5c3a78452")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "delete Edges", AddressOf setDeleteEdges, True, 0 = GetValue("methode", 0))
    End Sub


    Private Sub setDeleteEdges(ByVal sender As Object, ByVal e As EventArgs)
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub


    Private Sub SetMessage()
        Select Case GetValue("methode", 0)
            Case 0
                Message = "Delete Edges"
        End Select
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.manipulateEdges
        End Get
    End Property

    Protected Overrides Sub RegisterInputParams(pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("GraphEdgerepräsentation", "EG", "Edgerepräsentation of the Graph", GH_ParamAccess.tree)
        pManager.AddNumberParameter("EdgeCosts", "EC", "Costs Matching Edgerepräsentation Datatree Structure", GH_ParamAccess.tree)
        pManager(1).Optional = True
        pManager.AddIntegerParameter("Edge Indices", "EI", "Index of Edges to Manipulate", GH_ParamAccess.list)
        pManager(2).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("Edgerepräsentation of the Graph", "EG", "Edgerepräsentation of the Graph")
        pManager.Register_DoubleParam("Remaped Edge Costs", "EC", "Values Matching Edgerepräsentation Datatree Structure")
    End Sub

    ' ACHTUNG HIER FUNCTIONIERT ETWAS NOCH NICHT - Vielleicht INPUT ANPASSEN
    Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim EG_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing
        Dim EC_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number) = Nothing
        Dim index_L As New List(Of Integer)

        If (Not DA.GetDataList("Vertices", index_L)) Then
            index_L = New List(Of Integer)
        End If

        If (Not DA.GetDataTree("GraphEdgerepräsentation", EG_DATATREE)) Then Return
        If (Not DA.GetDataTree("EdgeCosts", EC_DATATREE)) Then
            EC_DATATREE = New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Number)
        End If

        If (EG_DATATREE.PathCount <= 0) Then Return
        If (EC_DATATREE.PathCount > 0 And EG_DATATREE.PathCount <> EC_DATATREE.PathCount) Then Return

        Dim M As Integer = GetValue("methode", 0)

        Dim GH_gEL As New GH_graphEdgeList()

        GH_gEL.parseEdgeDataTree(EG_DATATREE, EC_DATATREE)

        If (GH_gEL.vertexCount() > 65534) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Callulation might return false results. Graph limited to 65534 vertices!")
        End If
        Select Case M
            Case 0
                GH_gEL.Delete(index_L)
        End Select

        DA.SetDataTree(0, GH_gEL.EG_DATATREE())
        DA.SetDataTree(1, GH_gEL.EC_DATATREE())
    End Sub
End Class
