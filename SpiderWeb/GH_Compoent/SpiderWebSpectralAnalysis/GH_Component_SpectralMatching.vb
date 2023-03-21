Option Explicit On

Imports Grasshopper
Imports Rhino.Geometry
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphTools
Imports MathNet.Numerics.LinearAlgebra

Public Class GH_Component_SpectralMatching
    Inherits GH_Component
    Public Sub New()
        MyBase.New("SpectralMatching", "SM", "computes n-dimensional vectors for spectralGraphMatching", "Extra", "SpiderWebSpectral")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("B2FFC8DA-247E-403A-A575-DC023ECF8E36")
        End Get
    End Property

    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)
        Menu_AppendItem(menu, "Remove Degenerated (D)", AddressOf setDegenerated, True, True = GetValue("degenerated", False))
        Menu_AppendItem(menu, "Remove Zero (Z)", AddressOf setZero, True, True = GetValue("zero", False))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "Dominant Sign (dS)", AddressOf setDominantSign, True, 0 = GetValue("type", 0))
        Menu_AppendItem(menu, "Minimze Costs (mC)", AddressOf setMinimzeCosts, True, 1 = GetValue("type", 0))
        Menu_AppendSeparator(menu)
        Menu_AppendItem(menu, "Normalize (N)", AddressOf setNorm, True, True = GetValue("norm", True))
    End Sub

    Private Sub SetMessage()
        If GetValue("degenerated", False) And GetValue("zero", False) Then
            Me.Message = "Remove(D, Z)"
        ElseIf GetValue("degenerated", False) Then
            Me.Message = "Remove(D)"
        ElseIf GetValue("zero", False) Then
            Me.Message = "Remove(Z)"
        Else
            Me.Message = "Remove()"
        End If
        Select Case GetValue("type", 0)
            Case 0
                Me.Message = Me.Message & " / dS"
            Case 1
                Me.Message = Me.Message & " / mC"
        End Select
        Select Case GetValue("norm", True)
            Case True
                Me.Message = Me.Message & " / N"
        End Select
    End Sub

    Private Sub setDegenerated(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("degenerated", Not (GetValue("degenerated", False)))
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setZero(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("zero", Not (GetValue("zero", False)))
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setNorm(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        If GetValue("norm", True) = False Then
            SetValue("norm", True)
        Else
            SetValue("norm", False)
        End If
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setDominantSign(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 0)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Private Sub setMinimzeCosts(ByVal sender As Object, ByVal e As EventArgs)
        RecordUndoEvent("methodeUndoEvent")
        SetValue("type", 1)
        SetMessage()
        ExpireSolution(True)
    End Sub

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_graphMatrixParam(), "graphMatrixA", "gMA", "graphMatrix A", GH_ParamAccess.item)
        pManager.AddParameter(New GH_graphMatrixParam(), "graphMatrixB", "gMB", "graphMatrix B", GH_ParamAccess.item)
        pManager.AddIntegerParameter("n-dimensional", "n", "number of maximal dimensions of the resulting position vectors", GH_ParamAccess.item, Integer.MaxValue)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_DoubleParam("spectralMappingA", "sMA", "spectralMapping of graphMatrix A")
        pManager.Register_DoubleParam("spectralMappingB", "sMB", "spectralMapping of graphMatrix B")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.spectralMatching
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gMA As New GH_graphMatrix()
        Dim gMB As New GH_graphMatrix()
        Dim n As Integer

        If (Not DA.GetData("graphMatrixA", gMA)) Then Return
        If (Not DA.GetData("graphMatrixB", gMB)) Then Return
        If (Not DA.GetData("n-dimensional", n)) Then Return

        If gMA.type <> gMB.type Then
            Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "graphMatrix Types do not match!")
            Return
        End If

        Dim sGMA, sGMB As spectralGraphMatrix

        sGMA = New spectralGraphMatrix(gMA)
        sGMB = New spectralGraphMatrix(gMB)

        If GetValue("degenerated", False) Then
            sGMA.removeDegenerated()
            sGMB.removeDegenerated()
        End If

        If GetValue("zero", False) Then
            sGMA.removeZero()
            sGMB.removeZero()
        End If

        If sGMA.columnCount = 0 Or sGMB.columnCount = 0 Then
            Me.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "From at least on Matrix, all Eigenvectors/Eigenvalues got removed.")
            Return
        End If

        n = Math.Min(n, Math.Min(sGMA.columnCount, sGMB.columnCount))

        sGMA.truncate(n)
        sGMB.truncate(n)

        If GetValue("type", 0) = 0 Then
            sGMA.dominantSignCorrection(sGMB)
        Else
            sGMA.minimizeCostsSignCorrection(sGMB)
        End If

        Dim vecA As New DataTree(Of Double)
        Dim vecB As New DataTree(Of Double)
        Dim tmpRV As Vector(Of Double)

        If GetValue("norm", True) Then
            For i As Integer = 0 To sGMA.rowCount - 1
                vecA.EnsurePath(i)
                tmpRV = sGMA.featureVector(i)
                tmpRV = tmpRV / tmpRV.L2Norm()
                vecA.Branch(i).AddRange(tmpRV)
            Next

            For i As Integer = 0 To sGMB.rowCount - 1
                vecB.EnsurePath(i)
                tmpRV = sGMB.featureVector(i)
                tmpRV = tmpRV / tmpRV.L2Norm()
                vecB.Branch(i).AddRange(tmpRV)
            Next
        Else
            For i As Integer = 0 To sGMA.rowCount - 1
                vecA.EnsurePath(i)
                tmpRV = sGMA.featureVector(i)
                vecA.Branch(i).AddRange(tmpRV)
            Next

            For i As Integer = 0 To sGMB.rowCount - 1
                vecB.EnsurePath(i)
                tmpRV = sGMB.featureVector(i)
                vecB.Branch(i).AddRange(tmpRV)
            Next
        End If

        DA.SetDataTree(0, vecA)
        DA.SetDataTree(1, vecB)
    End Sub
End Class
