Option Explicit On

Imports Rhino.Geometry
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions

Public Class GH_Component_RSC
    Inherits GH_Component

    Public Sub New()
        MyBase.New("recursiveShadowCasting", "RSC", "preformes recursive shadow casting on a visualGraph", "Extra", "SpiderWebTools")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("ADE08589-6173-464F-9F40-A5123C8FB6E7")
        End Get
    End Property
    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)

        Menu_AppendItem(menu, "Directed (d)", AddressOf setDirected, True, False = GetValue("undirected", False))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "From All (A)", AddressOf setAll, True, True = GetValue("all", False))
        Menu_AppendItem(menu, "From SP (SP)", AddressOf setSP, True, False = GetValue("all", False))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "Euclidian Distance (eD)", AddressOf setEuclidianDistance, True, 0 = GetValue("edgeWeight", 0))
        Menu_AppendItem(menu, "Visibility % (V)", AddressOf setVisiblity, True, 1 = GetValue("edgeWeight", 0))
    End Sub

    Private Sub setEuclidianDistance()
        RecordUndoEvent("edgeWeightUndoEvent")
        SetValue("edgeWeight", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setVisiblity()
        RecordUndoEvent("edgeWeightUndoEvent")
        SetValue("edgeWeight", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setAll()
        RecordUndoEvent("allUndoEvent")
        SetValue("all", True)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setSP()
        RecordUndoEvent("allUndoEvent")
        SetValue("all", False)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setDirected()
        SetValue("undirected", False)
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
                Message = "d"
            Case True
                Message = "u"
        End Select
        Select Case GetValue("all", False)
            Case False
                Message = Message & " / SP"
            Case True
                Message = Message & " / A"
        End Select
        Select Case GetValue("edgeWeight", 0)
            Case 1
                Message = Message & " / V"
            Case Else
                Message = Message & " / eD"
        End Select
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_visualGraphParam(), "visualGraph", "vG", "visualGraph representation", GH_ParamAccess.item)
        pManager.AddIntegerParameter("StartPoints", "SP", "Index of Starting Points", GH_ParamAccess.list)
        pManager.Param(1).Optional = True
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_GenericParam("visualGraph", "vG", "visualGraph representation")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.RSC
        End Get
    End Property

    Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
        Dim vG As New GH_visualGraph()

        Dim SP As New List(Of Integer)
        Dim all As Boolean = GetValue("all", False)

        If (Not DA.GetData("visualGraph", vG)) Then Return
        If (Not DA.GetDataList("StartPoints", SP) And Not all) Then Return

        Dim tmpVG As New GH_visualGraph(vG)
        Dim eW As Integer = GetValue("edgeWeight", 0)

        If all Then
            tmpVG.recursiveAll(eW)
        Else
            Dim x, y As Integer
            For i As Integer = 0 To SP.Count - 1
                x = SP.Item(i) Mod vG.width
                y = Math.Floor(SP.Item(i) / vG.width)
                tmpVG.recursive(x, y, eW)
            Next
        End If

        DA.SetData(0, tmpVG)
    End Sub

End Class
