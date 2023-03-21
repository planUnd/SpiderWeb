Option Explicit On

Imports Grasshopper
Imports Rhino.Geometry
Imports Grasshopper.Kernel
Imports GH_SpiderWebLibrary.GH_graphRepresentaions
Imports SpiderWebLibrary.graphTools

Public Class GH_Component_Eigensystem
    Inherits GH_Component
    Public Sub New()
        MyBase.New("Eigensystem", "ESYS", "Computes the Eigensystem of graphMatrix", "Extra", "SpiderWebSpectral")
    End Sub

    Public Overrides ReadOnly Property ComponentGuid As System.Guid
        Get
            Return New Guid("8C8F0C2F-FF76-446D-8ADA-73747353AB43")
        End Get
    End Property


    Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)
        MyBase.AppendAdditionalComponentMenuItems(menu)
        Menu_AppendItem(menu, "Remove Degenerated (D)", AddressOf setDegenerated, True, True = GetValue("degenerated", False))
        Menu_AppendItem(menu, "Remove Zero (Z)", AddressOf setZero, True, True = GetValue("zero", False))
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

    Public Overrides Sub AddedToDocument(document As Grasshopper.Kernel.GH_Document)
        MyBase.AddedToDocument(document)
        SetMessage()
    End Sub

    Protected Overrides Sub RegisterInputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_InputParamManager)
        pManager.AddParameter(New GH_graphMatrixParam(), "graphMatrix", "gM", "graphMatrix", GH_ParamAccess.item)
    End Sub

    Protected Overrides Sub RegisterOutputParams(ByVal pManager As Grasshopper.Kernel.GH_Component.GH_OutputParamManager)
        pManager.Register_DoubleParam("eigenValues", "eV", "Eigenvalues of the graphMatrix")
        pManager.Register_DoubleParam("eigenVectors", "eVec", "Eigenvectors of the graphMatrix")
        pManager.Register_IntegerParam("k-dimension", "k", "Dimensionality selection Based on Profil Liklyhood")
    End Sub

    Protected Overrides ReadOnly Property Internal_Icon_24x24() As System.Drawing.Bitmap
        Get
            Return My.Resources.eigensystem
        End Get
    End Property

    Protected Overrides Sub SolveInstance(ByVal DA As Grasshopper.Kernel.IGH_DataAccess)
        Dim gM As New GH_graphMatrix()
        If (Not DA.GetData("graphMatrix", gM)) Then Return

        Dim sGM As New spectralGraphMatrix(gM)
        Dim eVec As New DataTree(Of Double)

        If GetValue("degenerated", False) Then
            sGM.removeDegenerated()
        End If

        If GetValue("zero", False) Then
            sGM.removeZero()
        End If

        For i As Integer = 0 To sGM.columnCount - 1
            eVec.EnsurePath(i)
            eVec.Branch(i).AddRange(sGM.eigenVector(i))
        Next

        DA.SetDataList(0, sGM.eigenValue)
        DA.SetDataTree(1, eVec)
        DA.SetData(2, sGM.profilLiklyhoodDimension())
    End Sub
End Class
