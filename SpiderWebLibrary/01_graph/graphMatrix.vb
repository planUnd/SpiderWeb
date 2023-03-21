Option Explicit On

Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports MathNet.Numerics.LinearAlgebra

Namespace graphRepresentaions
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : graphMatrix
    ''' 
    ''' <summary>
    ''' Represents a Graph as Matrix.
    ''' </summary>
    ''' <remarks>
    ''' Uses Meta.Net for Matrix Calculation    
    ''' http://www.mathdotnet.com/
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   06/06/2014 created
    ''' [Richard Schaffranek]   16/11/2014 changed: meta.numerics -> math.net
    ''' </history>

    Public Class graphMatrix

        Protected gM As Matrix(Of Double)
        Protected MT As matrixType

        Enum matrixType As Integer
            null = 0
            laplacianMatrix = 1
            relaxedLaplacianMatrix = 2
            weightedLaplacianMatrix = 3
            normalizedLaplacianMatrix = 4
            gausianWeightedMatrix = 5
            weightedAdjacencyMatirx = 6
        End Enum

        ' ------------------------------------------ Constructor ------------------------------------
        ''' <summary>
        '''  Construct a empty Graph represented by a 1 dimensional Matrix
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            Me.gM = Nothing
            Me.MT = matrixType.null
        End Sub

        ''' <summary>
        ''' Construct a new graphMatrix from a Graph
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal G As Graph, Optional ByVal t As matrixType = matrixType.weightedAdjacencyMatirx, Optional ByVal p As Double = 1)
            G.ensureUndirected()
            Me.MT = t
            Select Case t
                Case matrixType.laplacianMatrix
                    Me.laplacianMatrix(G)
                Case matrixType.relaxedLaplacianMatrix
                    Me.relexedLaplacianMatrix(G, p)
                Case matrixType.weightedLaplacianMatrix
                    Me.weightedLaplacianMatrix(G)
                Case matrixType.normalizedLaplacianMatrix
                    Me.normalizedLaplacianMatrix(G)
                Case matrixType.weightedAdjacencyMatirx
                    Me.adjacencyMatrix(G)
                Case matrixType.gausianWeightedMatrix
                    Me.getGausianWeightedMatrix(G, p)
                Case Else
                    Me.gM = Nothing
            End Select
        End Sub

        ''' <summary>
        ''' Construct a new graphMatrix from a graphMatrix
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal G As graphMatrix)
            Me.gM = Matrix(Of Double).Build.DenseOfMatrix(Me.gM)
            Me.MT = G.MT
        End Sub

        ' ------------------------------------------ Properties ------------------------------------
        ''' <summary>
        ''' Dimension of the graphMatrix
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns the dimension of a graphMatrix as Integer</returns>
        ''' <remarks>The highest index within the graphMatrix is dimension-1</remarks>
        ReadOnly Property dimension() As Integer
            Get
                Return Me.gM.ColumnCount
            End Get
        End Property

        ReadOnly Property getMatrix() As Matrix(Of Double)
            Get
                Return Matrix(Of Double).Build.DenseOfMatrix(Me.gM)
            End Get
        End Property

        ReadOnly Property type() As graphMatrix.matrixType
            Get
                Return Me.MT
            End Get
        End Property

        '------------------------------------ MAtrices ------------------------------------
        ''' <summary>
        ''' Construct the relexed Laplacian Matrix representation of a Graph
        ''' </summary>
        ''' <param name="G">Existing Graph</param>
        ''' <remarks>for further details see: https://kops.ub.uni-konstanz.de/xmlui/bitstream/handle/urn:nbn:de:bsz:352-opus-15332/thesis.pdf?sequence=1
        ''' </remarks>
        Private Sub relexedLaplacianMatrix(ByVal G As Graph, Optional ByVal p As Double = 0)
            Me.gM = Matrix(Of Double).Build.Dense(G.vertexCount, G.vertexCount)
            Dim gVList = G.getVertexList
            For Each gV As graphVertex In gVList
                Me.gM(gV.index, gV.index) = gV.outDegree * (1 - p)
                For i As Integer = 0 To gV.outDegree - 1
                    Me.gM(gV.index, gV.neighbours(i)) = -1
                Next
            Next
        End Sub

        ''' <summary>
        ''' Construct the degree normalized Laplacian Matrix representation of a Graph
        ''' </summary>
        ''' <param name="G">Existing Graph</param>
        ''' <remarks>for further details see: https://kops.ub.uni-konstanz.de/xmlui/bitstream/handle/urn:nbn:de:bsz:352-opus-15332/thesis.pdf?sequence=1
        ''' </remarks>
        Private Sub normalizedLaplacianMatrix(ByVal G As Graph)
            Me.gM = Matrix(Of Double).Build.Dense(G.vertexCount, G.vertexCount)
            Dim gVList = G.getVertexList
            For Each gV As graphVertex In gVList
                    Me.gM(gV.index, gV.index) = 1
                    For i As Integer = 0 To gV.outDegree - 1
                        Me.gM(gV.index, gV.neighbours(i)) = -1 / Math.Sqrt((gV.outDegree * gVList.Item(gV.neighbours(i)).outDegree))
                    Next
            Next
        End Sub

        ''' <summary>
        ''' Construct the weighted Laplacian Matrix representation of a Graph
        ''' </summary>
        ''' <param name="G">Existing Graph</param>
        ''' <remarks></remarks>
        Private Sub weightedLaplacianMatrix(ByVal G As Graph)
            Me.gM = Matrix(Of Double).Build.Dense(G.vertexCount, G.vertexCount)
            Dim gVList = G.getVertexList
            For Each gV As graphVertex In gVList
                Me.gM(gV.index, gV.index) = gV.cost.Sum
                For i As Integer = 0 To gV.outDegree - 1
                    Me.gM(gV.index, gV.neighbours(i)) = -gV.cost(i)
                Next
            Next
        End Sub

        ''' <summary>
        ''' Construct the Laplacian Matrix representation of a Graph
        ''' </summary>
        ''' <param name="G">Existing Graph</param>
        ''' <remarks></remarks>
        Private Sub laplacianMatrix(ByVal G As Graph)
            Me.gM = Matrix(Of Double).Build.Dense(G.vertexCount, G.vertexCount)
            Dim gVList = G.getVertexList
            For Each gV As graphVertex In gVList
                Me.gM(gV.index, gV.index) = gV.outDegree
                For i As Integer = 0 To gV.outDegree - 1
                    Me.gM(gV.index, gV.neighbours(i)) = -1
                Next
            Next
        End Sub

        ''' <summary>
        ''' Construct the Adjacency Matrix representation of a Graph
        ''' </summary>
        ''' <param name="G">Existing Graph</param>
        ''' <remarks></remarks>
        Private Sub adjacencyMatrix(ByVal G As Graph)
            Me.gM = Matrix(Of Double).Build.Dense(G.vertexCount, G.vertexCount)
            Dim gVList = G.getVertexList
            For Each gV As graphVertex In gVList
                For i As Integer = 0 To gV.outDegree - 1
                    Me.gM(gV.index, gV.neighbours(i)) = gV.cost(i)
                Next
            Next
        End Sub

        ''' <summary>
        ''' Construct a Gaussian Weighted Matrix of a Graph
        ''' </summary>
        ''' <param name="G">Existing Graph</param>
        ''' <remarks>for further details see: http://image.ntua.gr/iva/files/ShapiroBrady_IVC1992%20-%20Feature-Based%20Correspondence-%20an%20Eigenvector%20Approach.pdf
        ''' </remarks>
        Private Sub getGausianWeightedMatrix(ByVal G As graphVertexList, Optional ByVal p As Double = 1)
            Me.gM = Matrix(Of Double).Build.Dense(G.vertexCount, G.vertexCount)
            Dim s As Double = (2 * Math.Pow(p, 2))
            Dim gVList = G.getVertexList
            For Each gV As graphVertex In gVList
                Me.gM(gV.index, gV.index) = 1S
                For i As Integer = 0 To gV.outDegree - 1
                    Me.gM(gV.index, gV.neighbours(i)) = Math.Pow(Math.E, (-Math.Pow(gV.cost(i), 2) / s))
                Next
            Next
        End Sub


        '------------------------------------ Function ------------------------------------
        Public Overrides Function toString() As String
            Select Case Me.MT
                Case matrixType.laplacianMatrix
                    Return "laplacianMatrix: " & Me.dimension & ", " & Me.dimension
                Case matrixType.relaxedLaplacianMatrix
                    Return "relaxedLaplacianMatrix: " & Me.dimension & ", " & Me.dimension
                Case matrixType.weightedLaplacianMatrix
                    Return "weightedLaplacianMatrix: " & Me.dimension & ", " & Me.dimension
                Case matrixType.normalizedLaplacianMatrix
                    Return "normalizedLaplacianMatrix: " & Me.dimension & ", " & Me.dimension
                Case matrixType.weightedAdjacencyMatirx
                    Return "weightedAdjacencyMatirx: " & Me.dimension & ", " & Me.dimension
                Case matrixType.gausianWeightedMatrix
                    Return "gausianWeightedMatrix: " & Me.dimension & ", " & Me.dimension
            End Select
            Return "nullMatrix"
        End Function
    End Class
End Namespace