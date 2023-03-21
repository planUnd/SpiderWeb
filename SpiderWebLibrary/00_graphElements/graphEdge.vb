Option Explicit On


Imports SpiderWebLibrary.graphElements.compare


Namespace graphElements

    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : graphEdge
    ''' 
    ''' <summary>
    ''' graphEdge Class contains multible methodes to help the creation and manipulation of graphEdges
    ''' </summary>
    ''' <remarks>
    ''' Graphs represented through this class are limited to 65536 vertices. This is due to it's simple GetHashCode() function.
    ''' HashCode = Int32.MinValue + (tmpA "leftBitShift" 16) + tmpB. Further multible edges are not allowed.  
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    '''     ''' [Richard Schaffranek]   20/06/2014 allow Loops
    ''' </history>
    ''' 

    Public Class graphEdge
        Implements gEComp, graphElement

        Private Const maxVertices As Integer = 65536

        ''' <summary>
        ''' cost of the graphEdge.
        ''' </summary>
        ''' <remarks>
        ''' If set to  Double.PositiveInfinity, than the edge will be treated as having no costs.
        ''' This is different to a edgeCost of 0!
        ''' </remarks>
        Private edgeCost As Double

        Private edgeA, edgeB As Integer

        ' -------------------- Constructor --------------------------
        ''' <overloads>
        ''' Constructor
        ''' </overloads>
        ''' <summary>
        ''' Creates a new  inValid instance of graphEdge
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Sub New()
            Me.edgeA = -1
            Me.edgeB = -1
            Me.edgeCost = -1
        End Sub

        ''' <overloads>
        ''' Constructor
        ''' </overloads>
        ''' <summary>
        ''' Creates a new instance of graphEdge based on an existing graphEdge
        ''' </summary>
        ''' <param name="E">Existing graphEdge</param>
        ''' <remarks>
        ''' </remarks>
        Public Sub New(ByVal E As graphEdge)
            Me.edgeA = E.A
            Me.edgeB = E.B
            Me.edgeCost = E.Cost
        End Sub

        ''' <overloads>
        ''' Constructor
        ''' </overloads>
        ''' <summary>
        ''' Creates a new instance of graphEdge form existing values.
        ''' </summary>
        ''' <param name="A">Starting point of graphEdge as index.</param>
        ''' <param name="B">End point of graphEdge as index.</param>
        ''' <param name="cost">Optional, Cost of the graphEdge</param>
        ''' <remarks>
        ''' </remarks>
        Public Sub New(ByVal A As Integer, ByVal B As Integer, Optional ByVal cost As Double = Double.PositiveInfinity)
            Me.edgeA = A
            Me.edgeB = B
            Me.edgeCost = cost
        End Sub

        ' -------------------- graphElement --------------------------
        ''' <inheritdoc/>
        ReadOnly Property maxIndex As Integer Implements graphElement.maxIndex
            Get
                Return Math.Max(Me.A, Me.B)
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property negativeCost(Optional ByVal tol As Double = 1) As Boolean Implements graphElement.negativeCost
            Get
                Return Math.Round(Me.Cost / tol) < 0
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property isValid As Boolean Implements graphElement.isValid
            Get
                Return (A >= 0 And A < maxVertices And _
                B >= 0 And B < maxVertices) ' And  A <> B)
            End Get
        End Property

        ''' <inheritdoc/>
        Protected Friend Sub remapVertex(ByVal map As List(Of Integer)) Implements graphElement.remapVertex
            Me.edgeA = map.BinarySearch(Me.A)
            Me.edgeB = map.BinarySearch(Me.B)
        End Sub

        ' -------------------- Property --------------------------

        ReadOnly Property tAt(ByVal cA As Double, ByVal cB As Double, ByVal t As Double)
            Get
                If Double.IsPositiveInfinity(Me.Cost) Then
                    Return ((t - cA) / (cB - cA))
                Else
                    Return ((t - cA) / Me.Cost)
                End If
            End Get
        End Property

        ''' <summary>
        ''' Cost to travel along the graphEdge
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property Cost() As Double
            Get
                Return edgeCost
            End Get
            Set(value As Double)
                edgeCost = value
            End Set
        End Property

        ''' <summary>
        ''' Starting point of the graphEdge
        ''' </summary>
        ''' <value>Must be different to the end point of the GraphEdge (B)</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property A() As Integer
            Get
                Return edgeA
            End Get
            Set(value As Integer)
                'If value <> B Then
                edgeA = value
                ' End If
            End Set
        End Property

        ''' <summary>
        ''' End point of the GraphEdge
        ''' </summary>
        ''' <value>Must be different to the starting point of the GraphEdge (A)</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property B() As Integer
            Get
                Return edgeB
            End Get
            Set(value As Integer)
                'If value <> A Then
                edgeB = value
                'End If
            End Set
        End Property

        ''' <summary>
        ''' A,B as List.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property indexList() As List(Of Integer)
            Get
                Dim retList As New List(Of Integer)
                retList.Add(A)
                retList.Add(B)
                Return retList
            End Get
        End Property

        ''' <summary>
        ''' Add Cost to the graphEdge
        ''' </summary>
        ''' <value>Value to add to the cost property.</value>
        ''' <remarks></remarks>
        WriteOnly Property addCost As Double
            Set(value As Double)
                edgeCost = edgeCost + value
            End Set
        End Property

        ' -------------------- Overrides --------------------------

        ''' <summary>
        ''' Tests for equality. Uses HashCode as basis.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns>Return true if equal, otherwise false or if inValid false.</returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Nothing OrElse Not [GetType]().Equals(obj.GetType()) Then
                Return False
            End If
            Dim E As graphEdge = CType(obj, graphEdge)
            If E.isValid And Me.isValid And E.GetHashCode() = MyBase.GetHashCode() Then
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Hashfunction of the GraphEdge used to compare two GraphEdges.
        ''' </summary>
        ''' <returns>HashCode as Integer</returns>
        ''' <remarks>HashCode = Int 32.MinValue + (tmpA "leftBitShift" 16) + tmpB. This limites the Graph stored by the GraphEdge class to a maximum of 65534 vertices.</remarks>
        Public Overrides Function GetHashCode() As Integer
            Dim tmpA, tmpB As Long
            tmpA = A
            tmpB = B
            Return Convert.ToInt32(Int32.MinValue + (tmpA << 16) + tmpB)
        End Function

        ''' <summary>
        ''' Returns the String representation.
        ''' </summary>
        ''' <returns>String</returns>
        ''' <remarks>
        '''  (A, B) : Cost, Hashcode
        ''' </remarks>
        Public Overrides Function ToString() As String
            Return "(" & A & ", " & B & " ) :" & Cost & ", " & GetHashCode()
        End Function

        ' -------------------- IComparer --------------------------
        ''' <summary>
        ''' Compare two graphEdges
        ''' </summary>
        ''' <param name="E1"></param>
        ''' <param name="E2"></param>
        ''' <returns>Returns 1 if HashCode of E1 larger HashCode of E2, 0 if equal, -1 if  HashCode of E1 smaller HashCode of E2</returns>
        ''' <remarks>
        ''' Sorts graphEdges with asscenting HashCode.
        ''' </remarks>
        Public Function Compare(ByVal E1 As graphEdge, ByVal E2 As graphEdge) As Integer Implements System.Collections.Generic.IComparer(Of graphEdge).Compare
            If Not E1.isValid Then
                If Not E2.isValid Then
                    Return 0
                Else
                    Return -1
                End If
            Else
                If Not E2.isValid Then
                    Return 1
                Else
                    If E1.GetHashCode() > E2.GetHashCode() Then
                        Return 1
                    ElseIf E2.GetHashCode() > E1.GetHashCode() Then
                        Return -1
                    Else
                        Return 0
                    End If
                End If
            End If
        End Function

        ' -------------------- Function --------------------------
        ''' <summary>
        ''' Merge two grapEdges.
        ''' </summary>
        ''' <param name="gE">graphEdge to merge</param>
        ''' <returns>Returns true if successful.</returns>
        ''' <remarks>Merges two graphEdges into the existing graphEdge.</remarks>
        Public Function Merge(ByVal gE As graphEdge) As Boolean
            If Me.B = gE.A Then
                Me.A = Me.A
                Me.B = gE.B
                Me.Cost = Me.Cost + gE.Cost
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Splits the edge into two
        ''' </summary>
        ''' <param name="nI">index for the newly generated graphVertex</param>
        ''' <param name="t">Optional parameter for splitting the costs. If not set t = 0.5.</param>
        ''' <returns>Return new two new graphEdges.</returns>
        ''' <remarks></remarks>
        Public Function split(ByVal nI As Integer, Optional ByVal t As Double = 0.5) As graphEdge()
            Dim gE(2) As graphEdge

            gE(0) = New graphEdge(Me.A, nI, Me.Cost * t)
            gE(1) = New graphEdge(nI, Me.B, Me.Cost * (1 - t))

            Return gE
        End Function

        ''' <summary>
        ''' Flip the edge.
        ''' </summary>
        ''' <remarks>This will also change the HashCode of the graphEdge!</remarks>
        Public Sub Flip()
            Dim tmp As Integer
            tmp = Me.edgeA
            Me.edgeA = Me.edgeB
            Me.edgeB = tmp
        End Sub

    End Class
End Namespace