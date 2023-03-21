Imports Grasshopper.Kernel
Imports SpiderWebLibrary

Public Class GraphFromDatatree
    Inherits GH_Component

    Public Sub New()
        MyBase.New("graphFromDatatree", "graphFromDatatree", "Create a Graph From a Datatree", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("304273eb-ac62-44c0-9af1-e996a700ea6c")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "undirected", AddressOf setUndirected, True, True = GetValue("undirected", True))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "VertexList", AddressOf setVL, True, False = GetValue("representation", False))
        Menu_AppendItem(menu, "EdgeList", AddressOf setEL, True, True = GetValue("representation", False))
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
        pManager.AddGenericParameter("DatatreePath", "P", "Path to Built Graph From", GH_ParamAccess.tree)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph representation of datatree")
        'pManager.Register_StringParam("Str", "Str", "Str")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.graphFormDatatree
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim rep As Boolean = GetValue("representation", False)

        Dim P_DATATREE As New Grasshopper.Kernel.Data.GH_Structure(Of Grasshopper.Kernel.Types.IGH_Goo)

        If (Not DA.GetDataTree("DatatreePath", P_DATATREE)) Then Return
        If (P_DATATREE.Count = 0) Then Return
        P_DATATREE.Simplify(Data.GH_SimplificationMode.CollapseAllOverlaps)

        If (P_DATATREE.LongestPathIndex() > 2) Then
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Some datatree paths where ignored (Pathlength must equal 2).")
        End If

        Dim maxPath As Grasshopper.Kernel.Data.GH_Path
        maxPath = P_DATATREE.Path(P_DATATREE.PathCount - 1)

        If maxPath.Length = 2 Then
            If (maxPath.Dimension(0) > 65534 Or maxPath.Dimension(1) > 65534) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Callulation might return false results. Graph limited to 65534 vertices!")
            End If
        End If



        If rep Then
            Dim GH_gEL As New GH_graphEdgeList()
            GH_gEL.GraphFromDatatree(P_DATATREE)
            DA.SetData(0, GH_gEL)
        Else
            Dim GH_gVL As New GH_graphVertexList()
            GH_gVL.GraphFromDatatree(P_DATATREE)
            DA.SetData(0, GH_gVL)
        End If
    End Sub
End Class
