Option Explicit On


Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports SpiderWebLibrary.graphRepresentaions

Namespace graphTools
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : eulerianPath
    ''' 
    ''' <summary>
    ''' Uses Vertex List representation to compute the EularianPath.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 
    Public Class eulerianPath
        Inherits graphVertexList
        Private SP, EP As List(Of Integer)
        ' -------------------- Constructor --------------------------
        ''' <inheritdoc/>
        Public Sub New()
            MyBase.New()
            Me.initialize()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal G As Graph)
            MyBase.New(G)
            Me.initialize()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal tmpVertexList As List(Of graphVertex))
            MyBase.New(tmpVertexList)
            Me.initialize()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal tmpVertex As graphVertex)
            MyBase.New(tmpVertex)
            Me.initialize()
        End Sub

        ''' <inheritdoc/>
        Public Sub New(ByVal vertices As Integer)
            MyBase.New(vertices)
            Me.initialize()
        End Sub

        ' -------------------- Property --------------------------
        ''' <summary>
        ''' Get the possible starting points of an eularian path
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns a list of possible SP for an eularian path as graphVertex.</returns>
        ''' <remarks>
        ''' Function computeSPEP() has to be called first before SP can be collected.
        ''' If computeSPEP() was not called or no eularian path exists this will return an empty list.
        ''' </remarks>
        ReadOnly Property getSP() As List(Of graphVertex)
            Get
                Dim tmpSP As New List(Of graphVertex)
                For Each e In SP
                    tmpSP.Add(Me.Item(e))
                Next
                Return tmpSP
            End Get
        End Property

        ''' <summary>
        ''' Get the possible end points of an eularian path
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns a list of possible EP for an eularian path as graphVertex.</returns>
        ''' <remarks>
        ''' Function computeSPEP() has to be called first before SP can be collected.
        ''' If computeSPEP() was not called or no eularian path exists this will return an empty list.
        ''' </remarks>
        ReadOnly Property getEP() As List(Of graphVertex)
            Get
                Dim tmpEP As New List(Of graphVertex)
                For Each e In EP
                    tmpEP.Add(Me.Item(e))
                Next
                Return tmpEP
            End Get
        End Property

        ''' <summary>
        ''' Get the possible starting points of an eularian path
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns a list of possible SP for an eularian path as graphVertex.index.</returns>
        ''' <remarks>
        ''' Function computeSPEP() has to be called first before SP can be collected.
        ''' If computeSPEP() was not called or no eularian path exists this will return an empty list.
        ''' </remarks>
        ReadOnly Property getSPindex() As List(Of Integer)
            Get
                Return Me.SP
            End Get
        End Property

        ''' <summary>
        ''' Get the possible end points of an eularian path
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns a list of possible EP for an eularian path as graphVertex.index.</returns>
        ''' <remarks>
        ''' Function computeSPEP() has to be called first before SP can be collected.
        ''' If computeSPEP() was not called or no eularian path exists this will return an empty list.
        ''' </remarks>
        ReadOnly Property getEPindex() As List(Of Integer)
            Get
                Return Me.EP
            End Get
        End Property

        ' -------------------- Private --------------------------
        Private Sub initialize()
            Me.EP = New List(Of Integer)
            Me.SP = New List(Of Integer)
        End Sub

        ' -------------------- Public --------------------------
        ''' <summary>
        ''' Compute the possible starting and endpoints of the eularian path.
        ''' </summary>
        ''' <returns>Returns true if an eularian path exists, false otherwise.</returns>
        ''' <remarks></remarks>
        Public Function computeSPEP() As Boolean
            Dim SPEP As New List(Of Integer)
            Dim IN_Edges As New List(Of Integer)
            Dim OUT_Edges As New List(Of Integer)
            Dim gV As graphVertex
            Dim diff As Integer

            For ip As Integer = 0 To MyClass.vertexCount - 1
                OUT_Edges.Add(0)
                IN_Edges.Add(0)
            Next

            For i As Integer = 0 To MyClass.vertexCount - 1
                gV = Me.Item(i)
                OUT_Edges.Item(i) = gV.outDegree
                For Each nb In gV.neighbours
                    IN_Edges.Item(nb) += 1
                Next
            Next

            For i As Integer = 0 To IN_Edges.Count - 1
                diff = OUT_Edges.Item(i) - IN_Edges.Item(i)
                If diff = 0 Then
                    SPEP.Add(i)
                ElseIf diff = 1 Then
                    SP.Add(i)
                ElseIf diff = -1 Then
                    EP.Add(i)
                Else ' wenn irgendwo mehr als zwei Kanten hinausgehen
                    SP.Clear()
                    EP.Clear()
                    Return False
                End If
            Next

            If SP.Count = 0 And EP.Count = 0 Then
                SP.AddRange(SPEP)
                EP.AddRange(SPEP)
            ElseIf SP.Count + EP.Count > 2 Then
                SP.Clear()
                EP.Clear()
            End If

            Return True
        End Function
    End Class

End Namespace