Imports Grasshopper.Kernel

Public Class Eulerian_path
    Inherits GH_Component

    Public Sub New()
        MyBase.New("EulerianPath", "EulerianPath", "Checks if the Given Graph Has an Eulerian Path and Eeturns All Possible Starting Points", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("00d89f0c-a48b-4b66-beeb-d470d532e399")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)
        Menu_AppendItem(menu, "directed", AddressOf setDirected, True, False = GetValue("undirected", False))
        Menu_AppendItem(menu, "undirected", AddressOf setUndirected, True, True = GetValue("undirected", False))
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
                Message = "directed"
            Case True
                Message = "undirected"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddIntegerParameter("Graph", "G", "Graph as Datatree", GH_ParamAccess.tree)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_IntegerParam("StartingPoints", "SP", "Possible Starting Points of the Eulerian Path")
        pManager.Register_IntegerParam("EndPoints", "EP", "Possible End Points of the Eulerian Path")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.EulerPath
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim tmpG_DATATREE As Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.GH_Integer) = Nothing

        If (Not DA.GetDataTree("Graph", tmpG_DATATREE)) Then Return
        If (tmpG_DATATREE.DataCount <= 0) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No valid data collected in input G(Graph)")
            Return
        End If

        Dim undirected As Boolean = GetValue("undirected", False)

        Dim SP As New List(Of Integer)
        Dim EP As New List(Of Integer)

        If undirected Then
            Dim G_DATATREE As Grasshopper.DataTree(Of Integer)
            G_DATATREE = GH_GraphBasic.ensureUndirected(tmpG_DATATREE)
            If Not GH_GraphTools.EulerianPath(G_DATATREE, undirected, SP, EP) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Eulerian path doesn't exist")
            End If
        Else
            If Not GH_GraphTools.EulerianPath(tmpG_DATATREE, undirected, SP, EP) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Eulerian path doesn't exist")
            End If

        End If

        DA.SetDataList(0, SP)
        DA.SetDataList(1, EP)
    End Sub
End Class
