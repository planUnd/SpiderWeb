Option Explicit On

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_DataTreeToGraph
    Inherits GH_Component

    Public Sub New()
        MyBase.New("DataTreeToGraph", "DTG", "parse a DataTree", "Extra", "SpiderWebBasic")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As Guid
        Get
            Return New Guid("E2D3FBC3-CEA0-4D5D-8132-DA2A32D744C5")
        End Get
    End Property

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.graphFormDatatree
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)
        Menu_AppendItem(menu, "Parse DataTree (DT)", AddressOf parseDataTree, True, 0 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Parse VertexList (VL)", AddressOf parseVertexList, True, 1 = GetValue("methode", 0))
        Menu_AppendItem(menu, "Parse EdgeList (EL)", AddressOf parseEdgeList, True, 2 = GetValue("methode", 0))
    End Sub

    Private Sub parseDataTree()
        SetValue("methode", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub parseVertexList()
        SetValue("methode", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub parseEdgeList()
        SetValue("methode", 2)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub SetMessage()
        Message = ""
        Select Case GetValue("methode", 0)
            Case 0
                Message = Message & "DT"
            Case 1
                Message = Message & "VL"
            Case 2
                Message = Message & "EL"
        End Select
    End Sub

    Public Overrides Sub AddedToDocument(document As GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
        pManager.AddGenericParameter("DataTree", "T", "DataTree to built the Graph from", GH_ParamAccess.tree)
        pManager.AddNumberParameter("Cost", "C", "Edge costs of the Graph. Structure must match the Structure of T", GH_ParamAccess.tree)
        pManager.Param(1).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("Graph", "G", "Graph representation of DataTree")
        ' pManager.Register_StringParam("dbg", "dbg", "dbg")
    End Sub

    ' Parsing GH_Integer / IGH_Goo

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim pDT As New GH_Structure(Of IGH_Goo)
        Dim gecDT As New GH_Structure(Of GH_Number)
        Dim costs As Boolean = False

        If (Not DA.GetDataTree("DataTree", pDT)) Then Return
        If (pDT.Count = 0) Then Return
        DA.GetDataTree("Cost", gecDT)


        Dim maxIndex As Integer = pDT.LongestPathIndex()

        ' reworkSoThatItCanOutputMultibleGraphss

        Select Case GetValue("methode", 0)
            Case 0
                Dim gVL As New GH_GraphVertexList()
                pDT.Simplify(Data.GH_SimplificationMode.CollapseAllOverlaps)
                If (pDT.Path(maxIndex).Length < 2) Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Insoficiant information to create Graph.")
                    Return
                ElseIf (pDT.Path(maxIndex).Length > 2) Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Some datatree paths where ignored (Pathlength must equal 2).")
                End If

                If (gecDT.DataCount <> 0 And pDT.PathCount = gecDT.PathCount) Then
                    costs = True
                Else
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Edges of Graph have the cost 0.")
                End If

                If costs Then
                    gVL.GraphFromDatatree(pDT, gecDT)
                Else
                    gVL.GraphFromDatatree(pDT)
                End If
                DA.SetData(0, gVL)
            Case 1
                Dim gDT As New GH_Structure(Of GH_Integer)
                Dim gVL As New GH_GraphVertexList()

                gDT = convertDTToInt(pDT)

                ' If (gDT.DataCount = 0) Then Return
                If (gecDT.DataCount <> 0 And gDT.DataCount = gecDT.DataCount And gDT.PathCount = gecDT.PathCount) Then
                    costs = True
                Else
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Edges of Graph have the cost +Infinity.")
                End If

                If (costs) Then
                    gVL.parseVertexDataTree(gDT, gecDT)
                Else
                    gVL.parseVertexDataTree(gDT)
                End If
                DA.SetData(0, gVL)

            Case 2
                Dim gDT As New GH_Structure(Of GH_Integer)
                Dim gEL As New GH_GraphEdgeList()

                gDT = convertDTToInt(pDT)

                ' If (gDT.DataCount = 0) Then Return
                If (gecDT.DataCount <> 0 And gDT.PathCount = gecDT.PathCount) Then
                    costs = True
                Else
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Edges of Graph have the cost +Infinity.")
                End If

                If (costs) Then
                    gEL.parseEdgeDataTree(gDT, gecDT)
                Else
                    gEL.parseEdgeDataTree(gDT)
                End If
                DA.SetData(0, gEL)

        End Select

    End Sub

    Private Function convertDTToInt(ByVal DT As GH_Structure(Of IGH_Goo)) As GH_Structure(Of GH_Integer)
        Dim dtI As New GH_Structure(Of GH_Integer)
        Dim int As Integer

        For Each P As GH_Path In DT.Paths
            dtI.EnsurePath(P)
            For Each item As IGH_Goo In DT.DataList(P)
                If GH_Convert.ToInt32(item, int, GH_Conversion.Both) Then
                    dtI.DataList(P).Add(New GH_Integer(int))
                End If
            Next
        Next

        Return dtI
    End Function
End Class
